using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization
{
	/// <inheritdoc cref="IStringLocalizer" />
	public class StringLocalizer(IAssembly assembly, CultureInfo culture, ILocalizationProvider localizationProvider, ILogger logger, string path) : ICloneable, IStringLocalizer
	{
		#region Properties

		protected internal virtual IAssembly Assembly { get; } = assembly ?? throw new ArgumentNullException(nameof(assembly));
		protected internal virtual CultureInfo Culture { get; } = culture;

		[CLSCompliant(false)]
		public virtual LocalizedString this[string name] => this.GetLocalizedString(name, null);

		[CLSCompliant(false)]
		public virtual LocalizedString this[string name, params object[] arguments] => this.GetLocalizedString(name, arguments);

		protected internal virtual ILocalizationProvider LocalizationProvider { get; } = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
		protected internal virtual ILogger Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));
		protected internal virtual string Path { get; } = path;

		#endregion

		#region Methods

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		[CLSCompliant(false)]
		public virtual IStringLocalizer Clone(CultureInfo culture = null)
		{
			culture ??= this.Culture;

			return new StringLocalizer(this.Assembly, culture, this.LocalizationProvider, this.Logger, this.Path != null ? string.Copy(this.Path) : null);
		}

		[CLSCompliant(false)]
		public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		{
			return this.LocalizationProvider.List(this.Assembly, this.ResolveCulture(this.Culture), includeParentCultures).Cast<LocalizedString>();
		}

		[CLSCompliant(false)]
		protected internal virtual LocalizedString GetLocalizedString(string name, IEnumerable<object> arguments)
		{
			return (LocalizedString)this.LocalizationProvider.Get(arguments, this.Assembly, this.ResolveCulture(this.Culture), name, this.Path);
		}

		protected internal virtual CultureInfo ResolveCulture(CultureInfo culture)
		{
			return culture ?? CultureInfo.CurrentUICulture;
		}

		[CLSCompliant(false)]
		[Obsolete("Use Clone(CultureInfo culture = null) instead. This method will be removed later. This method already have been removed from the Microsoft.Extensions.Localization.IStringLocalizer inteface.")]
		public virtual IStringLocalizer WithCulture(CultureInfo culture = null)
		{
			return this.Clone(culture);
		}

		#endregion
	}
}