using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization;
using RegionOrebroLan.Localization.Resourcing;

namespace IntegrationTests.Json.Serialization
{
	[TestClass]
	public class LocalizationParserTest : IntegrationTest
	{
		#region Methods

		[TestMethod]
		public void Parse_IfAnEntryIsOnADeepNode_ShouldWorkProperly()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-File-Resources-Directory-Path-Only.json");
			var localizationProvider = (DynamicCacheLocalizationProvider)serviceProvider.GetService<ILocalizationProvider>();

			var resource = localizationProvider.Resources.OfType<IFileResource>().First(item => Path.GetFileName(item.Path).Equals("Texts.en.json", StringComparison.Ordinal));
			var localization = resource.Parser.Parse(resource, resource.Read()).First();
			var node = localization.Nodes.Last();

			Assert.AreEqual("Very", node.Name);
			Assert.AreEqual("Deep", node.Nodes.First().Name);
			Assert.AreEqual("Test", node.Nodes.First().Nodes.First().Name);
			Assert.AreEqual("First", node.Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("Second", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("Third", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("Fourth", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("Fifth", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("Sixth", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("Seventh", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("Eighth", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("Ninth", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("Tenth", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("Text", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Entries.First().Key);
		}

		#endregion
	}
}