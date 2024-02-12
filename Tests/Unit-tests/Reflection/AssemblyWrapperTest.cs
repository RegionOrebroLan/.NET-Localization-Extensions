using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Reflection;

namespace UnitTests.Reflection
{
	[TestClass]
	public class AssemblyWrapperTest
	{
		#region Methods

		protected internal virtual IAssembly CreateAbstractAssembly(string fullName)
		{
			var assemblyMock = new Mock<IAssembly>();

			assemblyMock.SetupAllProperties();
			assemblyMock.Setup(assembly => assembly.FullName).Returns(fullName);

			return assemblyMock.Object;
		}

		protected internal virtual AssemblyWrapper CreateAssemblyWrapper()
		{
			return this.CreateAssemblyWrapper(Mock.Of<Assembly>());
		}

		protected internal virtual AssemblyWrapper CreateAssemblyWrapper(Assembly assembly)
		{
			return new AssemblyWrapper(assembly, Mock.Of<IRootNamespaceResolver>());
		}

		protected internal virtual AssemblyWrapper CreateAssemblyWrapper(string fullName)
		{
			var assemblyMock = new Mock<Assembly> { CallBase = true };

			assemblyMock.SetupAllProperties();
			assemblyMock.Setup(assembly => assembly.FullName).Returns(fullName);

			return this.CreateAssemblyWrapper(assemblyMock.Object);
		}

		[TestMethod]
		public void Equals_IfTheOtherObjectDoesNotImplementAssemblyInterface_ShouldReturnFalse()
		{
			// ReSharper disable SuspiciousTypeConversion.Global
			Assert.IsFalse(this.CreateAssemblyWrapper().Equals(string.Empty));
			// ReSharper restore SuspiciousTypeConversion.Global
		}

		[TestMethod]
		[SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
		public void Equals_IfTheOtherObjectImplementsAssemblyInterfaceAndTheFullNamesAreEqualByCaseInsensitiveComparing_ShouldReturnTrue()
		{
			var fullName = Guid.NewGuid().ToString();

			var assemblyWrapper = this.CreateAssemblyWrapper(fullName.ToUpperInvariant());
			var abstractAssembly = this.CreateAbstractAssembly(fullName.ToLowerInvariant());

			Assert.IsTrue(assemblyWrapper.Equals(abstractAssembly));
			Assert.IsTrue(Equals(assemblyWrapper, abstractAssembly));
			Assert.IsFalse(Equals(abstractAssembly, assemblyWrapper));
		}

		[TestMethod]
		public void Equals_IfTheOtherObjectIsNull_ShouldReturnFalse()
		{
			Assert.IsFalse(this.CreateAssemblyWrapper().Equals(null));
		}

		#endregion
	}
}