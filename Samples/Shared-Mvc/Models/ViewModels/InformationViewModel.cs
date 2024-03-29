using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application.Models.ViewModels
{
	public class InformationViewModel(HttpContext httpContext) : ViewModel(httpContext), IInformationViewModel
	{
		#region Fields

		private Lazy<IRequestCultureFeature> _requestCultureFeature;
		private Lazy<RequestLocalizationOptions> _requestLocalizationOptions;
		private IDictionary<string, object> _routeDictionary;

		#endregion

		#region Properties

		public virtual IRequestCultureFeature RequestCultureFeature
		{
			get
			{
				this._requestCultureFeature ??= new Lazy<IRequestCultureFeature>(this.HttpContext.Features.Get<IRequestCultureFeature>);

				return this._requestCultureFeature.Value;
			}
		}

		public virtual RequestLocalizationOptions RequestLocalizationOptions
		{
			get
			{
				this._requestLocalizationOptions ??= new Lazy<RequestLocalizationOptions>(() => this.HttpContext.RequestServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

				return this._requestLocalizationOptions.Value;
			}
		}

		public virtual IDictionary<string, object> RouteDictionary
		{
			get
			{
				if(this._routeDictionary == null)
				{
					var routeValueDictionary = this.HttpContext.GetRouteData().Values;

					this._routeDictionary = routeValueDictionary.Keys.OrderBy(key => key).ToDictionary(key => key, key => routeValueDictionary[key]);
				}

				return this._routeDictionary;
			}
		}

		#endregion
	}
}