using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Validation;

namespace IntegrationTests.Validation
{
	[TestClass]
	public class EmbeddedResourceAssembliesValidatorTest
	{
		#region Fields

		private static IAssemblyHelper _assemblyHelper;
		private static IServiceProvider _serviceProvider;

		#endregion

		#region Properties

		protected internal virtual IAssemblyHelper AssemblyHelper => _assemblyHelper ?? (_assemblyHelper = this.ServiceProvider.GetService<IAssemblyHelper>());
		protected internal virtual IServiceProvider ServiceProvider => _serviceProvider;

		#endregion

		#region Methods

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			if(testContext == null)
				throw new ArgumentNullException(nameof(testContext));

			var services = new ServiceCollection();

			services.TryAddSingleton<IAssemblyHelper, AssemblyHelper>();
			services.TryAddSingleton<IRootNamespaceResolver, RootNamespaceResolver>();

			_serviceProvider = services.BuildServiceProvider();
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Validate_WithAssemblyCollectionParameter_IfTheCollectionParameterContainsDuplicateValues_ShouldThrowAnArgumentException()
		{
			try
			{
				new EmbeddedResourceAssembliesValidator().Validate(new[] { this.AssemblyHelper.Wrap(this.GetType().Assembly), this.AssemblyHelper.Wrap(typeof(string).Assembly), this.AssemblyHelper.Wrap(this.GetType().Assembly) });
			}
			catch(ArgumentException argumentException)
			{
				var stringAssemblyFullName = typeof(string).Assembly.FullName;
				var thisAssemblyFullname = this.GetType().Assembly.FullName;

				var messageStart = "Embedded-resource-assemblies-exception: The assemblies-collection can not contain duplicate values. Values: \"" + thisAssemblyFullname + "\", \"" + stringAssemblyFullName + "\", \"" + thisAssemblyFullname + "\"";

				if(argumentException.ParamName.Equals("assemblies", StringComparison.Ordinal) && argumentException.Message.StartsWith(messageStart, StringComparison.Ordinal))
					throw;
			}
		}

		#endregion
	}
}