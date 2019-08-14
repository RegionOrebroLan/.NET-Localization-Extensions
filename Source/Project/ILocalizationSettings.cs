using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.Abstractions;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization
{
	public interface ILocalizationSettings
	{
		#region Events

		event EventHandler AlphabeticalSortingChanged;
		event EventHandler<NotifyCollectionChangedEventArgs> EmbeddedResourceAssembliesChanged;
		event EventHandler FileResourcesDirectoryChanged;
		event EventHandler IncludeParentCulturesChanged;

		#endregion

		#region Properties

		bool AlphabeticalSorting { get; set; }
		IList<IAssembly> EmbeddedResourceAssemblies { get; }
		IDirectoryInfo FileResourcesDirectory { get; set; }
		bool IncludeParentCultures { get; set; }
		bool ThrowErrors { get; set; }

		#endregion
	}
}