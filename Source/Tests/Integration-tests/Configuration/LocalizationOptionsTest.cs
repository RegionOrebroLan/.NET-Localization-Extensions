using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RegionOrebroLan.Localization.Configuration;
using RegionOrebroLan.Localization.Reflection;

namespace IntegrationTests.Configuration
{
	[TestClass]
	public class LocalizationOptionsTest : SerializationTest
	{
		#region Fields

		private static IAssemblyHelper _assemblyHelper;
		private static IServiceProvider _serviceProvider;

		#endregion

		#region Properties

		protected internal virtual IAssemblyHelper AssemblyHelper => _assemblyHelper ??= this.ServiceProvider.GetService<IAssemblyHelper>();
		protected internal virtual IServiceProvider ServiceProvider => _serviceProvider;

		#endregion

		#region Methods

		[TestMethod]
		public void ConfigurationBinding_IfTheConfigurationIsEmpty_ShouldReturnANullFileResourcesDirectoryPath()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");

			var localizationOptions = serviceProvider.GetRequiredService<IOptions<LocalizationOptions>>();
			var localizationOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<LocalizationOptions>>();

			Assert.IsNull(localizationOptions.Value.FileResourcesDirectoryPath);
			Assert.IsNull(localizationOptionsMonitor.CurrentValue.FileResourcesDirectoryPath);
		}

		[TestMethod]
		public void ConfigurationBinding_IfTheFileResourcesDirectoryPathIsAnEmptyString_ShouldReturnAnEmptyFileResourcesDirectoryPath()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-Empty-File-Resources-Directory-Path.json");

			var localizationOptions = serviceProvider.GetRequiredService<IOptions<LocalizationOptions>>();
			var localizationOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<LocalizationOptions>>();

			Assert.AreEqual(string.Empty, localizationOptions.Value.FileResourcesDirectoryPath);
			Assert.AreEqual(string.Empty, localizationOptionsMonitor.CurrentValue.FileResourcesDirectoryPath);
		}

		[TestMethod]
		public void ConfigurationBinding_IfTheFileResourcesDirectoryPathIsNull_ShouldReturnAnEmptyFileResourcesDirectoryPath_EvenIfItShouldReturnANullFileResourcesDirectoryPath_AsISeeIt()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-Null-Valued-File-Resources-Directory-Path.json");

			var localizationOptions = serviceProvider.GetRequiredService<IOptions<LocalizationOptions>>();
			var localizationOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<LocalizationOptions>>();

			Assert.AreEqual(string.Empty, localizationOptions.Value.FileResourcesDirectoryPath);
			Assert.AreEqual(string.Empty, localizationOptionsMonitor.CurrentValue.FileResourcesDirectoryPath);
		}

		[TestMethod]
		public void ConfigurationBinding_IfTheLocalizationIsEmpty_ShouldReturnANullFileResourcesDirectoryPath()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-Empty-Localization.json");

			var localizationOptions = serviceProvider.GetRequiredService<IOptions<LocalizationOptions>>();
			var localizationOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<LocalizationOptions>>();

			Assert.IsNull(localizationOptions.Value.FileResourcesDirectoryPath);
			Assert.IsNull(localizationOptionsMonitor.CurrentValue.FileResourcesDirectoryPath);
		}

		[TestMethod]
		public void ConfigurationBinding_IfThereIsNoFileResourcesDirectoryPath_ShouldReturnANullFileResourcesDirectoryPath()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Without-File-Resources-Directory-Path.json");

			var localizationOptions = serviceProvider.GetRequiredService<IOptions<LocalizationOptions>>();
			var localizationOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<LocalizationOptions>>();

			Assert.IsNull(localizationOptions.Value.FileResourcesDirectoryPath);
			Assert.IsNull(localizationOptionsMonitor.CurrentValue.FileResourcesDirectoryPath);
		}

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			if(testContext == null)
				throw new ArgumentNullException(nameof(testContext));

			var services = new ServiceCollection();

			services.TryAddSingleton<IAssemblyHelper, AssemblyHelper>();
			services.TryAddSingleton<IRootNamespaceResolver, RootNamespaceResolver>();

			_serviceProvider = services.BuildServiceProvider();
		}

		[TestMethod]
		public void JsonDeserialization_IfTheJsonValueContainsDuplicateEmbeddedResourceAssemblies_ShouldDeserializeCorrectly()
		{
			var localizationOptions = JsonConvert.DeserializeObject<LocalizationOptions>("{\"EmbeddedResourceAssemblies\":[\"System.Private.CoreLib\",\"System.Private.CoreLib\"]}");
			Assert.AreEqual(2, localizationOptions.EmbeddedResourceAssemblies.Count);
			Assert.AreEqual("System.Private.CoreLib", localizationOptions.EmbeddedResourceAssemblies[0]);
			Assert.AreEqual("System.Private.CoreLib", localizationOptions.EmbeddedResourceAssemblies[1]);
		}

		[TestMethod]
		public void JsonDeserialization_ShouldDeserializeCorrectly()
		{
			var expectedLocalizationOptions = new LocalizationOptions
			{
				FileResourcesDirectoryPath = "Test"
			};

			expectedLocalizationOptions.EmbeddedResourceAssemblies.Add(this.AssemblyHelper.Wrap(typeof(string).Assembly).Name);
			expectedLocalizationOptions.EmbeddedResourceAssemblies.Add(this.AssemblyHelper.Wrap(this.GetType().Assembly).Name);

			var localizationOptions = JsonConvert.DeserializeObject<LocalizationOptions>("{\"EmbeddedResourceAssemblies\":[\"System.Private.CoreLib\",\"IntegrationTests\"],\"FileResourcesDirectoryPath\":\"Test\"}");

			Assert.IsTrue(localizationOptions.AlphabeticalSorting);
			Assert.AreEqual(2, localizationOptions.EmbeddedResourceAssemblies.Count);
			Assert.AreEqual("System.Private.CoreLib", localizationOptions.EmbeddedResourceAssemblies.ElementAt(0));
			Assert.AreEqual("IntegrationTests", localizationOptions.EmbeddedResourceAssemblies.ElementAt(1));
			Assert.IsTrue(expectedLocalizationOptions.EmbeddedResourceAssemblies.SequenceEqual(localizationOptions.EmbeddedResourceAssemblies));
			Assert.AreEqual("Test", localizationOptions.FileResourcesDirectoryPath);
			Assert.AreEqual(expectedLocalizationOptions.FileResourcesDirectoryPath, localizationOptions.FileResourcesDirectoryPath);
			Assert.IsFalse(localizationOptions.ThrowErrors);

			localizationOptions = JsonConvert.DeserializeObject<LocalizationOptions>("{\"AlphabeticalSorting\":false,\"EmbeddedResourceAssemblies\":[\"System.Private.CoreLib\",\"IntegrationTests\"],\"FileResourcesDirectoryPath\":\"Test\",\"ThrowErrors\":true}");

			Assert.IsFalse(localizationOptions.AlphabeticalSorting);
			Assert.AreEqual(2, localizationOptions.EmbeddedResourceAssemblies.Count);
			Assert.AreEqual("System.Private.CoreLib", localizationOptions.EmbeddedResourceAssemblies.ElementAt(0));
			Assert.AreEqual("IntegrationTests", localizationOptions.EmbeddedResourceAssemblies.ElementAt(1));
			Assert.IsTrue(expectedLocalizationOptions.EmbeddedResourceAssemblies.SequenceEqual(localizationOptions.EmbeddedResourceAssemblies));
			Assert.AreEqual("Test", localizationOptions.FileResourcesDirectoryPath);
			Assert.AreEqual(expectedLocalizationOptions.FileResourcesDirectoryPath, localizationOptions.FileResourcesDirectoryPath);
			Assert.IsTrue(localizationOptions.ThrowErrors);
		}

		[TestMethod]
		public void JsonSerialization_IfTheLocalizationOptionsInstanceHaveNoPropertiesSet_ShouldSerializeCorrectly()
		{
			Assert.AreEqual("{\"AlphabeticalSorting\":true,\"EmbeddedResourceAssemblies\":[],\"IncludeParentCultures\":false,\"ThrowErrors\":false}", this.JsonSerialize(new LocalizationOptions()));
		}

		[TestMethod]
		public void JsonSerialization_IfTheLocalizationOptionsInstanceHavePropertiesSet_ShouldSerializeCorrectly()
		{
			var localizationOptions = new LocalizationOptions
			{
				FileResourcesDirectoryPath = "Test"
			};

			localizationOptions.EmbeddedResourceAssemblies.Add(this.AssemblyHelper.Wrap(typeof(string).Assembly).FullName);
			localizationOptions.EmbeddedResourceAssemblies.Add(this.AssemblyHelper.Wrap(this.GetType().Assembly).FullName);

#if NETCOREAPP3_1
			const int version = 4;
#elif NET5_0
			const int version = 5;
#elif NET6_0
			const int version = 6;
#endif

			Assert.AreEqual($"{{\"AlphabeticalSorting\":true,\"EmbeddedResourceAssemblies\":[\"System.Private.CoreLib, Version={version}.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\",\"IntegrationTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9aeba83ffb1feacc\"],\"FileResourcesDirectoryPath\":\"Test\",\"IncludeParentCultures\":false,\"ThrowErrors\":false}}", this.JsonSerialize(localizationOptions));
		}

		#endregion
	}
}