using System;
using System.Xml.Linq;

namespace RegionOrebroLan.Localization.Xml.Linq.Extensions
{
	public static class XAttributeExtension
	{
		#region Methods

		public static string DeclaredName(this XAttribute attribute)
		{
			if(attribute == null)
				throw new ArgumentNullException(nameof(attribute));

			return attribute.Name?.DeclaredName();
		}

		public static string LocalName(this XAttribute attribute)
		{
			if(attribute == null)
				throw new ArgumentNullException(nameof(attribute));

			return attribute.Name?.LocalName;
		}

		#endregion
	}
}