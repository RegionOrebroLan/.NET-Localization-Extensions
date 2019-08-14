using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegionOrebroLan.Globalization;

namespace RegionOrebroLan.DependencyInjection.Extensions
{
	public static class ServiceCollectionExtension
	{
		#region Methods

		public static IServiceCollection ConfigureRequestLocalization(this IServiceCollection services, IConfiguration configuration)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			var cultureFactory = services.BuildServiceProvider().GetRequiredService<ICultureFactory>();

			services.Configure<RequestLocalizationOptions>(options =>
			{
				var requestLocalizationOptions = CreateRequestLocalizationOptions();

				var requestLocalizationSection = configuration.GetSection("RequestLocalization");

				requestLocalizationSection.Bind(requestLocalizationOptions);

				var defaultRequestCultureSection = requestLocalizationSection.GetSection("DefaultRequestCulture");
				var defaultRequestCulture = cultureFactory.Create(defaultRequestCultureSection.GetSection("Culture").Get<string>(), true);
				var defaultRequestUiCulture = cultureFactory.Create(defaultRequestCultureSection.GetSection("UiCulture").Get<string>(), true);

				options.DefaultRequestCulture = new RequestCulture(defaultRequestCulture, defaultRequestUiCulture);
				options.FallBackToParentCultures = requestLocalizationOptions.FallBackToParentCultures;
				options.FallBackToParentUICultures = requestLocalizationOptions.FallBackToParentUICultures;

				options.RequestCultureProviders.Clear();

				foreach(var item in requestLocalizationSection.GetSection("RequestCultureProviders").Get<IEnumerable<string>>())
				{
					var type = Type.GetType(item, true, true);

					options.RequestCultureProviders.Add((IRequestCultureProvider) Activator.CreateInstance(type));
				}

				options.SupportedCultures = requestLocalizationSection.GetSection("SupportedCultures").Get<IEnumerable<string>>().Select(item => cultureFactory.Create(item, true)).ToList();
				options.SupportedUICultures = requestLocalizationSection.GetSection("SupportedUiCultures").Get<IEnumerable<string>>().Select(item => cultureFactory.Create(item, true)).ToList();
			});

			return services;
		}

		private static RequestLocalizationOptions CreateRequestLocalizationOptions()
		{
			var requestLocalizationOptions = new RequestLocalizationOptions();

			requestLocalizationOptions.RequestCultureProviders.Clear();
			requestLocalizationOptions.SupportedCultures.Clear();
			requestLocalizationOptions.SupportedUICultures.Clear();

			return requestLocalizationOptions;
		}

		#endregion
	}
}