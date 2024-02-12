using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Shared.Web.Routing
{
	public class UiCultureRouteConstraint(IOptions<RequestLocalizationOptions> options) : LocalizationRouteConstraint(options, RouteKeys.UiCulture)
	{
		#region Properties

		protected internal override IEnumerable<CultureInfo> SupportedCultures => this.Options.Value.SupportedUICultures;

		#endregion
	}
}