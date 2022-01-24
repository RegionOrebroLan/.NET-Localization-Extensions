using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Configuration;

namespace UnitTests.Configuration
{
	[TestClass]
	public class LocalizationOptionsTest
	{
		#region Methods

		[TestMethod]
		public void AlphabeticalSorting_ShouldReturnTrueByDefault()
		{
			Assert.IsTrue(new LocalizationOptions().AlphabeticalSorting);
		}

		[TestMethod]
		public void EmbeddedResourceAssemblies_ShouldReturnAnEmptyArrayByDefault()
		{
			var localizationOptions = new LocalizationOptions();
			Assert.IsNotNull(localizationOptions.EmbeddedResourceAssemblies);
			Assert.IsFalse(localizationOptions.EmbeddedResourceAssemblies.Any());
		}

		[TestMethod]
		public void FileResourcesDirectoryPath_ShouldReturnNullByDefault()
		{
			Assert.IsNull(new LocalizationOptions().FileResourcesDirectoryPath);
		}

		[TestMethod]
		public void IncludeParentCultures_ShouldReturnFalseByDefault()
		{
			Assert.IsFalse(new LocalizationOptions().IncludeParentCultures);
		}

		[TestMethod]
		public void ThrowErrors_ShouldReturnFalseByDefault()
		{
			Assert.IsFalse(new LocalizationOptions().ThrowErrors);
		}

		#endregion
	}
}