using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Xml.Serialization;

namespace IntegrationTests.Xml.Serialization
{
	[TestClass]
	public class SerializableLocalizationNodeTest : SerializationTest
	{
		#region Methods

		[TestMethod]
		public void Deserialize_IfTheStringContainsAnEntryThatHasAnEmptyNameAttribute_ShouldDeserializeProperly()
		{
			var serializableLocalizationNode = this.XmlDeserialize<SerializableLocalizationNode>("<node><entry name=\"\" /></node>");

			Assert.AreEqual(1, serializableLocalizationNode.Entries.Count);
			Assert.AreEqual(string.Empty, serializableLocalizationNode.Entries.First().Key);
		}

		[TestMethod]
		public void Serialize_IfAnEntryHasAnEmptyName_ShouldSerializeProperly()
		{
			var serializableLocalizationNode = new SerializableLocalizationNode();
			serializableLocalizationNode.Entries.Add(string.Empty, new SerializableLocalizationEntry());

			Assert.AreEqual("<node><entry name=\"\" /></node>", this.XmlSerialize(serializableLocalizationNode));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Serialize_IfANodeHasAnEmptyName_ShouldThrowAnInvalidOperationException()
		{
			var serializableLocalizationNode = new SerializableLocalizationNode();
			serializableLocalizationNode.Nodes.Add(new SerializableLocalizationNode { NameInternal = string.Empty });

			this.XmlSerialize(serializableLocalizationNode);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Serialize_IfANodeHasANullName_ShouldThrowAnInvalidOperationException()
		{
			var serializableLocalizationNode = new SerializableLocalizationNode();
			serializableLocalizationNode.Nodes.Add(new SerializableLocalizationNode { NameInternal = null });

			this.XmlSerialize(serializableLocalizationNode);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Serialize_IfANodeHasAWhitespaceName_ShouldThrowAnInvalidOperationException()
		{
			var serializableLocalizationNode = new SerializableLocalizationNode();
			serializableLocalizationNode.Nodes.Add(new SerializableLocalizationNode { NameInternal = " " });

			this.XmlSerialize(serializableLocalizationNode);
		}

		[TestMethod]
		public void Serialize_IfTheEntriesAndNodesAreEmpty_ShouldSerializeProperly()
		{
			var serializableLocalizationNode = new SerializableLocalizationNode();

			Assert.AreEqual("<node />", this.XmlSerialize(serializableLocalizationNode));
		}

		[TestMethod]
		public void Serialize_IfTheEntriesAndNodesAreNotEmpty_ShouldSerializeProperly()
		{
			var serializableLocalizationEntry = new SerializableLocalizationEntry
			{
				Value = "Entry-value"
			};

			var serializableLocalizationNode = new SerializableLocalizationNode();
			serializableLocalizationNode.Entries.Add("Invalid-xml-element-name", serializableLocalizationEntry);
			serializableLocalizationNode.Nodes.Add(new SerializableLocalizationNode { NameInternal = "MyNode" });

			Assert.AreEqual("<node><myNode /><entry name=\"Invalid-xml-element-name\">Entry-value</entry></node>", this.XmlSerialize(serializableLocalizationNode));
		}

		[TestMethod]
		public void Serialize_IfTheEntriesContainsAnEntryWithAnInvalidNameAsXmlName_ShouldSerializeProperly()
		{
			var serializableLocalizationEntry = new SerializableLocalizationEntry
			{
				Value = "Entry-value"
			};

			var serializableLocalizationNode = new SerializableLocalizationNode();
			serializableLocalizationNode.Entries.Add("Invalid-xml-element-name", serializableLocalizationEntry);

			Assert.AreEqual("<node><entry name=\"Invalid-xml-element-name\">Entry-value</entry></node>", this.XmlSerialize(serializableLocalizationNode));
		}

		[TestMethod]
		public void Serialize_IfTheEntriesContainsAnEntryWithAValidNameAsXmlName_ShouldSerializeProperly()
		{
			var serializableLocalizationEntry = new SerializableLocalizationEntry
			{
				Value = "Entry-value"
			};

			var serializableLocalizationNode = new SerializableLocalizationNode();
			serializableLocalizationNode.Entries.Add("Valid_Name", serializableLocalizationEntry);

			Assert.AreEqual("<node><valid_Name>Entry-value</valid_Name></node>", this.XmlSerialize(serializableLocalizationNode));
		}

		[TestMethod]
		public void Serialize_IfTheEntriesContainsAnEntryWithLookupAndAnInvalidNameAsXmlName_ShouldSerializeProperly()
		{
			var serializableLocalizationEntry = new SerializableLocalizationEntry
			{
				Lookup = "Lookup-path",
				Value = "Entry-value"
			};

			var serializableLocalizationNode = new SerializableLocalizationNode();
			serializableLocalizationNode.Entries.Add("Invalid-xml-element-name", serializableLocalizationEntry);

			Assert.AreEqual("<node><entry lookup=\"Lookup-path\" name=\"Invalid-xml-element-name\">Entry-value</entry></node>", this.XmlSerialize(serializableLocalizationNode));
		}

		[TestMethod]
		public void Serialize_IfTheEntriesContainsAnEntryWithLookupAndAValidNameAsXmlName_ShouldSerializeProperly()
		{
			var serializableLocalizationEntry = new SerializableLocalizationEntry
			{
				Lookup = "Lookup-path",
				Value = "Entry-value"
			};

			var serializableLocalizationNode = new SerializableLocalizationNode();
			serializableLocalizationNode.Entries.Add("Valid_Name", serializableLocalizationEntry);

			Assert.AreEqual("<node><valid_Name lookup=\"Lookup-path\">Entry-value</valid_Name></node>", this.XmlSerialize(serializableLocalizationNode));
		}

		#endregion
	}
}