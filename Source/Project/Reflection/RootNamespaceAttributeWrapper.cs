using Microsoft.Extensions.Localization;

namespace RegionOrebroLan.Localization.Reflection
{
	[CLSCompliant(false)]
	public class RootNamespaceAttributeWrapper(RootNamespaceAttribute rootNamespaceAttribute) : IRootNamespace
	{
		#region Properties

		public virtual string Name => this.RootNamespaceAttribute.RootNamespace;
		protected internal virtual RootNamespaceAttribute RootNamespaceAttribute { get; } = rootNamespaceAttribute ?? throw new ArgumentNullException(nameof(rootNamespaceAttribute));

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