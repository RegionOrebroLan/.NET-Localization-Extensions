using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Resourcing;
using RegionOrebroLan.Logging.Extensions;

namespace RegionOrebroLan.Localization
{
	public class DynamicCacheLocalizationProvider : BasicCacheLocalizationProvider<IDynamicResourceProvider, IDynamicLocalizationSettings>
	{
		#region Constructors

		public DynamicCacheLocalizationProvider(ILocalizationPathResolver localizationPathResolver, ILocalizedStringFactory localizedStringFactory, ILoggerFactory loggerFactory, IDynamicResourceProvider resourceProvider, IDynamicLocalizationSettings settings) : base(localizationPathResolver, localizedStringFactory, loggerFactory, resourceProvider, settings)
		{
			resourceProvider.EmbeddedResourcesChanged += this.OnEmbeddedResourcesChanged;
			resourceProvider.FileResourceContentChanged += this.OnFileResourceContentChanged;
			resourceProvider.FileResourcesChanged += this.OnFileResourcesChanged;
			settings.AlphabeticalSortingChanged += this.OnAlphabeticalSortingChanged;
			settings.IncludeParentCulturesChanged += this.OnIncludeParentCulturesChanged;
		}

		#endregion

		#region Methods

		protected internal virtual void ClearLocalizationsCache()
		{
			this.Logger.LogInformationIfEnabled("Clearing the localizations cache.");

			lock(this.LocalizationsLock)
			{
				this.LocalizationsCache = null;

				this.ClearLocalizedStringListCache();

				this.ClearLocalizedStringCache();
			}
		}

		protected internal virtual void ClearLocalizedStringCache()
		{
			this.Logger.LogInformationIfEnabled("Clearing the localized-string cache.");

			this.LocalizedStringCache.Clear();
		}

		protected internal virtual void ClearLocalizedStringListCache()
		{
			this.Logger.LogInformationIfEnabled("Clearing the localized-string-list cache.");

			this.LocalizedStringsWithoutParentCulturesIncludedCache.Clear();
			this.LocalizedStringsWithParentCulturesIncludedCache.Clear();
		}

		protected internal virtual void ClearResourceContentCache()
		{
			this.Logger.LogInformationIfEnabled("Clearing the resource-content cache.");

			lock(this.ResourceContentLock)
			{
				this.ResourceContentCache = null;

				this.ClearLocalizationsCache();
			}
		}

		protected internal virtual void OnAlphabeticalSortingChanged(object sender, EventArgs e)
		{
			this.ClearLocalizedStringListCache();
		}

		protected internal virtual void OnEmbeddedResourcesChanged(object sender, EventArgs e)
		{
			this.ClearResourceContentCache();
		}

		protected internal virtual void OnFileResourceContentChanged(object sender, FileResourceChangedEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException(nameof(e));

			this.UpdateResourceContent(e.Path);
		}

		protected internal virtual void OnFileResourcesChanged(object sender, EventArgs e)
		{
			this.ClearResourceContentCache();
		}

		protected internal virtual void OnIncludeParentCulturesChanged(object sender, EventArgs e)
		{
			this.ClearLocalizedStringCache();
		}

		[SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code")]
		protected internal virtual void UpdateResourceContent(string path)
		{
			if(path == null)
				throw new ArgumentNullException(nameof(path));

			if(this.ResourceContentCache == null)
				return;

			var fileResource = this.ResourceProvider.FileResources.FirstOrDefault(item => string.Equals(item.Path, path, StringComparison.OrdinalIgnoreCase));

			if(fileResource == null)
			{
				this.Logger.LogWarningIfEnabled("The file-resource with path \"{0}\" could not be found.", path);
				return;
			}

			if(this.ResourceContentCache == null || !this.ResourceContentCache.ContainsKey(fileResource))
				return;

			lock(this.ResourceContentLock)
			{
				this.ResourceContentCache[fileResource] = fileResource.Read();
			}

			this.ClearLocalizationsCache();
		}

		#endregion

		#region Other members

		#region Finalizers

		~DynamicCacheLocalizationProvider()
		{
			this.Settings.IncludeParentCulturesChanged -= this.OnIncludeParentCulturesChanged;
			this.Settings.AlphabeticalSortingChanged -= this.OnAlphabeticalSortingChanged;
		}

		#endregion

		#endregion
	}
}