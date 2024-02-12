using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Xml.Serialization;

namespace IntegrationTests.Xml.Serialization
{
	[TestClass]
	public class SerializableLocalizationTest : SerializationTest
	{
		#region Methods

		[TestMethod]
		public void Deserialize_IfTheStringContainsACultureWithAnEntryWithLookupAndNameAttribute_ShouldDeserializeProperly()
		{
			var serializableLocalization = this.XmlDeserialize<SerializableLocalization>("<culture name=\"\"><first lookup=\"a.b.c\" name=\"First-name\">Test</first></culture>");

			Assert.AreEqual(1, serializableLocalization.Entries.Count);
			Assert.AreEqual(0, serializableLocalization.Nodes.Count);

			var entry = serializableLocalization.Entries.First();
			Assert.AreEqual("First-name", entry.Key);
			Assert.AreEqual("a.b.c", entry.Value.Lookup);
			Assert.AreEqual("Test", entry.Value.Value);
		}

		[TestMethod]
		public void Deserialize_IfTheStringIsAnEmptyCultureTagWithANameAttribute_ShouldDeserializeProperly()
		{
			var serializableLocalization = this.XmlDeserialize<SerializableLocalization>("<culture name=\"en-US\"></culture>");

			Assert.AreEqual("en-US", serializableLocalization.CultureName);
			Assert.AreEqual(0, serializableLocalization.Entries.Count);
			Assert.AreEqual(0, serializableLocalization.Nodes.Count);
			Assert.IsNull(serializableLocalization.Name);
			Assert.IsNull(serializableLocalization.Priority);
		}

		[TestMethod]
		public void Deserialize_IfTheStringIsASelfClosedCultureTagWithANameAttribute_ShouldDeserializeProperly()
		{
			var serializableLocalization = this.XmlDeserialize<SerializableLocalization>("<culture name=\"\" />");

			Assert.AreEqual(string.Empty, serializableLocalization.CultureName);
			Assert.AreEqual(0, serializableLocalization.Entries.Count);
			Assert.AreEqual(0, serializableLocalization.Nodes.Count);
			Assert.IsNull(serializableLocalization.Name);
			Assert.IsNull(serializableLocalization.Priority);
		}

		#endregion
	}
}