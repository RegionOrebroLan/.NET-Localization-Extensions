using System;
using System.Collections.Generic;
using Application.DependencyInjection.Extensions;
using Application.Localization.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegionOrebroLan.Localization.DependencyInjection.Extensions;
using Shared.DependencyInjection.Extensions;
using Shared.Globalization;
using Shared.Globalization.Configuration;
using Shared.Web.Mvc.Configuration;
using Shared.Web.Mvc.Filters;
using Shared.Web.Routing;

namespace Application
{
	public class Startup
	{
		#region Constructors

		public Startup(IConfiguration configuration)
		{
			this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		#endregion

		#region Properties

		public virtual IConfiguration Configuration { get; }

		#endregion

		#region Methods

		public virtual void Configure(IApplicationBuilder applicationBuilder)
		{
			if(applicationBuilder == null)
				throw new ArgumentNullException(nameof(applicationBuilder));

			applicationBuilder.UseDeveloperExceptionPage();
			applicationBuilder.UseStaticFiles();
			applicationBuilder.UseRouting();
			applicationBuilder.UseRequestLocalization();
			applicationBuilder.UseEndpoints(routeBuilder =>
			{
				var certificateControllerParameter = new { controller = "Certificate" };
				var notCertificateControllerConstraints = new { controller = "((?!Certificate).)*" }; // Another expression could be: "Form|Home|Information|Localization"

				routeBuilder.MapControllerRoute("Default", "{controller=Home}/{action=Index}/{id?}");

				routeBuilder.MapControllerRoute("Default-Localized", "{culture:culture}/{ui-culture:ui-culture}/{controller=Home}/{action=Index}/{id?}", null, notCertificateControllerConstraints);
				routeBuilder.MapControllerRoute("Default-Localized-Culture", "{culture:culture}/{controller=Home}/{action=Index}/{id?}", null, notCertificateControllerConstraints);
				routeBuilder.MapControllerRoute("Default-Localized-UI-Culture", "{ui-culture:ui-culture}/{controller=Home}/{action=Index}/{id?}", null, notCertificateControllerConstraints);

				routeBuilder.MapControllerRoute("Certificate-Localized", "Certificate/{culture:culture}/{ui-culture:ui-culture}/{action=Index}/{id?}", certificateControllerParameter, certificateControllerParameter);
				routeBuilder.MapControllerRoute("Certificate-Localized-Culture", "Certificate/{culture:culture}/{action=Index}/{id?}", certificateControllerParameter, certificateControllerParameter);
				routeBuilder.MapControllerRoute("Certificate-Localized-UI-Culture", "Certificate/{ui-culture:ui-culture}/{action=Index}/{id?}", certificateControllerParameter, certificateControllerParameter);
			});
		}

		public virtual void ConfigureServices(IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.AddSingleton<ICultureFactory, CultureFactory>();
			services.AddSingleton(this.Configuration.GetSection("CustomCultures").Get<IDictionary<string, CustomCultureOptions>>());

			services.AddPathBasedLocalization(this.Configuration);

			services.ConfigureRequestLocalization(this.Configuration);

			services.Configure<RouteOptions>(routeOptions =>
			{
				routeOptions.ConstraintMap.Add(RouteKeys.Culture, typeof(CultureRouteConstraint));
				routeOptions.ConstraintMap.Add(RouteKeys.UiCulture, typeof(UiCultureRouteConstraint));
			});

			services.AddControllersWithViews(mvcOptions =>
					{
						mvcOptions.Filters.Add(typeof(LocalizationFilter));
						mvcOptions.Filters.Add(new MiddlewareFilterAttribute(typeof(RequestLocalizationMiddlewareConfigurator)));
					}
				)
				.AddDataAnnotationsLocalization(options =>
				{
					// This is needed only because we have our shared MVC-library. The name of that assembly is SharedMvc. If we did not have the code-snippet below the localizer would not be created correctly. It would have an incorrect path/resource-base-name.
					options.DataAnnotationLocalizerProvider = (type, localizerFactory) => type.Assembly.Equals(typeof(StringLocalizerFactoryExtension).Assembly) ? localizerFactory.Create(type.FullName) : localizerFactory.Create(type);
				})
				.AddViewLocalization();
			//.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			services.ConfigureExample(this.Configuration);
		}

		#endregion
	}
}