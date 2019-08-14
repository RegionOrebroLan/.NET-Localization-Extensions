using System.Collections.Generic;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization.Resourcing
{
	public interface IResourceLocator
	{
		#region Properties

		IEnumerable<IResourceResolver> Resolvers { get; }

		#endregion

		#region Methods

		IEnumerable<IEmbeddedResource> GetEmbeddedResources(IAssembly assembly);
		IEnumerable<IFileResource> GetFileResources(string directoryPath, bool recursive);

		#endregion
	}
}