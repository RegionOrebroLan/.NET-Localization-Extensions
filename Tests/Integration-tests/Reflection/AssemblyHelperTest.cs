using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Reflection;

namespace IntegrationTests.Reflection
{
	[TestClass]
	public class AssemblyHelperTest
	{
		#region Fields

		private static AssemblyHelper _assemblyHelper;
		private static readonly IRootNamespaceResolver _rootNamespaceResolver = new RootNamespaceResolver();

		#endregion

		#region Properties

		protected internal virtual AssemblyHelper AssemblyHelper => _assemblyHelper ??= new AssemblyHelper(this.RootNamespaceResolver);
		protected internal virtual IRootNamespaceResolver RootNamespaceResolver => _rootNamespaceResolver;

		#endregion

		#region Methods

		[TestMethod]
		public void ApplicationAssembly_ShouldReturnTheAssemblyOfTheRunningApplication()
		{
			// ReSharper disable PossibleNullReferenceException
			Assert.AreEqual(Assembly.GetEntryAssembly().FullName, this.AssemblyHelper.ApplicationAssembly.FullName);
			// ReSharper restore PossibleNullReferenceException
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Find_IfThePatternParameterIsNotAWildcardAndThereIsNoAssemblyWithThatNameOrFullNameLoadedAtRuntime_ShouldThrowAnInvalidOperationException()
		{
			this.AssemblyHelper.Find(Guid.NewGuid().ToString());
		}

		[TestMethod]
		public void Find_ShouldWorkProperly()
		{
			var assemblies = this.AssemblyHelper.Find("*orebro*").ToArray();

			Assert.IsTrue(assemblies.Any());
			Assert.AreEqual(assemblies.Length, assemblies.Distinct().Count());

			foreach(var assembly in assemblies)
			{
				Assert.IsTrue(assembly.Name.Contains("orebro", StringComparison.OrdinalIgnoreCase) || assembly.FullName.Contains("orebro", StringComparison.OrdinalIgnoreCase));
			}

			assemblies = this.AssemblyHelper.Find("MICROSOFT*").ToArray();

			Assert.IsTrue(assemblies.Any());
			Assert.AreEqual(assemblies.Length, assemblies.Distinct().Count());

			foreach(var assembly in assemblies)
			{
				Assert.IsTrue(assembly.Name.Contains("MICROSOFT", StringComparison.OrdinalIgnoreCase) || assembly.FullName.Contains("MICROSOFT", StringComparison.OrdinalIgnoreCase));
			}

			var name = this.GetType().Assembly.GetName().Name;
			assemblies = this.AssemblyHelper.Find(name).ToArray();
			Assert.AreEqual(1, assemblies.Length);
			Assert.AreEqual(name, assemblies.First().Name);

			var fullName = this.GetType().Assembly.FullName;
			assemblies = this.AssemblyHelper.Find(fullName).ToArray();
			Assert.AreEqual(1, assemblies.Length);
			Assert.AreEqual(fullName, assemblies.First().FullName);
		}

		[TestMethod]
		public void LoadByName_IfTheNameParameterIsTheFullNameOfAnAssembly_ShouldReturnTheAssembly()
		{
			var fullName = typeof(string).Assembly.FullName;

			Assert.AreEqual(fullName, this.AssemblyHelper.LoadByName(fullName).FullName);
		}

		[TestMethod]
		public void LoadByName_IfTheNameParameterIsTheNameOfAnAssembly_ShouldReturnTheAssembly()
		{
			var name = typeof(string).Assembly.GetName().Name;

			Assert.AreEqual(name, this.AssemblyHelper.LoadByName(name).Name);
		}

		#endregion
	}
}