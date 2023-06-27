using System.Collections.Generic;

namespace RegionOrebroLan.Localization.Resourcing
{
	public interface IResourceProvider
	{
		#region Properties

		IEnumerable<IEmbeddedResource> EmbeddedResources { get; }
		IEnumerable<IFileResource> FileResources { get; }

		#endregion
	}
}