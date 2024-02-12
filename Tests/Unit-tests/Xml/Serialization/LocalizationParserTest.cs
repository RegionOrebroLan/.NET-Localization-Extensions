using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Resourcing;
using RegionOrebroLan.Localization.Xml.Serialization;

namespace UnitTests.Xml.Serialization
{
	[TestClass]
	public class LocalizationParserTest
	{
		#region Methods

		[TestMethod]
		public void Parse_IfTheValueParameterIsAnEmptyString_ShouldReturnAnEmptyCollection()
		{
			Assert.IsFalse(new LocalizationParser().Parse(Mock.Of<IResource>(), string.Empty).Any());
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Parse_IfTheValueParameterIsAWhiteSpace_ShouldThrowAnInvalidOperationExceptionSayingRootElementIsMissing()
		{
			Assert.IsFalse(new LocalizationParser().Parse(Mock.Of<IResource>(), " ").Any());
		}

		[TestMethod]
		public void Parse_IfTheValueParameterIsNull_ShouldReturnAnEmptyCollection()
		{
			Assert.IsFalse(new LocalizationParser().Parse(Mock.Of<IResource>(), null).Any());
		}

		#endregion
	}
}