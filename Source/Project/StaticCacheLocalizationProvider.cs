using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Resourcing;

namespace RegionOrebroLan.Localization
{
	public class StaticCacheLocalizationProvider(ILocalizationPathResolver localizationPathResolver, ILocalizedStringFactory localizedStringFactory, ILoggerFactory loggerFactory, IResourceProvider resourceProvider, ILocalizationSettings settings) : BasicCacheLocalizationProvider<IResourceProvider, ILocalizationSettings>(localizationPathResolver, localizedStringFactory, loggerFactory, resourceProvider, settings) { }
}