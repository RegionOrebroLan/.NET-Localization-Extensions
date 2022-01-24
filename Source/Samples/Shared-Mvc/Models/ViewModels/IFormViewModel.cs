using Application.Models.Forms;

namespace Application.Models.ViewModels
{
	public interface IFormViewModel : IViewModel
	{
		#region Properties

		Form Form { get; }
		bool SuccessfullyPosted { get; }

		#endregion
	}
}