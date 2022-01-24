using Application.Models.ViewModels;

namespace Application.Controllers
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