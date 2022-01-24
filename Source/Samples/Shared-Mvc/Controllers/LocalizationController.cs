using Application.Models.ViewModels;

namespace Application.Controllers
{
	public class LocalizationController : SiteController<LocalizationViewModel>
	{
		#region Methods

		protected internal override LocalizationViewModel CreateViewModel()
		{
			return new LocalizationViewModel(this.HttpContext);
		}

		#endregion
	}
}