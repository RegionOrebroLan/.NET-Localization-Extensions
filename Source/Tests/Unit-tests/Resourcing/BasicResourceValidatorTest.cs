using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Moq;
using RegionOrebroLan.Localization.Reflection;

namespace UnitTests.Resourcing
{
	public abstract class BasicResourceValidatorTest
	{
		#region Methods

		protected internal virtual IAssembly CreateAssembly(string resourceName)
		{
			return this.CreateAssembly(null, resourceName);
		}

		protected internal virtual IAssembly CreateAssembly(IEnumerable<string> resourceNames)
		{
			return this.CreateAssembly(null, resourceNames);
		}

		protected internal virtual IAssembly CreateAssembly(string content, string resourceName)
		{
			return this.CreateAssembly(content, new[] { resourceName });
		}

		protected internal virtual IAssembly CreateAssembly(string content, IEnumerable<string> resourceNames)
		{
			resourceNames = (resourceNames ?? Enumerable.Empty<string>()).ToArray();

			var assemblyMock = new Mock<IAssembly>();

			assemblyMock.SetupAllProperties();
			assemblyMock.Setup(assembly => assembly.GetManifestResourceNames()).Returns(resourceNames);

			foreach(var resourceName in resourceNames)
			{
				assemblyMock.Setup(assembly => assembly.GetManifestResourceStream(resourceName)).Returns(this.CreateStream(content));
			}

			return assemblyMock.Object;
		}

		protected internal virtual IFileSystem CreateFileSystem(string content, string path)
		{
			return this.CreateFileSystem(content, true, path);
		}

		protected internal virtual IFileSystem CreateFileSystem(bool exists, string path)
		{
			return this.CreateFileSystem(string.Empty, exists, path);
		}

		protected internal virtual IFileSystem CreateFileSystem(string content, bool exists, string path)
		{
			var fileSystemMock = new Mock<IFileSystem>();
			fileSystemMock.SetupAllProperties();

			var fileMock = new Mock<IFile>();
			fileMock.SetupAllProperties();
			fileMock.Setup(file => file.Exists(path)).Returns(exists);

			var fileInfoMock = new Mock<IFileInfo>();
			fileInfoMock.SetupAllProperties();
			fileInfoMock.Setup(fileInfo => fileInfo.Exists).Returns(exists);
			fileInfoMock.Setup(fileInfo => fileInfo.Extension).Returns(Path.GetExtension(path));
			fileInfoMock.Setup(fileInfo => fileInfo.FullName).Returns(path);
			fileInfoMock.Setup(fileInfo => fileInfo.OpenRead()).Returns(this.CreateStream(content));

			var fileInfoFactoryMock = new Mock<IFileInfoFactory>();
			fileInfoFactoryMock.SetupAllProperties();
			fileInfoFactoryMock.Setup(fileInfoFactory => fileInfoFactory.FromFileName(path)).Returns(fileInfoMock.Object);

			fileSystemMock.Setup(fileSystem => fileSystem.File).Returns(fileMock.Object);
			fileSystemMock.Setup(fileSystem => fileSystem.FileInfo).Returns(fileInfoFactoryMock.Object);
			fileSystemMock.Setup(fileSystem => fileSystem.Path).Returns(new PathWrapper(fileSystemMock.Object));

			return fileSystemMock.Object;
		}

		[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Should be disposed by the caller.")]
		protected internal virtual Stream CreateStream(string value)
		{
			var memoryStream = new MemoryStream();
			var streamWriter = new StreamWriter(memoryStream);

			streamWriter.Write(value);
			streamWriter.Flush();

			memoryStream.Position = 0;

			return memoryStream;
		}

		#endregion
	}
}