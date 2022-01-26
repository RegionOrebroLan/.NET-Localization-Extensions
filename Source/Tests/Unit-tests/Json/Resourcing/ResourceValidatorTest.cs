using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Json.Resourcing;
using UnitTests.Resourcing;

namespace UnitTests.Json.Resourcing
{
	[TestClass]
	public class ResourceValidatorTest : BasicResourceValidatorTest
	{
		#region Methods

		protected internal virtual string GetRandomJsonExtension()
		{
			var rest = DateTime.Now.Millisecond % 3;

			return rest switch
			{
				1 => ".JSON",
				2 => ".JsOn",
				_ => ".json",
			};
		}

		[TestMethod]
		public void IsValidEmbeddedResource_IfTheAssemblyContainsTheResourceNameAndTheResourceNameDoesNotEndsWithDotJson_ShouldReturnFalse()
		{
			const string resourceName = "Resource";
			var assembly = this.CreateAssembly(resourceName);
			var resourceValidator = new ResourceValidator(this.CreateFileSystem(false, null));

			Assert.IsFalse(resourceValidator.IsValidEmbeddedResource(assembly, resourceName));
		}

		[TestMethod]
		public void IsValidEmbeddedResource_IfTheAssemblyContainsTheResourceNameAndTheResourceNameEndsWithDotJson_ShouldReturnTrue()
		{
			var resourceName = "Resource" + this.GetRandomJsonExtension();
			var assembly = this.CreateAssembly(resourceName);
			var resourceValidator = new ResourceValidator(this.CreateFileSystem(false, null));

			Assert.IsTrue(resourceValidator.IsValidEmbeddedResource(assembly, resourceName));
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
		public void IsValidExtension_IfTheExtensionParameterIsAnEmptyString_ShouldReturnFalse()
		{
			Assert.IsFalse(new ResourceValidator(Mock.Of<IFileSystem>()).IsValidExtension(string.Empty));
		}

		[TestMethod]
		public void IsValidExtension_IfTheExtensionParameterIsNull_ShouldReturnFalse()
		{
			Assert.IsFalse(new ResourceValidator(Mock.Of<IFileSystem>()).IsValidExtension(null));
		}

		[TestMethod]
		public void IsValidFileResource_WithOneStringParameter_IfThePathExistsAndThePathExtensionIsDotJson_ShouldReturnTrue()
		{
			var path = @"Q:\Test" + this.GetRandomJsonExtension();
			var resourceValidator = new ResourceValidator(this.CreateFileSystem(true, path));

			Assert.IsTrue(resourceValidator.IsValidFileResource(path));
		}

		[TestMethod]
		public void IsValidFileResource_WithOneStringParameter_IfThePathExistsButThePathExtensionIsNotDotJson_ShouldReturnFalse()
		{
			var path = @"Q:\Test.test";
			var resourceValidator = new ResourceValidator(this.CreateFileSystem(true, path));

			Assert.IsFalse(resourceValidator.IsValidFileResource(path));
		}

		[TestMethod]
		public void IsValidFileResource_WithOneStringParameter_IfThePathExtensionIsDotJsonButThePathDoesNotExist_ShouldReturnFalse()
		{
			var path = @"Q:\Test" + this.GetRandomJsonExtension();
			var resourceValidator = new ResourceValidator(this.CreateFileSystem(false, path));

			Assert.IsFalse(resourceValidator.IsValidFileResource(path));
		}

		[TestMethod]
		public void IsValidFileResource_WithOneStringParameter_IfThePathParameterIsAnEmptyString_ShouldReturnFalse()
		{
			Assert.IsFalse(new ResourceValidator(Mock.Of<IFileSystem>()).IsValidFileResource(string.Empty));
		}

		[TestMethod]
		public void IsValidFileResource_WithOneStringParameter_IfThePathParameterIsAWhiteSpace_ShouldReturnFalse()
		{
			Assert.IsFalse(new ResourceValidator(Mock.Of<IFileSystem>()).IsValidFileResource(" "));
		}

		[TestMethod]
		public void IsValidFileResource_WithOneStringParameter_IfThePathParameterIsNull_ShouldReturnFalse()
		{
			Assert.IsFalse(new ResourceValidator(Mock.Of<IFileSystem>()).IsValidFileResource((string)null));
		}

		[TestMethod]
		[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
		public void IsValidFileResource_WithOneStringParameter_IfThePathParameterMakesFileCreationFromItThrowAnException_ShouldReturnFalse()
		{
			const string path = "Invalid-path";

			var fileInfoFactoryMock = new Mock<IFileInfoFactory>();
			fileInfoFactoryMock.Setup(fileInfoFactory => fileInfoFactory.FromFileName(path)).Throws<ArgumentException>();
			var fileSystemMock = new Mock<IFileSystem>();
			fileSystemMock.Setup(theFileSystem => theFileSystem.FileInfo).Returns(fileInfoFactoryMock.Object);
			var fileSystem = fileSystemMock.Object;

			var valid = false;

			try
			{
				fileSystem.FileInfo.FromFileName(path);
			}
			catch(Exception exception)
			{
				valid = exception is ArgumentException;
			}

			if(!valid)
				Assert.Fail("Creating file-information from \"{0}\" should throw an argument-exception.", path);

			Assert.IsFalse(new ResourceValidator(fileSystemMock.Object).IsValidFileResource(path));
		}

		[TestMethod]
		public void Prerequisite_Path_GetExtension_IfThePathParamterIsAnEmptyString_ShouldReturnNull()
		{
			Assert.AreEqual(string.Empty, Path.GetExtension(string.Empty));
		}

		[TestMethod]
		public void Prerequisite_Path_GetExtension_IfThePathParamterIsNull_ShouldReturnNull()
		{
			Assert.IsNull(Path.GetExtension(null));
		}

		#endregion
	}
}