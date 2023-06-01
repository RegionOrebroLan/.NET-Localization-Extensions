using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Extensions;

namespace UnitTests.Extensions
{
	[TestClass]
	public class StringExtensionTest
	{
		#region Methods

		[TestMethod]
		public void FirstCharacterToLowerInvariant_IfTheStringIsASingleLowerLetter_ShouldReturnTheStringToLower()
		{
			Assert.AreEqual("a", "a".FirstCharacterToLowerInvariant());
		}

		[TestMethod]
		public void FirstCharacterToLowerInvariant_IfTheStringIsASingleUpperLetter_ShouldReturnTheStringToLower()
		{
			Assert.AreEqual("a", "A".FirstCharacterToLowerInvariant());
		}

		[TestMethod]
		public void FirstCharacterToLowerInvariant_IfTheStringIsEmpty_ShouldReturnAnEmptyString()
		{
			Assert.AreEqual(string.Empty, string.Empty.FirstCharacterToLowerInvariant());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FirstCharacterToLowerInvariant_IfTheStringIsNull_ShouldThrowAnArgumentNullException()
		{
			((string)null).FirstCharacterToLowerInvariant();
		}

		#endregion
	}
}