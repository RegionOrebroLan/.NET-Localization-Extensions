using System;
using System.Xml.Linq;

namespace RegionOrebroLan.Localization.Xml.Linq.Extensions
{
	public static class XNameExtension
	{
		#region Methods

		public static string DeclaredName(this XName name)
		{
			if(name == null)
				throw new ArgumentNullException(nameof(name));

			return (name.IsNamespaceDeclaration() ? "xmlns:" : string.Empty) + name.LocalName;
		}

		public static bool IsNamespaceDeclaration(this XName name)
		{
			if(name == null)
				throw new ArgumentNullException(nameof(name));

			return !string.IsNullOrEmpty(name.NamespaceName);
		}

		#endregion
	}
}