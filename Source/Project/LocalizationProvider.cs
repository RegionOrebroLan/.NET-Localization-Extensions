using System;
using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Resourcing;

namespace RegionOrebroLan.Localization
{
	[Obsolete("Will be removed. Use DynamicCacheLocalizationProvider instead.")]
	public class LocalizationProvider : DynamicCacheLocalizationProvider
	{
		#region Constructors

		public LocalizationProvider(ILocalizationPathResolver localizationPathResolver, ILocalizedStringFactory localizedStringFactory, ILoggerFactory loggerFactory, IDynamicResourceProvider resourceProvider, IDynamicLocalizationSettings settings) : base(localizationPathResolver, localizedStringFactory, loggerFactory, resourceProvider, settings) { }

		#endregion
	}
}