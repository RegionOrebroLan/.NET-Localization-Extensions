using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Shared.Web.Routing
{
	public class CultureRouteConstraint(IOptions<RequestLocalizationOptions> options) : LocalizationRouteConstraint(options, RouteKeys.Culture)
	{
		#region Properties

		protected internal override IEnumerable<CultureInfo> SupportedCultures => this.Options.Value.SupportedCultures;

		#endregion
	}
}