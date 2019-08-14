using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Xml.Serialization;

namespace RegionOrebroLan.Localization.UnitTests.Xml.Serialization
{
	[TestClass]
	public class SerializableLocalizationNodeTest
	{
		#region Methods

		[TestMethod]
		public void DefaultElementLocalName_ShouldReturnNode()
		{
			Assert.AreEqual("node", new SerializableLocalizationNode().DefaultElementLocalName);
		}

		#endregion
	}
}