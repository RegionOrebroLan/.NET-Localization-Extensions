﻿using Company.WebApplication.Models.Forms;
using Microsoft.AspNetCore.Http;

namespace Company.WebApplication.Models.ViewModels
{
	public class FormViewModel : ViewModel, IFormViewModel
	{
		#region Fields

		private Form _form;

		#endregion

		#region Constructors

		public FormViewModel(HttpContext httpContext) : base(httpContext) { }

		#endregion

		#region Properties

		public virtual Form Form
		{
			get => this._form ?? (this._form = new Form());
			set => this._form = value;
		}

		public virtual bool SuccessfullyPosted { get; set; }

		#endregion
	}
}