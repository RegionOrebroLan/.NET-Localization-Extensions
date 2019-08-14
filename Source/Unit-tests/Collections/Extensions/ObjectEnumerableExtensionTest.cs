using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Collections.Extensions;

namespace RegionOrebroLan.Localization.UnitTests.Collections.Extensions
{
	[TestClass]
	public class ObjectEnumerableExtensionTest
	{
		#region Methods

		[TestMethod]
		public void ToCommaSeparatedArgumentString_IfTheArgumentContainsItems_ShouldReturnTheItemsSeparatedByCommasAndNullsAsTheStringNullAndNotNullsToStringWithinQuotes()
		{
			Assert.AreEqual("\"System.Object\", null, \"Test\", \"5\"", new[] {new object(), null, "Test", 5}.ToCommaSeparatedArgumentString());
		}

		[TestMethod]
		public void ToCommaSeparatedArgumentString_IfTheArgumentContainsOneItemThatIsNotNull_ShouldReturnTheItemToStringWithinQuotes()
		{
			Assert.AreEqual("\"System.Object\"", new[] {new object()}.ToCommaSeparatedArgumentString());
		}

		[TestMethod]
		public void ToCommaSeparatedArgumentString_IfTheArgumentContainsOneItemThatIsNull_ShouldReturnTheStringNull()
		{
			Assert.AreEqual("null", new object[] {null}.ToCommaSeparatedArgumentString());
		}

		[TestMethod]
		public void ToCommaSeparatedArgumentString_IfTheArgumentContainsTwoItemThatAreNull_ShouldReturnTheStringNullCommaNull()
		{
			Assert.AreEqual("null, null", new object[] {null, null}.ToCommaSeparatedArgumentString());
		}

		[TestMethod]
		public void ToCommaSeparatedArgumentString_IfTheArgumentIsEmpty_ShouldReturnAnEmptyString()
		{
			Assert.AreEqual(string.Empty, Enumerable.Empty<string>().ToCommaSeparatedArgumentString());
		}

		[TestMethod]
		public void ToCommaSeparatedArgumentString_IfTheArgumentIsNull_ShouldReturnTheStringNullEnumerable()
		{
			Assert.AreEqual("null-enumerable", ((IEnumerable<object>) null).ToCommaSeparatedArgumentString());
		}

		[TestMethod]
		public void ToSeparatedArgumentString_IfTheArgumentContainsOneItemThatIsNotNull_ShouldReturnTheItemToStringWithinQuotes()
		{
			Assert.AreEqual("\"System.Object\"", new[] {new object()}.ToSeparatedArgumentString("Test"));
		}

		[TestMethod]
		public void ToSeparatedArgumentString_IfTheArgumentContainsOneItemThatIsNull_ShouldReturnTheStringNull()
		{
			Assert.AreEqual("null", new object[] {null}.ToSeparatedArgumentString("Test"));
		}

		[TestMethod]
		public void ToSeparatedArgumentString_IfTheArgumentIsEmpty_ShouldReturnAnEmptyString()
		{
			Assert.AreEqual(string.Empty, Enumerable.Empty<string>().ToSeparatedArgumentString("Test"));
		}

		[TestMethod]
		public void ToSeparatedArgumentString_IfTheArgumentIsNull_ShouldReturnTheStringNullEnumerable()
		{
			Assert.AreEqual("null-enumerable", ((IEnumerable<object>) null).ToSeparatedArgumentString("Test"));
		}

		#endregion
	}
}