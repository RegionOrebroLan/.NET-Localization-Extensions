using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization.IntegrationTests.Reflection
{
	[TestClass]
	public class AssemblyTest
	{
		#region Methods

		[TestMethod]
		public void TypeDescriptor_GetConverter_IfTheTypeParameterIsOfTypeIAssembly_ShouldAlwaysReturnTheSameInstanceOfAnAssemblyInterfaceTypeConverter()
		{
			var firstInstance = (AssemblyInterfaceTypeConverter) TypeDescriptor.GetConverter(typeof(IAssembly));
			var secondInstance = (AssemblyInterfaceTypeConverter) TypeDescriptor.GetConverter(typeof(IAssembly));

			Assert.IsTrue(ReferenceEquals(firstInstance, secondInstance));
		}

		[TestMethod]
		public void TypeDescriptor_GetConverter_IfTheTypeParameterIsOfTypeIAssembly_ShouldReturnAnAssemblyInterfaceTypeConverter()
		{
			Assert.IsTrue(TypeDescriptor.GetConverter(typeof(IAssembly)) is AssemblyInterfaceTypeConverter);
		}

		#endregion
	}
}