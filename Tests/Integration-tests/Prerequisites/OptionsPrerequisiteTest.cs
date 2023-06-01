using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using IntegrationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftLocalizationOptions = Microsoft.Extensions.Localization.LocalizationOptions;

namespace IntegrationTests.Prerequisites
{
	[TestClass]
	public class OptionsPrerequisiteTest : IntegrationTest
	{
		#region Methods

		[TestMethod]
		public void ChangingTheConfigurationFile_IfOptionsAreSetupWithConfigurationFiles_ShouldTriggerOptionsMonitorOnChange1()
		{
			var serviceProvider = this.BuildServiceProvider((configuration, services) => { services.Configure<MicrosoftLocalizationOptions>(configuration.GetSection("Localization")); }, "Options-Prerequisite-Test.json");

			var testContext = serviceProvider.GetService<ITestContext>();

			var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<MicrosoftLocalizationOptions>>();
			Assert.AreEqual(testContext.ConfiguredFileResourcesDirectoryRelativePath, optionsMonitor.CurrentValue.ResourcesPath);

			var isChanged = false;
			var onChangeListener = optionsMonitor.OnChange(changedOptions => { isChanged = true; });
			const string changedResourcesPath = "Changed-resources-path";
			var initialValue = File.ReadAllText(testContext.ConfigurationFilePath);
			// We need to escape the back-slashes.
			var initialResourcesPath = testContext.ConfiguredFileResourcesDirectoryRelativePath.Replace(@"\", @"\\", StringComparison.OrdinalIgnoreCase);
			var changedValue = initialValue.Replace(initialResourcesPath, changedResourcesPath, StringComparison.Ordinal);
			File.WriteAllText(testContext.ConfigurationFilePath, changedValue);

			Thread.Sleep(500);

			onChangeListener.Dispose();

			Assert.IsTrue(isChanged);
		}

		[TestMethod]
		public void ChangingTheConfigurationFile_IfOptionsAreSetupWithConfigurationFiles_ShouldTriggerOptionsMonitorOnChange2()
		{
			var serviceProvider = this.BuildServiceProvider((configuration, services) => { services.Configure<MicrosoftLocalizationOptions>(configuration.GetSection("Localization")); }, "Options-Prerequisite-Test.json");

			var testContext = serviceProvider.GetService<ITestContext>();

			var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<MicrosoftLocalizationOptions>>();
			Assert.AreEqual(testContext.ConfiguredFileResourcesDirectoryRelativePath, optionsMonitor.CurrentValue.ResourcesPath);

			var isChanged = false;
			MicrosoftLocalizationOptions onChangeOptions = null;
			string onChangeOptionsName = null;

			var onChangeListener = optionsMonitor.OnChange((options, optionsName) =>
			{
				isChanged = true;
				onChangeOptions = options;
				onChangeOptionsName = optionsName;
			});

			Assert.IsFalse(isChanged);
			Assert.IsNull(onChangeOptions);
			Assert.IsNull(onChangeOptionsName);

			const string changedResourcesPath = "Changed-resources-path";

			var initialValue = File.ReadAllText(testContext.ConfigurationFilePath);
			// We need to escape the back-slashes.
			var initialResourcesPath = testContext.ConfiguredFileResourcesDirectoryRelativePath.Replace(@"\", @"\\", StringComparison.OrdinalIgnoreCase);
			var changedValue = initialValue.Replace(initialResourcesPath, changedResourcesPath, StringComparison.Ordinal);
			File.WriteAllText(testContext.ConfigurationFilePath, changedValue);

			Thread.Sleep(500);

			onChangeListener.Dispose();

			Assert.IsTrue(isChanged);
			Assert.IsNotNull(onChangeOptions);
			Assert.AreEqual(changedResourcesPath, onChangeOptions.ResourcesPath);
			Assert.IsNotNull(onChangeOptionsName);
			Assert.AreEqual(string.Empty, onChangeOptionsName);
		}

		protected internal virtual IEnumerable<IDisposable> GetRegistrations<TOptions>(IOptionsMonitor<TOptions> optionsMonitor) where TOptions : class, new()
		{
			var registrationsField = typeof(OptionsMonitor<TOptions>).GetField("_registrations", BindingFlags.Instance | BindingFlags.NonPublic);

			return (IEnumerable<IDisposable>)registrationsField?.GetValue(optionsMonitor);
		}

		[TestMethod]
		public void Options_DefaultName_ShouldReturnAnEmptyString()
		{
			Assert.AreEqual(string.Empty, Options.DefaultName);
		}

		[TestMethod]
		public void Options_Property_Set_IfOptionsAreSetupWithActionAndNotWithConfigurationFiles_ShouldNotTriggerOptionsMonitorOnChange()
		{
			const string initialResourcesPath = "Initial-resources-path";
			var serviceProvider = new ServiceCollection().Configure<MicrosoftLocalizationOptions>(localizationOptions => { localizationOptions.ResourcesPath = initialResourcesPath; }).BuildServiceProvider();

			var options = serviceProvider.GetService<IOptions<MicrosoftLocalizationOptions>>();
			var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<MicrosoftLocalizationOptions>>();

			Assert.AreEqual(initialResourcesPath, options.Value.ResourcesPath);
			Assert.AreEqual(initialResourcesPath, optionsMonitor.CurrentValue.ResourcesPath);

			const string changedResourcesPath = "Changed-resources-path";
			var isChanged = false;

			optionsMonitor.OnChange(changedOptions => { isChanged = true; });

			options.Value.ResourcesPath = changedResourcesPath;

			Assert.IsFalse(isChanged);
		}

		[TestMethod]
		public void Options_Property_Set_IfOptionsAreSetupWithConfigurationFiles_ShouldNotTriggerOptionsMonitorOnChange()
		{
			var serviceProvider = this.BuildServiceProvider((configuration, services) => { services.Configure<MicrosoftLocalizationOptions>(configuration.GetSection("Localization")); }, "Options-Prerequisite-Test.json");

			var testContext = serviceProvider.GetService<ITestContext>();

			var options = serviceProvider.GetService<IOptions<MicrosoftLocalizationOptions>>();
			var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<MicrosoftLocalizationOptions>>();

			Assert.AreEqual(testContext.ConfiguredFileResourcesDirectoryRelativePath, options.Value.ResourcesPath);
			Assert.AreEqual(testContext.ConfiguredFileResourcesDirectoryRelativePath, optionsMonitor.CurrentValue.ResourcesPath);

			const string changedResourcesPath = "Changed-resources-path";
			var isChanged = false;

			optionsMonitor.OnChange(changedOptions => { isChanged = true; });

			options.Value.ResourcesPath = changedResourcesPath;

			Assert.IsFalse(isChanged);
		}

		[TestMethod]
		public void Options_Value_And_OptionsMonitor_CurrentValue_IfOptionsAreSetupWithActionAndNotWithConfigurationFiles_ShouldNotReturnTheSameInstance()
		{
			var serviceProvider = new ServiceCollection()
				.Configure<MicrosoftLocalizationOptions>(localizationOptions => { localizationOptions.ResourcesPath = "Initial-resources-path"; })
				.BuildServiceProvider();

			var options = serviceProvider.GetService<IOptions<MicrosoftLocalizationOptions>>();
			var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<MicrosoftLocalizationOptions>>();

			Assert.IsFalse(ReferenceEquals(options.Value, optionsMonitor.CurrentValue)); // Do not really understand this.
		}

		[TestMethod]
		public void Options_Value_And_OptionsMonitor_CurrentValue_IfOptionsAreSetupWithConfigurationFiles_ShouldNotReturnTheSameInstance()
		{
			var serviceProvider = this.BuildServiceProvider((configuration, services) => { services.Configure<MicrosoftLocalizationOptions>(configuration.GetSection("Localization")); }, "Options-Prerequisite-Test.json");

			var options = serviceProvider.GetService<IOptions<MicrosoftLocalizationOptions>>();
			var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<MicrosoftLocalizationOptions>>();

			Assert.IsFalse(ReferenceEquals(options.Value, optionsMonitor.CurrentValue)); // Do not really understand this.
		}

		[TestMethod]
		public void OptionsMonitor_IfOptionsAreSetupWithActionAndNotWithConfigurationFiles_ShouldHaveNoRegistrations()
		{
			var serviceProvider = new ServiceCollection().Configure<MicrosoftLocalizationOptions>(localizationOptions => { }).BuildServiceProvider();
			var optionsMonitor = (OptionsMonitor<MicrosoftLocalizationOptions>)serviceProvider.GetService<IOptionsMonitor<MicrosoftLocalizationOptions>>();
			var registrations = this.GetRegistrations(optionsMonitor).ToArray();
			Assert.IsFalse(registrations.Any());
			Assert.AreEqual(0, registrations.Length);
		}

		[TestMethod]
		public void OptionsMonitor_IfOptionsAreSetupWithConfigurationFiles_ShouldHaveRegistrations()
		{
			var serviceProvider = this.BuildServiceProvider((configuration, services) => { services.Configure<MicrosoftLocalizationOptions>(configuration.GetSection("Localization")); }, "Options-Prerequisite-Test.json");

			var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<MicrosoftLocalizationOptions>>();
			var registrations = this.GetRegistrations(optionsMonitor).ToArray();
			Assert.IsTrue(registrations.Any());
			Assert.AreEqual(1, registrations.Length);
		}

		[TestMethod]
		public void ServiceProvider_IfOptionsAreSetupWithActionAndNotWithConfigurationFiles_ShouldReturnTheCorrectOptionsRelatedInstances()
		{
			const string resourcesPath = "Test";
			var serviceProvider = new ServiceCollection().Configure<MicrosoftLocalizationOptions>(localizationOptions => localizationOptions.ResourcesPath = resourcesPath).BuildServiceProvider();

			Assert.AreEqual(resourcesPath, serviceProvider.GetService<IOptions<MicrosoftLocalizationOptions>>().Value.ResourcesPath);
			Assert.AreEqual(resourcesPath, serviceProvider.GetService<IOptionsMonitor<MicrosoftLocalizationOptions>>().CurrentValue.ResourcesPath);
			Assert.AreEqual(resourcesPath, serviceProvider.GetService<IOptionsMonitorCache<MicrosoftLocalizationOptions>>().GetOrAdd(Options.DefaultName, () => throw new InvalidOperationException()).ResourcesPath);
		}

		[TestMethod]
		public void ServiceProvider_IfOptionsAreSetupWithConfigurationFiles_ShouldReturnTheCorrectOptionsRelatedInstances()
		{
			var serviceProvider = this.BuildServiceProvider((configuration, services) => { services.Configure<MicrosoftLocalizationOptions>(configuration.GetSection("Localization")); }, "Options-Prerequisite-Test.json");

			var testContext = serviceProvider.GetService<ITestContext>();

			Assert.AreEqual(testContext.ConfiguredFileResourcesDirectoryRelativePath, serviceProvider.GetService<IOptions<MicrosoftLocalizationOptions>>().Value.ResourcesPath);
			Assert.AreEqual(testContext.ConfiguredFileResourcesDirectoryRelativePath, serviceProvider.GetService<IOptionsMonitor<MicrosoftLocalizationOptions>>().CurrentValue.ResourcesPath);
			Assert.AreEqual(testContext.ConfiguredFileResourcesDirectoryRelativePath, serviceProvider.GetService<IOptionsMonitorCache<MicrosoftLocalizationOptions>>().GetOrAdd(Options.DefaultName, () => throw new InvalidOperationException()).ResourcesPath);
		}

		#endregion
	}
}