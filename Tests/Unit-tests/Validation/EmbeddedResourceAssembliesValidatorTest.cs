using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Validation;

namespace UnitTests.Validation
{
	[TestClass]
	public class EmbeddedResourceAssembliesValidatorTest
	{
		#region Methods

		protected internal virtual IAssembly CreateAssembly(string name)
		{
			var assemblyMock = new Mock<IAssembly>();
			assemblyMock.Setup(assembly => assembly.ToString()).Returns(name);

			return assemblyMock.Object;
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Validate_WithAssemblyCollectionParameter_IfTheCollectionParameterContainsNullValues_ShouldThrowAnArgumentException_1()
		{
			try
			{
				new EmbeddedResourceAssembliesValidator().Validate(new IAssembly[] { null });
			}
			catch(ArgumentException argumentException)
			{
				const string messageStart = "Embedded-resource-assemblies-exception: The assemblies-collection can not contain null-values. Values: null";

				if(argumentException.ParamName.Equals("assemblies", StringComparison.Ordinal) && argumentException.Message.StartsWith(messageStart, StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Validate_WithAssemblyCollectionParameter_IfTheCollectionParameterContainsNullValues_ShouldThrowAnArgumentException_2()
		{
			try
			{
				new EmbeddedResourceAssembliesValidator().Validate(new[] { this.CreateAssembly("First-assembly"), null, this.CreateAssembly("Second-assembly") });
			}
			catch(ArgumentException argumentException)
			{
				const string messageStart = "Embedded-resource-assemblies-exception: The assemblies-collection can not contain null-values. Values: \"First-assembly\", null, \"Second-assembly\"";

				if(argumentException.ParamName.Equals("assemblies", StringComparison.Ordinal) && argumentException.Message.StartsWith(messageStart, StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Validate_WithAssemblyCollectionParameter_IfTheCollectionParameterContainsNullValues_ShouldThrowAnArgumentException_3()
		{
			try
			{
				new EmbeddedResourceAssembliesValidator().Validate(new[] { null, this.CreateAssembly("First-assembly"), null, this.CreateAssembly("Second-assembly"), null });
			}
			catch(ArgumentException argumentException)
			{
				const string messageStart = "Embedded-resource-assemblies-exception: The assemblies-collection can not contain null-values. Values: null, \"First-assembly\", null, \"Second-assembly\", null";

				if(argumentException.ParamName.Equals("assemblies", StringComparison.Ordinal) && argumentException.Message.StartsWith(messageStart, StringComparison.Ordinal))
					throw;

				Assert.Fail(argumentException.Message);
			}
		}

		[TestMethod]
		public void Validate_WithAssemblyCollectionParameter_IfTheParameterIsNull_ShouldNotThrowAnException()
		{
			try
			{
				new EmbeddedResourceAssembliesValidator().Validate((IEnumerable<IAssembly>)null);
			}
			catch
			{
				Assert.Fail("A parameter that is null should not throw an exception.");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Validate_WithPatterCollectionParameter_IfTheCollectionParameterContainsDuplicateValues_ShouldThrowAnArgumentException()
		{
			try
			{
				new EmbeddedResourceAssembliesValidator().Validate(new[] { "First", "Second*", "Third", "Second*" });
			}
			catch(ArgumentException argumentException)
			{
				const string messageStart = "Embedded-resource-assemblies-exception: The patterns-collection can not contain duplicate values. Values: \"First\", \"Second*\", \"Third\", \"Second*\"";

				if(argumentException.ParamName.Equals("patterns", StringComparison.Ordinal) && argumentException.Message.StartsWith(messageStart, StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Validate_WithPatterCollectionParameter_IfTheCollectionParameterContainsEmptyValues_ShouldThrowAnArgumentException()
		{
			try
			{
				new EmbeddedResourceAssembliesValidator().Validate(new[] { string.Empty, "First", string.Empty, "Second*", string.Empty });
			}
			catch(ArgumentException argumentException)
			{
				const string messageStart = "Embedded-resource-assemblies-exception: The patterns-collection can not contain empty values. Values: \"\", \"First\", \"\", \"Second*\", \"\"";

				if(argumentException.ParamName.Equals("patterns", StringComparison.Ordinal) && argumentException.Message.StartsWith(messageStart, StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Validate_WithPatterCollectionParameter_IfTheCollectionParameterContainsNullValues_ShouldThrowAnArgumentException()
		{
			try
			{
				new EmbeddedResourceAssembliesValidator().Validate(new[] { null, "First", null, "Second*", null });
			}
			catch(ArgumentException argumentException)
			{
				const string messageStart = "Embedded-resource-assemblies-exception: The patterns-collection can not contain null-values. Values: null, \"First\", null, \"Second*\", null";

				if(argumentException.ParamName.Equals("patterns", StringComparison.Ordinal) && argumentException.Message.StartsWith(messageStart, StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Validate_WithPatterCollectionParameter_IfTheCollectionParameterContainsWhiteSpaceOnlyValues_ShouldThrowAnArgumentException()
		{
			try
			{
				new EmbeddedResourceAssembliesValidator().Validate(new[] { " ", "First", "  ", "Second*", "   " });
			}
			catch(ArgumentException argumentException)
			{
				const string messageStart = "Embedded-resource-assemblies-exception: The patterns-collection can not contain white-space-only values. Values: \" \", \"First\", \"  \", \"Second*\", \"   \"";

				if(argumentException.ParamName.Equals("patterns", StringComparison.Ordinal) && argumentException.Message.StartsWith(messageStart, StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		public void Validate_WithPatternCollectionParameter_IfTheParameterIsNull_ShouldNotThrowAnException()
		{
			try
			{
				new EmbeddedResourceAssembliesValidator().Validate((IEnumerable<string>)null);
			}
			catch
			{
				Assert.Fail("A parameter that is null should not throw an exception.");
			}
		}

		#endregion
	}
}