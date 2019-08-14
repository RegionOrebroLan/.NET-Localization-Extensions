using System;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Hosting;
using RegionOrebroLan.Localization.Collections.Extensions;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Reflection.Extensions;
using RegionOrebroLan.Localization.Validation;

namespace RegionOrebroLan.Localization.Configuration
{
	public class LocalizationOptionsResolver : ILocalizationOptionsResolver
	{
		#region Constructors

		[CLSCompliant(false)]
		public LocalizationOptionsResolver(IAssemblyHelper assemblyHelper, IEmbeddedResourceAssembliesValidator embeddedResourceAssembliesValidator, IFileResourcesDirectoryValidator fileResourcesDirectoryValidator, IFileSystem fileSystem, IHostingEnvironment hostingEnvironment)
		{
			this.AssemblyHelper = assemblyHelper ?? throw new ArgumentNullException(nameof(assemblyHelper));
			this.EmbeddedResourceAssembliesValidator = embeddedResourceAssembliesValidator ?? throw new ArgumentNullException(nameof(embeddedResourceAssembliesValidator));
			this.FileResourcesDirectoryValidator = fileResourcesDirectoryValidator ?? throw new ArgumentNullException(nameof(fileResourcesDirectoryValidator));
			this.FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
			this.HostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
		}

		#endregion

		#region Properties

		protected internal virtual IAssemblyHelper AssemblyHelper { get; }
		protected internal virtual IEmbeddedResourceAssembliesValidator EmbeddedResourceAssembliesValidator { get; }
		protected internal virtual IFileResourcesDirectoryValidator FileResourcesDirectoryValidator { get; }
		protected internal virtual IFileSystem FileSystem { get; }

		[CLSCompliant(false)]
		protected internal virtual IHostingEnvironment HostingEnvironment { get; }

		#endregion

		#region Methods

		public virtual IResolvedLocalizationOptions Resolve(LocalizationOptions localizationOptions)
		{
			if(localizationOptions == null)
				throw new ArgumentNullException(nameof(localizationOptions));

			this.EmbeddedResourceAssembliesValidator.Validate(localizationOptions.EmbeddedResourceAssemblies);

			IAssembly[] embeddedResourceAssemblies;

			try
			{
				embeddedResourceAssemblies = this.AssemblyHelper.Find(localizationOptions.EmbeddedResourceAssemblies).ToArray();
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"{Validation.EmbeddedResourceAssembliesValidator.ExceptionMessagePrefix}The patterns-collection contains invalid values. Values: {localizationOptions.EmbeddedResourceAssemblies.ToCommaSeparatedArgumentString()}", exception);
			}

			this.EmbeddedResourceAssembliesValidator.Validate(embeddedResourceAssemblies);

			var fileResourcesDirectory = this.ResolveFileResourcesDirectory(localizationOptions);

			return new ResolvedLocalizationOptions(embeddedResourceAssemblies, fileResourcesDirectory, localizationOptions);
		}

		protected internal virtual IDirectoryInfo ResolveFileResourcesDirectory(LocalizationOptions localizationOptions)
		{
			IDirectoryInfo fileResourcesDirectory = null;

			var path = localizationOptions?.FileResourcesDirectoryPath?.Trim(this.FileSystem.Path.AltDirectorySeparatorChar, this.FileSystem.Path.DirectorySeparatorChar);

			// ReSharper disable InvertIf
			if(path != null)
			{
				if(!this.FileSystem.Path.IsPathRooted(path))
					path = this.FileSystem.Path.Combine(this.HostingEnvironment.ContentRootPath, path);

				fileResourcesDirectory = this.FileSystem.DirectoryInfo.FromDirectoryName(path);
			}
			// ReSharper restore InvertIf

			this.FileResourcesDirectoryValidator.Validate(fileResourcesDirectory);

			return fileResourcesDirectory;
		}

		#endregion
	}
}