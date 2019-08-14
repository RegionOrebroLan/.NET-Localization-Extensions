using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Resourcing
{
	public interface IResource
	{
		#region Properties

		/// <summary>
		/// Information about the assembly the resource belongs to. Used to get the root-namespace to include in partial resource-names.
		/// </summary>
		IAssembly Assembly { get; }

		ILocalizationParser Parser { get; }

		#endregion

		#region Methods

		string Read();

		#endregion
	}
}