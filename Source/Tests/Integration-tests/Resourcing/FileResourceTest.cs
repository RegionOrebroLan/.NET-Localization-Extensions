using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Resourcing;
using RegionOrebroLan.Localization.Serialization;

namespace IntegrationTests.Resourcing
{
	[TestClass]
	public class FileResourceTest
	{
		#region Fields

		private static readonly IAssembly _defaultAssembly = new AssemblyWrapper(typeof(EmbeddedResourceTest).Assembly, new RootNamespaceResolver());
		private const string _defaultFileResourcePath = @"Q:\A\B\C\Test.test";

		#endregion

		#region Properties

		protected internal virtual IAssembly DefaultAssembly => _defaultAssembly;
		protected internal virtual string DefaultFileResourcePath => _defaultFileResourcePath;

		#endregion

		#region Methods

		protected internal virtual FileResource CreateFileResource()
		{
			return this.CreateFileResource(this.DefaultAssembly, this.DefaultFileResourcePath);
		}

		protected internal virtual FileResource CreateFileResource(IAssembly assembly)
		{
			return this.CreateFileResource(assembly, this.DefaultFileResourcePath);
		}

		protected internal virtual FileResource CreateFileResource(string path)
		{
			return this.CreateFileResource(this.DefaultAssembly, path);
		}

		protected internal virtual FileResource CreateFileResource(IAssembly assembly, string path)
		{
			return new FileResource(assembly, new FileInfoWrapper(new FileSystem(), new FileInfo(path)), Mock.Of<ILocalizationParser>());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		[SuppressMessage("Usage", "CA1806:Do not ignore method results")]
		public void DictionaryKey_IfAddingAnEqualFileResourceKeyASecondTime_ShouldThrowAnArgumentException()
		{
			// ReSharper disable ObjectCreationAsStatement
			new Dictionary<IResource, string>
			{
				{ this.CreateFileResource(), "First" },
				{ this.CreateFileResource(), "Second" }
			};
			// ReSharper restore ObjectCreationAsStatement
		}

		[TestMethod]
		[SuppressMessage("Usage", "CA1806:Do not ignore method results")]
		public void DictionaryKey_IfAddingAnUnequalFileResourceKey_ShouldNotThrowAnArgumentException()
		{
			var dictionary = new Dictionary<IResource, string>
			{
				{ this.CreateFileResource(), "First" },
				{ this.CreateFileResource("Not-test"), "Second" }
			};

			Assert.AreEqual(2, dictionary.Count);
		}

		#endregion
	}
}