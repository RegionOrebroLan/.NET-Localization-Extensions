using System;
using System.IO;
using System.Reflection;
using Investigation.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Prerequisites
{
	[TestClass]
	public class ResourceManagerStringLocalizerFactoryPrerequisiteTest
	{
		#region Fields

		private static InvestigatableResourceManagerStringLocalizerFactory _investigatableResourceManagerStringLocalizerFactory;
		private static readonly LocalizationOptions _localizationOptions = new LocalizationOptions { ResourcesPath = "TestResources/ResxResources" };
		private static readonly ILoggerFactory _loggerFactory = new LoggerFactory();

		#endregion

		#region Properties

		protected internal virtual InvestigatableResourceManagerStringLocalizerFactory InvestigatableResourceManagerStringLocalizerFactory => _investigatableResourceManagerStringLocalizerFactory ?? (_investigatableResourceManagerStringLocalizerFactory = new InvestigatableResourceManagerStringLocalizerFactory(Options.Create(this.LocalizationOptions), this.LoggerFactory));
		protected internal virtual LocalizationOptions LocalizationOptions => _localizationOptions;
		protected internal virtual ILoggerFactory LoggerFactory => _loggerFactory;

		#endregion

		#region Methods

		[TestMethod]
		public void Constructor_IfTheLocalizationOptionsResourcesPathIsNull_ShouldSetTheResourcesRelativePathFieldToAnEmptyString()
		{
			var localizationOptions = new LocalizationOptions { ResourcesPath = null };

			var stringLocalizerFactory = new ResourceManagerStringLocalizerFactory(Options.Create(localizationOptions), this.LoggerFactory);

			var resourcesRelativePath = (string)typeof(ResourceManagerStringLocalizerFactory).GetField("_resourcesRelativePath", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(stringLocalizerFactory);

			Assert.AreEqual(string.Empty, resourcesRelativePath);
		}

		[TestMethod]
		public void Create_ShouldCreateAStringLocalizerWithCorrectResourceBaseName()
		{
			var stringLocalizer = (InvestigatableResourceManagerStringLocalizer)this.InvestigatableResourceManagerStringLocalizerFactory.Create(typeof(string));
			var firstBaseName = stringLocalizer.ResourceBaseName;
			Assert.AreEqual("System.Private.CoreLib.TestResources.ResxResources.System.String", firstBaseName);

			stringLocalizer = (InvestigatableResourceManagerStringLocalizer)this.InvestigatableResourceManagerStringLocalizerFactory.Create(this.GetType());
			var secondBaseName = stringLocalizer.ResourceBaseName;
			Assert.AreEqual("IntegrationTests.TestResources.ResxResources.Prerequisites.ResourceManagerStringLocalizerFactoryPrerequisiteTest", secondBaseName);
		}

		[TestMethod]
		public void Create_WithOneTypeParameter_IfTheLocalizationOptionsResourcesPathIsNull_ShouldReturnAStringLocalizerWithTheResourceBaseNameFieldSetToTheFullNameOfTheTypeParameter()
		{
			var localizationOptions = new LocalizationOptions { ResourcesPath = null };

			var stringLocalizerFactory = new ResourceManagerStringLocalizerFactory(Options.Create(localizationOptions), this.LoggerFactory);

			var stringLocalizer = stringLocalizerFactory.Create(typeof(string));

			var resourceBaseName = (string)typeof(ResourceManagerStringLocalizer).GetField("_resourceBaseName", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(stringLocalizer);

			Assert.AreEqual(typeof(string).FullName, resourceBaseName);
		}

		[TestMethod]
		public void Create_WithOneTypeParameter_IfTheLocalizationOptionsResourcesPathIsTest_ShouldReturnAStringLocalizerWithTheResourceBaseNameFieldSetToTheNameOfTheAssemblyPlusDotPlusTestPlusDotPlusTheFullNameOfTheTypeParameter()
		{
			var localizationOptions = new LocalizationOptions { ResourcesPath = "Test" };

			var stringLocalizerFactory = new ResourceManagerStringLocalizerFactory(Options.Create(localizationOptions), this.LoggerFactory);

			var stringLocalizer = stringLocalizerFactory.Create(typeof(string));

			var resourceBaseName = (string)typeof(ResourceManagerStringLocalizer).GetField("_resourceBaseName", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(stringLocalizer);

			Assert.AreEqual(typeof(string).Assembly.GetName().Name + ".Test." + typeof(string).FullName, resourceBaseName);
		}

		[TestMethod]
		public void Create_WithOneTypeParameter_IfTheLocalizationOptionsResourcesPathIsWhiteSpace_ShouldReturnAStringLocalizerWithTheResourceBaseNameFieldSetToTheNameOfTheAssemblyPlusDotPlusWhiteSpacePlusDotPlusTheFullNameOfTheTypeParameter()
		{
			var localizationOptions = new LocalizationOptions { ResourcesPath = " " };

			var stringLocalizerFactory = new ResourceManagerStringLocalizerFactory(Options.Create(localizationOptions), this.LoggerFactory);

			var stringLocalizer = stringLocalizerFactory.Create(typeof(string));

			var resourceBaseName = (string)typeof(ResourceManagerStringLocalizer).GetField("_resourceBaseName", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(stringLocalizer);

			Assert.AreEqual(typeof(string).Assembly.GetName().Name + ". ." + typeof(string).FullName, resourceBaseName);
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void Create_WithTwoParameters_IfTheLocationParameterIsAnInvalidAssemblyName_ShouldThrowAFileNotFoundException()
		{
			var stringLocalizerFactory = new ResourceManagerStringLocalizerFactory(Options.Create(this.LocalizationOptions), this.LoggerFactory);

			try
			{
				stringLocalizerFactory.Create("Test", "Invalid-assembly-name");
			}
			catch(FileNotFoundException fileNotFoundException)
			{
				if(fileNotFoundException.FileName.Equals("Invalid-assembly-name, Culture=neutral, PublicKeyToken=null", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		public void Create_WithTwoParameters_IfTheLocationParameterIsAValidAssemblyName_ShouldNotThrowAnException()
		{
			var stringLocalizerFactory = new ResourceManagerStringLocalizerFactory(Options.Create(this.LocalizationOptions), this.LoggerFactory);

			stringLocalizerFactory.Create("Test", "System");
		}

		[TestMethod]
		public void GetResourceLocationAttribute_Test()
		{
			var stringLocalizerFactory = new InvestigatableResourceManagerStringLocalizerFactory(Options.Create(this.LocalizationOptions), this.LoggerFactory);

			Assert.IsNull(stringLocalizerFactory.GetProtectedResourceLocationAttribute(typeof(string).Assembly));
			Assert.IsNull(stringLocalizerFactory.GetProtectedResourceLocationAttribute(this.GetType().Assembly));
		}

		[TestMethod]
		public void GetRootNamespace_Test()
		{
			var getRootNamespaceMethod = typeof(ResourceManagerStringLocalizerFactory).GetMethod("GetRootNamespace", BindingFlags.Instance | BindingFlags.NonPublic);
			var stringLocalizerFactory = new ResourceManagerStringLocalizerFactory(Options.Create(this.LocalizationOptions), this.LoggerFactory);

			var assembly = typeof(string).Assembly;
			Assert.AreEqual(assembly.GetName().Name, getRootNamespaceMethod.Invoke(stringLocalizerFactory, new object[] { assembly }));

			assembly = this.GetType().Assembly;
			Assert.AreEqual(assembly.GetName().Name, getRootNamespaceMethod.Invoke(stringLocalizerFactory, new object[] { assembly }));
		}

		[TestMethod]
		public void GetRootNamespaceAttribute_Test()
		{
			var stringLocalizerFactory = new InvestigatableResourceManagerStringLocalizerFactory(Options.Create(this.LocalizationOptions), this.LoggerFactory);

			Assert.IsNull(stringLocalizerFactory.GetProtectedRootNamespaceAttribute(typeof(string).Assembly));
			Assert.IsNull(stringLocalizerFactory.GetProtectedRootNamespaceAttribute(this.GetType().Assembly));
		}

		#endregion
	}
}