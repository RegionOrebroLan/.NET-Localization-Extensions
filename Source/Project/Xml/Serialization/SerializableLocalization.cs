using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using RegionOrebroLan.Localization.Resourcing;
using RegionOrebroLan.Localization.Serialization;
using RegionOrebroLan.Localization.Xml.Linq.Extensions;

namespace RegionOrebroLan.Localization.Xml.Serialization
{
	[XmlRoot("culture")]
	public class SerializableLocalization : BasicSerializableLocalizationNode, ILocalization
	{
		#region Fields

		private static string _attributeValidationExceptionMessagePrefix;
		private const string _priorityAttributeName = "priority";
		private static IEnumerable<string> _validAttributeNames;

		#endregion

		#region Properties

		protected internal override string AttributeValidationExceptionMessagePrefix
		{
			get
			{
				// ReSharper disable ConvertIfStatementToNullCoalescingExpression
				if(_attributeValidationExceptionMessagePrefix == null)
					_attributeValidationExceptionMessagePrefix = $"The \"{this.DefaultElementLocalName}\"-element has invalid attributes.";
				// ReSharper restore ConvertIfStatementToNullCoalescingExpression

				return _attributeValidationExceptionMessagePrefix;
			}
		}

		public virtual string CultureName { get; set; }
		public override string Name => null;
		public virtual int? Priority { get; set; }
		public virtual string PriorityAttributeName => _priorityAttributeName;
		public virtual IResource Resource { get; set; }

		protected internal override IEnumerable<string> ValidAttributeNames
		{
			get
			{
				// ReSharper disable ConvertIfStatementToNullCoalescingExpression
				if(_validAttributeNames == null)
					_validAttributeNames = new[] {this.NameAttributeName, this.PriorityAttributeName};
				// ReSharper restore ConvertIfStatementToNullCoalescingExpression

				return _validAttributeNames;
			}
		}

		#endregion

		#region Methods

		[SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
		public override void Load(XElement element)
		{
			if(element == null)
				throw new ArgumentNullException(nameof(element));

			if(element.NodeType != XmlNodeType.Element)
				throw this.CreateXmlException($"Load called with invalid element for type \"{this.GetType()}\". Required node-type is \"{XmlNodeType.Element}\".", element);

			if(!string.Equals(element.LocalName(), this.DefaultElementLocalName, StringComparison.OrdinalIgnoreCase))
				throw this.CreateXmlException($"The localization-element has an invalid element-name. The local-name of the element must be \"{this.DefaultElementLocalName}\".", element);

			this.ValidateAttributes(element);

			var nameAttribute = element.Attributes().GetAttribute(this.NameAttributeName);

			if(nameAttribute != null)
				this.CultureName = nameAttribute.Value;

			if(this.CultureName == null)
				throw this.CreateXmlException("The name-attribute of the localization-element must be set.", element);

			var priorityAttribute = element.Attributes().GetAttribute(this.PriorityAttributeName);

			if(priorityAttribute != null)
				this.Priority = !string.IsNullOrEmpty(priorityAttribute.Value) ? int.Parse(priorityAttribute.Value, CultureInfo.InvariantCulture) : (int?) null;

			base.Load(element);
		}

		public override void WriteXml(XmlWriter writer)
		{
			if(writer == null)
				throw new ArgumentNullException(nameof(writer));

			if(this.CultureName != null)
				writer.WriteAttributeString(this.NameAttributeName, this.CultureName);

			if(this.Priority != null)
				writer.WriteAttributeString(this.PriorityAttributeName, this.Priority.ToString());

			base.WriteXml(writer);
		}

		#endregion
	}
}