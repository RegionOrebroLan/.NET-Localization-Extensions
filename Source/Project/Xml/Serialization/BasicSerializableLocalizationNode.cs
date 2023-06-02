using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using RegionOrebroLan.Localization.Extensions;
using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Xml.Serialization
{
	public abstract class BasicSerializableLocalizationNode : SerializableObject, ILocalizationNode
	{
		#region Properties

		IDictionary<string, ILocalizationEntry> ILocalizationNode.Entries => this.Entries.ToDictionary(item => item.Key, item => (ILocalizationEntry)item.Value);
		public virtual IDictionary<string, SerializableLocalizationEntry> Entries { get; } = new Dictionary<string, SerializableLocalizationEntry>(StringComparer.OrdinalIgnoreCase);
		public abstract string Name { get; }
		IEnumerable<ILocalizationNode> ILocalizationNode.Nodes => this.Nodes;
		public virtual IList<SerializableLocalizationNode> Nodes { get; } = new List<SerializableLocalizationNode>();

		#endregion

		#region Methods

		public override void Load(XElement element)
		{
			if(element == null)
				throw new ArgumentNullException(nameof(element));

			foreach(var child in element.Elements())
			{
				if(child.NodeType == XmlNodeType.Element)
				{
					if(child.HasElements)
					{
						var node = new SerializableLocalizationNode();
						node.Load(child);
						this.Nodes.Add(node);
					}
					else
					{
						var entry = new SerializableLocalizationEntry();
						entry.Load(child);
						this.Entries[entry.Name] = entry;
					}
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

			foreach(var node in this.Nodes)
			{
				// ReSharper disable AssignNullToNotNullAttribute
				writer.WriteStartElement(node.Name?.FirstCharacterToLowerInvariant());
				// ReSharper restore AssignNullToNotNullAttribute

				node.WriteXml(writer);

				writer.WriteEndElement();
			}

			foreach(var entry in this.Entries)
			{
				var isValidElementName = entry.Value.IsValidElementName(entry.Key);

				writer.WriteStartElement(isValidElementName ? entry.Key.FirstCharacterToLowerInvariant() : this.DefaultEntryElementLocalName);

				if(isValidElementName)
					entry.Value.WriteXml(writer);
				else
					entry.Value.WriteXml(entry.Key, writer);

				writer.WriteEndElement();
			}
		}

		#endregion
	}
}