using System;
using Microsoft.Extensions.Localization;

namespace RegionOrebroLan.Localization.Reflection
{
	public class RootNamespaceAttributeWrapper : IRootNamespace
	{
		#region Constructors

		[CLSCompliant(false)]
		public RootNamespaceAttributeWrapper(RootNamespaceAttribute rootNamespaceAttribute)
		{
			this.RootNamespaceAttribute = rootNamespaceAttribute ?? throw new ArgumentNullException(nameof(rootNamespaceAttribute));
		}

		#endregion

		#region Properties

		public virtual string Name => this.RootNamespaceAttribute.RootNamespace;

		[CLSCompliant(false)]
		protected internal virtual RootNamespaceAttribute RootNamespaceAttribute { get; }

		#endregion

		#region Methods

		[CLSCompliant(false)]
		public static implicit operator RootNamespaceAttributeWrapper(RootNamespaceAttribute rootNamespaceAttribute)
		{
			return rootNamespaceAttribute != null ? new RootNamespaceAttributeWrapper(rootNamespaceAttribute) : null;
		}

		[CLSCompliant(false)]
		public static RootNamespaceAttributeWrapper ToRootNamespaceAttributeWrapper(RootNamespaceAttribute rootNamespaceAttribute)
		{
			return rootNamespaceAttribute;
		}

		#endregion
	}
}