using System.IO.Abstractions;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization.Configuration
{
	public class ResolvedLocalizationOptions(IEnumerable<IAssembly> embeddedResourceAssemblies, IDirectoryInfo fileResourcesDirectory, LocalizationOptions localizationOptions) : IResolvedLocalizationOptions
	{
		#region Properties

		public virtual bool AlphabeticalSorting => this.LocalizationOptions.AlphabeticalSorting;
		public virtual IEnumerable<IAssembly> EmbeddedResourceAssemblies { get; } = embeddedResourceAssemblies ?? throw new ArgumentNullException(nameof(embeddedResourceAssemblies));
		public virtual IDirectoryInfo FileResourcesDirectory { get; } = fileResourcesDirectory;
		public virtual bool IncludeParentCultures => this.LocalizationOptions.IncludeParentCultures;
		protected internal virtual LocalizationOptions LocalizationOptions { get; } = localizationOptions ?? throw new ArgumentNullException(nameof(localizationOptions));
		public virtual bool ThrowErrors => this.LocalizationOptions.ThrowErrors;

		#endregion
	}
}