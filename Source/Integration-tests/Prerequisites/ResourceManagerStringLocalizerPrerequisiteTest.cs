using System;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Investigation.Localization;

namespace RegionOrebroLan.Localization.IntegrationTests.Prerequisites
{
	[TestClass]
	public class ResourceManagerStringLocalizerPrerequisiteTest
	{
		#region Fields

		private static readonly LocalizationOptions _localizationOptions = new LocalizationOptions();
		private static readonly ILoggerFactory _loggerFactory = new LoggerFactory();

		#endregion

		#region Properties

		protected internal virtual LocalizationOptions LocalizationOptions => _localizationOptions;

		protected internal virtual ILoggerFactory LoggerFactory => _loggerFactory;

		#endregion

		#region Methods

		protected internal virtual InvestigatableResourceManagerStringLocalizerFactory CreateInvestigatableResourceManagerStringLocalizerFactory(string resourcePath)
		{
			return this.CreateInvestigatableResourceManagerStringLocalizerFactory(new LocalizationOptions {ResourcesPath = resourcePath});
		}

		protected internal virtual InvestigatableResourceManagerStringLocalizerFactory CreateInvestigatableResourceManagerStringLocalizerFactory(LocalizationOptions localizationOptions)
		{
			return new InvestigatableResourceManagerStringLocalizerFactory(Options.Create(localizationOptions), this.LoggerFactory);
		}

		[TestMethod]
		public void GetAllStrings_IncludeParentCultures_Test()
		{
			var stringLocalizer = new ResourceManagerStringLocalizerFactory(Options.Create(this.LocalizationOptions), Global.LoggerFactory).Create("Colors", "Colors");

			stringLocalizer = stringLocalizer.WithCulture(CultureInfo.GetCultureInfo("de-DE"));

			var allStrings = stringLocalizer.GetAllStrings(true).ToArray();

			Assert.AreEqual(3, allStrings.Length);

			var localizedString = allStrings[0];

			Assert.AreEqual("Red", localizedString.Name);
			Assert.IsFalse(localizedString.ResourceNotFound);
			Assert.AreEqual("Colors.Colors", localizedString.SearchedLocation);
			Assert.AreEqual("Red: \"\\Embedded-resources\\Colors\\Colors.resx\"", localizedString.Value);

			stringLocalizer = stringLocalizer.WithCulture(CultureInfo.GetCultureInfo("en-US"));

			allStrings = stringLocalizer.GetAllStrings(true).ToArray();

			Assert.AreEqual(3, allStrings.Length);

			localizedString = allStrings[0];

			Assert.AreEqual("Red", localizedString.Name);
			Assert.IsFalse(localizedString.ResourceNotFound);
			Assert.AreEqual("Colors.Colors", localizedString.SearchedLocation);
			Assert.AreEqual("Red: \"\\Embedded-resources\\Colors\\Colors.en.resx\"", localizedString.Value);
		}

		[TestMethod]
		public void GetAllStrings_Test()
		{
			var stringLocalizerFactory = this.CreateInvestigatableResourceManagerStringLocalizerFactory(string.Empty);
			var stringLocalizer = (InvestigatableResourceManagerStringLocalizer) stringLocalizerFactory.Create("Colors", "Colors");

			var allStrings = stringLocalizer.GetAllStrings();
			Assert.AreEqual(3, allStrings.Count());

			allStrings = stringLocalizer.GetProtectedAllStrings(true, CultureInfo.InvariantCulture);
			Assert.AreEqual(3, allStrings.Count());

			allStrings = stringLocalizer.GetProtectedAllStrings(true, CultureInfo.GetCultureInfo("en"));
			Assert.AreEqual(3, allStrings.Count());

			allStrings = stringLocalizer.GetProtectedAllStrings(true, CultureInfo.GetCultureInfo("en-US"));
			Assert.AreEqual(3, allStrings.Count());

			allStrings = stringLocalizer.GetProtectedAllStrings(true, CultureInfo.GetCultureInfo("fi"));
			Assert.AreEqual(3, allStrings.Count());

			allStrings = stringLocalizer.GetProtectedAllStrings(true, CultureInfo.GetCultureInfo("fi-FI"));
			Assert.AreEqual(3, allStrings.Count());

			allStrings = stringLocalizer.GetProtectedAllStrings(true, CultureInfo.GetCultureInfo("sv"));
			Assert.AreEqual(3, allStrings.Count());

			allStrings = stringLocalizer.GetProtectedAllStrings(true, CultureInfo.GetCultureInfo("sv-SE"));
			Assert.AreEqual(3, allStrings.Count());
		}

		#endregion
	}
}