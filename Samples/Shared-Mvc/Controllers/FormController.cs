using Application.Models.Forms;
using Application.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public class FormController : SiteController<FormViewModel>
	{
		#region Methods

		protected internal override FormViewModel CreateViewModel()
		{
			return new FormViewModel(this.HttpContext);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Index(Form form)
		{
			var model = this.CreateViewModel();

			if(this.ModelState.IsValid)
			{
				model.SuccessfullyPosted = true;
				this.ModelState.Clear();
			}
			else
			{
				model.Form = form;
			}

			return this.View(model);
		}

		#endregion
	}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}