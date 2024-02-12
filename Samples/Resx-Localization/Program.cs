using Application.DependencyInjection.Extensions;
using Application.Localization.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.DependencyInjection.Extensions;
using Shared.Globalization;
using Shared.Globalization.Configuration;
using Shared.Web.Mvc.Configuration;
using Shared.Web.Mvc.Filters;
using Shared.Web.Routing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ICultureFactory, CultureFactory>();
builder.Services.AddSingleton(builder.Configuration.GetSection("CustomCultures").Get<IDictionary<string, CustomCultureOptions>>());

builder.Services.Configure<LocalizationOptions>(builder.Configuration.GetSection("Localization"));
builder.Services.AddLocalization();

builder.Services.ConfigureRequestLocalization(builder.Configuration);

builder.Services.Configure<RouteOptions>(routeOptions =>
{
	routeOptions.ConstraintMap.Add(RouteKeys.Culture, typeof(CultureRouteConstraint));
	routeOptions.ConstraintMap.Add(RouteKeys.UiCulture, typeof(UiCultureRouteConstraint));
});

builder.Services.AddControllersWithViews(mvcOptions =>
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

builder.Services.ConfigureExample(builder.Configuration);

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseRouting();
app.UseRequestLocalization();

var certificateControllerParameter = new { controller = "Certificate" };
var notCertificateControllerConstraints = new { controller = "((?!Certificate).)*" }; // Another expression could be: "Form|Home|Information|Localization"

app.MapControllerRoute("Default", "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute("Default-Localized", "{culture:culture}/{ui-culture:ui-culture}/{controller=Home}/{action=Index}/{id?}", null, notCertificateControllerConstraints);
app.MapControllerRoute("Default-Localized-Culture", "{culture:culture}/{controller=Home}/{action=Index}/{id?}", null, notCertificateControllerConstraints);
app.MapControllerRoute("Default-Localized-UI-Culture", "{ui-culture:ui-culture}/{controller=Home}/{action=Index}/{id?}", null, notCertificateControllerConstraints);

app.MapControllerRoute("Certificate-Localized", "Certificate/{culture:culture}/{ui-culture:ui-culture}/{action=Index}/{id?}", certificateControllerParameter, certificateControllerParameter);
app.MapControllerRoute("Certificate-Localized-Culture", "Certificate/{culture:culture}/{action=Index}/{id?}", certificateControllerParameter, certificateControllerParameter);
app.MapControllerRoute("Certificate-Localized-UI-Culture", "Certificate/{ui-culture:ui-culture}/{action=Index}/{id?}", certificateControllerParameter, certificateControllerParameter);

app.Run();