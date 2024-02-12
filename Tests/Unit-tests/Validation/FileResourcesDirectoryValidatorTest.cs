using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Validation;

namespace UnitTests.Validation
{
	[TestClass]
	public class FileResourcesDirectoryValidatorTest
	{
		#region Methods

		[TestMethod]
		[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
		public void Validate_IfTheFileResourcesDirectoryParameterIsNull_ShouldNotThrowAnException()
		{
			try
			{
				new FileResourcesDirectoryValidator().Validate(null);
			}
			catch
			{
				Assert.Fail("A parameter that is null should not throw an exception.");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(DirectoryNotFoundException))]
		public void Validate_IfTheFileResourcesDirectoryParameterReturnsExistsFalse_ShouldThrowADirectoryNotFoundException()
		{
			var directoryInfoMock = new Mock<IDirectoryInfo>();

			directoryInfoMock.Setup(directory => directory.Exists).Returns(false);
			directoryInfoMock.Setup(directory => directory.FullName).Returns("Test");

			try
			{
				new FileResourcesDirectoryValidator().Validate(directoryInfoMock.Object);
			}
			catch(DirectoryNotFoundException directoryNotFoundException)
			{
				if(directoryNotFoundException.Message.Equals("File-resources-directory-exception: the directory \"Test\" does not exist.", StringComparison.OrdinalIgnoreCase))
					throw;
			}
		}

		[TestMethod]
		[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
		public void Validate_IfTheFileResourcesDirectoryParameterReturnsExistsTrue_ShouldNotThrowAnException()
		{
			var directoryInfoMock = new Mock<IDirectoryInfo>();

			directoryInfoMock.Setup(directory => directory.Exists).Returns(true);

			try
			{
				new FileResourcesDirectoryValidator().Validate(directoryInfoMock.Object);
			}
			catch
			{
				Assert.Fail("A directory that exists should not throw an exception.");
			}
		}

		#endregion
	}
}