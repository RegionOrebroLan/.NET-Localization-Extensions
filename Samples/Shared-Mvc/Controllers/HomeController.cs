using Application.Models.ViewModels;

namespace Application.Controllers
{
	public class HomeController : SiteController<HomeViewModel>
	{
		#region Methods

		protected internal override HomeViewModel CreateViewModel()
		{
			return new HomeViewModel(this.HttpContext);
		}

		#endregion
	}
}