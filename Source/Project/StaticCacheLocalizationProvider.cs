using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Resourcing;

namespace RegionOrebroLan.Localization
{
	public class StaticCacheLocalizationProvider : BasicCacheLocalizationProvider<IResourceProvider, ILocalizationSettings>
	{
		#region Constructors

		public StaticCacheLocalizationProvider(ILocalizationPathResolver localizationPathResolver, ILocalizedStringFactory localizedStringFactory, ILoggerFactory loggerFactory, IResourceProvider resourceProvider, ILocalizationSettings settings) : base(localizationPathResolver, localizedStringFactory, loggerFactory, resourceProvider, settings) { }

		#endregion
	}
}