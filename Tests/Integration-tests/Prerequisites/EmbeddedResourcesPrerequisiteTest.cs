using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Prerequisites
{
	[TestClass]
	public class EmbeddedResourcesPrerequisiteTest
	{
		#region Methods

		[TestMethod]
		public void Assembly_Compare_Test()
		{
			var assembly = typeof(Colors.TheClass).Assembly;

			var satelliteAssembly = assembly.GetSatelliteAssembly(CultureInfo.GetCultureInfo("en"));

			Assert.IsNotNull(satelliteAssembly);
			Assert.IsFalse(assembly.Equals(satelliteAssembly));
		}

		[TestMethod]
		public void Assembly_GetManifestResourceInfo_Test()
		{
			const string resourceName = "Colors.Colors.json";

			var manifestResourceInfo = typeof(Colors.TheClass).Assembly.GetManifestResourceInfo(resourceName);

			// ReSharper disable PossibleNullReferenceException
			Assert.IsNull(manifestResourceInfo.FileName);
			// ReSharper restore PossibleNullReferenceException
			Assert.IsNull(manifestResourceInfo.ReferencedAssembly);
			Assert.AreEqual(ResourceLocation.Embedded | ResourceLocation.ContainedInManifestFile, manifestResourceInfo.ResourceLocation);
		}

		[TestMethod]
		public void Assembly_GetManifestResourceNames_OnlyReturnsTheNonCulturePrefixedResources()
		{
			var manifestResourceNames = typeof(Colors.TheClass).Assembly.GetManifestResourceNames().OrderBy(resourceName => resourceName).ToArray();

			Assert.AreEqual(3, manifestResourceNames.Length);
			Assert.AreEqual("Colors.Colors.json", manifestResourceNames[0]);
			Assert.AreEqual("Colors.Colors.resources", manifestResourceNames[1]);
			Assert.AreEqual("Colors.Colors.txt", manifestResourceNames[2]);
		}

		[TestMethod]
		public void Assembly_GetManifestResourceStream_Test()
		{
			const string resourceName = "Colors.Colors.json";

			using(var stream = typeof(Colors.TheClass).Assembly.GetManifestResourceStream(resourceName))
			{
				// ReSharper disable AssignNullToNotNullAttribute
				using(var streamReader = new StreamReader(stream))
				{
					var content = streamReader.ReadToEnd();
					Assert.AreEqual("{\r\n\t\"Cultures\": [\r\n\t\t{\r\n\t\t\t\"Culture\": {\r\n\t\t\t\t\"Name\": \"\",\r\n\t\t\t\t\"Entries\": {\r\n\t\t\t\t\t\"Blue\": {\r\n\t\t\t\t\t\t\"Value\": \"Blue: invariant-culture, \\\"\\\\Embedded-resources\\\\Colors\\\\Colors.json\\\"\"\r\n\t\t\t\t\t},\r\n\t\t\t\t\t\"Green\": {\r\n\t\t\t\t\t\t\"Value\": \"Green: invariant-culture, \\\"\\\\Embedded-resources\\\\Colors\\\\Colors.json\\\"\"\r\n\t\t\t\t\t},\r\n\t\t\t\t\t\"Red\": {\r\n\t\t\t\t\t\t\"Value\": \"Red: invariant-culture, \\\"\\\\Embedded-resources\\\\Colors\\\\Colors.json\\\"\"\r\n\t\t\t\t\t}\r\n\t\t\t\t}\r\n\t\t\t}\r\n\t\t}\r\n\t]\r\n}", content);
				}
				// ReSharper restore AssignNullToNotNullAttribute
			}
		}

		[TestMethod]
		public void Assembly_GetSatelliteAssembly_Test()
		{
			const string resourceName = "Colors.Colors.json";

			var assembly = typeof(Colors.TheClass).Assembly;

			// ReSharper disable AssignNullToNotNullAttribute

			var satelliteAssembly = assembly.GetSatelliteAssembly(CultureInfo.GetCultureInfo("en"));

			using(var stream = satelliteAssembly.GetManifestResourceStream(resourceName))
			{
				using(var streamReader = new StreamReader(stream))
				{
					var content = streamReader.ReadToEnd();
					Assert.AreEqual("{\r\n\t\"Cultures\": [\r\n\t\t{\r\n\t\t\t\"Culture\": {\r\n\t\t\t\t\"Name\": \"en\",\r\n\t\t\t\t\"Entries\": {\r\n\t\t\t\t\t\"Blue\": {\r\n\t\t\t\t\t\t\"Value\": \"Blue: en, \\\"\\\\Embedded-resources\\\\Colors\\\\Colors.en.json\\\"\"\r\n\t\t\t\t\t},\r\n\t\t\t\t\t\"Green\": {\r\n\t\t\t\t\t\t\"Value\": \"Green: en, \\\"\\\\Embedded-resources\\\\Colors\\\\Colors.en.json\\\"\"\r\n\t\t\t\t\t},\r\n\t\t\t\t\t\"Red\": {\r\n\t\t\t\t\t\t\"Value\": \"Red: en, \\\"\\\\Embedded-resources\\\\Colors\\\\Colors.en.json\\\"\"\r\n\t\t\t\t\t}\r\n\t\t\t\t}\r\n\t\t\t}\r\n\t\t}\r\n\t]\r\n}", content);
				}
			}

			satelliteAssembly = assembly.GetSatelliteAssembly(CultureInfo.GetCultureInfo("fi"));

			using(var stream = satelliteAssembly.GetManifestResourceStream(resourceName))
			{
				using(var streamReader = new StreamReader(stream))
				{
					var content = streamReader.ReadToEnd();
					Assert.AreEqual("{\r\n\t\"Cultures\": [\r\n\t\t{\r\n\t\t\t\"Culture\": {\r\n\t\t\t\t\"Name\": \"fi\",\r\n\t\t\t\t\"Entries\": {\r\n\t\t\t\t\t\"Blue\": {\r\n\t\t\t\t\t\t\"Value\": \"Sininen: fi, \\\"\\\\Embedded-resources\\\\Colors\\\\Colors.fi.json\\\"\"\r\n\t\t\t\t\t},\r\n\t\t\t\t\t\"Green\": {\r\n\t\t\t\t\t\t\"Value\": \"Vihreä: fi, \\\"\\\\Embedded-resources\\\\Colors\\\\Colors.fi.json\\\"\"\r\n\t\t\t\t\t},\r\n\t\t\t\t\t\"Red\": {\r\n\t\t\t\t\t\t\"Value\": \"Punainen: fi, \\\"\\\\Embedded-resources\\\\Colors\\\\Colors.fi.json\\\"\"\r\n\t\t\t\t\t}\r\n\t\t\t\t}\r\n\t\t\t}\r\n\t\t}\r\n\t]\r\n}", content);
				}
			}

			satelliteAssembly = assembly.GetSatelliteAssembly(CultureInfo.GetCultureInfo("sv"));

			using(var stream = satelliteAssembly.GetManifestResourceStream(resourceName))
			{
				using(var streamReader = new StreamReader(stream))
				{
					var content = streamReader.ReadToEnd();
					Assert.AreEqual("{\r\n\t\"Cultures\": [\r\n\t\t{\r\n\t\t\t\"Culture\": {\r\n\t\t\t\t\"Name\": \"sv\",\r\n\t\t\t\t\"Entries\": {\r\n\t\t\t\t\t\"Blue\": {\r\n\t\t\t\t\t\t\"Value\": \"Blå: sv, \\\"\\\\Embedded-resources\\\\Colors\\\\Colors.sv.json\\\"\"\r\n\t\t\t\t\t},\r\n\t\t\t\t\t\"Green\": {\r\n\t\t\t\t\t\t\"Value\": \"Grön: sv, \\\"\\\\Embedded-resources\\\\Colors\\\\Colors.sv.json\\\"\"\r\n\t\t\t\t\t},\r\n\t\t\t\t\t\"Red\": {\r\n\t\t\t\t\t\t\"Value\": \"Röd: sv, \\\"\\\\Embedded-resources\\\\Colors\\\\Colors.sv.json\\\"\"\r\n\t\t\t\t\t}\r\n\t\t\t\t}\r\n\t\t\t}\r\n\t\t}\r\n\t]\r\n}", content);
				}
			}

			// ReSharper restore AssignNullToNotNullAttribute
		}

		[TestMethod]
		public void EmbeddedFileProvider_GetDirectoryContents_Test()
		{
			Assert.AreEqual(3, new EmbeddedFileProvider(typeof(Colors.TheClass).Assembly).GetDirectoryContents("/").ToArray().Length);
		}

		#endregion
	}
}