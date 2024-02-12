using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Abstractions;
using System.Reflection;
using IntegrationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization;
using LocalizationOptions = RegionOrebroLan.Localization.Configuration.LocalizationOptions;

namespace IntegrationTests
{
	/// <summary>
	/// Tests for localization in general.
	/// </summary>
	[TestClass]
	public class GeneralLocalizationTest : IntegrationTest
	{
		#region Methods

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Configuration_IfAnyDuplicateValueIsConfiguredForEmbeddedResourceAssemblies_ShouldThrowAnInvalidOperationException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-Duplicate-EmbeddedResourceAssemblies.json");

			try
			{
				// ReSharper disable UnusedVariable
				var localizationOptions = serviceProvider.GetService<IOptions<LocalizationOptions>>().Value;
				// ReSharper restore UnusedVariable
			}
			catch(InvalidOperationException invalidOperationException)
			{
				if(invalidOperationException.Message.Equals("Localization configuration-error: The embedded-resource-assemblies collection can not contain duplicates. Embedded-resource-assemblies: \"System.Private.CoreLib\", \"System.Private.CoreLib\"", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Configuration_IfAnyEmptyValueIsConfiguredForEmbeddedResourceAssemblies_ShouldThrowAnInvalidOperationException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-EmbeddedResourceAssemblies-Where-One-Is-Empty.json");

			try
			{
				// ReSharper disable UnusedVariable
				var localizationOptions = serviceProvider.GetService<IOptions<LocalizationOptions>>().Value;
				// ReSharper restore UnusedVariable
			}
			catch(InvalidOperationException invalidOperationException)
			{
				if(invalidOperationException.Message.Equals("Localization configuration-error: The embedded-resource-assemblies collection can not contain null-values or empty values.", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Configuration_IfAnyNullValueIsConfiguredForEmbeddedResourceAssemblies_ShouldThrowAnInvalidOperationException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-EmbeddedResourceAssemblies-Where-One-Is-Null.json");

			try
			{
				// ReSharper disable UnusedVariable
				var localizationOptions = serviceProvider.GetService<IOptions<LocalizationOptions>>().Value;
				// ReSharper restore UnusedVariable
			}
			catch(InvalidOperationException invalidOperationException)
			{
				if(invalidOperationException.Message.Equals("Localization configuration-error: The embedded-resource-assemblies collection can not contain null-values or empty values.", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		public void Configuration_IfChanged_WillBeHandled()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Default.json");
			var stringLocalizer = (StringLocalizer)((StringLocalizer)serviceProvider.GetService<IStringLocalizer>()).Clone(CultureInfo.GetCultureInfo("en"));
			var testContext = serviceProvider.GetService<ITestContext>();

			Assert.AreEqual(34, stringLocalizer.GetAllStrings(false).Count(), this.PossibleReasonForFailure);

			var configurationContent = File.ReadAllText(testContext.ConfigurationFilePath);
			var configuredFileResourcesDirectoryRelativePathValue = testContext.ConfiguredFileResourcesDirectoryRelativePath.Replace(@"\", @"\\", StringComparison.Ordinal);
			var emptyDirectoryRelativePathValue = testContext.EmptyDirectoryRelativePath.Replace(@"\", @"\\", StringComparison.Ordinal);
			configurationContent = configurationContent.Replace(configuredFileResourcesDirectoryRelativePathValue, emptyDirectoryRelativePathValue, StringComparison.Ordinal);
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(300);

			Assert.AreEqual(24, stringLocalizer.GetAllStrings(false).Count(), this.PossibleReasonForFailure);

			var configurationContentLines = File.ReadAllLines(testContext.ConfigurationFilePath).Where((value, index) => index is < 2 or > 9).ToArray();
			configurationContent = string.Join(Environment.NewLine, configurationContentLines);
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(300);

			Assert.AreEqual(0, stringLocalizer.GetAllStrings(false).Count(), this.PossibleReasonForFailure);

			configurationContent = File.ReadAllText(testContext.ConfigurationFilePath);
			configurationContent = configurationContent.Replace(emptyDirectoryRelativePathValue, configuredFileResourcesDirectoryRelativePathValue, StringComparison.Ordinal);
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(300);

			Assert.AreEqual(10, stringLocalizer.GetAllStrings(false).Count(), this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void Configuration_IfEmbeddedResourceAssembliesAreConfigured_TheLocalizationSettingsShouldIncludeThem()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Default.json");
			var localizationSettings = serviceProvider.GetRequiredService<IDynamicLocalizationSettings>();

			Assert.AreEqual(6, localizationSettings.EmbeddedResourceAssemblies.Count);
			Assert.AreEqual("Animals, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9aeba83ffb1feacc", localizationSettings.EmbeddedResourceAssemblies[0].FullName);
			Assert.AreEqual("Colors, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9aeba83ffb1feacc", localizationSettings.EmbeddedResourceAssemblies[1].FullName);
			Assert.AreEqual("Numbers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9aeba83ffb1feacc", localizationSettings.EmbeddedResourceAssemblies[2].FullName);
			Assert.AreEqual("Prioritized-words, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9aeba83ffb1feacc", localizationSettings.EmbeddedResourceAssemblies[3].FullName);
			Assert.AreEqual("Root-namespaced-resources, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9aeba83ffb1feacc", localizationSettings.EmbeddedResourceAssemblies[4].FullName);
			Assert.AreEqual("Words, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9aeba83ffb1feacc", localizationSettings.EmbeddedResourceAssemblies[5].FullName);
		}

		[TestMethod]
		[ExpectedException(typeof(DirectoryNotFoundException))]
		public void Configuration_IfTheFileResourcesDirectoryPathDoesNotExist_ShouldThrowADirectoryNotFoundException()
		{
			IServiceProvider serviceProvider = null;

			try
			{
				serviceProvider = this.BuildServiceProvider("Configuration-With-Nonexistent-File-Resources-Directory-Path.json");
			}
			catch
			{
				Assert.Fail("Unexpected failure.");
			}

			try
			{
				serviceProvider.GetService<IStringLocalizer>();
			}
			catch(DirectoryNotFoundException directoryNotFoundException)
			{
				if(directoryNotFoundException.Message.StartsWith("File-resources-directory-exception: the directory \"", StringComparison.OrdinalIgnoreCase) && directoryNotFoundException.Message.EndsWith("\" does not exist.", StringComparison.OrdinalIgnoreCase))
					throw;
			}
		}

		[TestMethod]
		public void Configuration_IfTheFileResourcesDirectoryPathIsAnEmptyString_TheFileResourcesDirectoryPathOfTheLocalizationOptionsShouldReturnAnEmptyString()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-Empty-File-Resources-Directory-Path.json");

			var localizationOptions = serviceProvider.GetService<IOptions<LocalizationOptions>>().Value;

			Assert.AreEqual(string.Empty, localizationOptions.FileResourcesDirectoryPath);
		}

		[TestMethod]
		public void Configuration_IfTheFileResourcesDirectoryPathIsAnEmptyString_WillResultInALookupForFileResourcesInTheResourcesDirectoryOfTheApplicationRoot()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-Empty-File-Resources-Directory-Path.json");

			var stringLocalizer = (StringLocalizer)((StringLocalizer)serviceProvider.GetService<IStringLocalizer>()).Clone(CultureInfo.GetCultureInfo("en-US"));

			Assert.AreEqual(24, stringLocalizer.GetAllStrings().Count());
		}

		[TestMethod]
		public void Configuration_IfTheFileResourcesDirectoryPathIsIsAnEmptyString_WillResultInALookupForFileResourcesInTheApplicationRoot()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-Empty-File-Resources-Directory-Path.json");

			var stringLocalizer = (StringLocalizer)((StringLocalizer)serviceProvider.GetService<IStringLocalizer>()).Clone(CultureInfo.GetCultureInfo("en-US"));

			Assert.AreEqual(24, stringLocalizer.GetAllStrings().Count());
		}

		[TestMethod]
		public void Configuration_IfTheFileResourcesDirectoryPathIsNotConfigured_TheFileResourcesDirectoryPathOfTheLocalizationOptionsShouldReturnNull()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");

			var localizationOptions = serviceProvider.GetService<IOptions<LocalizationOptions>>().Value;

			Assert.IsNull(localizationOptions.FileResourcesDirectoryPath);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Configuration_OnChange_IfAnyDuplicateValueIsConfiguredForEmbeddedResourceAssemblies_ShouldThrowAnInvalidOperationException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");
			var localizationOptionsMonitor = serviceProvider.GetService<IOptionsMonitor<LocalizationOptions>>();

			try
			{
				// ReSharper disable UnusedVariable
				var localizationOptions = localizationOptionsMonitor.CurrentValue;
				// ReSharper restore UnusedVariable
			}
			catch(InvalidOperationException)
			{
				Assert.Fail("An exception should not be thrown when initial configuration is done with \"Configuration-Empty.json\".");
			}

			var testContext = serviceProvider.GetService<ITestContext>();
			File.WriteAllText(testContext.ConfigurationFilePath, File.ReadAllText(Path.Combine(testContext.ConfigurationDirectoryPath, "Configuration-With-Duplicate-EmbeddedResourceAssemblies.json")));

			Thread.Sleep(400);

			try
			{
				// ReSharper disable UnusedVariable
				var localizationOptions = localizationOptionsMonitor.CurrentValue;
				// ReSharper restore UnusedVariable
			}
			catch(InvalidOperationException invalidOperationException)
			{
				if(invalidOperationException.Message.Equals("Localization configuration-error: The embedded-resource-assemblies collection can not contain duplicates. Embedded-resource-assemblies: \"System.Private.CoreLib\", \"System.Private.CoreLib\"", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Configuration_OnChange_IfAnyEmptyValueIsConfiguredForEmbeddedResourceAssemblies_ShouldThrowAnInvalidOperationException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");
			var localizationOptionsMonitor = serviceProvider.GetService<IOptionsMonitor<LocalizationOptions>>();

			try
			{
				// ReSharper disable UnusedVariable
				var localizationOptions = localizationOptionsMonitor.CurrentValue;
				// ReSharper restore UnusedVariable
			}
			catch(InvalidOperationException)
			{
				Assert.Fail("An exception should not be thrown when initial configuration is done with \"Configuration-Empty.json\".");
			}

			var testContext = serviceProvider.GetService<ITestContext>();
			File.WriteAllText(testContext.ConfigurationFilePath, File.ReadAllText(Path.Combine(testContext.ConfigurationDirectoryPath, "Configuration-With-EmbeddedResourceAssemblies-Where-One-Is-Empty.json")));

			Thread.Sleep(400);

			try
			{
				// ReSharper disable UnusedVariable
				var localizationOptions = localizationOptionsMonitor.CurrentValue;
				// ReSharper restore UnusedVariable
			}
			catch(InvalidOperationException invalidOperationException)
			{
				if(invalidOperationException.Message.Equals("Localization configuration-error: The embedded-resource-assemblies collection can not contain null-values or empty values.", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		public void Configuration_OnChange_IfAnyInvalidAssemblyNameIsConfiguredForEmbeddedResourceAssemblies_ShouldNotChangeTheStateOfLocalizationSettingsButLogAnErrorAndRegisterARuntimeConfigurationException()
		{
			var testStart = DateTime.Now;
			var logger = new TestLogger();

			var serviceProvider = this.BuildServiceProvider((configuration, services) =>
			{
				var loggerFactoryMock = new Mock<ILoggerFactory>();
				loggerFactoryMock.Setup(loggerFactory => loggerFactory.CreateLogger(It.IsAny<string>())).Returns(logger);

				services.AddSingleton(loggerFactoryMock.Object);
			}, "Configuration-With-File-Resources-Directory-Path-Only.json");

			var localizationSettings = (DynamicLocalizationSettings)serviceProvider.GetRequiredService<IDynamicLocalizationSettings>();

			var alphabeticalSorting = localizationSettings.AlphabeticalSorting;
			var embeddedResourceAssemblies = localizationSettings.EmbeddedResourceAssemblies;
			var fileResourcesDirectory = localizationSettings.FileResourcesDirectory;
			var includeParentCultures = localizationSettings.IncludeParentCultures;
			var throwErrors = localizationSettings.ThrowErrors;

			Assert.IsFalse(localizationSettings.RuntimeConfigurationExceptions.Any());

			var testContext = serviceProvider.GetService<ITestContext>();
			File.WriteAllText(testContext.ConfigurationFilePath, File.ReadAllText(Path.Combine(testContext.ConfigurationDirectoryPath, "Configuration-With-EmbeddedResourceAssemblies-Where-Any-Has-An-Invalid-Name.json")));

			Thread.Sleep(500);

			var errorLog = logger.Logs.FirstOrDefault(log => log.Item1 == LogLevel.Error);

			Assert.IsNotNull(errorLog, this.PossibleReasonForFailure);

			Assert.AreEqual(alphabeticalSorting, localizationSettings.AlphabeticalSorting);
			Assert.IsTrue(embeddedResourceAssemblies.SequenceEqual(localizationSettings.EmbeddedResourceAssemblies));
			Assert.AreEqual(fileResourcesDirectory?.FullName, localizationSettings.FileResourcesDirectory.FullName);
			Assert.AreEqual(includeParentCultures, localizationSettings.IncludeParentCultures);
			Assert.AreEqual(throwErrors, localizationSettings.ThrowErrors);

			var exception = errorLog.Item4;
			var message = errorLog.Item3;

			Assert.IsNotNull(exception);
			Assert.IsTrue(exception is InvalidOperationException);

			const string expectedExceptionMessage = "Embedded-resource-assemblies-exception: The patterns-collection contains invalid values. Values: \"System.Private.CoreLib\", \"Invalid-Assembly-Name\", \"System\"";
			Assert.AreEqual(expectedExceptionMessage, exception.Message);

			Assert.IsNotNull(message);
			Assert.AreEqual("Error on options-changed.", message.ToString());

			Assert.AreEqual(1, localizationSettings.RuntimeConfigurationExceptions.Count);

			var runtimeConfigurationExceptionEntry = localizationSettings.RuntimeConfigurationExceptions.FirstOrDefault();

			var runtimeConfigurationException = runtimeConfigurationExceptionEntry.Key;
			Assert.IsNotNull(runtimeConfigurationException);
			Assert.IsTrue(runtimeConfigurationException is InvalidOperationException);
			Assert.AreEqual("Error on options-changed.", runtimeConfigurationException.Message);

			var exceptionTimestamp = runtimeConfigurationExceptionEntry.Value;
			Assert.IsTrue(exceptionTimestamp > testStart);

			var runtimeConfigurationInnerException = runtimeConfigurationException.InnerException;
			Assert.IsNotNull(runtimeConfigurationInnerException);
			Assert.IsTrue(runtimeConfigurationInnerException is InvalidOperationException);
			Assert.AreEqual(expectedExceptionMessage, runtimeConfigurationInnerException.Message);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Configuration_OnChange_IfAnyNullValueIsConfiguredForEmbeddedResourceAssemblies_ShouldThrowAnInvalidOperationException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");
			var localizationOptionsMonitor = serviceProvider.GetService<IOptionsMonitor<LocalizationOptions>>();

			try
			{
				// ReSharper disable UnusedVariable
				var localizationOptions = localizationOptionsMonitor.CurrentValue;
				// ReSharper restore UnusedVariable
			}
			catch(InvalidOperationException)
			{
				Assert.Fail("An exception should not be thrown when initial configuration is done with \"Configuration-Empty.json\".");
			}

			var testContext = serviceProvider.GetService<ITestContext>();
			File.WriteAllText(testContext.ConfigurationFilePath, File.ReadAllText(Path.Combine(testContext.ConfigurationDirectoryPath, "Configuration-With-EmbeddedResourceAssemblies-Where-One-Is-Null.json")));

			Thread.Sleep(400);

			try
			{
				// ReSharper disable UnusedVariable
				var localizationOptions = localizationOptionsMonitor.CurrentValue;
				// ReSharper restore UnusedVariable
			}
			catch(InvalidOperationException invalidOperationException)
			{
				if(invalidOperationException.Message.Equals("Localization configuration-error: The embedded-resource-assemblies collection can not contain null-values or empty values.", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[SuppressMessage("Maintainability", "CA1506:Avoid excessive class coupling")]
		public void Configuration_OnChange_IfTheFileResourcesDirectoryPathIsChangedToAPathThatDoesNotExist_ShouldNotChangeTheStateOfLocalizationSettingsButLogAnErrorAndRegisterARuntimeConfigurationException()
		{
			var testStart = DateTime.Now;
			var logger = new TestLogger();

			var serviceProvider = this.BuildServiceProvider((configuration, services) =>
			{
				var loggerFactoryMock = new Mock<ILoggerFactory>();
				loggerFactoryMock.Setup(loggerFactory => loggerFactory.CreateLogger(It.IsAny<string>())).Returns(logger);

				services.AddSingleton(loggerFactoryMock.Object);
			}, "Configuration-With-File-Resources-Directory-Path-Only.json");

			var hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();
			var localizationSettings = (DynamicLocalizationSettings)serviceProvider.GetRequiredService<IDynamicLocalizationSettings>();
			var stringLocalizer = (StringLocalizer)((StringLocalizer)serviceProvider.GetService<IStringLocalizer>()).Clone(CultureInfo.GetCultureInfo("en"));
			var testContext = serviceProvider.GetRequiredService<ITestContext>();

			Assert.IsNotNull(localizationSettings.FileResourcesDirectory);
			Assert.IsTrue(localizationSettings.FileResourcesDirectory.Exists);
			Assert.IsFalse(localizationSettings.RuntimeConfigurationExceptions.Any());
			Assert.AreEqual(10, stringLocalizer.GetAllStrings(false).Count(), this.PossibleReasonForFailure);

			var fileResourcesDirectoryPath = localizationSettings.FileResourcesDirectory.FullName;
			var newFileResourcesDirectoryRelativePath = Guid.NewGuid().ToString();

			var newConfigurationContent = File.ReadAllText(Path.Combine(testContext.ConfigurationDirectoryPath, "Configuration-With-File-Resources-Directory-Path-Only.json")).Replace(testContext.ConfiguredFileResourcesDirectoryRelativePath.Replace(@"\", @"\\", StringComparison.Ordinal), newFileResourcesDirectoryRelativePath, StringComparison.OrdinalIgnoreCase);
			File.WriteAllText(testContext.ConfigurationFilePath, newConfigurationContent);

			Thread.Sleep(500);

			var errorLog = logger.Logs.FirstOrDefault(log => log.Item1 == LogLevel.Error);

			Assert.IsNotNull(errorLog, this.PossibleReasonForFailure);

			Assert.AreEqual(fileResourcesDirectoryPath, localizationSettings.FileResourcesDirectory.FullName);

			var exception = errorLog.Item4;
			var message = errorLog.Item3;

			Assert.IsNotNull(exception);
			Assert.IsTrue(exception is DirectoryNotFoundException);

			var expectedExceptionMessage = $"File-resources-directory-exception: The directory \"{hostEnvironment.ContentRootPath}\\{newFileResourcesDirectoryRelativePath}\" does not exist.";
			Assert.AreEqual(expectedExceptionMessage, exception.Message);

			Assert.IsNotNull(message);
			Assert.AreEqual("Error on options-changed.", message.ToString());

			Assert.AreEqual(1, localizationSettings.RuntimeConfigurationExceptions.Count, $"Should contain only one but contains the following: {string.Join(", ", localizationSettings.RuntimeConfigurationExceptions.Keys.Select(item => item.Message))}. {this.PossibleReasonForFailure}");

			var runtimeConfigurationExceptionEntry = localizationSettings.RuntimeConfigurationExceptions.FirstOrDefault();

			var runtimeConfigurationException = runtimeConfigurationExceptionEntry.Key;
			Assert.IsNotNull(runtimeConfigurationException);
			Assert.IsTrue(runtimeConfigurationException is InvalidOperationException);
			Assert.AreEqual("Error on options-changed.", runtimeConfigurationException.Message);

			var exceptionTimestamp = runtimeConfigurationExceptionEntry.Value;
			Assert.IsTrue(exceptionTimestamp > testStart);

			var runtimeConfigurationInnerException = runtimeConfigurationException.InnerException;
			Assert.IsNotNull(runtimeConfigurationInnerException);
			Assert.IsTrue(runtimeConfigurationInnerException is DirectoryNotFoundException);
			Assert.AreEqual(expectedExceptionMessage, runtimeConfigurationInnerException.Message);

			Assert.AreEqual(10, stringLocalizer.GetAllStrings(false).Count());
		}

		[TestMethod]
		public void Configuration_RuntimeChangesShouldBeHandled()
		{
			const int sleepTime = 1000;

			// Step-1
			var serviceProvider = this.BuildServiceProvider("Configuration-Test-Step-1.json");

			var hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();
			var localizationOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<LocalizationOptions>>();
			var localizationSettings = serviceProvider.GetRequiredService<IDynamicLocalizationSettings>();
			var stringLocalizer = (StringLocalizer)((StringLocalizer)serviceProvider.GetRequiredService<IStringLocalizer>()).Clone(CultureInfo.GetCultureInfo("en"));
			var testContext = serviceProvider.GetRequiredService<ITestContext>();

			var fileResourcesDirectoryPath = localizationOptionsMonitor.CurrentValue.FileResourcesDirectoryPath;

			Assert.AreEqual(0, ((DynamicLocalizationSettings)localizationSettings).ConfiguredEmbeddedResourceAssemblies.Count());
			Assert.AreEqual(0, localizationSettings.EmbeddedResourceAssemblies.Count);

			Assert.IsNotNull(localizationSettings.FileResourcesDirectory);
			Assert.IsTrue(localizationSettings.FileResourcesDirectory.Exists);

			var allLocalizedStrings = stringLocalizer.GetAllStrings(false).ToArray();
			Assert.AreEqual(10, allLocalizedStrings.Length);
			Assert.AreEqual("An-empty-entry", allLocalizedStrings.First().Name);

			// Step-2
			var configurationContent = File.ReadAllText(Path.Combine(testContext.ConfigurationDirectoryPath, "Configuration-Test-Step-2.json")).Replace("{RESOURCES-PATH}", fileResourcesDirectoryPath.Replace(@"\", @"\\", StringComparison.Ordinal), StringComparison.Ordinal);
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(sleepTime);

			Assert.AreEqual(6, ((DynamicLocalizationSettings)localizationSettings).ConfiguredEmbeddedResourceAssemblies.Count(), this.PossibleReasonForFailure);
			Assert.AreEqual(6, localizationSettings.EmbeddedResourceAssemblies.Count, this.PossibleReasonForFailure);

			Assert.IsFalse(((DynamicLocalizationSettings)localizationSettings).RuntimeConfigurationExceptions.Any());

			Assert.IsNotNull(localizationSettings.FileResourcesDirectory);
			Assert.IsTrue(localizationSettings.FileResourcesDirectory.Exists);

			allLocalizedStrings = stringLocalizer.GetAllStrings(false).ToArray();
			Assert.AreEqual(34, allLocalizedStrings.Length);
			Assert.AreEqual("Examples.First-example", allLocalizedStrings.First().Name);

			// Step-3
			configurationContent = File.ReadAllText(Path.Combine(testContext.ConfigurationDirectoryPath, "Configuration-Test-Step-3.json")).Replace("{RESOURCES-PATH}", fileResourcesDirectoryPath.Replace(@"\", @"\\", StringComparison.Ordinal), StringComparison.Ordinal);
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(sleepTime);

			Assert.AreEqual(6, localizationSettings.EmbeddedResourceAssemblies.Count, this.PossibleReasonForFailure);

			Assert.IsFalse(((DynamicLocalizationSettings)localizationSettings).RuntimeConfigurationExceptions.Any());

			Assert.IsNotNull(localizationSettings.FileResourcesDirectory);
			Assert.IsTrue(localizationSettings.FileResourcesDirectory.Exists);

			allLocalizedStrings = stringLocalizer.GetAllStrings(false).ToArray();
			Assert.AreEqual(34, allLocalizedStrings.Length);
			Assert.AreEqual("An-empty-entry", allLocalizedStrings.First().Name);

			// Step-4
			configurationContent = File.ReadAllText(Path.Combine(testContext.ConfigurationDirectoryPath, "Configuration-Test-Step-4.json"));
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(sleepTime);

			Assert.AreEqual(6, localizationSettings.EmbeddedResourceAssemblies.Count, this.PossibleReasonForFailure);

			Assert.IsFalse(((DynamicLocalizationSettings)localizationSettings).RuntimeConfigurationExceptions.Any());

			Assert.IsNotNull(localizationSettings.FileResourcesDirectory);
			Assert.IsTrue(localizationSettings.FileResourcesDirectory.Exists);
			Assert.AreEqual(hostEnvironment.ContentRootPath, localizationSettings.FileResourcesDirectory.FullName);

			allLocalizedStrings = stringLocalizer.GetAllStrings(false).ToArray();
			Assert.AreEqual(41, allLocalizedStrings.Length);
			Assert.AreEqual("An-empty-entry", allLocalizedStrings.First().Name);

			// Step-5
			configurationContent = File.ReadAllText(Path.Combine(testContext.ConfigurationDirectoryPath, "Configuration-Test-Step-5.json"));
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(sleepTime);

			Assert.AreEqual(6, localizationSettings.EmbeddedResourceAssemblies.Count, this.PossibleReasonForFailure);

			Assert.IsFalse(((DynamicLocalizationSettings)localizationSettings).RuntimeConfigurationExceptions.Any());

			Assert.IsNull(localizationSettings.FileResourcesDirectory);

			allLocalizedStrings = stringLocalizer.GetAllStrings(false).ToArray();
			Assert.AreEqual(24, allLocalizedStrings.Length);
			Assert.AreEqual("Blue", allLocalizedStrings.First().Name);

			// Step-6
			configurationContent = File.ReadAllText(Path.Combine(testContext.ConfigurationDirectoryPath, "Configuration-Test-Step-6.json"));
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(sleepTime);

			Assert.AreEqual(2, localizationSettings.EmbeddedResourceAssemblies.Count, this.PossibleReasonForFailure);

			Assert.IsFalse(((DynamicLocalizationSettings)localizationSettings).RuntimeConfigurationExceptions.Any());

			Assert.IsNull(localizationSettings.FileResourcesDirectory);

			allLocalizedStrings = stringLocalizer.GetAllStrings(false).ToArray();
			Assert.AreEqual(4, allLocalizedStrings.Length);
			Assert.AreEqual("Numbers.One", allLocalizedStrings.First().Name);

			// Step-7
			configurationContent = File.ReadAllText(Path.Combine(testContext.ConfigurationDirectoryPath, "Configuration-Test-Step-7.json"));
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(sleepTime);

			Assert.AreEqual(0, localizationSettings.EmbeddedResourceAssemblies.Count, this.PossibleReasonForFailure);

			Assert.IsFalse(((DynamicLocalizationSettings)localizationSettings).RuntimeConfigurationExceptions.Any());

			Assert.IsNull(localizationSettings.FileResourcesDirectory);

			allLocalizedStrings = stringLocalizer.GetAllStrings(false).ToArray();
			Assert.AreEqual(0, allLocalizedStrings.Length);
		}

		[TestMethod]
		public void Configuration_StaticCache_IfConfigurationChanged_ShouldNotClearTheCache()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Default-StaticCache.json");
			var stringLocalizer = (StringLocalizer)((StringLocalizer)serviceProvider.GetService<IStringLocalizer>()).Clone(CultureInfo.GetCultureInfo("en"));
			var testContext = serviceProvider.GetService<ITestContext>();

			Assert.AreEqual(34, stringLocalizer.GetAllStrings(false).Count(), this.PossibleReasonForFailure);

			var configurationContent = File.ReadAllText(testContext.ConfigurationFilePath);
			var configuredFileResourcesDirectoryRelativePathValue = testContext.ConfiguredFileResourcesDirectoryRelativePath.Replace(@"\", @"\\", StringComparison.Ordinal);
			var emptyDirectoryRelativePathValue = testContext.EmptyDirectoryRelativePath.Replace(@"\", @"\\", StringComparison.Ordinal);
			configurationContent = configurationContent.Replace(configuredFileResourcesDirectoryRelativePathValue, emptyDirectoryRelativePathValue, StringComparison.Ordinal);
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(300);

			Assert.AreEqual(34, stringLocalizer.GetAllStrings(false).Count(), this.PossibleReasonForFailure);

			var configurationContentLines = File.ReadAllLines(testContext.ConfigurationFilePath).Where((value, index) => index is < 2 or > 9).ToArray();
			configurationContent = string.Join(Environment.NewLine, configurationContentLines);
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(300);

			Assert.AreEqual(34, stringLocalizer.GetAllStrings(false).Count(), this.PossibleReasonForFailure);

			configurationContent = File.ReadAllText(testContext.ConfigurationFilePath);
			configurationContent = configurationContent.Replace(emptyDirectoryRelativePathValue, configuredFileResourcesDirectoryRelativePathValue, StringComparison.Ordinal);
			File.WriteAllText(testContext.ConfigurationFilePath, configurationContent);

			Thread.Sleep(300);

			Assert.AreEqual(34, stringLocalizer.GetAllStrings(false).Count(), this.PossibleReasonForFailure);
		}

		[TestMethod]
		public void Configuration_StaticCache_ShouldRegisterStaticCacheLocalizationProviderAsLocalizationProvider()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Default-StaticCache.json");
			var localizationProvider = serviceProvider.GetService<ILocalizationProvider>();
			Assert.IsTrue(localizationProvider is StaticCacheLocalizationProvider);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void LocalizationSettings_IfAnyInvalidAssemblyNameIsConfiguredForEmbeddedResourceAssemblies_ShouldThrowAnInvalidOperationException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-EmbeddedResourceAssemblies-Where-Any-Has-An-Invalid-Name.json");

			try
			{
				serviceProvider.GetRequiredService<IDynamicLocalizationSettings>();
			}
			catch(InvalidOperationException invalidOperationException)
			{
				const string message = "Embedded-resource-assemblies-exception: The patterns-collection contains invalid values. Values: \"System.Private.CoreLib\", \"Invalid-Assembly-Name\", \"System\"";
				const string innerExceptionMessage = "The assembly \"Invalid-Assembly-Name\" is not loaded at runtime.";

				if(invalidOperationException.Message.Equals(message, StringComparison.OrdinalIgnoreCase) && invalidOperationException.InnerException is InvalidOperationException && invalidOperationException.InnerException.Message.Equals(innerExceptionMessage, StringComparison.OrdinalIgnoreCase))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(DirectoryNotFoundException))]
		public void LocalizationSettings_IfTheFileResourcesDirectoryIsChangedToADirectoryThatDoesNotExist_ShouldThrowADirectoryNotFoundException()
		{
			IFileSystem fileSystem = null;
			IDynamicLocalizationSettings settings = null;

			try
			{
				var serviceProvider = this.BuildServiceProvider("Configuration-With-File-Resources-Directory-Path-Only.json", true);
				fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
				settings = serviceProvider.GetRequiredService<IDynamicLocalizationSettings>();
				var stringLocalizer = (StringLocalizer)((StringLocalizer)serviceProvider.GetService<IStringLocalizer>()).Clone(CultureInfo.GetCultureInfo("en-US"));

				Assert.AreEqual(0, stringLocalizer.GetAllStrings(false).Count(), this.PossibleReasonForFailure);
			}
			catch
			{
				Assert.Fail("Unexpected failure.");
			}

			var fileResourcesDirectoryPath = "Z:\\" + Guid.NewGuid();

			try
			{
				settings.FileResourcesDirectory = fileSystem.DirectoryInfo.New(fileResourcesDirectoryPath);
			}
			catch(DirectoryNotFoundException directoryNotFoundException)
			{
				var message = $"File-resources-directory-exception: The directory \"{fileResourcesDirectoryPath}\" does not exist.";

				if(directoryNotFoundException.Message.Equals(message, StringComparison.OrdinalIgnoreCase))
					throw;
			}
		}

		[TestMethod]
		public void ResourceFile_Entries_IfAnEntryHasALookupToAPathThatDoesNotExist_ItShouldNotBeFound()
		{
			const string nonExistingPath = "this.path.does.not.exist";
			var serviceProvider = this.BuildServiceProvider("Configuration-With-File-Resources-Directory-Path-Only.json");
			var testContext = serviceProvider.GetService<ITestContext>();

			// Remove the xml-files.
			foreach(var xmlFilePath in Directory.GetFiles(testContext.ConfiguredFileResourcesDirectoryPath, "*.xml"))
			{
				File.Delete(xmlFilePath);
			}

			var resourceFilePath = Path.Combine(testContext.ConfiguredFileResourcesDirectoryPath, "Texts.en.json");
			var resourceContentLines = new List<string>();

			foreach(var line in File.ReadAllLines(resourceFilePath))
			{
				if(line.Contains("First text: en", StringComparison.OrdinalIgnoreCase))
					resourceContentLines.Add("\t\t\t\t\t\t\t\t\t\t\t\t\"Lookup\": \"" + nonExistingPath + "\"");
				else
					resourceContentLines.Add(line);
			}

			File.WriteAllText(resourceFilePath, string.Join(Environment.NewLine, resourceContentLines));

			var stringLocalizer = (StringLocalizer)((StringLocalizer)serviceProvider.GetService<IStringLocalizer>()).Clone(CultureInfo.GetCultureInfo("en"));
			var localizedStrings = stringLocalizer.GetAllStrings(false).ToArray();

			Assert.AreEqual(5, localizedStrings.Length);

			var localizedString = localizedStrings[0];
			Assert.AreEqual("Controllers.HomeController.First-text", localizedString.Name);
			Assert.AreEqual("[Value for name \"Controllers.HomeController.First-text\" and culture \"en\" is missing.]", localizedString.Value);
			Assert.IsTrue(localizedString.ResourceNotFound);
			Assert.IsTrue(localizedString.SearchedLocation.Contains("Lookup: \"" + nonExistingPath + "\"", StringComparison.OrdinalIgnoreCase));
			Assert.IsTrue(localizedString.SearchedLocation.Contains("Lookup-value: null", StringComparison.OrdinalIgnoreCase));
			Assert.IsTrue(localizedString.SearchedLocation.Contains("Value: null", StringComparison.OrdinalIgnoreCase));

			localizedString = localizedStrings[1];
			Assert.AreEqual("ReSharperTestRunner.Controllers.HomeController.Second-text", localizedString.Name);
			Assert.AreEqual("Second text: en, \"\\Integration-tests\\Resources\\Texts.en.json\"", localizedString.Value);
			Assert.IsFalse(localizedString.ResourceNotFound);

			localizedString = localizedStrings[4];
			Assert.AreEqual("Very.Deep.Test.First.Second.Third.Fourth.Fifth.Sixth.Seventh.Eighth.Ninth.Tenth.Text", localizedString.Name);
			Assert.AreEqual("Very deep test-value: en, \"\\Integration-tests\\Resources\\Texts.en.json\"", localizedString.Value);
			Assert.IsFalse(localizedString.ResourceNotFound);
		}

		[TestMethod]
		public void ResourceFile_Entries_IfAnEntryHasBothALookupAndAValue_ThenTheValueWillApply()
		{
			const string nonExistingPath = "this.path.does.not.exist";
			var serviceProvider = this.BuildServiceProvider("Configuration-With-File-Resources-Directory-Path-Only.json");
			var testContext = serviceProvider.GetService<ITestContext>();

			// Remove the xml-files.
			foreach(var xmlFilePath in Directory.GetFiles(testContext.ConfiguredFileResourcesDirectoryPath, "*.xml"))
			{
				File.Delete(xmlFilePath);
			}

			var resourceFilePath = Path.Combine(testContext.ConfiguredFileResourcesDirectoryPath, "Texts.en.json");
			var resourceContentLines = new List<string>();

			foreach(var line in File.ReadAllLines(resourceFilePath))
			{
				if(line.Contains("First text: en", StringComparison.OrdinalIgnoreCase))
					resourceContentLines.Add("\t\t\t\t\t\t\t\t\t\t\t\t\"Lookup\": \"" + nonExistingPath + "\",");

				resourceContentLines.Add(line);
			}

			File.WriteAllText(resourceFilePath, string.Join(Environment.NewLine, resourceContentLines));

			var stringLocalizer = (StringLocalizer)((StringLocalizer)serviceProvider.GetService<IStringLocalizer>()).Clone(CultureInfo.GetCultureInfo("en"));
			var localizedStrings = stringLocalizer.GetAllStrings(false).ToArray();

			Assert.AreEqual(5, localizedStrings.Length);

			var localizedString = localizedStrings[0];
			Assert.AreEqual("Controllers.HomeController.First-text", localizedString.Name);
			Assert.AreEqual("First text: en, \"\\Integration-tests\\Resources\\Texts.en.json\"", localizedString.Value);
			Assert.IsFalse(localizedString.ResourceNotFound);
			Assert.IsTrue(localizedString.SearchedLocation.Contains("Lookup: \"" + nonExistingPath + "\"", StringComparison.OrdinalIgnoreCase));
			Assert.IsTrue(localizedString.SearchedLocation.Contains("Lookup-value: null", StringComparison.OrdinalIgnoreCase));
			Assert.IsTrue(localizedString.SearchedLocation.Contains("Value: \"First text: en, \"\\Integration-tests\\Resources\\Texts.en.json\"\"", StringComparison.OrdinalIgnoreCase));

			localizedString = localizedStrings[1];
			Assert.AreEqual("ReSharperTestRunner.Controllers.HomeController.Second-text", localizedString.Name);
			Assert.AreEqual("Second text: en, \"\\Integration-tests\\Resources\\Texts.en.json\"", localizedString.Value);
			Assert.IsFalse(localizedString.ResourceNotFound);

			localizedString = localizedStrings[4];
			Assert.AreEqual("Very.Deep.Test.First.Second.Third.Fourth.Fifth.Sixth.Seventh.Eighth.Ninth.Tenth.Text", localizedString.Name);
			Assert.AreEqual("Very deep test-value: en, \"\\Integration-tests\\Resources\\Texts.en.json\"", localizedString.Value);
			Assert.IsFalse(localizedString.ResourceNotFound);
		}

		[TestMethod]
		public void ServiceProvider_Test()
		{
			var serviceProvider = this.BuildServiceProvider();

			Assert.IsNotNull(serviceProvider.GetService<ILocalizationProvider>());
			Assert.IsTrue(serviceProvider.GetService<ILocalizationProvider>() is DynamicCacheLocalizationProvider);

			Assert.IsNotNull(serviceProvider.GetService<IOptions<LocalizationOptions>>());

			Assert.IsNotNull(serviceProvider.GetService<IStringLocalizerFactory>());
			Assert.IsTrue(serviceProvider.GetService<IStringLocalizerFactory>() is StringLocalizerFactory);

			Assert.IsNotNull(serviceProvider.GetService<IStringLocalizer>());
			Assert.IsTrue(serviceProvider.GetService<IStringLocalizer>() is StringLocalizer);
		}

		[TestMethod]
		public void StringLocalizer_GetAllStrings_ShouldReturnACollectionOfLocalizedStringsOrderedByNameByDefault()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Default.json");

			// Remove embedded xml-resources.
			var localizationSettings = serviceProvider.GetRequiredService<IDynamicLocalizationSettings>();
			Assert.AreEqual("Animals", localizationSettings.EmbeddedResourceAssemblies[0].Name);
			localizationSettings.EmbeddedResourceAssemblies.RemoveAt(0);

			// Remove the xml-files.
			var testContext = serviceProvider.GetService<ITestContext>();
			foreach(var xmlFilePath in Directory.GetFiles(testContext.ConfiguredFileResourcesDirectoryPath, "*.xml"))
			{
				File.Delete(xmlFilePath);
			}

			var stringLocalizer = serviceProvider.GetService<IStringLocalizer>();

			var culture = CultureInfo.InvariantCulture;
			this.StringLocalizer_GetAllStrings_ShouldReturnACollectionOfLocalizedStringsOrderedByNameByDefault(culture, false, 7, stringLocalizer);
			this.StringLocalizer_GetAllStrings_ShouldReturnACollectionOfLocalizedStringsOrderedByNameByDefault(culture, true, 7, stringLocalizer);

			culture = CultureInfo.GetCultureInfo("en");
			this.StringLocalizer_GetAllStrings_ShouldReturnACollectionOfLocalizedStringsOrderedByNameByDefault(culture, false, 22, stringLocalizer);
			this.StringLocalizer_GetAllStrings_ShouldReturnACollectionOfLocalizedStringsOrderedByNameByDefault(culture, true, 26, stringLocalizer);

			culture = CultureInfo.GetCultureInfo("fi");
			this.StringLocalizer_GetAllStrings_ShouldReturnACollectionOfLocalizedStringsOrderedByNameByDefault(culture, false, 19, stringLocalizer);
			this.StringLocalizer_GetAllStrings_ShouldReturnACollectionOfLocalizedStringsOrderedByNameByDefault(culture, true, 23, stringLocalizer);

			culture = CultureInfo.GetCultureInfo("sv");
			this.StringLocalizer_GetAllStrings_ShouldReturnACollectionOfLocalizedStringsOrderedByNameByDefault(culture, false, 19, stringLocalizer);
			this.StringLocalizer_GetAllStrings_ShouldReturnACollectionOfLocalizedStringsOrderedByNameByDefault(culture, true, 23, stringLocalizer);
		}

		protected internal virtual void StringLocalizer_GetAllStrings_ShouldReturnACollectionOfLocalizedStringsOrderedByNameByDefault(CultureInfo culture, bool includeParentCultures, int numberOfItems, IStringLocalizer stringLocalizer)
		{
			var localizedStrings = ((StringLocalizer)stringLocalizer).Clone(culture).GetAllStrings(includeParentCultures).ToArray();
			var localizedStringNames = localizedStrings.Select(item => item.Name).ToArray();
			var localizedStringNameList = localizedStringNames.ToList();
			localizedStringNameList.Sort();
			var failMessage = $"Failed for culture \"{culture}\", number-of-items should be \"{numberOfItems}\" and include-parent-cultures set to \"{includeParentCultures}\".";
			Assert.AreEqual(numberOfItems, localizedStrings.Length, failMessage);
			Assert.AreEqual(numberOfItems, localizedStringNames.Length, failMessage);
			Assert.AreEqual(numberOfItems, localizedStringNameList.Count, failMessage);
			Assert.IsTrue(localizedStringNames.SequenceEqual(localizedStringNameList), failMessage);
		}

		/// <summary>
		/// This works because there is a node with name "TestHost.Controllers.HomeController" in resource-file "Integration-tests\Resources\Texts.en-US.json".
		/// When executing this integration-test the application-assembly will be "testhost" and therefore "Controllers.HomeController.Second-text" will be found.
		/// Depending on how we run the tests, depending on the host, "testhost" may also be "ReSharperTestRunner" or "TestHost.x86". In the future maybe also some other name.
		/// </summary>
		[TestMethod]
		public void StringLocalizer_NotTyped_IfThereAreResourceNamesPrefixedWithTheAssemblyName_ShouldResolveLocalizedStringsRequestedWithoutTheAssemblyNamePrefix()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-File-Resources-Directory-Path-Only.json");
			var stringLocalizer = (StringLocalizer)((StringLocalizer)serviceProvider.GetService<IStringLocalizer>()).Clone(CultureInfo.GetCultureInfo("en"));

			// Just to keep track of this test. So we, in the future, can fix this test with any other possible assembly running the tests.
			var entryAssembly = Assembly.GetEntryAssembly();
			Assert.AreEqual(entryAssembly?.FullName, stringLocalizer.Assembly.FullName);
			var possibleAssemblies = new HashSet<string>(new[] { "ReSharperTestRunner", "TestHost", "TestHost.x86" }, StringComparer.OrdinalIgnoreCase);
			Assert.IsTrue(possibleAssemblies.Contains(stringLocalizer.Assembly.Name), $"The entry-assembly is {entryAssembly?.GetName().Name} and is not contained in the list: {string.Join(", ", possibleAssemblies)}. You need to add the current entry-assembly name in Resources/Texts.json and Resources/Texts.en.json.");

			var localizedString = stringLocalizer["Controllers.HomeController.Second-text"];
			Assert.IsFalse(localizedString.ResourceNotFound);
			Assert.AreEqual("Second text: en, \"\\Integration-tests\\Resources\\Texts.en.json\"", localizedString.Value);
		}

		[TestMethod]
		public void StringLocalizer_ShouldHandleFormatArgumentsProperly()
		{
			var arguments = new List<object>
			{
				"A",
				"b",
				"C",
				"d",
				"E",
				"f"
			};

			var serviceProvider = this.BuildServiceProvider("Configuration-With-File-Resources-Directory-Path-Only.json");
			var stringLocalizer = (StringLocalizer)((StringLocalizer)serviceProvider.GetService<IStringLocalizer>()).Clone(CultureInfo.GetCultureInfo("en"));

			Assert.AreEqual("\"A\", b, \"C\", d, \"E\"", stringLocalizer[@"format-examples\five-arguments", arguments.ToArray()].Value);

			arguments.RemoveAt(5);
			Assert.AreEqual("\"A\", b, \"C\", d, \"E\"", stringLocalizer[@"format-examples\five-arguments", arguments.ToArray()].Value);

			arguments.RemoveAt(4);
			var formatExceptionThrown = false;

			try
			{
				// ReSharper disable UnusedVariable
				var localizedString = stringLocalizer[@"format-examples\five-arguments", arguments.ToArray()];
				// ReSharper restore UnusedVariable
			}
			catch(FormatException)
			{
				formatExceptionThrown = true;
			}

			if(!formatExceptionThrown)
				Assert.Fail("A format-exception should have been thrown.");
		}

		#endregion
	}
}