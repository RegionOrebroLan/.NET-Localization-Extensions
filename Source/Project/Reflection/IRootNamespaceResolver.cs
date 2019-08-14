using System.Reflection;

namespace RegionOrebroLan.Localization.Reflection
{
	public interface IRootNamespaceResolver
	{
		#region Methods

		IRootNamespace GetRootNamespace(Assembly assembly);

		#endregion
	}
}