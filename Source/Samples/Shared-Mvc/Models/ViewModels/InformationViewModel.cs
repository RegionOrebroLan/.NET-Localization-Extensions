using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Company.WebApplication.Models.ViewModels
{
	public class InformationViewModel : ViewModel, IInformationViewModel
	{
		#region Fields

		private Lazy<IRequestCultureFeature> _requestCultureFeature;
		private Lazy<RequestLocalizationOptions> _requestLocalizationOptions;
		private IDictionary<string, object> _routeDictionary;

		#endregion

		#region Constructors

		public InformationViewModel(HttpContext httpContext) : base(httpContext) { }

		#endregion

		#region Properties

		public virtual IRequestCultureFeature RequestCultureFeature
		{
			get
			{
				if(this._requestCultureFeature == null)
					this._requestCultureFeature = new Lazy<IRequestCultureFeature>(() => this.HttpContext.Features.Get<IRequestCultureFeature>());

				return this._requestCultureFeature.Value;
			}
		}

		public virtual RequestLocalizationOptions RequestLocalizationOptions
		{
			get
			{
				if(this._requestLocalizationOptions == null)
					this._requestLocalizationOptions = new Lazy<RequestLocalizationOptions>(() => this.HttpContext.RequestServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

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