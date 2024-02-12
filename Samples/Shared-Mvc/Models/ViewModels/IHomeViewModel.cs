namespace Application.Models.ViewModels
{
	public interface IHomeViewModel : IViewModel
	{
		#region Properties

		DateTime Now { get; }

		#endregion
	}
}