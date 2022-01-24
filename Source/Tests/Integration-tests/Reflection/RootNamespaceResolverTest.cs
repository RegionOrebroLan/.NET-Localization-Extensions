using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Reflection;

namespace IntegrationTests.Reflection
{
	[TestClass]
	public class RootNamespaceResolverTest
	{
		#region Methods

		[TestMethod]
		public void GetRootNamespace_IfTheAssemblyHaveARootNamespaceAttribute_ShouldReturnARootNamespaceAttributeWrapper()
		{
			var rootNamespaceResolver = new RootNamespaceResolver();

			var rootNamespace = rootNamespaceResolver.GetRootNamespace(typeof(RootNamespacedResources.TheClass).Assembly);

			Assert.IsNotNull(rootNamespace);
			Assert.IsTrue(rootNamespace is RootNamespaceAttributeWrapper);
			Assert.AreEqual("My.Custom.Namespace", rootNamespace.Name);
		}

		#endregion
	}
}