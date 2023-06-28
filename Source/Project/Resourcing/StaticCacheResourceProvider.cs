using Microsoft.Extensions.Logging;

namespace RegionOrebroLan.Localization.Resourcing
{
	public class StaticCacheResourceProvider : BasicCacheResourceProvider<ILocalizationSettings>
	{
		#region Constructors

		public StaticCacheResourceProvider(ILoggerFactory loggerFactory, IResourceLocator resourceLocator, ILocalizationSettings settings) : base(loggerFactory, resourceLocator, settings) { }

		#endregion
	}
}