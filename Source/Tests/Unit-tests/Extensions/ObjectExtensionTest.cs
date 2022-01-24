using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Extensions;

namespace UnitTests.Extensions
{
	[TestClass]
	public class ObjectExtensionTest
	{
		#region Methods

		[TestMethod]
		public void ToArgumentString_IfTheArgumentIsNotNull_ShouldReturnTheArgumentToStringWithinQuotes()
		{
			Assert.AreEqual("\"System.Object\"", new object().ToArgumentString());
		}

		[TestMethod]
		public void ToArgumentString_IfTheArgumentIsNull_ShouldReturnTheStringNull()
		{
			Assert.AreEqual("null", ((object)null).ToArgumentString());
		}

		#endregion
	}
}