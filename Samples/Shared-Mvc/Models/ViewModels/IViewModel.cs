using Application.Models.ViewModels.Shared;
using Microsoft.Extensions.Localization;

namespace Application.Models.ViewModels
{
	public interface IViewModel
	{
		#region Properties

		ILayout Layout { get; }
		IStringLocalizer Localizer { get; }

		#endregion
	}
}