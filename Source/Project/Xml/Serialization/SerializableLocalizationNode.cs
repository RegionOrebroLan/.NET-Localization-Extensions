using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using RegionOrebroLan.Localization.Xml.Linq.Extensions;

namespace RegionOrebroLan.Localization.Xml.Serialization
{
	[XmlRoot("node")]
	public class SerializableLocalizationNode : BasicSerializableLocalizationNode
	{
		#region Fields

		private const string _attributeValidationExceptionMessagePrefix = "The localization-node only supports \"xmlns:*\"-attributes.";
		private static readonly IEnumerable<string> _validAttributeNames = Enumerable.Empty<string>();

		#endregion

		#region Properties

		protected internal override string AttributeValidationExceptionMessagePrefix => _attributeValidationExceptionMessagePrefix;
		public override string Name => this.NameInternal;
		protected internal virtual string NameInternal { get; set; }
		protected internal override IEnumerable<string> ValidAttributeNames => _validAttributeNames;

		#endregion

		#region Methods

		public override void Load(XElement element)
		{
			if(element == null)
				throw new ArgumentNullException(nameof(element));

			if(element.NodeType != XmlNodeType.Element)
				throw this.CreateXmlException($"Load called with invalid element for type \"{this.GetType()}\". Required node-type is \"{XmlNodeType.Element}\".", element);

			this.ValidateAttributes(element);

			this.NameInternal = element.LocalName();

			base.Load(element);
		}

		#endregion
	}
}