using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Shared.Web.Routing;

namespace Shared.Web.Mvc.Filters
{
	public class LocalizationFilter : IResourceFilter
	{
		#region Constructors

		public LocalizationFilter(IOptions<RequestLocalizationOptions> options)
		{
			this.Options = options ?? throw new ArgumentNullException(nameof(options));
		}

		#endregion

		#region Properties

		protected internal virtual IOptions<RequestLocalizationOptions> Options { get; }

		#endregion

		#region Methods

		protected internal virtual CultureInfo GetCulture(string name, IEnumerable<CultureInfo> supportedCultures)
		{
			return (supportedCultures ?? Enumerable.Empty<CultureInfo>()).FirstOrDefault(culture => string.Equals(culture.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		protected internal virtual CultureInfo GetMatchingCulture(CultureInfo uiCulture)
		{
			CultureInfo culture = null;

			// ReSharper disable InvertIf
			if(uiCulture != null)
			{
				var cultures = this.Options.Value.SupportedCultures;

				culture = this.GetMatchingCultureByDescendants(cultures, uiCulture) ?? this.GetMatchingCultureByAncestors(cultures, uiCulture);
			}
			// ReSharper restore InvertIf

			return culture;
		}

		protected internal virtual CultureInfo GetMatchingCultureByAncestors(IEnumerable<CultureInfo> cultures, CultureInfo cultureToMatch)
		{
			cultures = (cultures ?? Enumerable.Empty<CultureInfo>()).ToArray();

			var culture = cultures.FirstOrDefault(item => item.Equals(cultureToMatch));

			if(culture == null && cultureToMatch != null && !cultureToMatch.Equals(cultureToMatch.Parent))
				culture = this.GetMatchingCultureByAncestors(cultures, cultureToMatch.Parent);

			return culture;
		}

		protected internal virtual CultureInfo GetMatchingCultureByDescendants(IEnumerable<CultureInfo> cultures, CultureInfo cultureToMatch)
		{
			return this.GetMatchingCultureByDescendants((cultures ?? Enumerable.Empty<CultureInfo>()).Distinct().ToDictionary(culture => culture, culture => culture), cultureToMatch);
		}

		protected internal virtual CultureInfo GetMatchingCultureByDescendants(IDictionary<CultureInfo, CultureInfo> cultureMap, CultureInfo cultureToMatch)
		{
			CultureInfo culture = null;

			// ReSharper disable InvertIf
			if(cultureToMatch != null)
			{
				cultureMap ??= new Dictionary<CultureInfo, CultureInfo>();

				foreach(var mapping in cultureMap)
				{
					if(cultureToMatch.Equals(mapping.Value))
					{
						culture = mapping.Key;
						break;
					}
				}

				if(culture == null)
				{
					var map = new Dictionary<CultureInfo, CultureInfo>();

					// ReSharper disable LoopCanBeConvertedToQuery
					foreach(var mapping in cultureMap)
					{
						if(mapping.Value.Equals(mapping.Value.Parent))
							continue;

						map.Add(mapping.Key, mapping.Value.Parent);
					}
					// ReSharper restore LoopCanBeConvertedToQuery

					culture = this.GetMatchingCultureByDescendants(map, cultureToMatch);
				}
			}
			// ReSharper restore InvertIf

			return culture;
		}

		protected internal virtual CultureInfo GetMatchingUiCulture(CultureInfo culture)
		{
			CultureInfo uiCulture = null;

			// ReSharper disable InvertIf
			if(culture != null)
			{
				var cultures = this.Options.Value.SupportedUICultures;

				uiCulture = this.GetMatchingCultureByAncestors(cultures, culture) ?? this.GetMatchingCultureByDescendants(cultures, culture);
			}
			// ReSharper restore InvertIf

			return uiCulture;
		}

		public virtual void OnResourceExecuted(ResourceExecutedContext context) { }

		public virtual void OnResourceExecuting(ResourceExecutingContext context)
		{
			if(context == null)
				throw new ArgumentNullException(nameof(context));

			// ReSharper disable InvertIf
			if(this.TryGetRouteValue(context, RouteKeys.Culture, out var cultureName))
			{
				if(!context.RouteData.Values.ContainsKey(RouteKeys.UiCulture))
				{
					var culture = this.GetCulture(cultureName, this.Options.Value.SupportedCultures);

					if(culture != null)
					{
						var uiCulture = this.GetMatchingUiCulture(culture);

						if(uiCulture != null)
							context.RouteData.Values.Add(RouteKeys.UiCulture, uiCulture.Name);
					}
				}
			}
			else if(this.TryGetRouteValue(context, RouteKeys.UiCulture, out var uiCultureName))
			{
				var uiCulture = this.GetCulture(uiCultureName, this.Options.Value.SupportedUICultures);

				if(uiCulture != null)
				{
					var culture = this.GetMatchingCulture(uiCulture);

					if(culture != null)
						context.RouteData.Values.Add(RouteKeys.Culture, culture.Name);
				}
			}
			// ReSharper restore InvertIf
		}

		protected internal virtual bool TryGetRouteValue(ActionContext context, string key, out string value)
		{
			value = null;

			var routeValues = context?.RouteData?.Values;

			// ReSharper disable InvertIf
			if(routeValues != null && routeValues.TryGetValue(key, out var routeValue) && routeValue is string routeValueAsString)
			{
				value = routeValueAsString;
				return true;
			}
			// ReSharper restore InvertIf

			return false;
		}

		#endregion
	}
}