using IntegrationTests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegionOrebroLan.Localization.DependencyInjection.Extensions;

namespace IntegrationTests
{
	public abstract class IntegrationTest
	{
		#region Fields

		private const string _configurationDirectoryName = "Configuration";
		private const string _emptyDirectoryName = "Empty";
		private const string _fileResourcesDirectoryName = "Resources";
		private const string _possibleReasonForFailure = "Maybe because some IO-operation did not have time to complete. You may try to increase the time waiting for the IO-operation to complete. You can do it by increasing the \"Thread.Sleep()\" parameter.";
		private static string _testResourcesDirectoryPath;
		private static string _testRootDirectoryPath;

		#endregion

		#region Properties

		protected internal virtual string ConfigurationDirectoryName => _configurationDirectoryName;
		protected internal virtual string EmptyDirectoryName => _emptyDirectoryName;
		protected internal virtual string FileResourcesDirectoryName => _fileResourcesDirectoryName;
		protected internal virtual string PossibleReasonForFailure => _possibleReasonForFailure;

		// ReSharper disable PossibleNullReferenceException
		protected internal virtual string TestResourcesDirectoryPath => _testResourcesDirectoryPath ??= new DirectoryInfo(this.TestRootDirectoryPath).Parent.FullName;
		// ReSharper restore PossibleNullReferenceException

		protected internal virtual string TestRootDirectoryPath => _testRootDirectoryPath ??= Path.Combine(Global.ProjectDirectoryPath, Global.TestRootDirectoryRelativePath);

		#endregion

		#region Methods

		protected internal virtual IServiceProvider BuildServiceProvider()
		{
			return this.BuildServiceProvider(false);
		}

		protected internal virtual IServiceProvider BuildServiceProvider(string configurationFileName)
		{
			return this.BuildServiceProvider(null, configurationFileName);
		}

		protected internal virtual IServiceProvider BuildServiceProvider(IEnumerable<string> embeddedResourceAssemblies)
		{
			return this.BuildServiceProvider(embeddedResourceAssemblies, false);
		}

		protected internal virtual IServiceProvider BuildServiceProvider(bool emptyFileResourcesDirectory)
		{
			return this.BuildServiceProvider(Enumerable.Empty<string>(), emptyFileResourcesDirectory);
		}

		protected internal virtual IServiceProvider BuildServiceProvider(IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			return services.BuildServiceProvider();
		}

		protected internal virtual IServiceProvider BuildServiceProvider(Action<IConfiguration, IServiceCollection> additionalServicesConfiguration, string configurationFileName)
		{
			return this.BuildServiceProvider(additionalServicesConfiguration, configurationFileName, false);
		}

		protected internal virtual IServiceProvider BuildServiceProvider(string configurationFileName, bool emptyFileResourcesDirectory)
		{
			return this.BuildServiceProvider(null, configurationFileName, emptyFileResourcesDirectory);
		}

		protected internal virtual IServiceProvider BuildServiceProvider(IEnumerable<string> embeddedResourceAssemblies, bool emptyFileResourcesDirectory)
		{
			var testContext = this.PrepareTest(emptyFileResourcesDirectory);

			var services = Global.CreateDefaultServices();

			services.AddSingleton(testContext);

			services.AddPathBasedLocalization(options =>
			{
				foreach(var pattern in embeddedResourceAssemblies ?? Enumerable.Empty<string>())
				{
					options.EmbeddedResourceAssemblies.Add(pattern);
				}

				options.FileResourcesDirectoryPath = testContext.ConfiguredFileResourcesDirectoryRelativePath;
			});

			return this.BuildServiceProvider(services);
		}

		protected internal virtual IServiceProvider BuildServiceProvider(Action<IConfiguration, IServiceCollection> additionalServicesConfiguration, string configurationFileName, bool emptyFileResourcesDirectory)
		{
			var testContext = this.PrepareTest(configurationFileName, emptyFileResourcesDirectory);

			if(!File.Exists(testContext.ConfigurationFilePath))
				throw new FileNotFoundException("The configuration-file does not exist.", testContext.ConfigurationFilePath);

			var configurationFileContent = File.ReadAllText(testContext.ConfigurationFilePath);
			// We need to escape the back-slashes.
			configurationFileContent = configurationFileContent.Replace("{RESOURCES-PATH}", testContext.ConfiguredFileResourcesDirectoryRelativePath.Replace(@"\", @"\\", StringComparison.OrdinalIgnoreCase), StringComparison.OrdinalIgnoreCase);
			File.WriteAllText(testContext.ConfigurationFilePath, configurationFileContent);

			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.SetBasePath(testContext.RootPath);
			configurationBuilder.AddJsonFile(testContext.ConfigurationFileRelativePath, false, true);

			var configuration = configurationBuilder.Build();

			var services = Global.CreateDefaultServices();

			services.AddSingleton(testContext);

			services.AddPathBasedLocalization(configuration);

			additionalServicesConfiguration?.Invoke(configuration, services);

			return this.BuildServiceProvider(services);
		}

		protected internal virtual void CopyDirectory(string destinationDirectoryPath, string sourceDirectoryPath)
		{
			this.CopyDirectory(destinationDirectoryPath, false, sourceDirectoryPath);
		}

		protected internal virtual void CopyDirectory(string destinationDirectoryPath, bool overwrite, string sourceDirectoryPath)
		{
			if(!Directory.Exists(destinationDirectoryPath))
				throw new DirectoryNotFoundException($"The destination-directory \"{destinationDirectoryPath}\" does not exist.");

			if(!Directory.Exists(sourceDirectoryPath))
				throw new DirectoryNotFoundException($"The source-directory \"{sourceDirectoryPath}\" does not exist.");

			foreach(var subDirectoryPath in Directory.GetDirectories(sourceDirectoryPath, "*", SearchOption.AllDirectories))
			{
				Directory.CreateDirectory(subDirectoryPath.Replace(sourceDirectoryPath, destinationDirectoryPath, StringComparison.OrdinalIgnoreCase));
			}

			foreach(var filePath in Directory.GetFiles(sourceDirectoryPath, "*", SearchOption.AllDirectories))
			{
				File.Copy(filePath, filePath.Replace(sourceDirectoryPath, destinationDirectoryPath, StringComparison.OrdinalIgnoreCase), overwrite);
			}
		}

		protected internal virtual string GetProjectRelativePath(string path)
		{
			return string.IsNullOrWhiteSpace(path) ? path : path.Substring(Global.ProjectDirectoryPath.Length).Trim('\\');
		}

		protected internal virtual ITestContext PrepareTest(bool emptyFileResourcesDirectory)
		{
			return this.PrepareTest(null, emptyFileResourcesDirectory);
		}

		protected internal virtual ITestContext PrepareTest(string configurationFileName, bool emptyFileResourcesDirectory)
		{
			var testDirectoryPath = Path.Combine(this.TestRootDirectoryPath, Guid.NewGuid().ToString());

			var configurationDirectoryPath = Path.Combine(testDirectoryPath, this.ConfigurationDirectoryName);
			Directory.CreateDirectory(configurationDirectoryPath);
			this.CopyDirectory(configurationDirectoryPath, Path.Combine(this.TestResourcesDirectoryPath, this.ConfigurationDirectoryName));
			var configurationDirectoryRelativePath = this.GetProjectRelativePath(configurationDirectoryPath);

			string configurationFilePath = null;
			string configurationFileRelativePath = null;
			if(configurationFileName != null)
			{
				configurationFilePath = Path.Combine(configurationDirectoryPath, configurationFileName);
				configurationFileRelativePath = Path.Combine(configurationDirectoryRelativePath, configurationFileName);
			}

			var emptyDirectoryPath = Path.Combine(testDirectoryPath, this.EmptyDirectoryName);
			Directory.CreateDirectory(emptyDirectoryPath);
			var emptyDirectoryRelativePath = this.GetProjectRelativePath(emptyDirectoryPath);

			var fileResourcesDirectoryPath = Path.Combine(testDirectoryPath, this.FileResourcesDirectoryName);
			Directory.CreateDirectory(fileResourcesDirectoryPath);
			this.CopyDirectory(fileResourcesDirectoryPath, Path.Combine(Global.ProjectDirectoryPath, this.FileResourcesDirectoryName));
			var fileResourcesDirectoryRelativePath = this.GetProjectRelativePath(fileResourcesDirectoryPath);

			var configuredResourcesDirectoryRelativePath = emptyFileResourcesDirectory ? emptyDirectoryRelativePath : fileResourcesDirectoryRelativePath;

			return new TestContext
			{
				ConfigurationDirectoryRelativePath = configurationDirectoryRelativePath,
				ConfigurationDirectoryPath = configurationDirectoryPath,
				ConfigurationFilePath = configurationFilePath,
				ConfigurationFileRelativePath = configurationFileRelativePath,
				ConfiguredFileResourcesDirectoryPath = Path.Combine(Global.ProjectDirectoryPath, configuredResourcesDirectoryRelativePath),
				ConfiguredFileResourcesDirectoryRelativePath = configuredResourcesDirectoryRelativePath,
				EmptyDirectoryPath = emptyDirectoryPath,
				EmptyDirectoryRelativePath = emptyDirectoryRelativePath,
				FileResourcesDirectoryPath = fileResourcesDirectoryPath,
				FileResourcesDirectoryRelativePath = fileResourcesDirectoryRelativePath,
				RootPath = Global.ProjectDirectoryPath,
				TestDirectoryPath = testDirectoryPath,
				TestDirectoryRelativePath = this.GetProjectRelativePath(testDirectoryPath)
			};
		}

		#endregion
	}
}