using System.Reflection;

namespace RegionOrebroLan.Localization.Reflection
{
	public interface IAssemblyHelper
	{
		#region Properties

		IAssembly ApplicationAssembly { get; }
		IEnumerable<IAssembly> RuntimeAssemblies { get; }

		#endregion

		#region Methods

		IEnumerable<IAssembly> Find(string pattern);
		IAssembly Load(string path);
		IAssembly LoadByName(string name);
		IAssembly LoadSatelliteAssembly(IAssembly mainAssembly, string path);
		IAssembly Wrap(Assembly assembly);
		IAssembly Wrap(IAssembly mainAssembly, Assembly satelliteAssembly);

		#endregion
	}
}