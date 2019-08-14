using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using Company.WebApplication.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Company.WebApplication.Controllers
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public abstract class SiteController<TViewModel> : Controller where TViewModel : IViewModel
	{
		#region Methods

		protected internal abstract TViewModel CreateViewModel();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> DeleteCookie(Uri returnUrl)
		{
			this.Response.Cookies.Delete(CookieRequestCultureProvider.DefaultCookieName);

			return await this.Redirect(returnUrl).ConfigureAwait(false);
		}

		public virtual async Task<IActionResult> Index()
		{
			return this.View(this.CreateViewModel());
		}

		[SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings")]
		protected internal virtual async Task<IActionResult> Redirect(Uri returnUrl)
		{
			if(returnUrl == null || returnUrl.IsAbsoluteUri)
				return this.Redirect("/");

			return this.Redirect(returnUrl.ToString());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> SaveCookie(CultureInfo culture, Uri returnUrl, CultureInfo uiCulture)
		{
			this.HttpContext.Response.Cookies.Append(
				CookieRequestCultureProvider.DefaultCookieName,
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture, uiCulture)),
				new CookieOptions {MaxAge = TimeSpan.FromDays(365)}
			);

			return await this.Redirect(returnUrl).ConfigureAwait(false);
		}

		#endregion
	}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}