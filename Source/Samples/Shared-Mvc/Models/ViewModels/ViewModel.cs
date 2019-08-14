using System;
using Company.WebApplication.Localization.Extensions;
using Company.WebApplication.Models.ViewModels.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Company.WebApplication.Models.ViewModels
{
	public abstract class ViewModel : IViewModel
	{
		#region Fields

		private ILayout _layout;

		#endregion

		#region Constructors

		protected ViewModel(HttpContext httpContext)
		{
			this.HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));

			var localizerFactory = httpContext.RequestServices.GetRequiredService<IStringLocalizerFactory>();
			this.Localizer = localizerFactory.Create(this.GetType().FullName);
			this.LocalizerFactory = localizerFactory;

			this.LoggerFactory = httpContext.RequestServices.GetRequiredService<ILoggerFactory>();
		}

		#endregion

		#region Properties

		protected internal virtual HttpContext HttpContext { get; }
		public virtual ILayout Layout => this._layout ?? (this._layout = new Layout(this.HttpContext, this.LocalizerFactory, this.LoggerFactory));
		public virtual IStringLocalizer Localizer { get; }
		protected internal virtual IStringLocalizerFactory LocalizerFactory { get; }
		protected internal virtual ILoggerFactory LoggerFactory { get; }

		#endregion
	}
}