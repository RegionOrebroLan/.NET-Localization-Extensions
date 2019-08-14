using Company.WebApplication.Models.Forms;

namespace Company.WebApplication.Models.ViewModels
{
	public interface IFormViewModel : IViewModel
	{
		#region Properties

		Form Form { get; }
		bool SuccessfullyPosted { get; }

		#endregion
	}
}