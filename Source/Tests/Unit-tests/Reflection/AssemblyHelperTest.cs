using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Reflection;

namespace UnitTests.Reflection
{
	[TestClass]
	public class AssemblyHelperTest
	{
		#region Fields

		private static readonly AssemblyHelper _assemblyHelper = new(Mock.Of<IRootNamespaceResolver>());

		#endregion

		#region Properties

		protected internal virtual AssemblyHelper AssemblyHelper => _assemblyHelper;

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
		[ExpectedException(typeof(ArgumentNullException))]
		public void Find_IfThePatternParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			this.AssemblyHelper.Find(null);
		}

		#endregion
	}
}