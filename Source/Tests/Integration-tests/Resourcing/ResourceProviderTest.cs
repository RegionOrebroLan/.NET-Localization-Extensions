using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading;
using IntegrationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Resourcing;

namespace IntegrationTests.Resourcing
{
	[TestClass]
	public class ResourceProviderTest : IntegrationTest
	{
		#region Fields

		private const string _validResourceFileExtension = ".json";

		#endregion

		#region Properties

		protected internal virtual string ValidResourceFileExtension => _validResourceFileExtension;

		#endregion

		#region Methods

		protected internal virtual void CreateFile(string path)
		{
			using(File.Create(path)) { }
		}

		protected internal virtual ResourceProvider CreateResourceProvider()
		{
			return (ResourceProvider)this.BuildServiceProvider().GetService<IResourceProvider>();
		}

		[TestMethod]
		public void EmbeddedResources_Test()
		{
			var serviceProvider = this.BuildServiceProvider(new[] { typeof(Colors.TheClass).Assembly.GetName().Name, typeof(Numbers.TheClass).Assembly.FullName });
			var resourceProvider = this.GetResourceProvider(serviceProvider);
			var assemblyHelper = serviceProvider.GetService<IAssemblyHelper>();

			Assert.AreEqual(5, resourceProvider.EmbeddedResources.Count());

			resourceProvider.Settings.EmbeddedResourceAssemblies.Clear();
			resourceProvider.Settings.EmbeddedResourceAssemblies.Add(assemblyHelper.Wrap(typeof(Words.TheClass).Assembly));

			Assert.AreEqual(3, resourceProvider.EmbeddedResources.Count());

			resourceProvider.Settings.EmbeddedResourceAssemblies.Clear();

			Assert.IsFalse(resourceProvider.EmbeddedResources.Any());
		}

		[TestMethod]
		public void FileResources_Test()
		{
			var resourceProvider = this.CreateResourceProvider();

			Assert.AreEqual(6, resourceProvider.FileResources.Count());
		}

		[TestMethod]
		public void FileResourcesDirectory_Configuration_IfConfigurationIsChangedWithANewFileResourcesDirectoryPathAndAValidResourceFileIsCreatedInASubDirectoryInThePreviousFileResourcesDirectory_TheFileResourcesCacheShouldNotBeCleared()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-File-Resources-Directory-Path-Only.json", true);
			var resourceProvider = (ResourceProvider)serviceProvider.GetService<IResourceProvider>();
			var testContext = serviceProvider.GetService<ITestContext>();
			var subDirectoryPath = Directory.CreateDirectory(Path.Combine(testContext.ConfiguredFileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			var configurationContent = File.ReadAllText(testContext.ConfigurationFilePath);
			var configuredResourcesDirectoryRelativePathValue = testContext.ConfiguredFileResourcesDirectoryRelativePath.Replace(@"\", @"\\", StringComparison.Ordinal);
			var fileResourcesDirectoryRelativePathValue = testContext.FileResourcesDirectoryRelativePath.Replace(@"\", @"\\", StringComparison.Ordinal);
			configurationContent = configurationContent.Replace(configuredResourcesDirectoryRelativePathValue, fileResourcesDirectoryRelativePathValue, StringComparison.Ordinal);
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(800);

			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var createdValidResourceFilePath = Path.Combine(subDirectoryPath, "Created-file" + this.ValidResourceFileExtension);

			this.CreateFile(createdValidResourceFilePath);

			Thread.Sleep(200);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_Configuration_IfConfigurationIsChangedWithANewFileResourcesDirectoryPathAndAValidResourceFileIsCreatedInThePreviousFileResourcesDirectory_TheFileResourcesCacheShouldNotBeCleared()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-File-Resources-Directory-Path-Only.json", true);
			var resourceProvider = (ResourceProvider)serviceProvider.GetService<IResourceProvider>();
			var testContext = serviceProvider.GetService<ITestContext>();
			var previousResourcesDirectoryPath = testContext.ConfiguredFileResourcesDirectoryPath;

			var configurationContent = File.ReadAllText(testContext.ConfigurationFilePath);
			var configuredResourcesDirectoryRelativePathValue = testContext.ConfiguredFileResourcesDirectoryRelativePath.Replace(@"\", @"\\", StringComparison.Ordinal);
			var fileResourcesDirectoryRelativePathValue = testContext.FileResourcesDirectoryRelativePath.Replace(@"\", @"\\", StringComparison.Ordinal);
			configurationContent = configurationContent.Replace(configuredResourcesDirectoryRelativePathValue, fileResourcesDirectoryRelativePathValue, StringComparison.Ordinal);
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(800);

			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var createdValidResourceFilePath = Path.Combine(previousResourcesDirectoryPath, "Created-file" + this.ValidResourceFileExtension);

			this.CreateFile(createdValidResourceFilePath);

			Thread.Sleep(200);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfADirectoryIsCreated_TheFileResourcesCacheShouldNotBeCleared()
		{
			var resourceProvider = this.CreateResourceProvider();

			Assert.AreEqual(6, resourceProvider.FileResources.Count());
			Assert.IsNotNull(resourceProvider.FileResourcesCache);

			var createdDirectoryPath = Path.Combine(resourceProvider.Settings.FileResourcesDirectory.FullName, "Created-directory");

			Directory.CreateDirectory(createdDirectoryPath);

			Thread.Sleep(100);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfADirectoryWithoutValidResourceFilesIsDeleted_TheFileResourcesCacheShouldNotBeCleared()
		{
			var resourceProvider = this.CreateResourceProvider();
			// ReSharper disable PossibleNullReferenceException
			var embeddedResourcesPath = Path.Combine(new DirectoryInfo(Global.ProjectDirectoryPath).Parent.Parent.FullName, @"Embedded-resources\Colors");
			// ReSharper restore PossibleNullReferenceException
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;
			var directoryToDeletePath = Directory.CreateDirectory(Path.Combine(fileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			foreach(var txtFilePath in Directory.GetFiles(embeddedResourcesPath, "*.txt"))
			{
				File.Copy(txtFilePath, txtFilePath.Replace(embeddedResourcesPath, directoryToDeletePath, StringComparison.OrdinalIgnoreCase));
			}

			Thread.Sleep(200);

			Assert.AreEqual(10, Directory.GetFiles(fileResourcesDirectoryPath, "*.*", SearchOption.AllDirectories).Length, this.PossibleReasonForFailure);
			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			Directory.Delete(directoryToDeletePath, true);

			Thread.Sleep(200);

			Assert.AreEqual(6, Directory.GetFiles(fileResourcesDirectoryPath, "*.*", SearchOption.AllDirectories).Length, this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfADirectoryWithoutValidResourceFilesIsRenamed_TheFileResourcesCacheShouldNotBeCleared()
		{
			var resourceProvider = this.CreateResourceProvider();
			// ReSharper disable PossibleNullReferenceException
			var embeddedResourcesPath = Path.Combine(new DirectoryInfo(Global.ProjectDirectoryPath).Parent.Parent.FullName, @"Embedded-resources\Colors");
			// ReSharper restore PossibleNullReferenceException
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;
			var directoryToRenamePath = Directory.CreateDirectory(Path.Combine(fileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			foreach(var txtFilePath in Directory.GetFiles(embeddedResourcesPath, "*.txt"))
			{
				File.Copy(txtFilePath, txtFilePath.Replace(embeddedResourcesPath, directoryToRenamePath, StringComparison.OrdinalIgnoreCase));
			}

			Thread.Sleep(200);

			Assert.AreEqual(10, Directory.GetFiles(fileResourcesDirectoryPath, "*.*", SearchOption.AllDirectories).Length, this.PossibleReasonForFailure);
			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			// ReSharper disable PossibleNullReferenceException
			var directoryToRenameToPath = Path.Combine(new DirectoryInfo(directoryToRenamePath).Parent.FullName, Guid.NewGuid().ToString());
			// ReSharper restore PossibleNullReferenceException

			Directory.Move(directoryToRenamePath, directoryToRenameToPath);

			Thread.Sleep(200);

			Assert.AreEqual(10, Directory.GetFiles(fileResourcesDirectoryPath, "*.*", SearchOption.AllDirectories).Length, this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfADirectoryWithValidResourceFilesIsDeleted_TheFileResourcesCacheShouldBeCleared()
		{
			const string resourceFileName = "Texts.json";
			var resourceProvider = this.CreateResourceProvider();
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;
			var directoryToDeletePath = Directory.CreateDirectory(Path.Combine(fileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			File.Copy(Path.Combine(fileResourcesDirectoryPath, resourceFileName), Path.Combine(directoryToDeletePath, resourceFileName));

			Thread.Sleep(200);

			Assert.AreEqual(7, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			Directory.Delete(directoryToDeletePath, true);

			Thread.Sleep(200);

			Assert.IsNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfADirectoryWithValidResourceFilesIsRenamed_TheFileResourcesCacheShouldBeCleared()
		{
			const string resourceFileName = "Texts.json";
			var resourceProvider = this.CreateResourceProvider();
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;
			var directoryToRenamePath = Directory.CreateDirectory(Path.Combine(fileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			File.Copy(Path.Combine(fileResourcesDirectoryPath, resourceFileName), Path.Combine(directoryToRenamePath, resourceFileName));

			Thread.Sleep(200);

			Assert.AreEqual(7, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			// ReSharper disable PossibleNullReferenceException
			var directoryToRenameToPath = Path.Combine(new DirectoryInfo(directoryToRenamePath).Parent.FullName, Guid.NewGuid().ToString());
			// ReSharper restore PossibleNullReferenceException

			Directory.Move(directoryToRenamePath, directoryToRenameToPath);

			Thread.Sleep(200);

			Assert.IsNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAnInvalidResourceFileIsChanged_TheFileResourceContentChangedEventShouldNotBeTriggered()
		{
			var resourceFileName = "Colors.resx";
			var resourceProvider = this.CreateResourceProvider();
			// ReSharper disable PossibleNullReferenceException
			var embeddedResourcesPath = Path.Combine(new DirectoryInfo(Global.ProjectDirectoryPath).Parent.Parent.FullName, @"Embedded-resources\Colors");
			// ReSharper restore PossibleNullReferenceException
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;

			File.Copy(Path.Combine(embeddedResourcesPath, resourceFileName), Path.Combine(fileResourcesDirectoryPath, resourceFileName));

			Thread.Sleep(200);

			Assert.AreEqual(7, Directory.GetFiles(fileResourcesDirectoryPath, "*.*", SearchOption.AllDirectories).Length, this.PossibleReasonForFailure);

			var eventTriggered = false;

			resourceProvider.FileResourceContentChanged += (sender, e) => eventTriggered = true;

			var fileToChangePath = Path.Combine(fileResourcesDirectoryPath, resourceFileName);

			File.WriteAllText(fileToChangePath, "Test", Encoding.UTF8);

			Thread.Sleep(200);

			Assert.IsFalse(eventTriggered, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAnInvalidResourceFileIsChangedInASubDirectory_TheFileResourceContentChangedEventShouldNotBeTriggered()
		{
			var resourceFileName = "Colors.resx";
			var resourceProvider = this.CreateResourceProvider();
			// ReSharper disable PossibleNullReferenceException
			var embeddedResourcesPath = Path.Combine(new DirectoryInfo(Global.ProjectDirectoryPath).Parent.Parent.FullName, @"Embedded-resources\Colors");
			// ReSharper restore PossibleNullReferenceException
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;
			var subDirectoryPath = Directory.CreateDirectory(Path.Combine(fileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			File.Copy(Path.Combine(embeddedResourcesPath, resourceFileName), Path.Combine(subDirectoryPath, resourceFileName));

			Thread.Sleep(200);

			Assert.AreEqual(7, Directory.GetFiles(fileResourcesDirectoryPath, "*.*", SearchOption.AllDirectories).Length, this.PossibleReasonForFailure);

			var eventTriggered = false;

			resourceProvider.FileResourceContentChanged += (sender, e) => eventTriggered = true;

			var fileToChangePath = Path.Combine(subDirectoryPath, resourceFileName);

			File.WriteAllText(fileToChangePath, "Test", Encoding.UTF8);

			Thread.Sleep(200);

			Assert.IsFalse(eventTriggered, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAnInvalidResourceFileIsCreated_TheFileResourcesCacheShouldNotBeCleared()
		{
			var resourceProvider = this.CreateResourceProvider();

			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var createdValidResourceFilePath = Path.Combine(resourceProvider.Settings.FileResourcesDirectory.FullName, "Created-file.txt");

			this.CreateFile(createdValidResourceFilePath);

			Thread.Sleep(200);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAnInvalidResourceFileIsCreatedInASubDirectory_TheFileResourcesCacheShouldNotBeCleared()
		{
			var resourceProvider = this.CreateResourceProvider();
			// ReSharper disable PossibleNullReferenceException
			var embeddedResourcesPath = Path.Combine(new DirectoryInfo(Global.ProjectDirectoryPath).Parent.Parent.FullName, @"Embedded-resources\Colors");
			// ReSharper restore PossibleNullReferenceException
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;
			var subDirectoryPath = Directory.CreateDirectory(Path.Combine(fileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			Thread.Sleep(200);

			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var createdValidResourceFilePath = Path.Combine(subDirectoryPath, "Created-file.txt");

			this.CreateFile(createdValidResourceFilePath);

			Thread.Sleep(200);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAnInvalidResourceFileIsDeleted_TheFileResourcesCacheShouldNotBeCleared()
		{
			var resourceFileName = "Colors.txt";
			var resourceProvider = this.CreateResourceProvider();
			// ReSharper disable PossibleNullReferenceException
			var embeddedResourcesPath = Path.Combine(new DirectoryInfo(Global.ProjectDirectoryPath).Parent.Parent.FullName, @"Embedded-resources\Colors");
			// ReSharper restore PossibleNullReferenceException
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;

			File.Copy(Path.Combine(embeddedResourcesPath, resourceFileName), Path.Combine(fileResourcesDirectoryPath, resourceFileName));

			Thread.Sleep(200);

			Assert.AreEqual(7, Directory.GetFiles(fileResourcesDirectoryPath, "*.*", SearchOption.AllDirectories).Length, this.PossibleReasonForFailure);
			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var fileToDeletePath = Path.Combine(fileResourcesDirectoryPath, resourceFileName);

			Assert.IsTrue(File.Exists(fileToDeletePath), "The file \"{0}\" should exist.", fileToDeletePath);

			File.Delete(fileToDeletePath);

			Thread.Sleep(200);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAnInvalidResourceFileIsDeletedInASubDirectory_TheFileResourcesCacheShouldNotBeCleared()
		{
			var resourceFileName = "Colors.txt";
			var resourceProvider = this.CreateResourceProvider();
			// ReSharper disable PossibleNullReferenceException
			var embeddedResourcesPath = Path.Combine(new DirectoryInfo(Global.ProjectDirectoryPath).Parent.Parent.FullName, @"Embedded-resources\Colors");
			// ReSharper restore PossibleNullReferenceException
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;
			var subDirectoryPath = Directory.CreateDirectory(Path.Combine(fileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			File.Copy(Path.Combine(embeddedResourcesPath, resourceFileName), Path.Combine(subDirectoryPath, resourceFileName));

			Thread.Sleep(200);

			Assert.AreEqual(7, Directory.GetFiles(fileResourcesDirectoryPath, "*.*", SearchOption.AllDirectories).Length, this.PossibleReasonForFailure);
			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var fileToDeletePath = Path.Combine(subDirectoryPath, resourceFileName);

			Assert.IsTrue(File.Exists(fileToDeletePath), "The file \"{0}\" should exist.", fileToDeletePath);

			File.Delete(fileToDeletePath);

			Thread.Sleep(200);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAnInvalidResourceFileIsRenamed_TheFileResourcesCacheShouldNotBeCleared()
		{
			var resourceFileName = "Colors.txt";
			var resourceProvider = this.CreateResourceProvider();
			// ReSharper disable PossibleNullReferenceException
			var embeddedResourcesPath = Path.Combine(new DirectoryInfo(Global.ProjectDirectoryPath).Parent.Parent.FullName, @"Embedded-resources\Colors");
			// ReSharper restore PossibleNullReferenceException
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;

			File.Copy(Path.Combine(embeddedResourcesPath, resourceFileName), Path.Combine(fileResourcesDirectoryPath, resourceFileName));

			Thread.Sleep(200);

			Assert.AreEqual(7, Directory.GetFiles(fileResourcesDirectoryPath, "*.*", SearchOption.AllDirectories).Length, this.PossibleReasonForFailure);
			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var fileToRenamePath = Path.Combine(fileResourcesDirectoryPath, resourceFileName);
			var fileToRenameToPath = Path.Combine(Path.GetDirectoryName(fileToRenamePath), Guid.NewGuid() + ".resx");

			File.Move(fileToRenamePath, fileToRenameToPath);

			Thread.Sleep(200);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAnInvalidResourceFileIsRenamedInASubDirectory_TheFileResourcesCacheShouldNotBeCleared()
		{
			var resourceFileName = "Colors.txt";
			var resourceProvider = this.CreateResourceProvider();
			// ReSharper disable PossibleNullReferenceException
			var embeddedResourcesPath = Path.Combine(new DirectoryInfo(Global.ProjectDirectoryPath).Parent.Parent.FullName, @"Embedded-resources\Colors");
			// ReSharper restore PossibleNullReferenceException
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;
			var subDirectoryPath = Directory.CreateDirectory(Path.Combine(fileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			File.Copy(Path.Combine(embeddedResourcesPath, resourceFileName), Path.Combine(subDirectoryPath, resourceFileName));

			Thread.Sleep(200);

			Assert.AreEqual(7, Directory.GetFiles(fileResourcesDirectoryPath, "*.*", SearchOption.AllDirectories).Length, this.PossibleReasonForFailure);
			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var fileToRenamePath = Path.Combine(subDirectoryPath, resourceFileName);
			var fileToRenameToPath = Path.Combine(Path.GetDirectoryName(fileToRenamePath), Guid.NewGuid() + ".resx");

			File.Move(fileToRenamePath, fileToRenameToPath);

			Thread.Sleep(200);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfASubDirectoryIsCreated_TheFileResourcesCacheShouldNotBeCleared()
		{
			var resourceProvider = this.CreateResourceProvider();
			var parentDirectoryPath = Directory.CreateDirectory(Path.Combine(resourceProvider.Settings.FileResourcesDirectory.FullName, @"Directory\Directory")).FullName;

			Thread.Sleep(200);

			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var createdDirectoryPath = Path.Combine(parentDirectoryPath, "Created-directory");

			Directory.CreateDirectory(createdDirectoryPath);

			Thread.Sleep(200);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAValidResourceFileIsChanged_TheFileResourceContentChangedEventShouldBeTriggered()
		{
			var resourceProvider = this.CreateResourceProvider();

			string eventPath = null;
			var eventTriggered = false;

			resourceProvider.FileResourceContentChanged += (sender, e) =>
			{
				eventPath = e.Path;
				eventTriggered = true;
			};

			var fileToChangePath = Path.Combine(resourceProvider.Settings.FileResourcesDirectory.FullName, "Resource" + this.ValidResourceFileExtension);

			File.WriteAllText(fileToChangePath, "Test", Encoding.UTF8);

			Thread.Sleep(200);

			Assert.IsTrue(eventTriggered, this.PossibleReasonForFailure);
			Assert.AreEqual(fileToChangePath, eventPath, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAValidResourceFileIsChanged_TheFileResourcesCacheShouldNotBeCleared()
		{
			var resourceProvider = this.CreateResourceProvider();

			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var fileToChangePath = Path.Combine(resourceProvider.Settings.FileResourcesDirectory.FullName, "Texts" + this.ValidResourceFileExtension);

			File.WriteAllText(fileToChangePath, "Test", Encoding.UTF8);

			Thread.Sleep(200);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAValidResourceFileIsChangedInASubDirectory_TheFileResourceContentChangedEventShouldBeTriggered()
		{
			var resourceFileName = "Texts" + this.ValidResourceFileExtension;
			var resourceProvider = this.CreateResourceProvider();
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;
			var subDirectoryPath = Directory.CreateDirectory(Path.Combine(fileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			File.Copy(Path.Combine(fileResourcesDirectoryPath, resourceFileName), Path.Combine(subDirectoryPath, resourceFileName));

			Thread.Sleep(200);

			string eventPath = null;
			var eventTriggered = false;

			resourceProvider.FileResourceContentChanged += (sender, e) =>
			{
				eventPath = e.Path;
				eventTriggered = true;
			};

			var fileToChangePath = Path.Combine(subDirectoryPath, resourceFileName);

			File.WriteAllText(fileToChangePath, "Test", Encoding.UTF8);

			Thread.Sleep(200);

			Assert.IsTrue(eventTriggered, this.PossibleReasonForFailure, this.PossibleReasonForFailure);
			Assert.AreEqual(fileToChangePath, eventPath, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAValidResourceFileIsChangedInASubDirectory_TheFileResourcesCacheShouldNotBeCleared()
		{
			var resourceFileName = "Texts" + this.ValidResourceFileExtension;
			var resourceProvider = this.CreateResourceProvider();
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;
			var subDirectoryPath = Directory.CreateDirectory(Path.Combine(fileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			File.Copy(Path.Combine(fileResourcesDirectoryPath, resourceFileName), Path.Combine(subDirectoryPath, resourceFileName));

			Thread.Sleep(200);

			Assert.AreEqual(7, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var fileToChangePath = Path.Combine(resourceProvider.Settings.FileResourcesDirectory.FullName, @"Directory\Directory", resourceFileName);

			File.WriteAllText(fileToChangePath, "Test", Encoding.UTF8);

			Thread.Sleep(200);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAValidResourceFileIsCreated_TheFileResourcesCacheShouldBeCleared()
		{
			var resourceProvider = this.CreateResourceProvider();

			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var createdValidResourceFilePath = Path.Combine(resourceProvider.Settings.FileResourcesDirectory.FullName, "Created-file" + this.ValidResourceFileExtension);

			this.CreateFile(createdValidResourceFilePath);

			Thread.Sleep(200);

			Assert.IsNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAValidResourceFileIsCreatedInASubDirectory_TheFileResourcesCacheShouldBeCleared()
		{
			var resourceProvider = this.CreateResourceProvider();
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;
			var subDirectoryPath = Directory.CreateDirectory(Path.Combine(fileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			Thread.Sleep(200);

			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var createdValidResourceFilePath = Path.Combine(subDirectoryPath, "Created-file" + this.ValidResourceFileExtension);

			this.CreateFile(createdValidResourceFilePath);

			Thread.Sleep(200);

			Assert.IsNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAValidResourceFileIsDeleted_TheFileResourcesCacheShouldBeCleared()
		{
			var resourceProvider = this.CreateResourceProvider();

			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var fileToDeletePath = Path.Combine(resourceProvider.Settings.FileResourcesDirectory.FullName, "Texts" + this.ValidResourceFileExtension);

			Assert.IsTrue(File.Exists(fileToDeletePath), "The file \"{0}\" should exist.", fileToDeletePath);

			File.Delete(fileToDeletePath);

			Thread.Sleep(200);

			Assert.IsNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAValidResourceFileIsDeletedInASubDirectory_TheFileResourcesCacheShouldBeCleared()
		{
			var resourceFileName = "Texts" + this.ValidResourceFileExtension;
			var resourceProvider = this.CreateResourceProvider();
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;
			var subDirectoryPath = Directory.CreateDirectory(Path.Combine(fileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			File.Copy(Path.Combine(fileResourcesDirectoryPath, resourceFileName), Path.Combine(subDirectoryPath, resourceFileName));

			Thread.Sleep(200);

			Assert.AreEqual(7, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var fileToDeletePath = Path.Combine(subDirectoryPath, resourceFileName);

			Assert.IsTrue(File.Exists(fileToDeletePath), "The file \"{0}\" should exist.", fileToDeletePath);

			File.Delete(fileToDeletePath);

			Thread.Sleep(200);

			Assert.IsNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAValidResourceFileIsRenamed_TheFileResourcesCacheShouldBeCleared()
		{
			var resourceProvider = this.CreateResourceProvider();

			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var fileToRenamePath = Path.Combine(resourceProvider.Settings.FileResourcesDirectory.FullName, "Texts" + this.ValidResourceFileExtension);
			var fileToRenameToPath = Path.Combine(Path.GetDirectoryName(fileToRenamePath), Guid.NewGuid() + this.ValidResourceFileExtension);

			File.Move(fileToRenamePath, fileToRenameToPath);

			Thread.Sleep(200);

			Assert.IsNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfAValidResourceFileIsRenamedInASubDirectory_TheFileResourcesCacheShouldBeCleared()
		{
			var resourceFileName = "Texts" + this.ValidResourceFileExtension;
			var resourceProvider = this.CreateResourceProvider();
			var fileResourcesDirectoryPath = resourceProvider.Settings.FileResourcesDirectory.FullName;
			var subDirectoryPath = Directory.CreateDirectory(Path.Combine(fileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			File.Copy(Path.Combine(fileResourcesDirectoryPath, resourceFileName), Path.Combine(subDirectoryPath, resourceFileName));

			Thread.Sleep(200);

			Assert.AreEqual(7, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var fileToRenamePath = Path.Combine(subDirectoryPath, resourceFileName);
			var fileToRenameToPath = Path.Combine(Path.GetDirectoryName(fileToRenamePath), Guid.NewGuid() + this.ValidResourceFileExtension);

			File.Move(fileToRenamePath, fileToRenameToPath);

			Thread.Sleep(200);

			Assert.IsNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfTheOptionsFileResourcesDirectoryPathIsChangedAndAValidResourceFileIsCreatedInASubDirectoryInThePreviousFileResourcesDirectory_TheFileResourcesCacheShouldNotBeCleared()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-File-Resources-Directory-Path-Only.json", true);
			var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
			var resourceProvider = (ResourceProvider)serviceProvider.GetService<IResourceProvider>();
			var testContext = serviceProvider.GetService<ITestContext>();
			var subDirectoryPath = Directory.CreateDirectory(Path.Combine(testContext.ConfiguredFileResourcesDirectoryPath, @"Directory\Directory")).FullName;

			resourceProvider.Settings.FileResourcesDirectory = fileSystem.DirectoryInfo.FromDirectoryName(testContext.FileResourcesDirectoryPath);

			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var createdValidResourceFilePath = Path.Combine(subDirectoryPath, "Created-file" + this.ValidResourceFileExtension);

			this.CreateFile(createdValidResourceFilePath);

			Thread.Sleep(200);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void FileResourcesDirectory_IfTheOptionsFileResourcesDirectoryPathIsChangedAndAValidResourceFileIsCreatedInThePreviousFileResourcesDirectory_TheFileResourcesCacheShouldNotBeCleared()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-File-Resources-Directory-Path-Only.json", true);
			var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
			var resourceProvider = (ResourceProvider)serviceProvider.GetService<IResourceProvider>();
			var testContext = serviceProvider.GetService<ITestContext>();
			var previousResourcesDirectoryPath = testContext.ConfiguredFileResourcesDirectoryPath;

			resourceProvider.Settings.FileResourcesDirectory = fileSystem.DirectoryInfo.FromDirectoryName(testContext.FileResourcesDirectoryPath);

			Assert.AreEqual(6, resourceProvider.FileResources.Count(), this.PossibleReasonForFailure);
			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);

			var createdValidResourceFilePath = Path.Combine(previousResourcesDirectoryPath, "Created-file" + this.ValidResourceFileExtension);

			this.CreateFile(createdValidResourceFilePath);

			Thread.Sleep(200);

			Assert.IsNotNull(resourceProvider.FileResourcesCache, this.PossibleReasonForFailure);
		}

		protected internal virtual ResourceProvider GetResourceProvider(IServiceProvider serviceProvider)
		{
			return (ResourceProvider)serviceProvider.GetService<IResourceProvider>();
		}

		[TestMethod]
		public void Settings_EmbeddedResourceAssemblies_Add_ShouldClearTheEmbeddedResourcesCache()
		{
			var serviceProvider = this.BuildServiceProvider();
			var resourceProvider = this.GetResourceProvider(serviceProvider);
			var assemblyHelper = serviceProvider.GetService<IAssemblyHelper>();

			Assert.IsFalse(resourceProvider.EmbeddedResources.Any());

			Assert.IsNotNull(resourceProvider.EmbeddedResourcesCache);

			resourceProvider.Settings.EmbeddedResourceAssemblies.Add(assemblyHelper.Wrap(typeof(string).Assembly));

			Assert.IsNull(resourceProvider.EmbeddedResourcesCache);
		}

		[TestMethod]
		public void Settings_EmbeddedResourceAssemblies_Set_ShouldClearTheEmbeddedResourcesCache()
		{
			var serviceProvider = this.BuildServiceProvider();
			var resourceProvider = this.GetResourceProvider(serviceProvider);
			var assemblyHelper = serviceProvider.GetService<IAssemblyHelper>();

			resourceProvider.Settings.EmbeddedResourceAssemblies.Add(assemblyHelper.Wrap(typeof(string).Assembly));

			// System.Private.CoreLib.xml
			Assert.AreEqual(1, resourceProvider.EmbeddedResources.Count());

			Assert.IsNotNull(resourceProvider.EmbeddedResourcesCache);

			resourceProvider.Settings.EmbeddedResourceAssemblies[0] = assemblyHelper.Wrap(this.GetType().Assembly);

			Assert.IsNull(resourceProvider.EmbeddedResourcesCache);
		}

		#endregion
	}
}