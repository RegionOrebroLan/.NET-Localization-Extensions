using Microsoft.AspNetCore.Http;

namespace Application.Models.ViewModels
{
	public class CertificateViewModel(HttpContext httpContext) : ViewModel(httpContext), ICertificateViewModel { }
}