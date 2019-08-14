using System.IO.Abstractions;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization.Resourcing
{
	public interface IResourceValidator
	{
		#region Methods

		bool IsValidEmbeddedResource(IAssembly assembly, string name);
		bool IsValidFileResource(IFileInfo file);
		bool IsValidFileResource(string path);

		#endregion
	}
}