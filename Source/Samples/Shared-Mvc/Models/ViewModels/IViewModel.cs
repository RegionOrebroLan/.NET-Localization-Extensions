using Company.WebApplication.Models.ViewModels.Shared;
using Microsoft.Extensions.Localization;

namespace Company.WebApplication.Models.ViewModels
{
	public interface IViewModel
	{
		#region Properties

		ILayout Layout { get; }
		IStringLocalizer Localizer { get; }

		#endregion
	}
}