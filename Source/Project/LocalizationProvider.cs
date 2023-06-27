using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Resourcing;

namespace RegionOrebroLan.Localization
{
	public class LocalizationProvider : DynamicCacheLocalizationProvider
	{
		#region Constructors

		public LocalizationProvider(ILocalizationPathResolver localizationPathResolver, ILocalizedStringFactory localizedStringFactory, ILoggerFactory loggerFactory, IResourceProvider resourceProvider, ILocalizationSettings settings) : base(localizationPathResolver, localizedStringFactory, loggerFactory, resourceProvider, settings) { }

		#endregion
	}
}