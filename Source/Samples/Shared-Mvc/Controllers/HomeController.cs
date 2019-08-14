using Company.WebApplication.Models.ViewModels;

namespace Company.WebApplication.Controllers
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