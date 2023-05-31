using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using RegionOrebroLan.Localization.Serialization;
using RegionOrebroLan.Localization.Xml.Linq.Extensions;

namespace RegionOrebroLan.Localization.Xml.Serialization
{
	[XmlRoot("entry")]
	public class SerializableLocalizationEntry : SerializableObject, ILocalizationEntry
	{
		#region Fields

		private const string _attributeValidationExceptionMessagePrefix = "The localization-entry has invalid attributes.";
		private const string _lookupAttributeName = "lookup";
		private static IEnumerable<string> _validAttributeNames;

		#endregion

		#region Properties

		protected internal override string AttributeValidationExceptionMessagePrefix => _attributeValidationExceptionMessagePrefix;
		public virtual string Lookup { get; set; }
		public virtual string LookupAttributeName => _lookupAttributeName;
		public virtual string Name { get; set; }

		protected internal override IEnumerable<string> ValidAttributeNames
		{
			get
			{
				_validAttributeNames ??= new[] { this.NameAttributeName, this.LookupAttributeName };

				return _validAttributeNames;
			}
		}

		public virtual string Value { get; set; }

		#endregion

		#region Methods

		public override void Load(XElement element)
		{
			if(element == null)
				throw new ArgumentNullException(nameof(element));

			if(element.NodeType != XmlNodeType.Element)
				throw this.CreateXmlException($"Load called with invalid element for type \"{this.GetType()}\". Required node-type is \"{XmlNodeType.Element}\".", element);

			this.ValidateAttributes(element);

			if(element.HasElements)
				throw this.CreateXmlException("The localization-entry can not have child elements.", element);

			var lookupAttribute = element.Attributes().GetAttribute(this.LookupAttributeName);

			if(lookupAttribute != null)
				this.Lookup = lookupAttribute.Value;

			var nameAttribute = element.Attributes().GetAttribute(this.NameAttributeName);

			if(nameAttribute != null)
				this.Name = nameAttribute.Value;

			this.Name ??= element.LocalName();

			if(!element.IsEmpty)
				this.Value = element.Value;
		}

		public override void WriteXml(XmlWriter writer)
		{
			this.WriteXml(null, writer);
		}

		public virtual void WriteXml(string name, XmlWriter writer)
		{
			if(writer == null)
				throw new ArgumentNullException(nameof(writer));

			if(this.Lookup != null)
				writer.WriteAttributeString(this.LookupAttributeName, this.Lookup);

			if(name != null)
				writer.WriteAttributeString(this.NameAttributeName, name);

			if(this.Value != null)
				writer.WriteValue(this.Value);
		}

		#endregion
	}
}