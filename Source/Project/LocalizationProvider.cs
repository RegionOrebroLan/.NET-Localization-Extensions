using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Resourcing;

namespace RegionOrebroLan.Localization
{
	[Obsolete("Will be removed. Use DynamicCacheLocalizationProvider instead.")]
	public class LocalizationProvider(ILocalizationPathResolver localizationPathResolver, ILocalizedStringFactory localizedStringFactory, ILoggerFactory loggerFactory, IDynamicResourceProvider resourceProvider, IDynamicLocalizationSettings settings) : DynamicCacheLocalizationProvider(localizationPathResolver, localizedStringFactory, loggerFactory, resourceProvider, settings) { }
}