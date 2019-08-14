using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.IntegrationTests.Helpers;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization.IntegrationTests
{
	[TestClass]
	public class LocalizationProviderTest : IntegrationTest
	{
		#region Methods

		[TestMethod]
		public void Configuration_LocalizationOptions_AlphabeticalSorting_Change_ShouldClearTheLocalizedStringListCache()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Default.json");

			var localizationProvider = (LocalizationProvider) serviceProvider.GetRequiredService<ILocalizationProvider>();

			this.ValidateLocalizedStringListCacheCleared(localizationProvider);
			this.PopulateLocalizedStringListCache(localizationProvider);

			var testContext = serviceProvider.GetService<ITestContext>();

			var configurationContent = File.ReadAllText(testContext.ConfigurationFilePath);

			var valueToReplace = "\"Localization\": {";
			var valueToReplaceWith = valueToReplace + Environment.NewLine + "\t\t\"AlphabeticalSorting\": false,";

			var newConfigurationContent = configurationContent.Replace(valueToReplace, valueToReplaceWith, StringComparison.Ordinal);

			File.WriteAllText(testContext.ConfigurationFilePath, newConfigurationContent);

			Thread.Sleep(500);

			this.ValidateLocalizedStringListCacheCleared(localizationProvider);
			this.PopulateLocalizedStringListCache(localizationProvider);

			valueToReplaceWith = valueToReplace + Environment.NewLine + "\t\t\"AlphabeticalSorting\": true,";

			newConfigurationContent = configurationContent.Replace(valueToReplace, valueToReplaceWith, StringComparison.Ordinal);

			File.WriteAllText(testContext.ConfigurationFilePath, newConfigurationContent);

			Thread.Sleep(500);

			this.ValidateLocalizedStringListCacheCleared(localizationProvider);
		}

		[TestMethod]
		public void Configuration_LocalizationOptions_IncludeParentCultures_Change_ShouldClearTheLocalizedStringCache()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Default.json");

			var localizationProvider = (LocalizationProvider) serviceProvider.GetRequiredService<ILocalizationProvider>();

			this.ValidateLocalizedStringCacheCleared(localizationProvider);
			this.PopulateLocalizedStringCache(localizationProvider);

			var testContext = serviceProvider.GetService<ITestContext>();

			var configurationContent = File.ReadAllText(testContext.ConfigurationFilePath);

			var valueToReplace = "],";
			var valueToReplaceWith = valueToReplace + Environment.NewLine + "\t\t\"IncludeParentCultures\": true,";

			var newConfigurationContent = configurationContent.Replace(valueToReplace, valueToReplaceWith, StringComparison.Ordinal);

			File.WriteAllText(testContext.ConfigurationFilePath, newConfigurationContent);

			Thread.Sleep(500);

			this.ValidateLocalizedStringCacheCleared(localizationProvider);
			this.PopulateLocalizedStringCache(localizationProvider);

			valueToReplaceWith = valueToReplace + Environment.NewLine + "\t\t\"IncludeParentCultures\": false,";

			newConfigurationContent = configurationContent.Replace(valueToReplace, valueToReplaceWith, StringComparison.Ordinal);

			File.WriteAllText(testContext.ConfigurationFilePath, newConfigurationContent);

			Thread.Sleep(500);

			this.ValidateLocalizedStringCacheCleared(localizationProvider);
		}

		[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
		protected internal virtual IEnumerable<ILocalizedString> GetAllLocalizedStrings(IAssemblyHelper assemblyHelper, bool includeParentCultures, LocalizationProvider localizationProvider)
		{
			var localizedStrings = new List<ILocalizedString>();

			foreach(var localization in localizationProvider.Localizations)
			{
				localizedStrings.AddRange(localizationProvider.List(localization.Resource?.Assembly ?? assemblyHelper.ApplicationAssembly, CultureInfo.GetCultureInfo(localization.CultureName), includeParentCultures));
			}

			return localizedStrings;
		}

		[TestMethod]
		public void GetLocalizationEntries_IfAnEntryIsOnADeepNode_ShouldWorkProperly()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-With-File-Resources-Directory-Path-Only.json");
			var localizationProvider = (LocalizationProvider) serviceProvider.GetService<ILocalizationProvider>();
			var localization = localizationProvider.Localizations.Where(item => item.CultureName.Equals("en", StringComparison.OrdinalIgnoreCase)).ElementAt(1);
			var localizationEntries = localizationProvider.GetLocalizationEntries(localization);

			var localizationEntry = localizationEntries.ElementAt(2);
			Assert.AreEqual("Very.Deep.Test.First.Second.Third.Fourth.Fifth.Sixth.Seventh.Eighth.Ninth.Tenth.Text", localizationEntry.Key);
			Assert.AreEqual("Very deep test-value: en, \"\\Integration-tests\\Resources\\Texts.en.json\"", localizationEntry.Value.Value);
		}

		[TestMethod]
		public void LocalizationSettings_AlphabeticalSorting_Change_ShouldClearTheLocalizedStringListCache()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Default.json");

			var localizationProvider = (LocalizationProvider) serviceProvider.GetRequiredService<ILocalizationProvider>();
			var localizationSettings = serviceProvider.GetRequiredService<ILocalizationSettings>();

			this.ValidateLocalizedStringListCacheCleared(localizationProvider);
			this.PopulateLocalizedStringListCache(localizationProvider);

			localizationSettings.AlphabeticalSorting = !localizationSettings.AlphabeticalSorting;

			this.ValidateLocalizedStringListCacheCleared(localizationProvider);
			this.PopulateLocalizedStringListCache(localizationProvider);

			localizationSettings.AlphabeticalSorting = !localizationSettings.AlphabeticalSorting;

			this.ValidateLocalizedStringListCacheCleared(localizationProvider);
		}

		[TestMethod]
		public void LocalizationSettings_IncludeParentCultures_Change_ShouldClearTheLocalizedStringCache()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Default.json");

			var localizationProvider = (LocalizationProvider) serviceProvider.GetRequiredService<ILocalizationProvider>();
			var localizationSettings = serviceProvider.GetRequiredService<ILocalizationSettings>();

			this.ValidateLocalizedStringCacheCleared(localizationProvider);
			this.PopulateLocalizedStringCache(localizationProvider);

			localizationSettings.IncludeParentCultures = !localizationSettings.IncludeParentCultures;

			this.ValidateLocalizedStringCacheCleared(localizationProvider);
			this.PopulateLocalizedStringCache(localizationProvider);

			localizationSettings.IncludeParentCultures = !localizationSettings.IncludeParentCultures;

			this.ValidateLocalizedStringCacheCleared(localizationProvider);
		}

		[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
		protected internal virtual void PopulateLocalizedStringCache(LocalizationProvider localizationProvider)
		{
			localizationProvider.LocalizedStringCache.GetOrAdd("Test", _ => null);
			Assert.AreEqual(1, localizationProvider.LocalizedStringCache.Count);
		}

		[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
		protected internal virtual void PopulateLocalizedStringListCache(LocalizationProvider localizationProvider)
		{
			localizationProvider.LocalizedStringsWithoutParentCulturesIncludedCache.GetOrAdd("Test", _ => Enumerable.Empty<ILocalizedString>());
			Assert.AreEqual(1, localizationProvider.LocalizedStringsWithoutParentCulturesIncludedCache.Count);

			localizationProvider.LocalizedStringsWithParentCulturesIncludedCache.GetOrAdd("Test", _ => Enumerable.Empty<ILocalizedString>());
			Assert.AreEqual(1, localizationProvider.LocalizedStringsWithParentCulturesIncludedCache.Count);
		}

		[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
		protected internal virtual void ValidateLocalizedStringCacheCleared(LocalizationProvider localizationProvider)
		{
			Assert.AreEqual(0, localizationProvider.LocalizedStringCache.Count, this.PossibleReasonForFailure);
		}

		[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
		protected internal virtual void ValidateLocalizedStringListCacheCleared(LocalizationProvider localizationProvider)
		{
			Assert.AreEqual(0, localizationProvider.LocalizedStringsWithoutParentCulturesIncludedCache.Count, this.PossibleReasonForFailure);
			Assert.AreEqual(0, localizationProvider.LocalizedStringsWithParentCulturesIncludedCache.Count, this.PossibleReasonForFailure);
		}

		#endregion

		//protected internal virtual void ValidateLocalizedStringsCache(IAssemblyHelper assemblyHelper, LocalizationProvider localizationProvider)
		//{
		//	Assert.AreEqual(0, localizationProvider.LocalizedStringsWithoutParentCulturesIncludedCache.Count);
		//	Assert.AreEqual(0, localizationProvider.LocalizedStringsWithParentCulturesIncludedCache.Count);

		//	var localizedStringsWithoutParentCulturesIncluded = this.GetAllLocalizedStrings(assemblyHelper, false, localizationProvider);

		//	Assert.AreEqual(782, localizedStringsWithoutParentCulturesIncluded.Count());
		//	Assert.AreEqual(20, localizationProvider.LocalizedStringsWithoutParentCulturesIncludedCache.Count);
		//	Assert.AreEqual(0, localizationProvider.LocalizedStringsWithParentCulturesIncludedCache.Count);

		//	var localizedStringsWithParentCulturesIncluded = this.GetAllLocalizedStrings(assemblyHelper, true, localizationProvider);

		//	Assert.AreEqual(899, localizedStringsWithParentCulturesIncluded.Count());
		//	Assert.AreEqual(45, localizationProvider.LocalizedStringsWithoutParentCulturesIncludedCache.Count);
		//	Assert.AreEqual(20, localizationProvider.LocalizedStringsWithParentCulturesIncludedCache.Count);
		//}
	}
}