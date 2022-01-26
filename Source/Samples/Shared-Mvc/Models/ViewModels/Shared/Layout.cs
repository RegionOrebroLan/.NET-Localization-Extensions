using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Application.Localization.Extensions;
using Application.Models.Navigation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Security.Cryptography;
using Shared.Web.Routing;

namespace Application.Models.ViewModels.Shared
{
	// ReSharper disable InvertIf
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
	public class Layout : ILayout
	{
		#region Fields

		private Lazy<ICertificate> _certificate;
		private const string _certificateController = "Certificate";
		private Lazy<string> _controllerSegment;
		private Lazy<string> _cultureCookieValue;
		private INavigationNode _cultureNavigation;
		private Lazy<string> _cultureSegment;
		private const string _homeController = "Home";
		private INavigationNode _mainNavigation;
		private static readonly IEnumerable<string> _navigationControllers = new[] { "Localization", "Information", "Form", _certificateController };
		private IEnumerable<string> _pathSegments;
		private IEnumerable<string> _pathSegmentsWithoutController;
		private IEnumerable<string> _pathSegmentsWithoutControllerAndCultureAndUiCulture;
		private const char _pathSeparator = '/';
		private Lazy<IRequestCultureFeature> _requestCultureFeature;
		private Lazy<RequestLocalizationOptions> _requestLocalizationOptions;
		private INavigationNode _uiCultureNavigation;
		private Lazy<string> _uiCultureSegment;

		#endregion

		#region Constructors

		public Layout(HttpContext httpContext, IStringLocalizerFactory localizerFactory, ILoggerFactory loggerFactory)
		{
			this.HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
			this.Localizer = (localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory))).Create(this.GetType().FullName);
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
		}

		#endregion

		#region Properties

		public virtual ICertificate Certificate
		{
			get
			{
				this._certificate ??= new Lazy<ICertificate>(() => (X509Certificate2Wrapper)this.HttpContext.Connection.ClientCertificate);

				return this._certificate.Value;
			}
		}

		protected internal virtual string ControllerSegment
		{
			get
			{
				this._controllerSegment ??= new Lazy<string>(() =>
				{
					if(this.HttpContext.GetRouteValue(RouteKeys.Controller) is string controller)
						return this.PathSegments.FirstOrDefault(segment => controller.Equals(segment, StringComparison.OrdinalIgnoreCase));

					return null;
				});

				return this._controllerSegment.Value;
			}
		}

		public virtual CultureInfo Culture => CultureInfo.CurrentCulture;
		public virtual string CultureCookieName => CookieRequestCultureProvider.DefaultCookieName;

		public virtual string CultureCookieValue
		{
			get
			{
				this._cultureCookieValue ??= new Lazy<string>(() => this.HttpContext.Request.Cookies.TryGetValue(this.CultureCookieName, out var value) ? value : null);

				return this._cultureCookieValue.Value;
			}
		}

		public virtual INavigationNode CultureNavigation => this._cultureNavigation ??= this.GetCultureNavigationInternal(() => this.Culture, () => this.CultureSegment, "Culture", "ui-culture", () => this.RequestLocalizationOptions.SupportedCultures, this.GetCultureUrl);

		protected internal virtual string CultureSegment
		{
			get
			{
				this._cultureSegment ??= new Lazy<string>(() =>
				{
					if(this.HttpContext.GetRouteValue(RouteKeys.Culture) is string culture)
					{
						var potentialCultureSegment = this.PathSegmentsWithoutController.FirstOrDefault();

						if(potentialCultureSegment != null && string.Equals(culture, potentialCultureSegment, StringComparison.OrdinalIgnoreCase))
							return potentialCultureSegment;
					}

					return null;
				});

				return this._cultureSegment.Value;
			}
		}

		protected internal virtual HttpContext HttpContext { get; }
		protected internal virtual IStringLocalizer Localizer { get; }
		protected internal virtual ILogger Logger { get; }

		public virtual INavigationNode MainNavigation
		{
			get
			{
				if(this._mainNavigation == null)
				{
					var mainNavigation = new NavigationNode(null)
					{
						Active = this.GetNavigationActive(_homeController),
						Text = this.Localizer.GetString(_homeController),
						Tooltip = this.GetNavigationTooltip(_homeController),
						Url = this.GetNavigationUrl(_homeController)
					};

					foreach(var item in _navigationControllers)
					{
						mainNavigation.Children.Add(new NavigationNode(mainNavigation)
						{
							Active = this.GetNavigationActive(item),
							Text = this.Localizer.GetString(item),
							Tooltip = this.GetNavigationTooltip(item),
							Url = this.GetNavigationUrl(item)
						});
					}

					this._mainNavigation = mainNavigation;
				}

				return this._mainNavigation;
			}
		}

		protected internal virtual IEnumerable<string> PathSegments => this._pathSegments ??= this.HttpContext.Request.Path.Value.Split(new[] { _pathSeparator }, StringSplitOptions.RemoveEmptyEntries);

		protected internal virtual IEnumerable<string> PathSegmentsWithoutController
		{
			get
			{
				if(this._pathSegmentsWithoutController == null)
				{
					var pathSegments = this.PathSegments.ToList();

					if(this.HttpContext.GetRouteValue(RouteKeys.Controller) is string controller)
					{
						for(var i = 0; i < pathSegments.Count; i++)
						{
							if(string.Equals(controller, pathSegments[i], StringComparison.OrdinalIgnoreCase))
							{
								pathSegments.RemoveAt(i);
								break;
							}
						}
					}

					this._pathSegmentsWithoutController = pathSegments.ToArray();
				}

				return this._pathSegmentsWithoutController;
			}
		}

		protected internal virtual IEnumerable<string> PathSegmentsWithoutControllerAndCultureAndUiCulture
		{
			get
			{
				if(this._pathSegmentsWithoutControllerAndCultureAndUiCulture == null)
				{
					var pathSegments = this.PathSegmentsWithoutController.ToList();

					if(this.CultureSegment != null && pathSegments.Any())
						pathSegments.RemoveAt(0);

					if(this.UiCultureSegment != null && pathSegments.Any())
						pathSegments.RemoveAt(0);

					this._pathSegmentsWithoutControllerAndCultureAndUiCulture = pathSegments.ToArray();
				}

				return this._pathSegmentsWithoutControllerAndCultureAndUiCulture;
			}
		}

		protected internal virtual IRequestCultureFeature RequestCultureFeature
		{
			get
			{
				this._requestCultureFeature ??= new Lazy<IRequestCultureFeature>(() => this.HttpContext.Features.Get<IRequestCultureFeature>());

				return this._requestCultureFeature.Value;
			}
		}

		protected internal virtual RequestLocalizationOptions RequestLocalizationOptions
		{
			get
			{
				this._requestLocalizationOptions ??= new Lazy<RequestLocalizationOptions>(() => this.HttpContext.RequestServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

				return this._requestLocalizationOptions.Value;
			}
		}

		public virtual CultureInfo UiCulture => CultureInfo.CurrentUICulture;
		public virtual INavigationNode UiCultureNavigation => this._uiCultureNavigation ??= this.GetCultureNavigationInternal(() => this.UiCulture, () => this.UiCultureSegment, "UI-culture", "culture", () => this.RequestLocalizationOptions.SupportedUICultures, this.GetUiCultureUrl);

		protected internal virtual string UiCultureSegment
		{
			get
			{
				this._uiCultureSegment ??= new Lazy<string>(() =>
				{
					if(this.HttpContext.GetRouteValue(RouteKeys.UiCulture) is string uiCulture)
					{
						var potentialUiCultureSegment = this.CultureSegment != null ? this.PathSegmentsWithoutController.Count() > 1 ? this.PathSegmentsWithoutController.ElementAt(1) : null : this.PathSegmentsWithoutController.FirstOrDefault();

						if(potentialUiCultureSegment != null && string.Equals(uiCulture, potentialUiCultureSegment, StringComparison.OrdinalIgnoreCase))
							return potentialUiCultureSegment;
					}

					return null;
				});

				return this._uiCultureSegment.Value;
			}
		}

		#endregion

		#region Methods

		protected internal virtual INavigationNode GetCultureNavigationInternal(Func<CultureInfo> cultureFunction, Func<string> cultureSegmentFunction, string label, string otherRouteKey, Func<IEnumerable<CultureInfo>> supportedCulturesFunction, Func<CultureInfo, Uri> urlFunction)
		{
			if(cultureFunction == null)
				throw new ArgumentNullException(nameof(cultureFunction));

			if(cultureSegmentFunction == null)
				throw new ArgumentNullException(nameof(cultureSegmentFunction));

			if(supportedCulturesFunction == null)
				throw new ArgumentNullException(nameof(supportedCulturesFunction));

			if(urlFunction == null)
				throw new ArgumentNullException(nameof(urlFunction));

			var culture = cultureFunction();
			var cultureSegment = cultureSegmentFunction();

			var cultureNavigation = new NavigationNode(null)
			{
				Active = culture != null,
				Text = this.GetCultureNavigationText(culture, label),
				Tooltip = this.GetCultureNavigationTooltip(cultureSegment, label, otherRouteKey)
			};

			if(cultureSegment != null)
			{
				cultureNavigation.Children.Add(new NavigationNode(cultureNavigation)
				{
					Text = this.Localizer.GetString("- Clear -"),
					Tooltip = this.Localizer.GetString("Clear the url-selected culture."),
					Url = urlFunction(null)
				});
			}

			foreach(var supportedCulture in supportedCulturesFunction().OrderBy(item => item.NativeName, StringComparer.Ordinal))
			{
				cultureNavigation.Children.Add(new NavigationNode(cultureNavigation)
				{
					Active = cultureSegment != null && supportedCulture.Equals(culture),
					Text = supportedCulture.NativeName,
					Tooltip = this.Localizer.GetString("Select culture {0}.", supportedCulture.Name),
					Url = urlFunction(supportedCulture)
				});
			}

			return cultureNavigation;
		}

		protected internal virtual string GetCultureNavigationText(CultureInfo culture, string label)
		{
			var hint = this.GetRequestCultureProviderHint(label, this.RequestCultureFeature.Provider);

			return this.Localizer.GetString(label + " from {0}: {1}", hint, culture?.NativeName);
		}

		[SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
		protected internal virtual string GetCultureNavigationTooltip(string cultureSegment, string label, string otherRouteKey)
		{
			string informationArgument = null;

			switch(this.RequestCultureFeature.Provider)
			{
				case null:
					informationArgument = this.Localizer.GetString("the default settings");
					break;
				case AcceptLanguageHeaderRequestCultureProvider:
					informationArgument = this.Localizer.GetString("the request-header (browser-settings)");
					break;
				case CookieRequestCultureProvider:
					informationArgument = this.Localizer.GetString("a cookie");
					break;
				case QueryStringRequestCultureProvider:
					informationArgument = this.Localizer.GetString("the query-string") + (this.HttpContext.Request.Query.Keys.Contains(label, StringComparer.OrdinalIgnoreCase) ? null : this.Localizer.GetString("or the default settings"));
					break;
				case RouteDataRequestCultureProvider:
					informationArgument = (cultureSegment == null ? this.Localizer.GetString("the " + otherRouteKey + "-route-key of") + " " : null) + this.Localizer.GetString("the url");
					break;
				default:
					break;
			}

			var labelAsLowerCase = label?.ToLowerInvariant();
			string tooltip = this.Localizer.GetString("Select a " + labelAsLowerCase + ".");

			if(informationArgument != null)
				tooltip += " " + this.Localizer.GetString("The current " + labelAsLowerCase + " is determined by {0}.", informationArgument);

			return tooltip;
		}

		protected internal virtual Uri GetCultureUrl(CultureInfo culture)
		{
			var segments = new List<string>();

			if(culture != null)
				segments.Add(culture.Name);

			if(this.UiCultureSegment != null)
				segments.Add(this.UiCultureSegment);

			return this.GetCultureUrl(segments);
		}

		protected internal virtual Uri GetCultureUrl(IEnumerable<string> cultureSegments)
		{
			var segments = new List<string>(cultureSegments ?? Enumerable.Empty<string>());

			if(this.ControllerSegment != null)
			{
				if(string.Equals(this.ControllerSegment, _certificateController, StringComparison.OrdinalIgnoreCase))
					segments.Insert(0, this.ControllerSegment);
				else if(!string.Equals(this.ControllerSegment, _homeController, StringComparison.OrdinalIgnoreCase))
					segments.Add(this.ControllerSegment);
			}

			segments.AddRange(this.PathSegmentsWithoutControllerAndCultureAndUiCulture);

			return new Uri(_pathSeparator + (segments.Any() ? string.Join(_pathSeparator.ToString(CultureInfo.InvariantCulture), segments) + _pathSeparator : string.Empty), UriKind.Relative);
		}

		protected internal virtual bool GetNavigationActive(string controller)
		{
			if(string.IsNullOrWhiteSpace(controller))
				return false;

			if(controller.Equals(_homeController, StringComparison.OrdinalIgnoreCase))
				return !this.PathSegments.Intersect(_navigationControllers, StringComparer.OrdinalIgnoreCase).Any();

			return this.PathSegments.Contains(controller, StringComparer.OrdinalIgnoreCase);
		}

		[SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
		protected internal virtual string GetNavigationTooltip(string controller)
		{
			var page = this.Localizer.GetString("the " + (controller ?? string.Empty).ToLowerInvariant() + "-page");
			return this.Localizer.GetString("Navigate to {0}.", page);
		}

		protected internal virtual Uri GetNavigationUrl(string controller)
		{
			var segments = new List<string>();

			if(this.CultureSegment != null)
				segments.Add(this.CultureSegment);

			if(this.UiCultureSegment != null)
				segments.Add(this.UiCultureSegment);

			if(string.Equals(controller, _certificateController, StringComparison.OrdinalIgnoreCase))
				segments.Insert(0, controller);
			else if(!string.Equals(controller, _homeController, StringComparison.OrdinalIgnoreCase))
				segments.Add(controller);

			return new Uri(_pathSeparator + (segments.Any() ? string.Join(_pathSeparator.ToString(CultureInfo.InvariantCulture), segments) + _pathSeparator : string.Empty), UriKind.Relative);
		}

		protected internal virtual string GetRequestCultureProviderHint(string label, IRequestCultureProvider requestCultureProvider)
		{
			return requestCultureProvider switch
			{
				null => this.Localizer.GetString("default-settings"),
				AcceptLanguageHeaderRequestCultureProvider => this.Localizer.GetString("header"),
				CookieRequestCultureProvider => this.Localizer.GetString("cookie"),
				QueryStringRequestCultureProvider => this.Localizer.GetString("query-string") + (this.HttpContext.Request.Query.Keys.Contains(label, StringComparer.OrdinalIgnoreCase) ? null : "/" + this.Localizer.GetString("default-settings")),
				RouteDataRequestCultureProvider => this.Localizer.GetString("url"),
				_ => null,
			};
		}

		protected internal virtual Uri GetUiCultureUrl(CultureInfo uiCulture)
		{
			var segments = new List<string>();

			if(this.CultureSegment != null)
				segments.Add(this.CultureSegment);

			if(uiCulture != null)
				segments.Add(uiCulture.Name);

			return this.GetCultureUrl(segments);
		}

		#endregion
	}
	// ReSharper restore InvertIf
}