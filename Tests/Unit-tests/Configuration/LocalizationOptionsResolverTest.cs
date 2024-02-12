using System.IO.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Configuration;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Validation;

namespace UnitTests.Configuration
{
	[TestClass]
	public class LocalizationOptionsResolverTest
	{
		#region Methods

		protected internal virtual LocalizationOptionsResolver CreateLocalizationOptionsResolver()
		{
			return new LocalizationOptionsResolver(Mock.Of<IAssemblyHelper>(), Mock.Of<IEmbeddedResourceAssembliesValidator>(), Mock.Of<IFileResourcesDirectoryValidator>(), Mock.Of<IFileSystem>(), Mock.Of<IHostEnvironment>());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Resolve_IfTheLocalizationOptionsParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			try
			{
				this.CreateLocalizationOptionsResolver().Resolve(null);
			}
			catch(ArgumentNullException argumentNullException)
			{
				if(argumentNullException.ParamName.Equals("localizationOptions", StringComparison.Ordinal))
					throw;
			}
		}

		#endregion
	}
}