using System.IO.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Xml.Resourcing;
using UnitTests.Resourcing;

namespace UnitTests.Xml.Resourcing
{
	[TestClass]
	public class ResourceValidatorTest : BasicResourceValidatorTest
	{
		#region Methods

		[TestMethod]
		public void IsValidEmbeddedResource_IfTheAssemblyContainsTheResourceNameAndTheResourceContainsXml_ShouldReturnTrue()
		{
			const string resourceName = "Resource";
			var assembly = this.CreateAssembly("<root />", resourceName);
			var resourceValidator = new ResourceValidator(Mock.Of<IFileSystem>());

			Assert.IsTrue(resourceValidator.IsValidEmbeddedResource(assembly, resourceName));
		}

		[TestMethod]
		public void IsValidEmbeddedResource_IfTheAssemblyContainsTheResourceNameAndTheResourceDoesNotContainXml_ShouldReturnFalse()
		{
			const string resourceName = "Resource";
			var assembly = this.CreateAssembly("Test", resourceName);
			var resourceValidator = new ResourceValidator(Mock.Of<IFileSystem>());

			Assert.IsFalse(resourceValidator.IsValidEmbeddedResource(assembly, resourceName));
		}

		[TestMethod]
		public void IsValidEmbeddedResource_IfTheAssemblyDoesNotContainTheResourceName_ShouldReturnFalse()
		{
			var assembly = this.CreateAssembly("Resource");
			var resourceValidator = new ResourceValidator(Mock.Of<IFileSystem>());

			Assert.IsFalse(resourceValidator.IsValidEmbeddedResource(assembly, "AnotherResource"));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IsValidEmbeddedResource_IfTheAssemblyParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			var resourceValidator = new ResourceValidator(Mock.Of<IFileSystem>());

			try
			{
				resourceValidator.IsValidEmbeddedResource(null, "Test");
			}
			catch(ArgumentNullException argumentNullException)
			{
				if(argumentNullException.ParamName.Equals("assembly", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		public void IsValidFileResource_WithOneStringParameter_IfThePathExistsAndThePathIsToAFileThatContainsXml_ShouldReturnTrue()
		{
			const string path = @"Q:\Test";
			var resourceValidator = new ResourceValidator(this.CreateFileSystem("<root />", true, path));

			Assert.IsTrue(resourceValidator.IsValidFileResource(path));
		}

		[TestMethod]
		public void IsValidFileResource_WithOneStringParameter_IfThePathExistsAndThePathIsToAFileThatNotContainsXml_ShouldReturnFalse()
		{
			const string path = @"Q:\Test";
			var resourceValidator = new ResourceValidator(this.CreateFileSystem("Test", true, path));

			Assert.IsFalse(resourceValidator.IsValidFileResource(path));
		}

		[TestMethod]
		public void IsValidFileResource_WithOneStringParameter_IfThePathExistsAndThePathIsToAnEmptyFile_ShouldReturnFalse()
		{
			const string path = @"Q:\Test";
			var resourceValidator = new ResourceValidator(this.CreateFileSystem(null, true, path));

			Assert.IsFalse(resourceValidator.IsValidFileResource(path));
		}

		#endregion
	}
}