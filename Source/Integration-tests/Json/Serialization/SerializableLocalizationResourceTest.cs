using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RegionOrebroLan.Localization.Json.Serialization;
using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.IntegrationTests.Json.Serialization
{
	[TestClass]
	public class SerializableLocalizationResourceTest
	{
		#region Fields

		private static string _testResourcesDirectoryPath;

		#endregion

		#region Properties

		protected internal virtual string TestResourcesDirectoryPath => _testResourcesDirectoryPath ?? (_testResourcesDirectoryPath = Path.Combine(Global.GetDirectoryPath(this.GetType()), "Test-resources"));

		#endregion

		#region Methods

		[TestMethod]
		public void Deserialize_IfTheJsonHasEntryDuplicates_ShouldReturnAnObjectWithTheLastEntry()
		{
			var json = File.ReadAllText(this.GetTestResourcePath("Localization-Resource-With-Entry-Duplicate.json"));

			var entries = JsonConvert.DeserializeObject<SerializableLocalizationResource>(json).Cultures.ElementAt(0).Culture.Nodes.ElementAt(0).Node.Entries;

			Assert.AreEqual(1, entries.Count);
			Assert.AreEqual("Entry", entries.First().Key);
			Assert.AreEqual("Second-entry-value", entries.First().Value.Value);
		}

		[TestMethod]
		public void Deserialize_IfTheJsonIsCorrect_ShouldReturnACorrectObject()
		{
			var json = File.ReadAllText(this.GetTestResourcePath("Correct-Serializable-Localization-Resource.json"));
			var serializableLocalizationResource = JsonConvert.DeserializeObject<SerializableLocalizationResource>(json);

			Assert.IsNotNull(serializableLocalizationResource);

			var localizations = serializableLocalizationResource.Cultures.Select(item => item.Culture).Cast<ILocalization>().ToArray();

			Assert.AreEqual(2, localizations.Length);

			// First culture
			var localization = localizations.First();
			Assert.AreEqual("en", localization.CultureName);
			Assert.AreEqual(1000, localization.Priority);
			var topLevelNodes = localization.Nodes.ToArray();
			Assert.AreEqual(2, topLevelNodes.Length);
			// First top-level node
			var topLevelNode = topLevelNodes.First();
			Assert.AreEqual("Common", topLevelNode.Name);
			Assert.IsFalse(topLevelNode.Nodes.Any());
			Assert.AreEqual(2, topLevelNode.Entries.Count);
			var localizationEntry = topLevelNode.Entries.First();
			Assert.AreEqual("No", localizationEntry.Key);
			Assert.IsNull(localizationEntry.Value.Lookup);
			Assert.AreEqual("No", localizationEntry.Value.Value);
			localizationEntry = topLevelNode.Entries.ElementAt(1);
			Assert.AreEqual("Yes", localizationEntry.Key);
			Assert.IsNull(localizationEntry.Value.Lookup);
			Assert.AreEqual("Yes", localizationEntry.Value.Value);
			// Second top-level node
			topLevelNode = topLevelNodes.ElementAt(1);
			Assert.AreEqual("Controllers", topLevelNode.Name);
			Assert.AreEqual(1, topLevelNode.Nodes.Count());
			Assert.AreEqual(2, topLevelNode.Entries.Count);
			localizationEntry = topLevelNode.Entries.First();
			Assert.AreEqual("First", localizationEntry.Key);
			Assert.IsNull(localizationEntry.Value.Lookup);
			Assert.AreEqual("First-value", localizationEntry.Value.Value);
			localizationEntry = topLevelNode.Entries.ElementAt(1);
			Assert.AreEqual("Second", localizationEntry.Key);
			Assert.IsNull(localizationEntry.Value.Lookup);
			Assert.AreEqual("Second-value", localizationEntry.Value.Value);
			var subLevelNode = topLevelNode.Nodes.First();
			Assert.AreEqual("HomeController", subLevelNode.Name);
			Assert.IsFalse(subLevelNode.Nodes.Any());
			Assert.AreEqual(2, subLevelNode.Entries.Count);
			localizationEntry = subLevelNode.Entries.First();
			Assert.AreEqual("No", localizationEntry.Key);
			Assert.IsNull(localizationEntry.Value.Lookup);
			Assert.AreEqual("No", localizationEntry.Value.Value);
			localizationEntry = subLevelNode.Entries.ElementAt(1);
			Assert.AreEqual("Yes", localizationEntry.Key);
			Assert.AreEqual("Common.Yes", localizationEntry.Value.Lookup);
			Assert.IsNull(localizationEntry.Value.Value);

			// Second culture
			localization = localizations.ElementAt(1);
			Assert.AreEqual("sv", localization.CultureName);
			Assert.IsNull(localization.Priority);
			topLevelNodes = localization.Nodes.ToArray();
			Assert.AreEqual(2, topLevelNodes.Length);
			// First top-level node
			topLevelNode = topLevelNodes.First();
			Assert.AreEqual("Common", topLevelNode.Name);
			Assert.IsFalse(topLevelNode.Nodes.Any());
			Assert.AreEqual(3, topLevelNode.Entries.Count);
			localizationEntry = topLevelNode.Entries.First();
			Assert.AreEqual("No", localizationEntry.Key);
			Assert.AreEqual("Controllers.HomeController.No", localizationEntry.Value.Lookup);
			Assert.IsNull(localizationEntry.Value.Value);
			localizationEntry = topLevelNode.Entries.ElementAt(1);
			Assert.AreEqual("Test", localizationEntry.Key);
			Assert.IsNull(localizationEntry.Value.Lookup);
			Assert.AreEqual("å Å ä Ä ö Ö", localizationEntry.Value.Value);
			localizationEntry = topLevelNode.Entries.ElementAt(2);
			Assert.AreEqual("Yes", localizationEntry.Key);
			Assert.IsNull(localizationEntry.Value.Lookup);
			Assert.AreEqual("Ja", localizationEntry.Value.Value);
			// Second top-level node
			topLevelNode = topLevelNodes.ElementAt(1);
			Assert.AreEqual("Controllers.HomeController", topLevelNode.Name);
			Assert.IsFalse(topLevelNode.Nodes.Any());
			Assert.AreEqual(3, topLevelNode.Entries.Count);
			localizationEntry = topLevelNode.Entries.First();
			Assert.AreEqual("No", localizationEntry.Key);
			Assert.IsNull(localizationEntry.Value.Lookup);
			Assert.AreEqual("Nej", localizationEntry.Value.Value);
			localizationEntry = topLevelNode.Entries.ElementAt(1);
			Assert.AreEqual("Test", localizationEntry.Key);
			Assert.IsNull(localizationEntry.Value.Lookup);
			Assert.AreEqual("å Å ä Ä ö Ö", localizationEntry.Value.Value);
			localizationEntry = topLevelNode.Entries.ElementAt(2);
			Assert.AreEqual("Yes", localizationEntry.Key);
			Assert.IsNull(localizationEntry.Value.Lookup);
			Assert.AreEqual("Ja", localizationEntry.Value.Value);
		}

		[TestMethod]
		[ExpectedException(typeof(TargetInvocationException))]
		public void Deserialize_IfTheJsonIsIncorrect_ShouldThrowATargetInvocationException_WithAnInnerExceptionOfTypeJsonSerializationException()
		{
			var json = File.ReadAllText(this.GetTestResourcePath("Incorrect-Serializable-Localization-Resource.json"));

			try
			{
				JsonConvert.DeserializeObject<SerializableLocalizationResource>(json);
			}
			catch(TargetInvocationException targetInvocationException)
			{
				if(targetInvocationException.InnerException is JsonSerializationException)
					throw;
			}
		}

		protected internal virtual string GetTestResourcePath(string fileName)
		{
			return Path.Combine(this.TestResourcesDirectoryPath, fileName);
		}

		[TestMethod]
		public void Serialize_IfTheSerializableLocalizationInternalIsEmpty_ShouldSerializeCorrectly()
		{
			var serializableLocalizationResource = new SerializableLocalizationResource();

			serializableLocalizationResource.Cultures.Add(new SerializableLocalization
			{
				Culture = new SerializableLocalizationInternal()
			});

			var json = JsonConvert.SerializeObject(serializableLocalizationResource);

			Assert.AreEqual("{\"Cultures\":[{\"Culture\":{\"Name\":null,\"Priority\":null,\"Nodes\":[],\"Entries\":{}}}]}", json);
		}

		[TestMethod]
		public void Serialize_IfTheSerializableLocalizationIsEmpty_ShouldSerializeCorrectly()
		{
			var serializableLocalizationResource = new SerializableLocalizationResource();

			serializableLocalizationResource.Cultures.Add(new SerializableLocalization());

			var json = JsonConvert.SerializeObject(serializableLocalizationResource);

			Assert.AreEqual("{\"Cultures\":[{\"Culture\":{\"Name\":null,\"Priority\":null,\"Nodes\":[],\"Entries\":{}}}]}", json);
		}

		[TestMethod]
		public void Serialize_IfTheSerializableLocalizationResourceIsEmpty_ShouldSerializeCorrectly()
		{
			var serializableLocalizationResource = new SerializableLocalizationResource();

			var json = JsonConvert.SerializeObject(serializableLocalizationResource);

			Assert.AreEqual("{\"Cultures\":[]}", json);
		}

		[TestMethod]
		public void Serialize_ShouldSerializeCorrectly()
		{
			var expectedJson = File.ReadAllText(this.GetTestResourcePath("Correct-Unindented-Serializable-Localization-Resource.json"));
			var serializableLocalizationResource = JsonConvert.DeserializeObject<SerializableLocalizationResource>(expectedJson);

			var json = JsonConvert.SerializeObject(serializableLocalizationResource, Formatting.None, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
			Assert.AreEqual(expectedJson, json);
		}

		#endregion
	}
}