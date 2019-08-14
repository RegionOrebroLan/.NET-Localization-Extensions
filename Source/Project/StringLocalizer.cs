using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization
{
	public class StringLocalizer : IStringLocalizer
	{
		#region Constructors

		[CLSCompliant(false)]
		public StringLocalizer(IAssembly assembly, CultureInfo culture, ILocalizationProvider localizationProvider, ILogger logger, string path)
		{
			this.Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
			this.Culture = culture;
			this.LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
			this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.Path = path;
		}

		#endregion

		#region Properties

		protected internal virtual IAssembly Assembly { get; }
		protected internal virtual CultureInfo Culture { get; }

		[CLSCompliant(false)]
		public virtual LocalizedString this[string name] => this.GetLocalizedString(name, null);

		[CLSCompliant(false)]
		public virtual LocalizedString this[string name, params object[] arguments] => this.GetLocalizedString(name, arguments);

		[CLSCompliant(false)]
		protected internal virtual ILocalizationProvider LocalizationProvider { get; }

		[CLSCompliant(false)]
		protected internal virtual ILogger Logger { get; }

		protected internal virtual string Path { get; }

		#endregion

		#region Methods

		[CLSCompliant(false)]
		public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		{
			return this.LocalizationProvider.List(this.Assembly, this.ResolveCulture(this.Culture), includeParentCultures).Cast<LocalizedString>();
		}

		[CLSCompliant(false)]
		protected internal virtual LocalizedString GetLocalizedString(string name, IEnumerable<object> arguments)
		{
			return (LocalizedString) this.LocalizationProvider.Get(arguments, this.Assembly, this.ResolveCulture(this.Culture), name, this.Path);
		}

		protected internal virtual CultureInfo ResolveCulture(CultureInfo culture)
		{
			return culture ?? CultureInfo.CurrentUICulture;
		}

		[CLSCompliant(false)]
		public virtual IStringLocalizer WithCulture(CultureInfo culture)
		{
			if(culture == null)
				throw new ArgumentNullException(nameof(culture));

			return new StringLocalizer(this.Assembly, culture, this.LocalizationProvider, this.Logger, this.Path);
		}

		#endregion
	}
}