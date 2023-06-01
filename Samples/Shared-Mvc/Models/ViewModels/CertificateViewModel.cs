using Microsoft.AspNetCore.Http;

namespace Application.Models.ViewModels
{
	public class CertificateViewModel : ViewModel, ICertificateViewModel
	{
		#region Constructors

		public CertificateViewModel(HttpContext httpContext) : base(httpContext) { }

		#endregion
	}
}