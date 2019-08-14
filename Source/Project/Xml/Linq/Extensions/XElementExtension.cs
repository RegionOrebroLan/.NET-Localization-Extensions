using System;
using System.Xml.Linq;

namespace RegionOrebroLan.Localization.Xml.Linq.Extensions
{
	public static class XElementExtension
	{
		#region Methods

		public static string DeclaredName(this XElement element)
		{
			if(element == null)
				throw new ArgumentNullException(nameof(element));

			return element.Name?.DeclaredName();
		}

		public static string LocalName(this XElement element)
		{
			if(element == null)
				throw new ArgumentNullException(nameof(element));

			return element.Name?.LocalName;
		}

		#endregion
	}
}