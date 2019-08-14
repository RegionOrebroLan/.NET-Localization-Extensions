using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization.UnitTests.Reflection
{
	[TestClass]
	public class RootNamespaceResolverTest
	{
		#region Methods

		[TestMethod]
		public void GetRootNamespace_IfInTheCache_ShouldGetItFromTheCache()
		{
			var rootNamespaceName = Guid.NewGuid().ToString();
			var rootNamespaceMock = new Mock<IRootNamespace>();
			rootNamespaceMock.Setup(@namespace => @namespace.Name).Returns(rootNamespaceName);
			var rootNamespace = rootNamespaceMock.Object;

			var rootNamespaceResolver = new RootNamespaceResolver();
			Assert.IsTrue(rootNamespaceResolver.Cache.TryAdd(typeof(string).Assembly.FullName, rootNamespace));
			Assert.AreEqual(1, rootNamespaceResolver.Cache.Count);

			var actualRootNamespace = rootNamespaceResolver.GetRootNamespace(typeof(string).Assembly);
			Assert.IsNotNull(actualRootNamespace);
			Assert.IsTrue(ReferenceEquals(rootNamespace, actualRootNamespace));
			Assert.AreEqual(rootNamespaceName, actualRootNamespace.Name);
		}

		[TestMethod]
		public void GetRootNamespace_IfNotInTheCache_ShouldAddItToTheCache()
		{
			var rootNamespaceResolver = new RootNamespaceResolver();
			Assert.IsFalse(rootNamespaceResolver.Cache.Any());
			rootNamespaceResolver.GetRootNamespace(typeof(string).Assembly);
			Assert.AreEqual(1, rootNamespaceResolver.Cache.Count);
			Assert.IsNull(rootNamespaceResolver.Cache.Values.First());
		}

		#endregion
	}
}