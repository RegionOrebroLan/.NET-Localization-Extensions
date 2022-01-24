using Application.Models.ViewModels;

namespace Application.Controllers
{
	public class InformationController : SiteController<InformationViewModel>
	{
		#region Methods

		protected internal override InformationViewModel CreateViewModel()
		{
			return new InformationViewModel(this.HttpContext);
		}

		#endregion
	}
}