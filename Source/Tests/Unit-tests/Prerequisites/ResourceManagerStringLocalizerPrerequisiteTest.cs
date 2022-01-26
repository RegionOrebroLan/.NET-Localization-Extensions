using System;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests.Prerequisites
{
	[TestClass]
	// ReSharper disable All
	public class ResourceManagerStringLocalizerPrerequisiteTest
	{
		#region Methods

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_IfTheBaseNameParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			try
			{
				var _ = new ResourceManagerStringLocalizer(Mock.Of<ResourceManager>(), Mock.Of<Assembly>(), null, Mock.Of<IResourceNamesCache>(), Mock.Of<ILogger>());
			}
			catch(ArgumentNullException argumentNullException)
			{
				if(argumentNullException.ParamName.Equals("baseName", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_IfTheLoggerParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			try
			{
				var _ = new ResourceManagerStringLocalizer(Mock.Of<ResourceManager>(), Mock.Of<Assembly>(), "Test", Mock.Of<IResourceNamesCache>(), null);
			}
			catch(ArgumentNullException argumentNullException)
			{
				if(argumentNullException.ParamName.Equals("logger", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_IfTheResourceAssemblyParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			try
			{
				var _ = new ResourceManagerStringLocalizer(Mock.Of<ResourceManager>(), null, "Test", Mock.Of<IResourceNamesCache>(), Mock.Of<ILogger>());
			}
			catch(ArgumentNullException argumentNullException)
			{
				if(argumentNullException.ParamName.Equals("assembly", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_IfTheResourceManagerParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			try
			{
				var _ = new ResourceManagerStringLocalizer(null, Mock.Of<Assembly>(), "Test", Mock.Of<IResourceNamesCache>(), Mock.Of<ILogger>());
			}
			catch(ArgumentNullException argumentNullException)
			{
				if(argumentNullException.ParamName.Equals("resourceManager", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_IfTheResourceNamesCacheParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			try
			{
				var _ = new ResourceManagerStringLocalizer(Mock.Of<ResourceManager>(), Mock.Of<Assembly>(), "Test", null, Mock.Of<ILogger>());
			}
			catch(ArgumentNullException argumentNullException)
			{
				if(argumentNullException.ParamName.Equals("resourceNamesCache", StringComparison.Ordinal))
					throw;
			}
		}

		#endregion
	}
	// ReSharper restore All
}