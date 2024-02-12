using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Xml.Serialization;

namespace UnitTests.Xml.Serialization
{
	[TestClass]
	public class SerializableLocalizationEntryTest
	{
		#region Methods

		[TestMethod]
		public void DefaultElementLocalName_ShouldReturnEntry()
		{
			Assert.AreEqual("entry", new SerializableLocalizationEntry().DefaultElementLocalName);
		}

		[TestMethod]
		public void IsValidElementName_ShouldWorkProperly()
		{
			var serializableLocalizationEntry = new SerializableLocalizationEntry();

			Assert.IsFalse(serializableLocalizationEntry.IsValidElementName(null), "NULL did not return false.");

			Assert.IsFalse(serializableLocalizationEntry.IsValidElementName(string.Empty), "\"\" did not return false.");

			Assert.IsFalse(serializableLocalizationEntry.IsValidElementName(" "), "\" \" did not return false.");

			var elementName = "a" + Environment.NewLine + "a";
			Assert.IsFalse(serializableLocalizationEntry.IsValidElementName(elementName), "\"" + elementName + "\" did not return false.");

			Assert.IsTrue(serializableLocalizationEntry.IsValidElementName("a"), "\"a\" did not return true.");

			Assert.IsTrue(serializableLocalizationEntry.IsValidElementName("_a_1_"), "\"_a_1_\" did not return true.");
		}

		#endregion
	}
}