using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization.Configuration
{
	public class ResolvedLocalizationOptions : IResolvedLocalizationOptions
	{
		#region Constructors

		public ResolvedLocalizationOptions(IEnumerable<IAssembly> embeddedResourceAssemblies, IDirectoryInfo fileResourcesDirectory, LocalizationOptions localizationOptions)
		{
			this.EmbeddedResourceAssemblies = embeddedResourceAssemblies ?? throw new ArgumentNullException(nameof(embeddedResourceAssemblies));
			this.FileResourcesDirectory = fileResourcesDirectory;
			this.LocalizationOptions = localizationOptions ?? throw new ArgumentNullException(nameof(localizationOptions));
		}

		#endregion

		#region Properties

		public virtual bool AlphabeticalSorting => this.LocalizationOptions.AlphabeticalSorting;
		public virtual IEnumerable<IAssembly> EmbeddedResourceAssemblies { get; }
		public virtual IDirectoryInfo FileResourcesDirectory { get; }
		public virtual bool IncludeParentCultures => this.LocalizationOptions.IncludeParentCultures;
		protected internal virtual LocalizationOptions LocalizationOptions { get; }
		public virtual bool ThrowErrors => this.LocalizationOptions.ThrowErrors;

		#endregion
	}
}