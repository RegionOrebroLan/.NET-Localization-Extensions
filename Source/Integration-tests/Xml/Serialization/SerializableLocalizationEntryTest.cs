using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Xml.Serialization;

namespace RegionOrebroLan.Localization.IntegrationTests.Xml.Serialization
{
	[TestClass]
	public class SerializableLocalizationEntryTest : SerializationTest
	{
		#region Methods

		[TestMethod]
		public void Deserialize_IfTheStringIsAnEmptyTag_ShouldDeserializeProperly()
		{
			var serializableLocalizationEntry = this.XmlDeserialize<SerializableLocalizationEntry>("<entry></entry>");

			Assert.IsNull(serializableLocalizationEntry.Lookup);
			Assert.AreEqual(string.Empty, serializableLocalizationEntry.Value);
		}

		[TestMethod]
		public void Deserialize_IfTheStringIsASelfClosedTag_ShouldDeserializeProperly()
		{
			var serializableLocalizationEntry = this.XmlDeserialize<SerializableLocalizationEntry>("<entry/>");

			Assert.IsNull(serializableLocalizationEntry.Lookup);
			Assert.IsNull(serializableLocalizationEntry.Value);
		}

		[TestMethod]
		public void Deserialize_IfTheStringIsATagWithALookupAndANameAttributeAndTestAsInnerText_ShouldDeserializeProperly()
		{
			var serializableLocalizationEntry = this.XmlDeserialize<SerializableLocalizationEntry>("<entry lookup=\"Test\" name=\"New-name\">Test</entry>");

			Assert.AreEqual("Test", serializableLocalizationEntry.Lookup);
			Assert.AreEqual("Test", serializableLocalizationEntry.Value);
		}

		[TestMethod]
		public void Deserialize_IfTheStringIsATagWithAWhitespace_ShouldDeserializeProperly()
		{
			var serializableLocalizationEntry = this.XmlDeserialize<SerializableLocalizationEntry>("<entry> </entry>");

			Assert.IsNull(serializableLocalizationEntry.Lookup);
			Assert.AreEqual(string.Empty, serializableLocalizationEntry.Value);
		}

		[TestMethod]
		public void Deserialize_IfTheStringIsATagWithCdataAsInnerText_ShouldDeserializeProperly()
		{
			var serializableLocalizationEntry = this.XmlDeserialize<SerializableLocalizationEntry>("<entry><![CDATA[Some escaped text as CDATA]]></entry>");

			Assert.IsNull(serializableLocalizationEntry.Lookup);
			Assert.AreEqual("Some escaped text as CDATA", serializableLocalizationEntry.Value);
		}

		[TestMethod]
		public void Deserialize_IfTheStringIsATagWithComentsAndNewLinesAndWhitespaces_ShouldDeserializeProperly()
		{
			var serializableLocalizationEntry = this.XmlDeserialize<SerializableLocalizationEntry>("<entry><!-- Comment -->" + Environment.NewLine + "<!-- Comment -->" + Environment.NewLine + "       </entry>");

			Assert.IsNull(serializableLocalizationEntry.Lookup);
			Assert.AreEqual(string.Empty, serializableLocalizationEntry.Value);
		}

		[TestMethod]
		public void Deserialize_IfTheStringIsATagWithCorrectAttributesAndAnInnerTextWithComentsAndCdataAndNewLinesAndTextAndWhitespaces_ShouldDeserializeProperly()
		{
			var serializableLocalizationEntry = this.XmlDeserialize<SerializableLocalizationEntry>("<entry lookup=\"Test\" name=\"New-name\"><!-- Comment -->" + Environment.NewLine + "Test" + Environment.NewLine + "<![CDATA[Some escaped text as CDATA]]>" + Environment.NewLine + "<!-- Comment -->" + Environment.NewLine + "       </entry>");

			Assert.AreEqual("Test", serializableLocalizationEntry.Lookup);
			Assert.AreEqual("New-name", serializableLocalizationEntry.Name);
			Assert.AreEqual("\nTest\nSome escaped text as CDATA", serializableLocalizationEntry.Value);
		}

		[TestMethod]
		public void Deserialize_IfTheStringIsNotAnEmptyTag_ShouldDeserializeProperly()
		{
			var serializableLocalizationEntry = this.XmlDeserialize<SerializableLocalizationEntry>("<entry>Test</entry>");

			Assert.IsNull(serializableLocalizationEntry.Lookup);
			Assert.AreEqual("Test", serializableLocalizationEntry.Value);
		}

		[TestMethod]
		public void Serialize_IfNoPropertiesAreSet_ShouldSerializeProperly()
		{
			Assert.AreEqual("<entry />", this.XmlSerialize(new SerializableLocalizationEntry()));
		}

		[TestMethod]
		public void Serialize_IfTheLookupAndTheValueIsSet_ShouldSerializeProperly()
		{
			Assert.AreEqual("<entry lookup=\"Test\">Test</entry>", this.XmlSerialize(new SerializableLocalizationEntry {Lookup = "Test", Value = "Test"}));
		}

		[TestMethod]
		public void Serialize_IfTheValueIsSet_ShouldSerializeProperly()
		{
			Assert.AreEqual("<entry>Test</entry>", this.XmlSerialize(new SerializableLocalizationEntry {Value = "Test"}));
		}

		[TestMethod]
		public void Serialize_IfTheValueIsSetToAnEmptyString_ShouldSerializeProperly()
		{
			Assert.AreEqual("<entry></entry>", this.XmlSerialize(new SerializableLocalizationEntry {Value = string.Empty}));
		}

		#endregion
	}
}