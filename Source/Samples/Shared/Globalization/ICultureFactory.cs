using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace RegionOrebroLan.Globalization
{
	public interface ICultureFactory
	{
		#region Methods

		CultureInfo Create(string name);

		[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
		CultureInfo Create(string name, bool readOnly);

		#endregion
	}
}