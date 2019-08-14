using System;

namespace Company.WebApplication.Models.ViewModels
{
	public interface IHomeViewModel : IViewModel
	{
		#region Properties

		DateTime Now { get; }

		#endregion
	}
}