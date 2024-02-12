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
	public class LocalizationOptionsResolver(IAssemblyHelper assemblyHelper, IEmbeddedResourceAssembliesValidator embeddedResourceAssembliesValidator, IFileResourcesDirectoryValidator fileResourcesDirectoryValidator, IFileSystem fileSystem, IHostEnvironment hostEnvironment) : ILocalizationOptionsResolver
	{
		#region Properties

		protected internal virtual IAssemblyHelper AssemblyHelper { get; } = assemblyHelper ?? throw new ArgumentNullException(nameof(assemblyHelper));
		protected internal virtual IEmbeddedResourceAssembliesValidator EmbeddedResourceAssembliesValidator { get; } = embeddedResourceAssembliesValidator ?? throw new ArgumentNullException(nameof(embeddedResourceAssembliesValidator));
		protected internal virtual IFileResourcesDirectoryValidator FileResourcesDirectoryValidator { get; } = fileResourcesDirectoryValidator ?? throw new ArgumentNullException(nameof(fileResourcesDirectoryValidator));
		protected internal virtual IFileSystem FileSystem { get; } = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
		protected internal virtual IHostEnvironment HostEnvironment { get; } = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

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
					path = this.FileSystem.Path.Combine(this.HostEnvironment.ContentRootPath, path);

				fileResourcesDirectory = this.FileSystem.DirectoryInfo.New(path);
			}
			// ReSharper restore InvertIf

			this.FileResourcesDirectoryValidator.Validate(fileResourcesDirectory);

			return fileResourcesDirectory;
		}

		#endregion
	}
}