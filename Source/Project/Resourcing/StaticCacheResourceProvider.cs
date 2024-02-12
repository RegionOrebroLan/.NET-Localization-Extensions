using Microsoft.Extensions.Logging;

namespace RegionOrebroLan.Localization.Resourcing
{
	public class StaticCacheResourceProvider(ILoggerFactory loggerFactory, IResourceLocator resourceLocator, ILocalizationSettings settings) : BasicCacheResourceProvider<ILocalizationSettings>(loggerFactory, resourceLocator, settings) { }
}