using System;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Xml.Serialization;

namespace RegionOrebroLan.Localization.IntegrationTests.Xml.Serialization
{
	[TestClass]
	public class SerializableLocalizationResourceTest : SerializationTest
	{
		#region Methods

		[TestMethod]
		public void Deserialize_IfTheStringContainsACultureWithANameAndAPriorityValueOfAnEmptyString_ShouldDeserializeProperly()
		{
			var serializableLocalizationResource = this.XmlDeserialize<SerializableLocalizationResource>("<cultures><culture name=\"\" priority=\"\" /></cultures>");

			Assert.AreEqual(1, serializableLocalizationResource.Localizations.Count);

			var serializableLocalization = serializableLocalizationResource.Localizations.First();

			Assert.IsNotNull(serializableLocalization);
			Assert.AreEqual(string.Empty, serializableLocalization.CultureName);
			Assert.IsNull(serializableLocalization.Priority);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Deserialize_IfTheStringContainsACultureWithAnInvalidAttribute_ShouldThrowAnInvalidOperationException()
		{
			this.XmlDeserialize<SerializableLocalizationResource>("<cultures><culture attribute=\"\" name=\"\" priority=\"\" /></cultures>");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Deserialize_IfTheStringContainsACultureWithAnInvalidPriorityValue_ShouldThrowInvalidOperationException()
		{
			this.XmlDeserialize<SerializableLocalizationResource>("<cultures><culture priority=\"Test\" /></cultures>");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Deserialize_IfTheStringContainsACultureWithAPriorityValueOfAWhitespace_ShouldThrowInvalidOperationException()
		{
			this.XmlDeserialize<SerializableLocalizationResource>("<cultures><culture priority=\" \" /></cultures>");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Deserialize_IfTheStringContainsACultureWithoutName_ShouldThrowAnInvalidOperationException()
		{
			this.XmlDeserialize<SerializableLocalizationResource>("<cultures><culture /></cultures>");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Deserialize_IfTheStringContainsARootElementWithAnAttribute_ShouldThrowAnInvalidOperationException()
		{
			try
			{
				this.XmlDeserialize<SerializableLocalizationResource>("<cultures attribute=\"\" />");
			}
			catch(InvalidOperationException invalidOperationException)
			{
				const string expectedMessage = "The \"cultures\"-element only supports \"xmlns:*\"-attributes. Invalid attributes: attribute. Line 1, position 11.";
				var actualMessage = (invalidOperationException.InnerException as XmlException)?.Message;

				Assert.AreEqual(expectedMessage, actualMessage);

				throw;
			}
		}

		[TestMethod]
		public void Deserialize_IfTheStringContainsCulturesMixedWithComments_ShouldDeserializeProperly()
		{
			var serializableLocalizationResource = this.XmlDeserialize<SerializableLocalizationResource>("<cultures><!-- Commment --><culture name=\"\" /><!-- Commment --><culture name=\"en-US\" /><!-- Commment --><culture name=\"fi-FI\" /><!-- Commment --><culture name=\"sv-SE\" /><!-- Commment --></cultures>");

			Assert.AreEqual(4, serializableLocalizationResource.Localizations.Count);
			Assert.AreEqual(string.Empty, serializableLocalizationResource.Localizations[0].CultureName);
			Assert.AreEqual("en-US", serializableLocalizationResource.Localizations[1].CultureName);
			Assert.AreEqual("fi-FI", serializableLocalizationResource.Localizations[2].CultureName);
			Assert.AreEqual("sv-SE", serializableLocalizationResource.Localizations[3].CultureName);
		}

		[TestMethod]
		public void Deserialize_IfTheStringContainsCulturesMixedWithCommentsAndNewLinesAndWhitespaces_ShouldDeserializeProperly()
		{
			var serializableLocalizationResource = this.XmlDeserialize<SerializableLocalizationResource>(Environment.NewLine + "   " + "<!-- Comment -->" + "   " + Environment.NewLine + "<cultures>" + Environment.NewLine + "   " + "<!-- Comment -->" + "   " + Environment.NewLine + "<culture name=\"\" /><!-- Commment --><culture name=\"en-US\" />" + Environment.NewLine + "   " + "<!-- Comment -->" + "   " + Environment.NewLine + "<culture name=\"fi-FI\" />" + Environment.NewLine + "   " + "<!-- Comment -->" + "   " + Environment.NewLine + "<culture name=\"sv-SE\" />" + Environment.NewLine + "   " + "<!-- Comment -->" + "   " + Environment.NewLine + "</cultures>" + Environment.NewLine + "   " + "<!-- Comment -->" + "   " + Environment.NewLine);

			Assert.AreEqual(4, serializableLocalizationResource.Localizations.Count);
			Assert.AreEqual(string.Empty, serializableLocalizationResource.Localizations[0].CultureName);
			Assert.AreEqual("en-US", serializableLocalizationResource.Localizations[1].CultureName);
			Assert.AreEqual("fi-FI", serializableLocalizationResource.Localizations[2].CultureName);
			Assert.AreEqual("sv-SE", serializableLocalizationResource.Localizations[3].CultureName);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Deserialize_IfTheStringIsAnInvalidTag_ShouldThrowInvalidOperationException()
		{
			this.XmlDeserialize<SerializableLocalizationResource>("<root />");
		}

		[TestMethod]
		public void Deserialize_IfTheStringIsAValidRootTag_ShouldDeserializeProperly()
		{
			var serializableLocalizationResource = this.XmlDeserialize<SerializableLocalizationResource>("<cultures></cultures>");

			Assert.IsFalse(serializableLocalizationResource.Localizations.Any());
		}

		[TestMethod]
		public void Deserialize_IfTheStringIsAValidRootTagMixedWithCommentsAndNewLinesAndWhitespaces_ShouldDeserializeProperly()
		{
			var serializableLocalizationResource = this.XmlDeserialize<SerializableLocalizationResource>(Environment.NewLine + "        " + Environment.NewLine + "<!-- Comment -->" + "           " + Environment.NewLine + "<cultures />" + Environment.NewLine + "        " + Environment.NewLine + "<!-- Comment -->" + "           " + Environment.NewLine);

			Assert.IsFalse(serializableLocalizationResource.Localizations.Any());
		}

		[TestMethod]
		public void Deserialize_IfTheStringIsAValidSelfClosedRootTag_ShouldDeserializeProperly()
		{
			var serializableLocalizationResource = this.XmlDeserialize<SerializableLocalizationResource>("<cultures />");

			Assert.IsFalse(serializableLocalizationResource.Localizations.Any());
		}

		[TestMethod]
		public void Deserialize_IfTheStringIsAValidSelfClosedRootTagMixedWithCommentsAndNewLinesAndWhitespaces_ShouldDeserializeProperly()
		{
			var serializableLocalizationResource = this.XmlDeserialize<SerializableLocalizationResource>(Environment.NewLine + "        " + Environment.NewLine + "<!-- Comment -->" + "           " + Environment.NewLine + "<cultures />" + Environment.NewLine + "        " + Environment.NewLine + "<!-- Comment -->" + "           " + Environment.NewLine);

			Assert.IsFalse(serializableLocalizationResource.Localizations.Any());
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Deserialize_IfTheStringIsEmpty_ShouldThrowInvalidOperationException()
		{
			this.XmlDeserialize<SerializableLocalizationResource>(string.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Deserialize_IfTheStringIsNotATag_ShouldThrowInvalidOperationException()
		{
			this.XmlDeserialize<SerializableLocalizationResource>("Test");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Deserialize_IfTheStringIsNull_ShouldThrowInvalidOperationException()
		{
			this.XmlDeserialize<SerializableLocalizationResource>(null);
		}

		[TestMethod]
		public void Serialize_IfTheSerializableLocalizationResourceHasNoLocalizations_ShouldSerializeCorrectly()
		{
			Assert.AreEqual("<cultures />", this.XmlSerialize(new SerializableLocalizationResource()));
		}

		[TestMethod]
		public void Serialize_IfTheSerializableLocalizationResourceHasOneLocalizationWithTheCultureNameNotSetAndThePriorityNotSet_ShouldSerializeCorrectly()
		{
			var serializableLocalizationResource = new SerializableLocalizationResource();

			serializableLocalizationResource.Localizations.Add(new SerializableLocalization());

			Assert.AreEqual("<cultures><culture /></cultures>", this.XmlSerialize(serializableLocalizationResource));
		}

		[TestMethod]
		public void Serialize_IfTheSerializableLocalizationResourceHasOneLocalizationWithThePriorityNotSet_ShouldSerializeCorrectly()
		{
			var serializableLocalizationResource = new SerializableLocalizationResource();

			serializableLocalizationResource.Localizations.Add(new SerializableLocalization
			{
				CultureName = "en-US"
			});

			Assert.AreEqual("<cultures><culture name=\"en-US\" /></cultures>", this.XmlSerialize(serializableLocalizationResource));
		}

		[TestMethod]
		public void Serialize_IfTheSerializableLocalizationResourceHasOneLocalizationWithThePrioritySet_ShouldSerializeCorrectly()
		{
			var serializableLocalizationResource = new SerializableLocalizationResource();

			serializableLocalizationResource.Localizations.Add(new SerializableLocalization
			{
				CultureName = string.Empty,
				Priority = 100
			});

			Assert.AreEqual("<cultures><culture name=\"\" priority=\"100\" /></cultures>", this.XmlSerialize(serializableLocalizationResource));
		}

		[TestMethod]
		public void Serialize_IfTheSerializableLocalizationResourceHasTwoLocalizationsWithTheCultureNameNotSetAndThePriorityNotSet_ShouldSerializeCorrectly()
		{
			var serializableLocalizationResource = new SerializableLocalizationResource();

			serializableLocalizationResource.Localizations.Add(new SerializableLocalization());
			serializableLocalizationResource.Localizations.Add(new SerializableLocalization());

			Assert.AreEqual("<cultures><culture /><culture /></cultures>", this.XmlSerialize(serializableLocalizationResource));
		}

		#endregion
	}
}