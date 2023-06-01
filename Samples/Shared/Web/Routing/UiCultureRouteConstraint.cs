using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Shared.Web.Routing
{
	public class UiCultureRouteConstraint : LocalizationRouteConstraint
	{
		#region Constructors

		public UiCultureRouteConstraint(IOptions<RequestLocalizationOptions> options) : base(options, RouteKeys.UiCulture) { }

		#endregion

		#region Properties

		protected internal override IEnumerable<CultureInfo> SupportedCultures => this.Options.Value.SupportedUICultures;

		#endregion
	}
}