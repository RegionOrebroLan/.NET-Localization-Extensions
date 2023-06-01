using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Resourcing;
using RegionOrebroLan.Localization.Serialization;

namespace IntegrationTests.Resourcing
{
	[TestClass]
	public class EmbeddedResourceTest
	{
		#region Fields

		private static readonly IAssembly _defaultAssembly = new AssemblyWrapper(typeof(EmbeddedResourceTest).Assembly, new RootNamespaceResolver());
		private const string _defaultEmbeddedResourceName = "Test";

		#endregion

		#region Properties

		protected internal virtual IAssembly DefaultAssembly => _defaultAssembly;
		protected internal virtual string DefaultEmbeddedResourceName => _defaultEmbeddedResourceName;

		#endregion

		#region Methods

		protected internal virtual EmbeddedResource CreateEmbeddedResource()
		{
			return this.CreateEmbeddedResource(this.DefaultAssembly, this.DefaultEmbeddedResourceName);
		}

		protected internal virtual EmbeddedResource CreateEmbeddedResource(IAssembly assembly)
		{
			return this.CreateEmbeddedResource(assembly, this.DefaultEmbeddedResourceName);
		}

		protected internal virtual EmbeddedResource CreateEmbeddedResource(string name)
		{
			return this.CreateEmbeddedResource(this.DefaultAssembly, name);
		}

		protected internal virtual EmbeddedResource CreateEmbeddedResource(IAssembly assembly, string name)
		{
			return new EmbeddedResource(assembly, name, Mock.Of<ILocalizationParser>());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		[SuppressMessage("Usage", "CA1806:Do not ignore method results")]
		public void DictionaryKey_IfAddingAnEqualEmbeddedResourceKeyASecondTime_ShouldThrowAnArgumentException()
		{
			// ReSharper disable ObjectCreationAsStatement
			new Dictionary<IResource, string>
			{
				{ this.CreateEmbeddedResource(), "First" },
				{ this.CreateEmbeddedResource(), "Second" }
			};
			// ReSharper restore ObjectCreationAsStatement
		}

		[TestMethod]
		[SuppressMessage("Usage", "CA1806:Do not ignore method results")]
		public void DictionaryKey_IfAddingAnUnequalEmbeddedResourceKey_ShouldNotThrowAnArgumentException()
		{
			var dictionary = new Dictionary<IResource, string>
			{
				{ this.CreateEmbeddedResource(), "First" },
				{ this.CreateEmbeddedResource("Not-test"), "Second" }
			};

			Assert.AreEqual(2, dictionary.Count);
		}

		#endregion
	}
}