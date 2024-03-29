using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization;
using RegionOrebroLan.Localization.Resourcing;

namespace IntegrationTests.Xml.Serialization
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

			var resource = localizationProvider.Resources.OfType<IFileResource>().First(item => Path.GetFileName(item.Path).Equals("Texts.xml", StringComparison.Ordinal));
			var localization = resource.Parser.Parse(resource, resource.Read()).Last();
			var node = localization.Nodes.Last();

			Assert.AreEqual("Very", node.Name);
			Assert.AreEqual("deep", node.Nodes.First().Name);
			Assert.AreEqual("test", node.Nodes.First().Nodes.First().Name);
			Assert.AreEqual("first", node.Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("Second", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("third", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("fourth", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("fifth", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("sixth", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("seventh", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("eighth", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("ninth", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("tenth", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Name);
			Assert.AreEqual("Text", node.Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Nodes.First().Entries.First().Key);
		}

		#endregion
	}
}