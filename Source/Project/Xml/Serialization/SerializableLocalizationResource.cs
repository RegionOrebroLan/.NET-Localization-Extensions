using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RegionOrebroLan.Localization.Xml.Serialization
{
	[XmlRoot("cultures")]
	public class SerializableLocalizationResource : SerializableObject
	{
		#region Fields

		private static string _attributeValidationExceptionMessagePrefix;
		private static readonly IEnumerable<string> _validAttributeNames = Enumerable.Empty<string>();

		#endregion

		#region Properties

		protected internal override string AttributeValidationExceptionMessagePrefix
		{
			get
			{
				_attributeValidationExceptionMessagePrefix ??= $"The \"{this.DefaultElementLocalName}\"-element only supports \"xmlns:*\"-attributes.";

				return _attributeValidationExceptionMessagePrefix;
			}
		}

		public virtual IList<SerializableLocalization> Localizations { get; } = new List<SerializableLocalization>();
		protected internal override IEnumerable<string> ValidAttributeNames => _validAttributeNames;

		#endregion

		#region Methods

		public override void Load(XElement element)
		{
			if(element == null)
				throw new ArgumentNullException(nameof(element));

			this.ValidateAttributes(element);

			foreach(var child in element.Elements())
			{
				if(child.NodeType == XmlNodeType.Element)
				{
					var localization = new SerializableLocalization();
					localization.Load(child);
					this.Localizations.Add(localization);
				}
				else
				{
					this.ValidateChildElement(child);
				}
			}
		}

		public override void WriteXml(XmlWriter writer)
		{
			if(writer == null)
				throw new ArgumentNullException(nameof(writer));

			foreach(var localization in this.Localizations)
			{
				writer.WriteStartElement(localization.DefaultElementLocalName);

				localization.WriteXml(writer);

				writer.WriteEndElement();
			}
		}

		#endregion
	}
}