using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using RegionOrebroLan.Localization.Extensions;
using RegionOrebroLan.Localization.Xml.Linq.Extensions;

namespace RegionOrebroLan.Localization.Xml.Serialization
{
	public abstract class SerializableObject : IXmlSerializable
	{
		#region Fields

		private Lazy<string> _defaultElementLocalName;
		private static readonly ConcurrentDictionary<Type, string> _defaultElementLocalNameDictionary = new ConcurrentDictionary<Type, string>();
		private static Lazy<string> _defaultEntryElementLocalName;
		private const string _nameAttributeName = "name";
		private static readonly Regex _validElementNameRegularExpression = new Regex("^[a-zA-Z0-9_]+$", RegexOptions.Compiled);

		#endregion

		#region Properties

		protected internal abstract string AttributeValidationExceptionMessagePrefix { get; }

		public virtual string DefaultElementLocalName
		{
			get
			{
				if(this._defaultElementLocalName == null)
				{
					this._defaultElementLocalName = new Lazy<string>(() =>
					{
						return this.DefaultElementLocalNameDictionary.GetOrAdd(this.GetType(), type =>
						{
							var elementName = type.GetCustomAttributes(typeof(XmlRootAttribute), true).OfType<XmlRootAttribute>().FirstOrDefault()?.ElementName;

							if(string.IsNullOrWhiteSpace(elementName))
								elementName = type.Name;

							return elementName.FirstCharacterToLowerInvariant();
						});
					});
				}

				return this._defaultElementLocalName.Value;
			}
		}

		protected internal virtual ConcurrentDictionary<Type, string> DefaultElementLocalNameDictionary => _defaultElementLocalNameDictionary;

		protected internal virtual string DefaultEntryElementLocalName
		{
			get
			{
				if(_defaultEntryElementLocalName == null)
					_defaultEntryElementLocalName = new Lazy<string>(() => new SerializableLocalizationEntry().DefaultElementLocalName);

				return _defaultEntryElementLocalName.Value;
			}
		}

		public virtual string NameAttributeName => _nameAttributeName;
		protected internal abstract IEnumerable<string> ValidAttributeNames { get; }
		protected internal virtual Regex ValidElementNameRegularExpression => _validElementNameRegularExpression;

		#endregion

		#region Methods

		protected internal virtual XmlException CreateXmlException(string message, XElement element)
		{
			var messages = new List<string>();

			if(!string.IsNullOrEmpty(message))
				messages.Add(message);

			if(element != null)
				messages.Add($"Element: name = \"{element.DeclaredName()}\", node-type = {element.NodeType}.");

			return this.CreateXmlException(string.Join(" ", messages), (IXmlLineInfo) element);
		}

		protected internal virtual XmlException CreateXmlException(string message, IXmlLineInfo lineInformation)
		{
			// ReSharper disable ConvertIfStatementToReturnStatement
			if(lineInformation != null)
				return new XmlException(message, null, lineInformation.LineNumber, lineInformation.LinePosition);
			// ReSharper restore ConvertIfStatementToReturnStatement

			return new XmlException(message);
		}

		public virtual XmlSchema GetSchema()
		{
			return null;
		}

		protected internal virtual bool IsValidAttribute(XAttribute attribute)
		{
			if(attribute == null)
				throw new ArgumentNullException(nameof(attribute));

			if(this.ValidAttributeNames.Contains(attribute.LocalName(), StringComparer.OrdinalIgnoreCase))
				return true;

			if(this.ValidAttributeNames.Contains(attribute.DeclaredName(), StringComparer.OrdinalIgnoreCase))
				return true;

			// ReSharper disable ConvertIfStatementToReturnStatement
			if(attribute.Name.IsNamespaceDeclaration())
				return true;
			// ReSharper restore ConvertIfStatementToReturnStatement

			return false;
		}

		public virtual bool IsValidElementName(string name)
		{
			return name != null && this.ValidElementNameRegularExpression.IsMatch(name);
		}

		public abstract void Load(XElement element);

		public void ReadXml(XmlReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException(nameof(reader));

			this.Load(XElement.Load(reader, LoadOptions.SetLineInfo));
		}

		protected internal virtual void ValidateAttributes(XElement element)
		{
			if(element == null)
				throw new ArgumentNullException(nameof(element));

			var attributes = element.Attributes().ToArray();

			var invalidAttributes = attributes.Where(attribute => !this.IsValidAttribute(attribute)).ToArray();

			if(!invalidAttributes.Any())
				return;

			var invalidAttributeNames = invalidAttributes.Select(attribute => attribute.DeclaredName()).ToArray();

			throw this.CreateXmlException(
				string.Format(
					CultureInfo.InvariantCulture,
					this.AttributeValidationExceptionMessagePrefix + " Invalid attributes: {0}.",
					string.Join(", ", invalidAttributeNames))
				, invalidAttributes.First()
			);
		}

		[SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
		protected internal virtual void ValidateChildElement(XElement element)
		{
			if(element == null)
				throw new ArgumentNullException(nameof(element));

			if(element.NodeType == XmlNodeType.Comment || element.NodeType == XmlNodeType.Whitespace)
				return;

			throw this.CreateXmlException("Invalid child-element.", element);
		}

		public abstract void WriteXml(XmlWriter writer);

		#endregion
	}
}