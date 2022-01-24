using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Json.Serialization;
using RegionOrebroLan.Localization.Resourcing;

namespace UnitTests.Json.Serialization
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
		public void Parse_IfTheValueParameterIsAWhiteSpace_ShouldReturnAnEmptyCollection()
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