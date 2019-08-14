using System;
using System.Collections.Generic;
using Company.WebApplication.Models.Navigation;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace Company.WebApplication.Models.ViewModels
{
	public interface ILocalizationViewModel : IViewModel
	{
		#region Properties

		INavigationNode Localizers { get; }
		IDictionary<string, string> Names { get; }
		IStringLocalizer SelectedLocalizer { get; }
		Exception SelectedLocalizerException { get; }

		#endregion

		#region Methods

		IDictionary<string, object> GetHtmlLocalizerInformation(IHtmlLocalizer htmlLocalizer);
		IDictionary<string, object> GetStringLocalizerInformation(IStringLocalizer stringLocalizer);

		#endregion
	}
}