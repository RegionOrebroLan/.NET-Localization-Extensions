using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Prerequisites
{
	[TestClass]
	public class ResourceManagerPrerequisiteTest
	{
		#region Fields

		private static string _mixedResourcesDirectoryPath;
		private static readonly FieldInfo _resourceSetsField = typeof(ResourceManager).GetField("_resourceSets", BindingFlags.Instance | BindingFlags.NonPublic);
		private static Type _runtimeResourceSetType;

		#endregion

		#region Properties

		protected internal virtual string MixedResourcesDirectoryPath => _mixedResourcesDirectoryPath ?? (_mixedResourcesDirectoryPath = Path.Combine(Global.ProjectDirectoryPath, "Test-resources", "Mixed"));
		protected internal virtual FieldInfo ResourceSetsField => _resourceSetsField;

		protected internal virtual Type RuntimeResourceSetType
		{
			get
			{
				if(_runtimeResourceSetType == null)
					_runtimeResourceSetType = Type.GetType(typeof(ResourceSet).AssemblyQualifiedName.Replace("ResourceSet", "RuntimeResourceSet", StringComparison.Ordinal), true, true);

				return _runtimeResourceSetType;
			}
		}

		#endregion

		#region Methods

		[TestMethod]
		public void Constructor_WithOneParameter_IfTheResourceSourceParameterIsTypeofString_ShouldSetTheBaseNameToString()
		{
			var resourceManager = new ResourceManager(typeof(string));

			Assert.AreEqual("String", resourceManager.BaseName);
		}

		[TestMethod]
		public void CreateFileBasedResourceManager_DoesNotWorkWithoutTheResgenTool()
		{
			// https://developers.de/blogs/nadine_storandt/archive/2010/10/26/howto-using-createfilebasedresourcemanager.aspx
			// https://stackoverflow.com/questions/6127259/using-net-resources-without-building

			var resourceManager = ResourceManager.CreateFileBasedResourceManager("Embedded-resource", this.MixedResourcesDirectoryPath, null);
			var resourceSets = this.GetResourceSets(resourceManager);
			Assert.IsNotNull(resourceSets);
			Assert.IsFalse(resourceSets.Any());
			try
			{
				resourceManager.GetString("Name", null);
			}
			catch(MissingManifestResourceException)
			{
				// This is the expected exception.
			}

			resourceManager = ResourceManager.CreateFileBasedResourceManager("Embedded-resource.resx", this.MixedResourcesDirectoryPath, null);
			resourceSets = this.GetResourceSets(resourceManager);
			Assert.IsNotNull(resourceSets);
			Assert.IsFalse(resourceSets.Any());
			try
			{
				resourceManager.GetString("Name", null);
			}
			catch(MissingManifestResourceException)
			{
				// This is the expected exception.
			}
		}

		protected internal virtual IDictionary<string, ResourceSet> GetResourceSets(ResourceManager resourceManager)
		{
			if(resourceManager == null)
				throw new ArgumentNullException(nameof(resourceManager));

			return (IDictionary<string, ResourceSet>)this.ResourceSetsField.GetValue(resourceManager);
		}

		[TestMethod]
		public void GetString_Test()
		{
			var resourceManager = new ResourceManager("Colors.Colors", typeof(Colors.TheClass).Assembly);

			var localizedName = resourceManager.GetString("Blue", CultureInfo.InvariantCulture);
			Assert.AreEqual("Blue: \"\\Embedded-resources\\Colors\\Colors.resx\"", localizedName);

			localizedName = resourceManager.GetString("Green", CultureInfo.GetCultureInfo("en"));
			Assert.AreEqual("Green: \"\\Embedded-resources\\Colors\\Colors.en.resx\"", localizedName);

			localizedName = resourceManager.GetString("Green", CultureInfo.GetCultureInfo("en-US"));
			Assert.AreEqual("Green: \"\\Embedded-resources\\Colors\\Colors.en.resx\"", localizedName);

			localizedName = resourceManager.GetString("Red", CultureInfo.GetCultureInfo("fi"));
			Assert.AreEqual("Punainen: \"\\Embedded-resources\\Colors\\Colors.fi.resx\"", localizedName);

			localizedName = resourceManager.GetString("Red", CultureInfo.GetCultureInfo("fi-FI"));
			Assert.AreEqual("Punainen: \"\\Embedded-resources\\Colors\\Colors.fi.resx\"", localizedName);

			localizedName = resourceManager.GetString("Blue", CultureInfo.GetCultureInfo("sv"));
			Assert.AreEqual("Blå: \"\\Embedded-resources\\Colors\\Colors.sv.resx\"", localizedName);

			localizedName = resourceManager.GetString("Blue", CultureInfo.GetCultureInfo("sv-SE"));
			Assert.AreEqual("Blå: \"\\Embedded-resources\\Colors\\Colors.sv.resx\"", localizedName);
		}

		[TestMethod]
		public void ResourceSets_IfTheResourceManagerIsConstructedFromTypeofString_ShouldBeEmpty()
		{
			var resourceManager = new ResourceManager(typeof(string));

			var resourceSets = this.GetResourceSets(resourceManager);

			Assert.IsNotNull(resourceSets);
			Assert.IsFalse(resourceSets.Any());
		}

		#endregion
	}
}