using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization.Resourcing;
using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.UnitTests
{
	[TestClass]
	public class LocalizationProviderTest
	{
		#region Fields

		private static IResourceProvider _resourceProvider;
		private static ILocalizationSettings _settings;

		#endregion

		#region Properties

		protected internal virtual IResourceProvider ResourceProvider
		{
			get
			{
				// ReSharper disable InvertIf
				if(_resourceProvider == null)
				{
					var resourceProviderMock = new Mock<IResourceProvider>();
					resourceProviderMock.SetupAllProperties();
					//resourceProviderMock.Setup(resourceProvider => resourceProvider.Settings).Returns(this.Settings);

					_resourceProvider = resourceProviderMock.Object;
				}
				// ReSharper restore InvertIf

				return _resourceProvider;
			}
		}

		protected internal virtual ILocalizationSettings Settings
		{
			get
			{
				// ReSharper disable InvertIf
				if(_settings == null)
				{
					var optionsMock = new Mock<ILocalizationSettings>();
					optionsMock.SetupAllProperties();

					_settings = optionsMock.Object;
				}
				// ReSharper restore InvertIf

				return _settings;
			}
		}

		#endregion

		#region Methods

		[TestMethod]
		public void CompareLocalizations_SortingAList_ShouldKeepTheOriginalOrderBetweenTheLocalizationsWherePriorityIsNull()
		{
			var localizations = new List<ILocalization>
			{
				this.CreateLocalization("1", null),
				this.CreateLocalization("5", 100),
				this.CreateLocalization("2", null),
				this.CreateLocalization("4", 10),
				this.CreateLocalization("3", null)
			};

			localizations.Sort(this.CreateLocalizationProvider().CompareLocalizations);

			Assert.AreEqual("1", localizations[0].CultureName);
			Assert.AreEqual("2", localizations[1].CultureName);
			Assert.AreEqual("3", localizations[2].CultureName);
			Assert.AreEqual("4", localizations[3].CultureName);
			Assert.AreEqual("5", localizations[4].CultureName);
		}

		[TestMethod]
		public void CompareLocalizations_SortingAList_ShouldPutNullValuedLocationsFirst()
		{
			var localizations = new List<ILocalization>
			{
				this.CreateLocalization("4", 10),
				null,
				this.CreateLocalization("3", null),
				null
			};

			localizations.Sort(this.CreateLocalizationProvider().CompareLocalizations);

			Assert.IsNull(localizations[0]);
			Assert.IsNull(localizations[1]);
			Assert.AreEqual("3", localizations[2].CultureName);
			Assert.AreEqual("4", localizations[3].CultureName);
		}

		protected internal virtual ILocalization CreateLocalization(string cultureName, int? priority)
		{
			var localizationMock = new Mock<ILocalization>();

			localizationMock.SetupAllProperties();
			localizationMock.Setup(localization => localization.CultureName).Returns(cultureName);
			localizationMock.Setup(localization => localization.Priority).Returns(priority);

			return localizationMock.Object;
		}

		protected internal virtual LocalizationProvider CreateLocalizationProvider()
		{
			return new LocalizationProvider(Mock.Of<ILocalizationPathResolver>(), Mock.Of<ILocalizedStringFactory>(), Mock.Of<ILoggerFactory>(), this.ResourceProvider, this.Settings);
		}

		protected internal virtual LocalizationProvider CreateLocalizationProvider(bool throwErrors)
		{
			var localizationProvider = this.CreateLocalizationProvider();
			localizationProvider.Settings.ThrowErrors = throwErrors;
			return localizationProvider;
		}

		#endregion
	}
}