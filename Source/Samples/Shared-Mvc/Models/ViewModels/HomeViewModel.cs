using System;
using Microsoft.AspNetCore.Http;

namespace Company.WebApplication.Models.ViewModels
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
				if(this._now == null)
					this._now = new Lazy<DateTime>(() => DateTime.Now);

				return this._now.Value;
			}
		}

		#endregion
	}
}