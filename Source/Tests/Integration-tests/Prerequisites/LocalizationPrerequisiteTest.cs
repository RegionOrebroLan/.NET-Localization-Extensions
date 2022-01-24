using System;
using Investigation.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Prerequisites
{
	/// <summary>
	/// Tests for localization in general.
	/// </summary>
	[TestClass]
	public class LocalizationPrerequisiteTest
	{
		#region Fields

		private static string _resourcesRelativePath;
		private static IServiceProvider _serviceProvider;

		#endregion

		#region Properties

		protected internal virtual IServiceProvider ServiceProvider => _serviceProvider;

		#endregion

		#region Methods

		[ClassCleanup]
		public static void Cleanup() { }

		private static IServiceProvider ConfigureServices(string resourcesRelativePath)
		{
			var services = Global.CreateDefaultServices();

			// ReSharper disable ImplicitlyCapturedClosure
			services.AddLocalization(localizationOptions => localizationOptions.ResourcesPath = resourcesRelativePath);
			// ReSharper restore ImplicitlyCapturedClosure

			services.AddSingleton<IStringLocalizerFactory, GitHubSourceCopyOfResourceManagerStringLocalizerFactory>();

			return services.BuildServiceProvider();
		}

		[TestMethod]
		public virtual void GetService_IStringLocalizerFactory_ShouldReturnAnInstanceOfTypeGitHubSourceCopyOfResourceManagerStringLocalizerFactory()
		{
			var stringLocalizerFactory = this.ServiceProvider.GetService<IStringLocalizerFactory>();

			Assert.IsTrue(stringLocalizerFactory.GetType() == typeof(GitHubSourceCopyOfResourceManagerStringLocalizerFactory));
		}

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			if(testContext == null)
				throw new ArgumentNullException(nameof(testContext));

			_resourcesRelativePath = null;

			_serviceProvider = ConfigureServices(_resourcesRelativePath);
		}

		[TestMethod]
		public virtual void LocalizationOptions_IfLocalizationOptionsResourcesPathIsSetToResources_ShouldReflectThat()
		{
			const string resourcesRelativePath = "Resources";

			var stringLocalizerFactory = (GitHubSourceCopyOfResourceManagerStringLocalizerFactory)this.ServiceProvider.GetService<IStringLocalizerFactory>();

			var previousLocalizationOptions = stringLocalizerFactory.LocalizationOptions;
			stringLocalizerFactory.LocalizationOptions = Options.Create(new LocalizationOptions { ResourcesPath = resourcesRelativePath });

			try
			{
				Assert.AreEqual(resourcesRelativePath, stringLocalizerFactory.LocalizationOptions.Value.ResourcesPath);
			}
			finally
			{
				stringLocalizerFactory.LocalizationOptions = previousLocalizationOptions;
			}
		}

		[TestMethod]
		public virtual void LocalizationOptions_ShouldReturnAResourcesPathThatIsNullByDefault()
		{
			var stringLocalizerFactory = (GitHubSourceCopyOfResourceManagerStringLocalizerFactory)this.ServiceProvider.GetService<IStringLocalizerFactory>();

			Assert.IsNull(stringLocalizerFactory.LocalizationOptions.Value.ResourcesPath);
		}

		[TestMethod]
		public virtual void ResourcesRelativePath_IfLocalizationOptionsResourcesPathIsSetToResources_ShouldReturnResourcesDot()
		{
			const string resourcesRelativePath = "Resources";

			var stringLocalizerFactory = (GitHubSourceCopyOfResourceManagerStringLocalizerFactory)this.ServiceProvider.GetService<IStringLocalizerFactory>();

			var previousLocalizationOptions = stringLocalizerFactory.LocalizationOptions;
			stringLocalizerFactory.LocalizationOptions = Options.Create(new LocalizationOptions { ResourcesPath = resourcesRelativePath });

			try
			{
				Assert.AreEqual(resourcesRelativePath + ".", stringLocalizerFactory.ResourcesRelativePath);
			}
			finally
			{
				stringLocalizerFactory.LocalizationOptions = previousLocalizationOptions;
			}
		}

		[TestMethod]
		public virtual void ResourcesRelativePath_ShouldReturnAnEmptyStringByDefault()
		{
			var stringLocalizerFactory = (GitHubSourceCopyOfResourceManagerStringLocalizerFactory)this.ServiceProvider.GetService<IStringLocalizerFactory>();

			Assert.AreEqual(string.Empty, stringLocalizerFactory.ResourcesRelativePath);
		}

		[TestMethod]
		public virtual void StringLocalizer_IfLocalizationOptionsResourcesPathIsSetToNull_Test()
		{
			var stringLocalizerFactory = (GitHubSourceCopyOfResourceManagerStringLocalizerFactory)this.ServiceProvider.GetService<IStringLocalizerFactory>();
			Assert.IsNull(stringLocalizerFactory.LocalizationOptions.Value.ResourcesPath);

			var stringLocalizer = this.ServiceProvider.GetService<IStringLocalizer<RootNamespacedResources.TheClass>>();

			var localizedString = stringLocalizer["Test"];

			Assert.AreEqual("Test", localizedString.Name);
			Assert.AreEqual("RootNamespacedResources.TheClass", localizedString.SearchedLocation);
			Assert.AreEqual("Test", localizedString.Value);
			Assert.IsTrue(localizedString.ResourceNotFound);
		}

		[TestMethod]
		public virtual void StringLocalizer_IfLocalizationOptionsResourcesPathIsSetToResources_Test()
		{
			const string resourcesRelativePath = "Resources";

			var stringLocalizerFactory = (GitHubSourceCopyOfResourceManagerStringLocalizerFactory)this.ServiceProvider.GetService<IStringLocalizerFactory>();

			var previousLocalizationOptions = stringLocalizerFactory.LocalizationOptions;
			stringLocalizerFactory.LocalizationOptions = Options.Create(new LocalizationOptions { ResourcesPath = resourcesRelativePath });

			try
			{
				Assert.AreEqual(resourcesRelativePath, stringLocalizerFactory.LocalizationOptions.Value.ResourcesPath);
				Assert.AreEqual(resourcesRelativePath + ".", stringLocalizerFactory.ResourcesRelativePath);

				var stringLocalizer = this.ServiceProvider.GetService<IStringLocalizer<RootNamespacedResources.TheClass>>();

				var localizedString = stringLocalizer["Test"];

				Assert.AreEqual("Test", localizedString.Name);
				Assert.AreEqual("My.Custom.Namespace.Resources.RootNamespacedResources.TheClass", localizedString.SearchedLocation);
				Assert.AreEqual("Test", localizedString.Value);
				Assert.IsTrue(localizedString.ResourceNotFound);
			}
			finally
			{
				stringLocalizerFactory.LocalizationOptions = previousLocalizationOptions;
			}
		}

		#endregion
	}
}