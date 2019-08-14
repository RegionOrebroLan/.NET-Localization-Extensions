using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Xml.Serialization;

namespace RegionOrebroLan.Localization.UnitTests.Xml.Serialization
{
	[TestClass]
	public class SerializableLocalizationResourceTest
	{
		#region Methods

		[TestMethod]
		public void DefaultElementLocalName_ShouldReturnCultures()
		{
			Assert.AreEqual("cultures", new SerializableLocalizationResource().DefaultElementLocalName);
		}

		#endregion
	}
}