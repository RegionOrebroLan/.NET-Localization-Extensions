using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Localization;

namespace RegionOrebroLan.Localization.Reflection
{
	public class RootNamespaceResolver : IRootNamespaceResolver
	{
		#region Properties

		protected internal virtual ConcurrentDictionary<string, IRootNamespace> Cache { get; } = new ConcurrentDictionary<string, IRootNamespace>(StringComparer.OrdinalIgnoreCase);

		#endregion

		#region Methods

		public virtual IRootNamespace GetRootNamespace(Assembly assembly)
		{
			if(assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			return this.Cache.GetOrAdd(assembly.FullName, key => (RootNamespaceAttributeWrapper) assembly.GetCustomAttribute<RootNamespaceAttribute>());
		}

		#endregion
	}
}