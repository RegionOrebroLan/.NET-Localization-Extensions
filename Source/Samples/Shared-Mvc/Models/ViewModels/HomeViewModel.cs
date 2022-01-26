using System;
using Microsoft.AspNetCore.Http;

namespace Application.Models.ViewModels
{
	public class HomeViewModel : ViewModel, IHomeViewModel
	{
		#region Fields

		private Lazy<DateTime> _now;

		#endregion

		#region Constructors

		public HomeViewModel(HttpContext httpContext) : base(httpContext) { }

		#endregion

		#region Properties

		public virtual DateTime Now
		{
			get
			{
				this._now ??= new Lazy<DateTime>(() => DateTime.Now);

				return this._now.Value;
			}
		}

		#endregion
	}
}