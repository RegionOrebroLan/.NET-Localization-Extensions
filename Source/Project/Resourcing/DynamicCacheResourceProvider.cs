using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Logging.Extensions;
using RegionOrebroLan.Localization.Resourcing.Extensions;

namespace RegionOrebroLan.Localization.Resourcing
{
	public class DynamicCacheResourceProvider : BasicCacheResourceProvider<IDynamicLocalizationSettings>, IDynamicResourceProvider
	{
		#region Constructors

		public DynamicCacheResourceProvider(IFileSystem fileSystem, IHostEnvironment hostEnvironment, ILoggerFactory loggerFactory, IResourceLocator resourceLocator, IDynamicLocalizationSettings settings) : base(loggerFactory, resourceLocator, settings)
		{
			this.FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
			this.HostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

			this.FileResourcesDirectoryWatcher = this.CreateFileResourcesDirectoryWatcher(fileSystem, hostEnvironment, settings);

			settings.EmbeddedResourceAssembliesChanged += this.OnEmbeddedResourceAssembliesChanged;
			settings.FileResourcesDirectoryChanged += this.OnFileResourcesDirectoryChanged;
		}

		#endregion

		#region Events

		public event EventHandler EmbeddedResourcesChanged;
		public event EventHandler<FileResourceChangedEventArgs> FileResourceContentChanged;
		public event EventHandler FileResourcesChanged;

		#endregion

		#region Properties

		protected internal virtual IFileSystemWatcher FileResourcesDirectoryWatcher { get; }
		protected internal virtual IFileSystem FileSystem { get; }
		protected internal virtual IHostEnvironment HostEnvironment { get; }

		#endregion

		#region Methods

		protected internal virtual void ClearEmbeddedResourcesCache()
		{
			this.Logger.LogInformationIfEnabled("Clearing the embedded-resources cache.");

			lock(this.EmbeddedResourcesLock)
			{
				this.EmbeddedResourcesCache = null;
			}

			this.OnEmbeddedResourcesChanged();
		}

		protected internal virtual void ClearFileResourcesCache()
		{
			this.Logger.LogInformationIfEnabled("Clearing the file-resources cache.");

			lock(this.FileResourcesLock)
			{
				this.FileResourcesCache = null;
			}

			this.OnFileResourcesChanged();
		}

		protected internal IFileSystemWatcher CreateFileResourcesDirectoryWatcher(IFileSystem fileSystem, IHostEnvironment hostEnvironment, IDynamicLocalizationSettings settings)
		{
			if(fileSystem == null)
				throw new ArgumentNullException(nameof(fileSystem));

			if(hostEnvironment == null)
				throw new ArgumentNullException(nameof(hostEnvironment));

			if(settings == null)
				throw new ArgumentNullException(nameof(settings));

			var fileSystemWatcher = fileSystem.FileSystemWatcher.New();

			this.SetFileSystemWatcher(settings.FileResourcesDirectory, fileSystemWatcher, hostEnvironment);

			fileSystemWatcher.IncludeSubdirectories = true;

			fileSystemWatcher.Changed += this.OnFileResourcesDirectoryEntryChanged;
			fileSystemWatcher.Created += this.OnFileResourcesDirectoryEntryCreated;
			fileSystemWatcher.Deleted += this.OnFileResourcesDirectoryEntryDeleted;
			fileSystemWatcher.Error += this.OnFileResourcesDirectoryEntryError;
			fileSystemWatcher.Renamed += this.OnFileResourcesDirectoryEntryRenamed;

			return fileSystemWatcher;
		}

		protected internal virtual void FinalizeResourcesDirectoryWatcher(IFileSystemWatcher resourcesDirectoryWatcher)
		{
			if(resourcesDirectoryWatcher == null)
				return;

			resourcesDirectoryWatcher.Changed -= this.OnFileResourcesDirectoryEntryChanged;
			resourcesDirectoryWatcher.Created -= this.OnFileResourcesDirectoryEntryCreated;
			resourcesDirectoryWatcher.Deleted -= this.OnFileResourcesDirectoryEntryDeleted;
			resourcesDirectoryWatcher.Error -= this.OnFileResourcesDirectoryEntryError;
			resourcesDirectoryWatcher.Renamed -= this.OnFileResourcesDirectoryEntryRenamed;

			resourcesDirectoryWatcher.Dispose();
		}

		protected internal virtual void OnEmbeddedResourceAssembliesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.ClearEmbeddedResourcesCache();
		}

		protected internal virtual void OnEmbeddedResourcesChanged()
		{
			this.EmbeddedResourcesChanged?.Invoke(this, EventArgs.Empty);
		}

		protected internal virtual void OnFileResourceContentChanged(string path)
		{
			this.FileResourceContentChanged?.Invoke(this, new FileResourceChangedEventArgs(path));
		}

		protected internal virtual void OnFileResourcesChanged()
		{
			this.FileResourcesChanged?.Invoke(this, EventArgs.Empty);
		}

		protected internal virtual void OnFileResourcesDirectoryChanged(object sender, EventArgs e)
		{
			try
			{
				this.Logger.LogDebugIfEnabled("OnFileResourcesDirectoryChanged: The file-resources cache will be cleared.");

				this.ClearFileResourcesCache();

				this.SetFileSystemWatcher(this.Settings.FileResourcesDirectory, this.FileResourcesDirectoryWatcher, this.HostEnvironment);
			}
			catch(Exception exception)
			{
				this.Logger.LogErrorIfEnabled(exception, "Error on resources-path-changed.");
				throw;
			}
		}

		protected internal virtual void OnFileResourcesDirectoryEntryChanged(object sender, FileSystemEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException(nameof(e));

			this.Logger.LogDebugIfEnabled(() => $"OnFileResourcesDirectoryEntryChanged: {e.FullPath} ({e.ChangeType})");

			if(!this.ResourceLocator.IsValidFileResource(e.FullPath))
				return;

			this.Logger.LogDebugIfEnabled(() => $"OnFileResourcesDirectoryEntryChanged: A valid file-resource was changed and the FileResourceContentChanged-event will be triggered. The file changed: {e.FullPath}");

			this.OnFileResourceContentChanged(e.FullPath);
		}

		protected internal virtual void OnFileResourcesDirectoryEntryCreated(object sender, FileSystemEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException(nameof(e));

			this.Logger.LogDebugIfEnabled(() => $"OnFileResourcesDirectoryEntryCreated: {e.FullPath} ({e.ChangeType})");

			if(this.FileSystem.Directory.Exists(e.FullPath))
				return;

			if(!this.ResourceLocator.IsValidFileResource(e.FullPath))
				return;

			this.Logger.LogDebugIfEnabled(() => $"OnFileResourcesDirectoryEntryCreated: A valid file-resource was created and the file-resources cache will be cleared. The file created: {e.FullPath}");

			this.ClearFileResourcesCache();
		}

		protected internal virtual void OnFileResourcesDirectoryEntryDeleted(object sender, FileSystemEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException(nameof(e));

			this.Logger.LogDebugIfEnabled(() => $"OnFileResourcesDirectoryEntryDeleted: {e.FullPath} ({e.ChangeType})");

			var matchingFileResource = (this.FileResourcesCache ?? Enumerable.Empty<IFileResource>()).FirstOrDefault(fileResource => string.Equals(fileResource.Path, e.FullPath, StringComparison.OrdinalIgnoreCase));

			if(matchingFileResource == null)
				return;

			this.Logger.LogDebugIfEnabled(() => $"OnFileResourcesDirectoryEntryDeleted: A cached file-resource was physically deleted and the file-resources cache will be cleared. The file deleted: {e.FullPath}");

			this.ClearFileResourcesCache();
		}

		protected internal virtual void OnFileResourcesDirectoryEntryError(object sender, ErrorEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException(nameof(e));

			this.Logger.LogWarningIfEnabled(() => $"OnFileResourcesDirectoryEntryError: {e.GetException()}");
		}

		protected internal virtual void OnFileResourcesDirectoryEntryRenamed(object sender, RenamedEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException(nameof(e));

			this.Logger.LogDebugIfEnabled(() => $"OnFileResourcesDirectoryEntryRenamed: from \"{e.OldFullPath}\" to \"{e.FullPath}\" ({e.ChangeType})");

			var renamedFilePaths = new List<string>();

			if(this.FileSystem.Directory.Exists(e.FullPath))
				renamedFilePaths.AddRange(this.FileSystem.Directory.GetFiles(e.FullPath, "*", SearchOption.AllDirectories));
			else
				renamedFilePaths.Add(e.FullPath);

			if(!renamedFilePaths.Any(this.ResourceLocator.IsValidFileResource))
				return;

			this.Logger.LogDebugIfEnabled(() => $"OnFileResourcesDirectoryEntryRenamed: Valid file-resources were renamed and the file-resources cache will be cleared. Files renamed: \"{string.Join("\", \"", renamedFilePaths)}\"");

			this.ClearFileResourcesCache();
		}

		[SuppressMessage("Performance", "CA1822:Mark members as static")]
		protected internal void SetFileSystemWatcher(IDirectoryInfo fileResourcesDirectory, IFileSystemWatcher fileSystemWatcher, IHostEnvironment hostEnvironment)
		{
			if(fileSystemWatcher == null)
				throw new ArgumentNullException(nameof(fileSystemWatcher));

			if(hostEnvironment == null)
				throw new ArgumentNullException(nameof(hostEnvironment));

			if(fileResourcesDirectory == null)
			{
				fileSystemWatcher.EnableRaisingEvents = false;
				fileSystemWatcher.Path = hostEnvironment.ContentRootPath;
			}
			else
			{
				if(!fileResourcesDirectory.Exists)
					throw new DirectoryNotFoundException($"The file-resources-directory \"{fileResourcesDirectory.FullName}\" does not exist.");

				fileSystemWatcher.Path = fileResourcesDirectory.FullName;
				fileSystemWatcher.EnableRaisingEvents = true;
			}
		}

		#endregion

		#region Other members

		#region Finalizers

		~DynamicCacheResourceProvider()
		{
			this.FinalizeResourcesDirectoryWatcher(this.FileResourcesDirectoryWatcher);
			this.Settings.EmbeddedResourceAssembliesChanged -= this.OnEmbeddedResourceAssembliesChanged;
			this.Settings.FileResourcesDirectoryChanged -= this.OnFileResourcesDirectoryChanged;
		}

		#endregion

		#endregion
	}
}