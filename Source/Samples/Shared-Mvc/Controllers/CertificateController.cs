using Company.WebApplication.Models.ViewModels;

namespace Company.WebApplication.Controllers
{
	public class CertificateController : SiteController<CertificateViewModel>
	{
		#region Methods

		protected internal override CertificateViewModel CreateViewModel()
		{
			return new CertificateViewModel(this.HttpContext);
		}

		#endregion
	}
}