using System.Globalization;
using Company.WebApplication.Models.Navigation;
using RegionOrebroLan.Security.Cryptography;

namespace Company.WebApplication.Models.ViewModels.Shared
{
	public interface ILayout
	{
		#region Properties

		ICertificate Certificate { get; }
		CultureInfo Culture { get; }
		string CultureCookieName { get; }
		string CultureCookieValue { get; }
		INavigationNode CultureNavigation { get; }
		INavigationNode MainNavigation { get; }
		CultureInfo UiCulture { get; }
		INavigationNode UiCultureNavigation { get; }

		#endregion
	}
}