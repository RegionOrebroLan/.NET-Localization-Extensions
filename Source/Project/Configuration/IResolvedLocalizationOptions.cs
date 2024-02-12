using System.IO.Abstractions;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization.Configuration
{
	public interface IResolvedLocalizationOptions
	{
		#region Properties

		bool AlphabeticalSorting { get; }
		IEnumerable<IAssembly> EmbeddedResourceAssemblies { get; }
		IDirectoryInfo FileResourcesDirectory { get; }
		bool IncludeParentCultures { get; }
		bool ThrowErrors { get; }

		#endregion
	}
}