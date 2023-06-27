using System;

namespace RegionOrebroLan.Localization.Resourcing
{
	public interface IDynamicResourceProvider : IResourceProvider
	{
		#region Events

		event EventHandler EmbeddedResourcesChanged;
		event EventHandler<FileResourceChangedEventArgs> FileResourceContentChanged;
		event EventHandler FileResourcesChanged;

		#endregion
	}
}