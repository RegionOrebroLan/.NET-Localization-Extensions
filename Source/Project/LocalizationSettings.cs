using System.IO.Abstractions;
using Microsoft.Extensions.Options;
using RegionOrebroLan.Localization.Configuration;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization
{
	public class LocalizationSettings : ILocalizationSettings
	{
		#region Constructors

		public LocalizationSettings(IOptions<LocalizationOptions> options, ILocalizationOptionsResolver optionsResolver)
		{
			if(options == null)
				throw new ArgumentNullException(nameof(options));

			var resolvedOptions = (optionsResolver ?? throw new ArgumentNullException(nameof(optionsResolver))).Resolve(options.Value);

			this.AlphabeticalSorting = resolvedOptions.AlphabeticalSorting;
			this.EmbeddedResourceAssemblies = resolvedOptions.EmbeddedResourceAssemblies;
			this.FileResourcesDirectory = resolvedOptions.FileResourcesDirectory;
			this.IncludeParentCultures = resolvedOptions.IncludeParentCultures;
			this.ThrowErrors = resolvedOptions.ThrowErrors;
		}

		#endregion

		#region Properties

		public virtual bool AlphabeticalSorting { get; }
		public virtual IEnumerable<IAssembly> EmbeddedResourceAssemblies { get; }
		public virtual IDirectoryInfo FileResourcesDirectory { get; }
		public virtual bool IncludeParentCultures { get; }
		public virtual bool ThrowErrors { get; }

		#endregion
	}
}