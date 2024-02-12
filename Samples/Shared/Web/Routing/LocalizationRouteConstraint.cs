using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Shared.Web.Routing
{
	public abstract class LocalizationRouteConstraint : IRouteConstraint
	{
		#region Constructors

		protected LocalizationRouteConstraint(IOptions<RequestLocalizationOptions> options, string routeKey)
		{
			this.Options = options ?? throw new ArgumentNullException(nameof(options));
			this.RouteKey = routeKey;
		}

		#endregion

		#region Properties

		protected internal virtual IOptions<RequestLocalizationOptions> Options { get; }
		protected internal virtual string RouteKey { get; }
		protected internal abstract IEnumerable<CultureInfo> SupportedCultures { get; }

		#endregion

		#region Methods

		public virtual bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
		{
			// ReSharper disable ConvertIfStatementToReturnStatement
			if(!string.Equals(this.RouteKey, routeKey, StringComparison.OrdinalIgnoreCase))
				return false;
			// ReSharper restore ConvertIfStatementToReturnStatement

			return this.SupportedCultures.Any(culture => culture.Name.Equals(values[routeKey] as string, StringComparison.OrdinalIgnoreCase));
		}

		#endregion
	}
}