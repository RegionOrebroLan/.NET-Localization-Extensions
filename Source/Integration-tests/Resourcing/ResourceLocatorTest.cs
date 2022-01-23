using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Resourcing;
using RegionOrebroLan.Localization.Serialization;
using JsonLocalizationParser = RegionOrebroLan.Localization.Json.Serialization.LocalizationParser;
using JsonResourceValidator = RegionOrebroLan.Localization.Json.Resourcing.ResourceValidator;
using XmlLocalizationParser = RegionOrebroLan.Localization.Xml.Serialization.LocalizationParser;
using XmlResourceValidator = RegionOrebroLan.Localization.Xml.Resourcing.ResourceValidator;

namespace RegionOrebroLan.Localization.IntegrationTests.Resourcing
{
	[TestClass]
	public class ResourceLocatorTest
	{
		#region Fields

		private static IResourceLocator _acceptingResourceLocator;
		private static IResourceValidator _acceptingResourceValidator;
		private static IAssemblyHelper _assemblyHelper;
		private static readonly IFileSystem _fileSystem = new FileSystem();
		private static readonly IHostingEnvironment _hostingEnvironment = Global.HostingEnvironment;
		private static readonly ILoggerFactory _loggerFactory = new LoggerFactory();
		private static readonly IRootNamespaceResolver _rootNamespaceResolver = new RootNamespaceResolver();
		private static IEnumerable<IResourceResolver> _singleAcceptingResourceResolverCollection;

		#endregion

		#region Properties

		protected internal virtual IResourceLocator AcceptingResourceLocator => _acceptingResourceLocator ?? (_acceptingResourceLocator = this.CreateResourceLocator(this.SingleAcceptingResourceResolverCollection));

		protected internal virtual IResourceValidator AcceptingResourceValidator
		{
			get
			{
				// ReSharper disable InvertIf
				if(_acceptingResourceValidator == null)
				{
					var resourceValidatorMock = new Mock<IResourceValidator>();

					resourceValidatorMock.Setup(resourceValidator => resourceValidator.IsValidEmbeddedResource(It.IsAny<IAssembly>(), It.IsAny<string>())).Returns(true);
					resourceValidatorMock.Setup(resourceValidator => resourceValidator.IsValidFileResource(It.IsAny<FileInfoBase>())).Returns(true);
					resourceValidatorMock.Setup(resourceValidator => resourceValidator.IsValidFileResource(It.IsAny<string>())).Returns(true);

					_acceptingResourceValidator = resourceValidatorMock.Object;
				}
				// ReSharper restore InvertIf

				return _acceptingResourceValidator;
			}
		}

		protected internal virtual IAssemblyHelper AssemblyHelper => _assemblyHelper ?? (_assemblyHelper = new AssemblyHelper(this.RootNamespaceResolver));
		protected internal virtual IFileSystem FileSystem => _fileSystem;

		protected internal virtual IHostingEnvironment HostingEnvironment => _hostingEnvironment;

		protected internal virtual ILoggerFactory LoggerFactory => _loggerFactory;

		protected internal virtual IRootNamespaceResolver RootNamespaceResolver => _rootNamespaceResolver;

		protected internal virtual IEnumerable<IResourceResolver> SingleAcceptingResourceResolverCollection
		{
			get
			{
				// ReSharper disable ConvertIfStatementToNullCoalescingExpression
				if(_singleAcceptingResourceResolverCollection == null)
					_singleAcceptingResourceResolverCollection = new[] {new ResourceResolver(Mock.Of<ILocalizationParser>(), this.AcceptingResourceValidator)};
				// ReSharper restore ConvertIfStatementToNullCoalescingExpression

				return _singleAcceptingResourceResolverCollection;
			}
		}

		#endregion

		#region Methods

		protected internal virtual IEnumerable<IResourceResolver> CreateResolversForJsonOnly()
		{
			return new[]
			{
				new ResourceResolver(new JsonLocalizationParser(), new JsonResourceValidator(this.FileSystem))
			};
		}

		protected internal virtual IEnumerable<IResourceResolver> CreateResolversForXmlOnly()
		{
			return new[]
			{
				new ResourceResolver(new XmlLocalizationParser(), new XmlResourceValidator(this.FileSystem))
			};
		}

		protected internal virtual ResourceLocator CreateResourceLocator(IEnumerable<IResourceResolver> resolvers)
		{
			return new ResourceLocator(this.AssemblyHelper, this.FileSystem, this.LoggerFactory, resolvers);
		}

		[TestMethod]
		public void GetEmbeddedResources_IfTheParameterIsTheColorsAssemblyAndTheResolversAcceptsAllResources_ShouldReturnAllTheEmbeddedResourcesFromTheColorsAssembly()
		{
			var assembly = typeof(Colors.TheClass).Assembly;
			var abstractAssembly = this.AssemblyHelper.Wrap(assembly);

			var embeddedResources = this.AcceptingResourceLocator.GetEmbeddedResources(abstractAssembly).ToArray();

			Assert.AreEqual(12, embeddedResources.Length);

			const string namePrefix = "Colors.Colors.";

			var cultureName = string.Empty;
			var assemblyName = assembly.FullName;
			this.ValidateEmbeddedResource(assemblyName, cultureName, namePrefix + "json", embeddedResources[0]);
			this.ValidateEmbeddedResource(assemblyName, cultureName, namePrefix + "resources", embeddedResources[1]);
			this.ValidateEmbeddedResource(assemblyName, cultureName, namePrefix + "txt", embeddedResources[2]);

			cultureName = "en";
			assemblyName = assembly.GetSatelliteAssembly(CultureInfo.GetCultureInfo(cultureName)).FullName;
			this.ValidateEmbeddedResource(assemblyName, cultureName, namePrefix + cultureName + ".resources", embeddedResources[3]);
			this.ValidateEmbeddedResource(assemblyName, cultureName, namePrefix + "json", embeddedResources[4]);
			this.ValidateEmbeddedResource(assemblyName, cultureName, namePrefix + "txt", embeddedResources[5]);

			cultureName = "fi";
			assemblyName = assembly.GetSatelliteAssembly(CultureInfo.GetCultureInfo(cultureName)).FullName;
			this.ValidateEmbeddedResource(assemblyName, cultureName, namePrefix + cultureName + ".resources", embeddedResources[6]);
			this.ValidateEmbeddedResource(assemblyName, cultureName, namePrefix + "json", embeddedResources[7]);
			this.ValidateEmbeddedResource(assemblyName, cultureName, namePrefix + "txt", embeddedResources[8]);

			cultureName = "sv";
			assemblyName = assembly.GetSatelliteAssembly(CultureInfo.GetCultureInfo(cultureName)).FullName;
			this.ValidateEmbeddedResource(assemblyName, cultureName, namePrefix + "json", embeddedResources[9]);
			this.ValidateEmbeddedResource(assemblyName, cultureName, namePrefix + cultureName + ".resources", embeddedResources[10]);
			this.ValidateEmbeddedResource(assemblyName, cultureName, namePrefix + "txt", embeddedResources[11]);
		}

		[TestMethod]
		public void GetEmbeddedResources_IfTheParameterIsTheColorsAssemblyAndTheResolversOnlyContainAJsonResolver_ShouldReturnAllTheEmbeddedJsonResourcesFromTheColorsAssembly()
		{
			var assembly = typeof(Colors.TheClass).Assembly;
			var abstractAssembly = this.AssemblyHelper.Wrap(assembly);

			var embeddedResources = this.CreateResourceLocator(this.CreateResolversForJsonOnly()).GetEmbeddedResources(abstractAssembly).ToArray();

			Assert.AreEqual(4, embeddedResources.Length);

			const string name = "Colors.Colors.json";

			var cultureName = string.Empty;
			var assemblyName = assembly.FullName;
			this.ValidateEmbeddedResource(assemblyName, cultureName, name, embeddedResources[0]);

			cultureName = "en";
			assemblyName = assembly.GetSatelliteAssembly(CultureInfo.GetCultureInfo(cultureName)).FullName;
			this.ValidateEmbeddedResource(assemblyName, cultureName, name, embeddedResources[1]);

			cultureName = "fi";
			assemblyName = assembly.GetSatelliteAssembly(CultureInfo.GetCultureInfo(cultureName)).FullName;
			this.ValidateEmbeddedResource(assemblyName, cultureName, name, embeddedResources[2]);

			cultureName = "sv";
			assemblyName = assembly.GetSatelliteAssembly(CultureInfo.GetCultureInfo(cultureName)).FullName;
			this.ValidateEmbeddedResource(assemblyName, cultureName, name, embeddedResources[3]);
		}

		[TestMethod]
		public void GetEmbeddedResources_IfTheParameterIsTheNumbersAssemblyAndTheResolversAcceptsAllResources_ShouldReturnAllTheEmbeddedResourcesFromTheNumbersAssembly()
		{
			var assembly = typeof(Numbers.TheClass).Assembly;
			var abstractAssembly = this.AssemblyHelper.Wrap(assembly);

			var embeddedResources = this.AcceptingResourceLocator.GetEmbeddedResources(abstractAssembly).ToArray();

			Assert.AreEqual(2, embeddedResources.Length);

			var cultureName = string.Empty;
			var assemblyName = assembly.FullName;
			this.ValidateEmbeddedResource(assemblyName, cultureName, "Numbers.Numbers.json", embeddedResources[0]);

			cultureName = "is";
			assemblyName = assembly.GetSatelliteAssembly(CultureInfo.GetCultureInfo(cultureName)).FullName;
			this.ValidateEmbeddedResource(assemblyName, cultureName, "Numbers.File.txt", embeddedResources[1]);
		}

		[TestMethod]
		public void GetEmbeddedResources_IfTheParameterIsThePrioritizedWordsAssemblyAndTheResolversAcceptsAllResources_ShouldReturnAllTheEmbeddedResourcesFromThePrioritizedWordsAssembly()
		{
			var assembly = typeof(Words.Prioritized.TheClass).Assembly;
			var abstractAssembly = this.AssemblyHelper.Wrap(assembly);

			var embeddedResources = this.AcceptingResourceLocator.GetEmbeddedResources(abstractAssembly).ToArray();

			Assert.AreEqual(1, embeddedResources.Length);

			this.ValidateEmbeddedResource(assembly.FullName, string.Empty, "Words.Prioritized.Prioritized-words.json", embeddedResources[0]);
		}

		[TestMethod]
		public void GetEmbeddedResources_IfTheParameterIsTheRootNamespacedResourcesAssemblyAndTheResolversAcceptsAllResources_ShouldReturnAllTheEmbeddedResourcesFromTheRootNamespacedResourcesAssembly()
		{
			var assembly = typeof(RootNamespacedResources.TheClass).Assembly;
			var abstractAssembly = this.AssemblyHelper.Wrap(assembly);

			var embeddedResources = this.AcceptingResourceLocator.GetEmbeddedResources(abstractAssembly).ToArray();

			Assert.AreEqual(1, embeddedResources.Length);

			this.ValidateEmbeddedResource(assembly.FullName, string.Empty, "RootNamespacedResources.Examples.json", embeddedResources[0]);
		}

		[TestMethod]
		public void GetEmbeddedResources_IfTheParameterIsTheWordsAssemblyAndTheResolversAcceptsAllResources_ShouldReturnAllTheEmbeddedResourcesFromTheWordsAssembly()
		{
			var assembly = typeof(Words.TheClass).Assembly;
			var abstractAssembly = this.AssemblyHelper.Wrap(assembly);

			var embeddedResources = this.AcceptingResourceLocator.GetEmbeddedResources(abstractAssembly).ToArray();

			Assert.AreEqual(3, embeddedResources.Length);

			this.ValidateEmbeddedResource(assembly.FullName, string.Empty, "Words.en.Words.json", embeddedResources[0]);
			this.ValidateEmbeddedResource(assembly.FullName, string.Empty, "Words.Other.fi.Words.json", embeddedResources[1]);
			this.ValidateEmbeddedResource(assembly.FullName, string.Empty, "Words.Other.sv.Words.json", embeddedResources[2]);
		}

		[TestMethod]
		public void GetFileResources_Test()
		{
			const string relativeDirectoryPath = "Resources";
			var directoryPath = Path.Combine(this.HostingEnvironment.ContentRootPath, relativeDirectoryPath);

			var fileResources = this.AcceptingResourceLocator.GetFileResources(directoryPath, true).ToArray();

			Assert.AreEqual(6, fileResources.Length);

			Assert.AreEqual(Path.Combine(directoryPath, "Texts.en.json"), fileResources[1].Path);
		}

		[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
		protected internal virtual void ValidateEmbeddedResource(string assemblyFullName, string cultureName, string name, IEmbeddedResource embeddedResource)
		{
			Assert.IsNotNull(embeddedResource);
			Assert.AreEqual(assemblyFullName, embeddedResource.Assembly.FullName);
			Assert.AreEqual(cultureName, embeddedResource.Assembly.Culture.Name);
			Assert.AreEqual(name, embeddedResource.Name);
		}

		#endregion
	}
}