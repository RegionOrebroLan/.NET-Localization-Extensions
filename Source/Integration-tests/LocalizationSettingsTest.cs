using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Configuration;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization.IntegrationTests
{
	[TestClass]
	public class LocalizationSettingsTest : IntegrationTest
	{
		#region Fields

		private static IAssemblyHelper _assemblyHelper;
		private static IServiceProvider _serviceProvider;

		#endregion

		#region Properties

		protected internal virtual IAssemblyHelper AssemblyHelper => _assemblyHelper ?? (_assemblyHelper = this.ServiceProvider.GetService<IAssemblyHelper>());
		protected internal virtual IServiceProvider ServiceProvider => _serviceProvider;

		#endregion

		#region Methods

		[TestMethod]
		public void AlphabeticalSorting_IfChanged_ShouldTriggerAlphabeticalSortingChanged()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");
			var localizationSettings = (LocalizationSettings) serviceProvider.GetRequiredService<ILocalizationSettings>();

			Assert.IsTrue(localizationSettings.AlphabeticalSorting);

			var numberOfTimesChanged = 0;

			localizationSettings.AlphabeticalSortingChanged += delegate { numberOfTimesChanged++; };

			Assert.AreEqual(0, numberOfTimesChanged);

			localizationSettings.AlphabeticalSorting = true;

			Assert.AreEqual(0, numberOfTimesChanged);

			localizationSettings.AlphabeticalSorting = false;

			Assert.AreEqual(1, numberOfTimesChanged);

			localizationSettings.AlphabeticalSorting = false;

			Assert.AreEqual(1, numberOfTimesChanged);

			localizationSettings.AlphabeticalSorting = true;

			Assert.AreEqual(2, numberOfTimesChanged);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Constructor_OptionsMonitor_IfTheEmbeddedResourceAssembliesParameterContainNullValues_ShouldThrowAnArguementException()
		{
			var serviceProvider = this.BuildServiceProvider((configuration, services) =>
			{
				var localizationOptions = new LocalizationOptions();
				// ReSharper disable AssignNullToNotNullAttribute
				localizationOptions.EmbeddedResourceAssemblies.Add(null);
				// ReSharper restore AssignNullToNotNullAttribute

				var optionsMonitorMock = new Mock<IOptionsMonitor<LocalizationOptions>>();

				optionsMonitorMock.Setup(optionsMonitor => optionsMonitor.CurrentValue).Returns(localizationOptions);

				services.AddSingleton(optionsMonitorMock.Object);
			}, "Configuration-Empty.json");

			try
			{
				serviceProvider.GetRequiredService<ILocalizationSettings>();
			}
			catch(ArgumentException argumentException)
			{
				const string messageStart = "Embedded-resource-assemblies-exception: The patterns-collection can not contain null-values. Values: null";

				if(argumentException.ParamName.Equals("patterns", StringComparison.Ordinal) && argumentException.Message.StartsWith(messageStart, StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void EmbeddedResourceAssemblies_Add_IfTheAssemblyAlreadyExists_ShouldThrowAnArguementException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Default.json");
			var localizationSettings = (LocalizationSettings) serviceProvider.GetRequiredService<ILocalizationSettings>();
			var animalAssembly = serviceProvider.GetRequiredService<IAssemblyHelper>().Wrap(typeof(Animals.TheClass).Assembly);

			Assert.IsTrue(localizationSettings.EmbeddedResourceAssemblies.Contains(animalAssembly));
			Assert.AreEqual(6, localizationSettings.EmbeddedResourceAssemblies.Count);

			try
			{
				localizationSettings.EmbeddedResourceAssemblies.Add(animalAssembly);
			}
			catch(ArgumentException)
			{
				Assert.AreEqual(6, localizationSettings.EmbeddedResourceAssemblies.Count);

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void EmbeddedResourceAssemblies_Add_IfTheValueIsNull_ShouldThrowAnArguementNullException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Default.json");
			var localizationSettings = (LocalizationSettings) serviceProvider.GetRequiredService<ILocalizationSettings>();

			Assert.AreEqual(6, localizationSettings.EmbeddedResourceAssemblies.Count);

			try
			{
				localizationSettings.EmbeddedResourceAssemblies.Add(null);
			}
			catch(ArgumentNullException)
			{
				Assert.AreEqual(6, localizationSettings.EmbeddedResourceAssemblies.Count);

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void EmbeddedResourceAssemblies_Set_IfTheAssemblyAlreadyExists_ShouldThrowAnArguementException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Default.json");
			var localizationSettings = (LocalizationSettings) serviceProvider.GetRequiredService<ILocalizationSettings>();
			var animalAssembly = serviceProvider.GetRequiredService<IAssemblyHelper>().Wrap(typeof(Animals.TheClass).Assembly);

			Assert.IsTrue(localizationSettings.EmbeddedResourceAssemblies.Contains(animalAssembly));
			Assert.AreEqual(6, localizationSettings.EmbeddedResourceAssemblies.Count);

			try
			{
				localizationSettings.EmbeddedResourceAssemblies[1] = animalAssembly;
			}
			catch(ArgumentException)
			{
				Assert.AreEqual(6, localizationSettings.EmbeddedResourceAssemblies.Count);

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void EmbeddedResourceAssemblies_Set_IfTheValueIsNull_ShouldThrowAnArguementException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Default.json");
			var localizationSettings = (LocalizationSettings) serviceProvider.GetRequiredService<ILocalizationSettings>();

			Assert.AreEqual(6, localizationSettings.EmbeddedResourceAssemblies.Count);

			try
			{
				localizationSettings.EmbeddedResourceAssemblies[0] = null;
			}
			catch(ArgumentNullException)
			{
				Assert.AreEqual(6, localizationSettings.EmbeddedResourceAssemblies.Count);

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(DirectoryNotFoundException))]
		public void FileResourcesDirectory_Set_IfTheParameterIsNotNullAndTheDirectoryNotExists_ShouldThrowADirectoryNotFoundException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");
			var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
			var localizationSettings = (LocalizationSettings) serviceProvider.GetRequiredService<ILocalizationSettings>();

			var fileResourcesDirectory = fileSystem.DirectoryInfo.FromDirectoryName("Z:\\" + Guid.NewGuid().ToString());

			Assert.IsFalse(fileResourcesDirectory.Exists);

			try
			{
				localizationSettings.FileResourcesDirectory = fileResourcesDirectory;
			}
			catch(DirectoryNotFoundException directoryNotFoundException)
			{
				if(directoryNotFoundException.Message.Equals($"File-resources-directory-exception: the directory \"{fileResourcesDirectory.FullName}\" does not exist.", StringComparison.OrdinalIgnoreCase))
					throw;
			}
		}

		[TestMethod]
		[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
		public void FileResourcesDirectory_Set_IfTheParameterIsNull_ShouldNotThrowAnException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");
			var localizationSettings = (LocalizationSettings) serviceProvider.GetRequiredService<ILocalizationSettings>();

			Assert.IsNull(localizationSettings.FileResourcesDirectory);

			try
			{
				localizationSettings.FileResourcesDirectory = null;
			}
			catch
			{
				Assert.Fail("Should not throw an exception.");
			}
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

		#endregion
	}
}