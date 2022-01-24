using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Shared.Web.Routing
{
	public class CultureRouteConstraint : LocalizationRouteConstraint
	{
		#region Constructors

		public CultureRouteConstraint(IOptions<RequestLocalizationOptions> options) : base(options, RouteKeys.Culture) { }

		#endregion

		#region Properties

		protected internal override IEnumerable<CultureInfo> SupportedCultures => this.Options.Value.SupportedCultures;

		#endregion
	}
}