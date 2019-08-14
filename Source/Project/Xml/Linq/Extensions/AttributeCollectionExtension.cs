using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RegionOrebroLan.Localization.Xml.Linq.Extensions
{
	public static class AttributeCollectionExtension
	{
		#region Methods

		public static XAttribute GetAttribute(this IEnumerable<XAttribute> attributes, string localName)
		{
			if(attributes == null)
				throw new ArgumentNullException(nameof(attributes));

			if(localName == null)
				throw new ArgumentNullException(nameof(localName));

			return attributes.FirstOrDefault(attribute => string.Equals(attribute.LocalName(), localName, StringComparison.OrdinalIgnoreCase));
		}

		#endregion
	}
}