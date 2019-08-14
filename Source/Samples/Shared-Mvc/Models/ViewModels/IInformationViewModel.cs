using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace Company.WebApplication.Models.ViewModels
{
	public interface IInformationViewModel : IViewModel
	{
		#region Properties

		IRequestCultureFeature RequestCultureFeature { get; }
		RequestLocalizationOptions RequestLocalizationOptions { get; }
		IDictionary<string, object> RouteDictionary { get; }

		#endregion
	}
}