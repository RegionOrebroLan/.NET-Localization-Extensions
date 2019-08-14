using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace RegionOrebroLan.Localization.Reflection
{
	[TypeConverter(typeof(AssemblyInterfaceTypeConverter))]
	public interface IAssembly
	{
		#region Properties

		CultureInfo Culture { get; }
		string FullName { get; }
		bool IsSatelliteAssembly { get; }
		string Location { get; }
		string Name { get; }
		string RootNamespace { get; }

		#endregion

		#region Methods

		IEnumerable<string> GetManifestResourceNames();
		Stream GetManifestResourceStream(string name);

		#endregion
	}
}