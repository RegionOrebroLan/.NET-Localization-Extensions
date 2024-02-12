using Application.Models.Forms;
using Microsoft.AspNetCore.Http;

namespace Application.Models.ViewModels
{
	public class FormViewModel(HttpContext httpContext) : ViewModel(httpContext), IFormViewModel
	{
		#region Fields

		private Form _form;

		#endregion

		#region Properties

		public virtual Form Form
		{
			get => this._form ??= new Form();
			set => this._form = value;
		}

		public virtual bool SuccessfullyPosted { get; set; }

		#endregion
	}
}