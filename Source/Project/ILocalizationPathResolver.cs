using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization
{
	public interface ILocalizationPathResolver
	{
		#region Properties

		char Root { get; }
		char Separator { get; }
		IEnumerable<char> SeparatorsToReplace { get; }

		#endregion

		#region Methods

		string Combine(params string[] paths);
		bool NameIsRooted(string name);
		IEnumerable<string> Resolve(IAssembly assembly, string name, string path);

		#endregion
	}
}