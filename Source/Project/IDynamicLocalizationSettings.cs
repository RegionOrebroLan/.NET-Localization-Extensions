using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.Abstractions;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization
{
	public interface IDynamicLocalizationSettings : ILocalizationSettings
	{
		#region Events

		event EventHandler AlphabeticalSortingChanged;
		event EventHandler<NotifyCollectionChangedEventArgs> EmbeddedResourceAssembliesChanged;
		event EventHandler FileResourcesDirectoryChanged;
		event EventHandler IncludeParentCulturesChanged;

		#endregion

		#region Properties

		new bool AlphabeticalSorting { get; set; }
		new IList<IAssembly> EmbeddedResourceAssemblies { get; }
		new IDirectoryInfo FileResourcesDirectory { get; set; }
		new bool IncludeParentCultures { get; set; }
		new bool ThrowErrors { get; set; }

		#endregion
	}
}