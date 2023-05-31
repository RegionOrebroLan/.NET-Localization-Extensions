using System;
using System.IO;
using System.Reflection;
using Investigation.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace UnitTests.Prerequisites
{
	[TestClass]
	// ReSharper disable All
	public class ResourceManagerStringLocalizerFactoryPrerequisiteTest
	{
		#region Fields

		private static readonly MethodInfo _getRootNamespaceMethod = typeof(ResourceManagerStringLocalizerFactory).GetMethod("GetRootNamespace", BindingFlags.Instance | BindingFlags.NonPublic);

		#endregion

		#region Properties

		protected internal virtual MethodInfo GetRootNamespaceMethod => _getRootNamespaceMethod;

		#endregion

		#region Methods

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_IfTheLocalizationOptionsParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			try
			{
				var _ = new ResourceManagerStringLocalizerFactory(null, Mock.Of<ILoggerFactory>());
			}
			catch(ArgumentNullException argumentNullException)
			{
				if(argumentNullException.ParamName.Equals("localizationOptions", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_IfTheLoggerFactoryParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			try
			{
				var _ = new ResourceManagerStringLocalizerFactory(Mock.Of<IOptions<LocalizationOptions>>(), null);
			}
			catch(ArgumentNullException argumentNullException)
			{
				if(argumentNullException.ParamName.Equals("loggerFactory", StringComparison.Ordinal))
					throw;
			}
		}

		protected internal virtual T CreateResourceManagerStringLocalizerFactory<T>(string resourcePath) where T : ResourceManagerStringLocalizerFactory
		{
			return this.CreateResourceManagerStringLocalizerFactoryMock<T>(resourcePath).Object;
		}

		protected internal virtual T CreateResourceManagerStringLocalizerFactory<T>(string resourcePath, string rootNamespace) where T : ResourceManagerStringLocalizerFactory
		{
			var stringLocalizerFactoryMock = this.CreateResourceManagerStringLocalizerFactoryMock<T>(resourcePath);

			stringLocalizerFactoryMock.Protected().Setup<RootNamespaceAttribute>("GetRootNamespaceAttribute", ItExpr.IsAny<Assembly>()).Returns(new RootNamespaceAttribute(rootNamespace));

			return stringLocalizerFactoryMock.Object;
		}

		protected internal virtual Mock<T> CreateResourceManagerStringLocalizerFactoryMock<T>(string resourcePath) where T : ResourceManagerStringLocalizerFactory
		{
			var localizationOptionsMock = new Mock<IOptions<LocalizationOptions>>();
			localizationOptionsMock.Setup(localizationOptions => localizationOptions.Value).Returns(new LocalizationOptions { ResourcesPath = resourcePath });

			var stringLocalizerFactoryMock = new Mock<T>(localizationOptionsMock.Object, Mock.Of<ILoggerFactory>()) { CallBase = true };

			return stringLocalizerFactoryMock;
		}

		[TestMethod]
		public void GetResourcePrefix_WithOneTypeInfoParameterAndTwoStringParameters_Test()
		{
			var stringLocalizerFactory = this.CreateResourceManagerStringLocalizerFactory<InvestigatableResourceManagerStringLocalizerFactory>("Test-resources");
			Assert.AreEqual("BaseNamespace.ResourcesRelativePathSystem.String", stringLocalizerFactory.GetProtectedResourcePrefix(typeof(string).GetTypeInfo(), "BaseNamespace", "ResourcesRelativePath"));
			Assert.AreEqual("BaseNamespace.ResourcesRelativePathUnitTests.Prerequisites.ResourceManagerStringLocalizerFactoryPrerequisiteTest", stringLocalizerFactory.GetProtectedResourcePrefix(this.GetType().GetTypeInfo(), "BaseNamespace", "ResourcesRelativePath"));

			stringLocalizerFactory = this.CreateResourceManagerStringLocalizerFactory<InvestigatableResourceManagerStringLocalizerFactory>(null);
			Assert.AreEqual("BaseNamespace.ResourcesRelativePathSystem.String", stringLocalizerFactory.GetProtectedResourcePrefix(typeof(string).GetTypeInfo(), "BaseNamespace", "ResourcesRelativePath"));
			Assert.AreEqual("BaseNamespace.ResourcesRelativePathUnitTests.Prerequisites.ResourceManagerStringLocalizerFactoryPrerequisiteTest", stringLocalizerFactory.GetProtectedResourcePrefix(this.GetType().GetTypeInfo(), "BaseNamespace", "ResourcesRelativePath"));

			stringLocalizerFactory = this.CreateResourceManagerStringLocalizerFactory<InvestigatableResourceManagerStringLocalizerFactory>(string.Empty);
			Assert.AreEqual("BaseNamespace.ResourcesRelativePathSystem.String", stringLocalizerFactory.GetProtectedResourcePrefix(typeof(string).GetTypeInfo(), "BaseNamespace", "ResourcesRelativePath"));
			Assert.AreEqual("BaseNamespace.ResourcesRelativePathUnitTests.Prerequisites.ResourceManagerStringLocalizerFactoryPrerequisiteTest", stringLocalizerFactory.GetProtectedResourcePrefix(this.GetType().GetTypeInfo(), "BaseNamespace", "ResourcesRelativePath"));
		}

		[TestMethod]
		public void GetResourcePrefix_WithThreeStringParameters_Test()
		{
			var stringLocalizerFactory = this.CreateResourceManagerStringLocalizerFactory<InvestigatableResourceManagerStringLocalizerFactory>("Test-resources");
			Assert.AreEqual("Location.ResourceLocationBaseName", stringLocalizerFactory.GetProtectedResourcePrefix("Location", "BaseName", "ResourceLocation"));

			stringLocalizerFactory = this.CreateResourceManagerStringLocalizerFactory<InvestigatableResourceManagerStringLocalizerFactory>(null);
			Assert.AreEqual("Location.ResourceLocationBaseName", stringLocalizerFactory.GetProtectedResourcePrefix("Location", "BaseName", "ResourceLocation"));

			stringLocalizerFactory = this.CreateResourceManagerStringLocalizerFactory<InvestigatableResourceManagerStringLocalizerFactory>(string.Empty);
			Assert.AreEqual("Location.ResourceLocationBaseName", stringLocalizerFactory.GetProtectedResourcePrefix("Location", "BaseName", "ResourceLocation"));
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void GetResourcePrefix_WithTwoStringParameters_IfTheBaseNamespaceIsNotAValidAssemblyName_ShouldThrowAFileNotFoundException()
		{
			var stringLocalizerFactory = this.CreateResourceManagerStringLocalizerFactory<InvestigatableResourceManagerStringLocalizerFactory>("Test-resources");

			try
			{
				stringLocalizerFactory.GetProtectedResourcePrefix("Base-resource-name", "Base-namespace");
			}
			catch(FileNotFoundException fileNotFoundException)
			{
				if(fileNotFoundException.FileName.Equals("Base-namespace, Culture=neutral, PublicKeyToken=null", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		public void GetResourcePrefix_WithTwoStringParameters_Test()
		{
			var stringLocalizerFactory = this.CreateResourceManagerStringLocalizerFactory<InvestigatableResourceManagerStringLocalizerFactory>("My-resources");
			Assert.AreEqual("System.My-resources.Base-resource-name", stringLocalizerFactory.GetProtectedResourcePrefix("Base-resource-name", "System"));

			stringLocalizerFactory = this.CreateResourceManagerStringLocalizerFactory<InvestigatableResourceManagerStringLocalizerFactory>(null);
			Assert.AreEqual("System.Base-resource-name", stringLocalizerFactory.GetProtectedResourcePrefix("Base-resource-name", "System"));
		}

		[TestMethod]
		public void GetResourcePrefix_WithTypeInfoParameter_Test()
		{
			const string resourcePath = "Resource_Path";
			const string rootNamespace = "My.Root.Namespace";

			var stringLocalizerFactory = this.CreateResourceManagerStringLocalizerFactory<InvestigatableResourceManagerStringLocalizerFactory>(resourcePath);
			Assert.AreEqual("System.Private.CoreLib.Resource_Path.System.String", stringLocalizerFactory.GetProtectedResourcePrefix(typeof(string).GetTypeInfo()));
			Assert.AreEqual("Unit-tests.Resource_Path.UnitTests.Prerequisites.ResourceManagerStringLocalizerFactoryPrerequisiteTest", stringLocalizerFactory.GetProtectedResourcePrefix(this.GetType().GetTypeInfo()));

			stringLocalizerFactory = this.CreateResourceManagerStringLocalizerFactory<InvestigatableResourceManagerStringLocalizerFactory>(resourcePath, rootNamespace);
			Assert.AreEqual("My.Root.Namespace.Resource_Path.System.String", stringLocalizerFactory.GetProtectedResourcePrefix(typeof(string).GetTypeInfo()));
			Assert.AreEqual("My.Root.Namespace.Resource_Path.UnitTests.Prerequisites.ResourceManagerStringLocalizerFactoryPrerequisiteTest", stringLocalizerFactory.GetProtectedResourcePrefix(this.GetType().GetTypeInfo()));
		}

		[TestMethod]
		public void GetRootNamespace_Test()
		{
			const string resourcePath = "Resource_Path";
			const string rootNamespace = "My.Root.Namespace";

			var stringLocalizerFactory = this.CreateResourceManagerStringLocalizerFactory<ResourceManagerStringLocalizerFactory>(resourcePath, rootNamespace);

			Assert.AreEqual(rootNamespace, this.GetRootNamespaceMethod.Invoke(stringLocalizerFactory, new object[] { Mock.Of<Assembly>() }));
			Assert.AreEqual(rootNamespace, this.GetRootNamespaceMethod.Invoke(stringLocalizerFactory, new object[] { typeof(string).Assembly }));
			Assert.AreEqual(rootNamespace, this.GetRootNamespaceMethod.Invoke(stringLocalizerFactory, new object[] { this.GetType().Assembly }));
		}

		#endregion
	}
	// ReSharper restore All
}