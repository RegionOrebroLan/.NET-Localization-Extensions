using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization;
using RegionOrebroLan.Localization.Configuration;

namespace IntegrationTests
{
	[TestClass]
	public class LocalizationSettingsTest : IntegrationTest
	{
		#region Methods

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Constructor_Options_IfTheEmbeddedResourceAssembliesParameterContainNullValues_ShouldThrowAnArguementException()
		{
			var serviceProvider = this.BuildServiceProvider((_, services) =>
			{
				var localizationOptions = new LocalizationOptions();
				// ReSharper disable AssignNullToNotNullAttribute
				localizationOptions.EmbeddedResourceAssemblies.Add(null);
				// ReSharper restore AssignNullToNotNullAttribute

				var optionsMock = new Mock<IOptions<LocalizationOptions>>();

				optionsMock.Setup(options => options.Value).Returns(localizationOptions);

				services.AddSingleton(optionsMock.Object);
			}, "Configuration-Empty.json");

			try
			{
				serviceProvider.GetRequiredService<ILocalizationSettings>();
			}
			catch(ArgumentException argumentException)
			{
				const string messageStart = "Embedded-resource-assemblies-exception: The patterns-collection can not contain null-values. Values: null";

				if(argumentException.ParamName.Equals("patterns", StringComparison.Ordinal) && argumentException.Message.StartsWith(messageStart, StringComparison.Ordinal))
					throw;
			}
		}

		#endregion
	}
}