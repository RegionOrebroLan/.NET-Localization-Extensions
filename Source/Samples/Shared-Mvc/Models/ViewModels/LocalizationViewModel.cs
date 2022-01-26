using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Web;
using Application.Configuration;
using Application.Models.Navigation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using RegionOrebroLan.Localization;

namespace Application.Models.ViewModels
{
	public class LocalizationViewModel : ViewModel, ILocalizationViewModel
	{
		#region Fields

		private const string _localizerBaseNameParameterKey = "LocalizerBaseName";
		private const string _localizerLocationParameterKey = "LocalizerLocation";
		private INavigationNode _localizers;
		private IDictionary<string, string> _names;
		private static IEnumerable<FieldInfo> _resourceManagerStringLocalizerFields;
		private Lazy<Tuple<IStringLocalizer, Exception>> _selectedLocalizerInformation;
		private static IEnumerable<PropertyInfo> _stringLocalizerProperties;

		#endregion

		#region Constructors

		public LocalizationViewModel(HttpContext httpContext) : base(httpContext)
		{
			if(httpContext == null)
				throw new ArgumentNullException(nameof(httpContext));

			this.ExampleOptions = httpContext.RequestServices.GetRequiredService<IOptions<ExampleOptions>>();
		}

		#endregion

		#region Properties

		protected internal virtual IOptions<ExampleOptions> ExampleOptions { get; }
		protected internal virtual string LocalizerBaseName => this.HttpContext.Request.Query[this.LocalizerBaseNameParameterKey];
		protected internal virtual string LocalizerBaseNameParameterKey => _localizerBaseNameParameterKey;
		protected internal virtual string LocalizerLocation => this.HttpContext.Request.Query[this.LocalizerLocationParameterKey];
		protected internal virtual string LocalizerLocationParameterKey => _localizerLocationParameterKey;

		public virtual INavigationNode Localizers
		{
			get
			{
				if(this._localizers == null)
				{
					var localizers = new NavigationNode(null)
					{
						Active = this.LocalizerSelected,
						Text = this.LocalizerSelected ? $"{this.LocalizerBaseName}, {this.LocalizerLocation}" : this.Localizer["Select a localizer"],
						Tooltip = this.Localizer["Select a localizer"] + "."
					};

					var uriBuilder = new UriBuilder(this.HttpContext.Request.GetDisplayUrl());

					var query = HttpUtility.ParseQueryString(uriBuilder.Query);
					query.Remove(this.LocalizerBaseNameParameterKey);
					query.Remove(this.LocalizerLocationParameterKey);

					if(this.LocalizerSelected)
					{
						uriBuilder.Query = query.ToString();

						localizers.Children.Add(new NavigationNode(localizers)
						{
							Text = " " + this.Localizer["- Clear -"] + " ",
							Tooltip = this.Localizer["Clear the selected localizer."],
							Url = new Uri(uriBuilder.Uri.PathAndQuery, UriKind.Relative)
						});
					}

					foreach(var item in this.ExampleOptions.Value.Localizers)
					{
						var localizerBaseName = item.Key ?? string.Empty;
						var localizerLocation = item.Value ?? string.Empty;

						query[this.LocalizerBaseNameParameterKey] = localizerBaseName;
						query[this.LocalizerLocationParameterKey] = localizerLocation;

						uriBuilder.Query = query.ToString();

						var text = $"{localizerBaseName}, {localizerLocation}";

						localizers.Children.Add(new NavigationNode(localizers)
						{
							Active = localizerBaseName.Equals(this.LocalizerBaseName, StringComparison.OrdinalIgnoreCase) && localizerLocation.Equals(this.LocalizerLocation, StringComparison.OrdinalIgnoreCase),
							Text = text,
							Tooltip = this.Localizer["Select localizer \"{0}\".", text],
							Url = new Uri(uriBuilder.Uri.PathAndQuery, UriKind.Relative)
						});
					}

					this._localizers = localizers;
				}

				return this._localizers;
			}
		}

		protected internal virtual bool LocalizerSelected => this.LocalizerBaseName != null || this.LocalizerLocation != null;
		public virtual IDictionary<string, string> Names => this._names ?? (this._names = this.ExampleOptions.Value.Names.ToDictionary(item => item.Key.Replace('~', ':'), item => item.Value));
		protected internal virtual IEnumerable<FieldInfo> ResourceManagerStringLocalizerFields => _resourceManagerStringLocalizerFields ?? (_resourceManagerStringLocalizerFields = typeof(ResourceManagerStringLocalizer).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).OrderBy(field => field.Name).ToArray());
		public virtual IStringLocalizer SelectedLocalizer => this.SelectedLocalizerInformation?.Item1;
		public virtual Exception SelectedLocalizerException => this.SelectedLocalizerInformation?.Item2;

		[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
		protected internal virtual Tuple<IStringLocalizer, Exception> SelectedLocalizerInformation
		{
			get
			{
				if(this._selectedLocalizerInformation == null)
				{
					this._selectedLocalizerInformation = new Lazy<Tuple<IStringLocalizer, Exception>>(() =>
					{
						if(!this.LocalizerSelected)
							return null;

						try
						{
							var localizer = this.LocalizerFactory.Create(this.LocalizerBaseName, this.LocalizerLocation);

							try
							{
								var _ = localizer.GetAllStrings(false).ToArray();

								return new Tuple<IStringLocalizer, Exception>(localizer, null);
							}
							catch(Exception executionException)
							{
								return new Tuple<IStringLocalizer, Exception>(null, new InvalidOperationException($"The string-localizer from base-name \"{this.LocalizerBaseName}\" and location \"{this.LocalizerLocation}\" can not execute.", executionException));
							}
						}
						catch(Exception exception)
						{
							return new Tuple<IStringLocalizer, Exception>(null, new InvalidOperationException($"Could not create a string-localizer from base-name \"{this.LocalizerBaseName}\" and location \"{this.LocalizerLocation}\".", exception));
						}
					});
				}

				return this._selectedLocalizerInformation.Value;
			}
		}

		protected internal virtual IEnumerable<PropertyInfo> StringLocalizerProperties => _stringLocalizerProperties ?? (_stringLocalizerProperties = typeof(StringLocalizer).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic).OrderBy(property => property.Name).ToArray());

		#endregion

		#region Methods

		public virtual IDictionary<string, object> GetHtmlLocalizerInformation(IHtmlLocalizer htmlLocalizer)
		{
			if(htmlLocalizer == null)
				throw new ArgumentNullException(nameof(htmlLocalizer));

			var information = new SortedDictionary<string, object>
			{
				{ "Type", htmlLocalizer.GetType() }
			};

			var internalStringLocaliser = (IStringLocalizer)this.GetInternalStringLocalizer(htmlLocalizer);

			// ReSharper disable InvertIf
			if(internalStringLocaliser != null)
			{
				foreach(var item in this.GetStringLocalizerInformation(internalStringLocaliser))
				{
					information.Add("Internal-StringLocalizer." + item.Key, item.Value);
				}
			}
			// ReSharper restore InvertIf

			return information;
		}

		protected internal virtual object GetInternalStringLocalizer(IHtmlLocalizer htmlLocalizer)
		{
			if(htmlLocalizer == null)
				return null;

			var internalLocalizer = htmlLocalizer.GetType().GetField("_localizer", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(htmlLocalizer);

			if(internalLocalizer is IHtmlLocalizer internalHtmlLocalizer)
				return this.GetInternalStringLocalizer(internalHtmlLocalizer);

			return internalLocalizer;
		}

		public virtual IDictionary<string, object> GetStringLocalizerInformation(IStringLocalizer stringLocalizer)
		{
			if(stringLocalizer == null)
				throw new ArgumentNullException(nameof(stringLocalizer));

			var information = new SortedDictionary<string, object>
			{
				{ "Type", stringLocalizer.GetType() }
			};

			// ReSharper disable ConvertIfStatementToSwitchStatement
			if(stringLocalizer is StringLocalizer concreteStringLocalizer)
			{
				foreach(var property in this.StringLocalizerProperties)
				{
					information.Add(property.Name, property.GetValue(concreteStringLocalizer));
				}
			}
			else if(stringLocalizer is ResourceManagerStringLocalizer resourceManagerStringLocalizer)
			{
				foreach(var field in this.ResourceManagerStringLocalizerFields)
				{
					information.Add(field.Name, field.GetValue(resourceManagerStringLocalizer));
				}
			}
			// ReSharper restore ConvertIfStatementToSwitchStatement

			return information;
		}

		#endregion
	}
}