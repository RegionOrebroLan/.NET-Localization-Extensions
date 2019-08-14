using System;
using System.Collections.Generic;

namespace RegionOrebroLan.Localization.Resourcing
{
	public interface IResourceProvider
	{
		#region Events

		event EventHandler EmbeddedResourcesChanged;
		event EventHandler<FileResourceChangedEventArgs> FileResourceContentChanged;
		event EventHandler FileResourcesChanged;

		#endregion

		#region Properties

		IEnumerable<IEmbeddedResource> EmbeddedResources { get; }
		IEnumerable<IFileResource> FileResources { get; }

		#endregion
	}
}