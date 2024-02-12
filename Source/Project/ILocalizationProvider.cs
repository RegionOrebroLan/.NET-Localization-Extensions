using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization
{
	public interface ILocalizationProvider
	{
		#region Methods

		[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
		ILocalizedString Get(IEnumerable<object> arguments, IAssembly assembly, CultureInfo culture, string name, string path);

		IEnumerable<ILocalizedString> List(IAssembly assembly, CultureInfo culture, bool includeParentCultures);

		#endregion
	}
}