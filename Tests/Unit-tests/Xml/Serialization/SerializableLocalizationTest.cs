using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Xml.Serialization;

namespace UnitTests.Xml.Serialization
{
	[TestClass]
	public class SerializableLocalizationTest
	{
		#region Methods

		[TestMethod]
		public void DefaultElementLocalName_ShouldReturnCulture()
		{
			Assert.AreEqual("culture", new SerializableLocalization().DefaultElementLocalName);
		}

		#endregion
	}
}