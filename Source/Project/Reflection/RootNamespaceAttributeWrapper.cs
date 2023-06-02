using System;
using Microsoft.Extensions.Localization;

namespace RegionOrebroLan.Localization.Reflection
{
	[CLSCompliant(false)]
	public class RootNamespaceAttributeWrapper : IRootNamespace
	{
		#region Constructors

		public RootNamespaceAttributeWrapper(RootNamespaceAttribute rootNamespaceAttribute)
		{
			this.RootNamespaceAttribute = rootNamespaceAttribute ?? throw new ArgumentNullException(nameof(rootNamespaceAttribute));
		}

		#endregion

		#region Properties

		public virtual string Name => this.RootNamespaceAttribute.RootNamespace;
		protected internal virtual RootNamespaceAttribute RootNamespaceAttribute { get; }

		#endregion

		#region Methods

		public static implicit operator RootNamespaceAttributeWrapper(RootNamespaceAttribute rootNamespaceAttribute)
		{
			return rootNamespaceAttribute != null ? new RootNamespaceAttributeWrapper(rootNamespaceAttribute) : null;
		}

		public static RootNamespaceAttributeWrapper ToRootNamespaceAttributeWrapper(RootNamespaceAttribute rootNamespaceAttribute)
		{
			return rootNamespaceAttribute;
		}

		#endregion
	}
}