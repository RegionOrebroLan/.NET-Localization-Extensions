using System.Globalization;

namespace RegionOrebroLan.Localization
{
	public interface ILocalizedStringFactory
	{
		#region Methods

		ILocalizedString Create(CultureInfo culture, string information, string name, string value);

		#endregion
	}
}