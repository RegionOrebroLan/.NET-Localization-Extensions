using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Collections.Extensions;
using RegionOrebroLan.Localization.Extensions;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Resourcing;
using RegionOrebroLan.Localization.Serialization;
using RegionOrebroLan.Logging.Extensions;

namespace RegionOrebroLan.Localization
{
	public class LocalizationProvider : ILocalizationProvider
	{
		#region Constructors

		[CLSCompliant(false)]
		public LocalizationProvider(ILocalizationPathResolver localizationPathResolver, ILocalizedStringFactory localizedStringFactory, ILoggerFactory loggerFactory, IResourceProvider resourceProvider, ILocalizationSettings settings)
		{
			this.LocalizationPathResolver = localizationPathResolver ?? throw new ArgumentNullException(nameof(localizationPathResolver));
			this.LocalizedStringFactory = localizedStringFactory ?? throw new ArgumentNullException(nameof(localizedStringFactory));
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
			this.ResourceProvider = resourceProvider ?? throw new ArgumentNullException(nameof(resourceProvider));
			this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));

			resourceProvider.EmbeddedResourcesChanged += this.OnEmbeddedResourcesChanged;
			resourceProvider.FileResourceContentChanged += this.OnFileResourceContentChanged;
			resourceProvider.FileResourcesChanged += this.OnFileResourcesChanged;
			settings.AlphabeticalSortingChanged += this.OnAlphabeticalSortingChanged;
			settings.IncludeParentCulturesChanged += this.OnIncludeParentCulturesChanged;
		}

		#endregion

		#region Properties

		protected internal virtual ILocalizationPathResolver LocalizationPathResolver { get; }

		protected internal virtual IEnumerable<ILocalization> Localizations
		{
			get
			{
				// ReSharper disable InvertIf
				if(this.LocalizationsCache == null)
				{
					lock(this.LocalizationsLock)
					{
						if(this.LocalizationsCache == null)
						{
							var localizations = new List<ILocalization>();

							foreach(var item in this.ResourceContent)
							{
								try
								{
									var parsedLocalizations = item.Key.Parser.Parse(item.Key, item.Value);

									if(parsedLocalizations == null)
										continue;

									localizations.AddRange(parsedLocalizations);
								}
								catch(Exception exception)
								{
									var message = $"Could not parse the following value, from resource \"{item.Key}\", to localizations: \"{item.Value}\"";

									this.Logger.LogErrorIfEnabled(exception, message);

									if(this.Settings.ThrowErrors)
										throw new InvalidOperationException(message, exception);
								}
							}

							localizations.Sort(this.CompareLocalizations);

							this.LocalizationsCache = localizations.ToArray();
						}
					}
				}
				// ReSharper restore InvertIf

				return this.LocalizationsCache;
			}
		}

		protected internal virtual IEnumerable<ILocalization> LocalizationsCache { get; set; }
		protected internal virtual object LocalizationsLock { get; } = new object();
		protected internal virtual ConcurrentDictionary<string, ILocalizedString> LocalizedStringCache { get; } = new ConcurrentDictionary<string, ILocalizedString>(StringComparer.OrdinalIgnoreCase);
		protected internal virtual ILocalizedStringFactory LocalizedStringFactory { get; }
		protected internal virtual ConcurrentDictionary<string, IEnumerable<ILocalizedString>> LocalizedStringsWithoutParentCulturesIncludedCache { get; } = new ConcurrentDictionary<string, IEnumerable<ILocalizedString>>(StringComparer.OrdinalIgnoreCase);
		protected internal virtual ConcurrentDictionary<string, IEnumerable<ILocalizedString>> LocalizedStringsWithParentCulturesIncludedCache { get; } = new ConcurrentDictionary<string, IEnumerable<ILocalizedString>>(StringComparer.OrdinalIgnoreCase);

		[CLSCompliant(false)]
		protected internal virtual ILogger Logger { get; }

		protected internal virtual IDictionary<IResource, string> ResourceContent
		{
			get
			{
				// ReSharper disable InvertIf
				if(this.ResourceContentCache == null)
				{
					lock(this.ResourceContentLock)
					{
						if(this.ResourceContentCache == null)
							this.ResourceContentCache = this.Resources.ToDictionary(resource => resource, resource => resource.Read());
					}
				}
				// ReSharper restore InvertIf

				return this.ResourceContentCache;
			}
		}

		[SuppressMessage("Usage", "CA2227:Collection properties should be read only")]
		protected internal virtual IDictionary<IResource, string> ResourceContentCache { get; set; }

		protected internal virtual object ResourceContentLock { get; } = new object();
		protected internal virtual IResourceProvider ResourceProvider { get; }
		protected internal virtual IEnumerable<IResource> Resources => ((IEnumerable<IResource>)this.ResourceProvider.EmbeddedResources).Concat(this.ResourceProvider.FileResources);
		protected internal virtual ILocalizationSettings Settings { get; }

		#endregion

		#region Methods

		protected internal virtual void ClearLocalizationsCache()
		{
			this.Logger.LogInformationIfEnabled("Clearing the localizations cache.");

			lock(this.LocalizationsLock)
			{
				this.LocalizationsCache = null;

				this.ClearLocalizedStringListCache();

				this.ClearLocalizedStringCache();
			}
		}

		protected internal virtual void ClearLocalizedStringCache()
		{
			this.Logger.LogInformationIfEnabled("Clearing the localized-string cache.");

			this.LocalizedStringCache.Clear();
		}

		protected internal virtual void ClearLocalizedStringListCache()
		{
			this.Logger.LogInformationIfEnabled("Clearing the localized-string-list cache.");

			this.LocalizedStringsWithoutParentCulturesIncludedCache.Clear();
			this.LocalizedStringsWithParentCulturesIncludedCache.Clear();
		}

		protected internal virtual void ClearResourceContentCache()
		{
			this.Logger.LogInformationIfEnabled("Clearing the resource-content cache.");

			lock(this.ResourceContentLock)
			{
				this.ResourceContentCache = null;

				this.ClearLocalizationsCache();
			}
		}

		protected internal virtual int CompareLocalizations(ILocalization firstLocalization, ILocalization secondLocalization)
		{
			if(firstLocalization == null)
				return secondLocalization != null ? -1 : 0;

			if(secondLocalization == null)
				return 1;

			if(firstLocalization.Priority == null)
				return secondLocalization.Priority != null ? -1 : 0;

			// ReSharper disable ConvertIfStatementToReturnStatement
			if(secondLocalization.Priority == null)
				return 1;
			// ReSharper restore ConvertIfStatementToReturnStatement

			return firstLocalization.Priority.Value.CompareTo(secondLocalization.Priority.Value);
		}

		[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
		public virtual ILocalizedString Get(IEnumerable<object> arguments, IAssembly assembly, CultureInfo culture, string name, string path)
		{
			if(assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			if(culture == null)
				throw new ArgumentNullException(nameof(culture));

			var argumentArray = arguments?.ToArray();

			var key = string.Format(CultureInfo.InvariantCulture, "Arguments={1}{0}Assembly={2}{0}Culture={3}{0}Name={4}{0}Path={5}", "&", argumentArray.ToCommaSeparatedArgumentString(), assembly.ToArgumentString(), culture.ToArgumentString(), name.ToArgumentString(), path.ToArgumentString());

			return this.LocalizedStringCache.GetOrAdd(key, _ => this.GetInternal(argumentArray, assembly, culture, name, path));
		}

		protected internal virtual ILocalizedString GetInternal(IEnumerable<object> arguments, IAssembly assembly, CultureInfo culture, string name, string path)
		{
			var information = new List<string>
			{
				"Get-function" + Environment.NewLine + "------------" + Environment.NewLine + this.GetLocalizedStringInformation(arguments, assembly, culture, this.Settings.IncludeParentCultures, name, path)
			};

			var localizedStrings = new List<ILocalizedString>();
			var resolvedPaths = this.LocalizationPathResolver.Resolve(assembly, name, path).ToArray();

			if(resolvedPaths.Any())
			{
				foreach(var resolvedPath in resolvedPaths)
				{
					var localizedString = this.List(assembly, culture, this.Settings.IncludeParentCultures).FirstOrDefault(item => string.Equals(item.Name, resolvedPath, StringComparison.OrdinalIgnoreCase));

					information.Add(this.GetLocalizedStringInformation(localizedString, resolvedPath));

					if(localizedString == null)
						continue;

					localizedStrings.Add(localizedString);

					if(localizedString.Culture.Equals(culture) && !localizedString.ResourceNotFound)
						break;
				}
			}
			else
			{
				information.Add("No localization-paths resolved.");
			}

			var bestLocalizedString = localizedStrings.FirstOrDefault(item => item.Culture.Equals(culture) && !item.ResourceNotFound)
			                          ?? localizedStrings.FirstOrDefault(item => !item.ResourceNotFound)
			                          ?? localizedStrings.FirstOrDefault();

			information.Add(bestLocalizedString != null ? bestLocalizedString.SearchedLocation : this.GetSearchedLocations(assembly, culture, this.Resources));

			var argumentArray = (arguments ?? Enumerable.Empty<object>()).ToArray();
			var value = bestLocalizedString?.Value;

			if(bestLocalizedString != null && !bestLocalizedString.ResourceNotFound && value != null && argumentArray.Any())
				value = string.Format(CultureInfo.InvariantCulture, value, argumentArray);

			return this.LocalizedStringFactory.Create(bestLocalizedString?.Culture ?? culture, string.Join(Environment.NewLine + Environment.NewLine, information), bestLocalizedString?.Name ?? resolvedPaths.FirstOrDefault() ?? string.Empty, value);
		}

		protected internal virtual IDictionary<string, ILocalizationEntry> GetLocalizationEntries(ILocalization localization)
		{
			if(localization == null)
				throw new ArgumentNullException(nameof(localization));

			var localizationEntries = new Dictionary<string, ILocalizationEntry>(StringComparer.OrdinalIgnoreCase);

			this.PopulateLocalizationEntries(localizationEntries, localization, null);

			return localizationEntries;
		}

		protected internal virtual string GetLocalizedStringInformation(ILocalizedString localizedString, string resolvedPath)
		{
			// ReSharper disable ConvertIfStatementToReturnStatement
			if(localizedString == null)
				return $"Resolved path: {resolvedPath.ToArgumentString()}{Environment.NewLine} - No hit";
			// ReSharper restore ConvertIfStatementToReturnStatement

			return string.Format(CultureInfo.InvariantCulture, "Resolved path: {2}{0}{1}Culture: {3}{0}{1}Name: {4}{0}{1}Resource not found: {5}{0}{1}Value: {6}", Environment.NewLine, " - ", resolvedPath.ToArgumentString(), localizedString.Culture.ToArgumentString(), localizedString.Name.ToArgumentString(), localizedString.ResourceNotFound, localizedString.Value.ToArgumentString());
		}

		protected internal virtual string GetLocalizedStringInformation(IEnumerable<object> arguments, IAssembly assembly, CultureInfo culture, bool includeParentCultures, string name, string path)
		{
			return string.Format(CultureInfo.InvariantCulture, "Arguments: {1}{0}Assembly: {2}{0}Culture: {3}{0}Include parent-cultures: {4}{0}Name: {5}{0}Path: {6}", Environment.NewLine, arguments.ToCommaSeparatedArgumentString(), assembly.ToArgumentString(), culture.ToArgumentString(), includeParentCultures, name.ToArgumentString(), path.ToArgumentString());
		}

		protected internal virtual string GetLocalizedStringInformation(CultureInfo culture, string lookup, string lookupValue, string name, string value)
		{
			return string.Format(CultureInfo.InvariantCulture, "Culture: {1}{0}Lookup: {2}{0}Lookup-value: {3}{0}Name: {4}{0}Value: {5}", Environment.NewLine, culture.ToArgumentString(), lookup.ToArgumentString(), lookupValue.ToArgumentString(), name.ToArgumentString(), value.ToArgumentString());
		}

		protected internal virtual string GetSearchedLocations(IAssembly assembly, CultureInfo culture, IEnumerable<IResource> resources)
		{
			resources = (resources ?? Enumerable.Empty<IResource>()).ToArray();
			var separator = Environment.NewLine + " - ";
			var resourcesAsString = (resources.Any() ? separator : string.Empty) + resources.ToSeparatedArgumentString(separator);

			return string.Format(CultureInfo.InvariantCulture, "Searched locations{0}------------------{0}Assembly: {1}{0}Culture: {2}{0}Resources:{3}", Environment.NewLine, assembly.ToArgumentString(), culture.ToArgumentString(), resourcesAsString);
		}

		public virtual IEnumerable<ILocalizedString> List(IAssembly assembly, CultureInfo culture, bool includeParentCultures)
		{
			return includeParentCultures ? this.ListWithParentCulturesIncluded(assembly, culture) : this.ListWithoutParentCulturesIncluded(assembly, culture);
		}

		protected internal virtual IEnumerable<ILocalizedString> ListWithoutParentCulturesIncluded(IAssembly assembly, CultureInfo culture)
		{
			if(assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			if(culture == null)
				throw new ArgumentNullException(nameof(culture));

			var key = string.Format(CultureInfo.InvariantCulture, "Assembly={1}{0}Culture={2}", "&", assembly.ToArgumentString(), culture.ToArgumentString());

			return this.LocalizedStringsWithoutParentCulturesIncludedCache.GetOrAdd(key, _ => this.ListWithoutParentCulturesIncludedInternal(assembly, culture));
		}

		protected internal virtual IEnumerable<ILocalizedString> ListWithoutParentCulturesIncludedInternal(IAssembly assembly, CultureInfo culture)
		{
			if(assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			if(culture == null)
				throw new ArgumentNullException(nameof(culture));

			var localizationEntries = new Dictionary<string, ILocalizationEntry>(StringComparer.OrdinalIgnoreCase);

			foreach(var localization in this.Localizations.Where(localization => string.Equals(localization.CultureName, culture.Name, StringComparison.OrdinalIgnoreCase)))
			{
				foreach(var localizationEntry in this.GetLocalizationEntries(localization))
				{
					localizationEntries[localizationEntry.Key] = localizationEntry.Value;
				}
			}

			var searchedLocations = this.GetSearchedLocations(assembly, culture, this.Resources);

			var localizedStrings = new List<ILocalizedString>();

			foreach(var localizationEntry in localizationEntries)
			{
				string lookupValue = null;

				if(localizationEntry.Value.Lookup != null)
				{
					if(localizationEntries.TryGetValue(localizationEntry.Value.Lookup, out var lookedupEntry))
						lookupValue = lookedupEntry.Value;
				}

				var information = new List<string>
				{
					"List-function" + Environment.NewLine + "-------------" + Environment.NewLine + this.GetLocalizedStringInformation(culture, localizationEntry.Value.Lookup, lookupValue, localizationEntry.Key, localizationEntry.Value.Value),
					searchedLocations
				};

				var value = this.ResolveLocalizedStringValue(lookupValue, localizationEntry.Value.Value);

				localizedStrings.Add(this.LocalizedStringFactory.Create(culture, string.Join(Environment.NewLine + Environment.NewLine, information), localizationEntry.Key, value));
			}

			return this.Settings.AlphabeticalSorting ? localizedStrings.OrderBy(item => item.Name).ToArray() : localizedStrings.ToArray();
		}

		protected internal virtual IEnumerable<ILocalizedString> ListWithParentCulturesIncluded(IAssembly assembly, CultureInfo culture)
		{
			if(assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			if(culture == null)
				throw new ArgumentNullException(nameof(culture));

			var key = string.Format(CultureInfo.InvariantCulture, "Assembly={1}{0}Culture={2}", "&", assembly.ToArgumentString(), culture.ToArgumentString());

			return this.LocalizedStringsWithParentCulturesIncludedCache.GetOrAdd(key, _ => this.ListWithParentCulturesIncludedInternal(assembly, culture));
		}

		protected internal virtual IEnumerable<ILocalizedString> ListWithParentCulturesIncludedInternal(IAssembly assembly, CultureInfo culture)
		{
			if(assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			if(culture == null)
				throw new ArgumentNullException(nameof(culture));

			var cultures = new List<CultureInfo>();

			while(culture != null)
			{
				cultures.Add(culture);

				culture = culture.Equals(culture.Parent) ? null : culture.Parent;
			}

			var localizedStringsPerCulture = new Dictionary<CultureInfo, IEnumerable<ILocalizedString>>();

			foreach(var item in cultures)
			{
				localizedStringsPerCulture.Add(item, this.ListWithoutParentCulturesIncluded(assembly, item));
			}

			var localizedStrings = localizedStringsPerCulture[cultures[0]].ToList();

			for(var i = 1; i < cultures.Count; i++)
			{
				foreach(var localizedString in localizedStringsPerCulture[cultures[i]])
				{
					if(!localizedStrings.Any(item => string.Equals(item.Name, localizedString.Name, StringComparison.OrdinalIgnoreCase)))
						localizedStrings.Add(localizedString);
				}
			}

			return this.Settings.AlphabeticalSorting ? localizedStrings.OrderBy(item => item.Name).ToArray() : localizedStrings.ToArray();
		}

		protected internal virtual void OnAlphabeticalSortingChanged(object sender, EventArgs e)
		{
			this.ClearLocalizedStringListCache();
		}

		protected internal virtual void OnEmbeddedResourcesChanged(object sender, EventArgs e)
		{
			this.ClearResourceContentCache();
		}

		protected internal virtual void OnFileResourceContentChanged(object sender, FileResourceChangedEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException(nameof(e));

			this.UpdateResourceContent(e.Path);
		}

		protected internal virtual void OnFileResourcesChanged(object sender, EventArgs e)
		{
			this.ClearResourceContentCache();
		}

		protected internal virtual void OnIncludeParentCulturesChanged(object sender, EventArgs e)
		{
			this.ClearLocalizedStringCache();
		}

		protected internal virtual void PopulateLocalizationEntries(IDictionary<string, ILocalizationEntry> localizationEntries, ILocalizationNode node, string parentPath)
		{
			if(localizationEntries == null)
				throw new ArgumentNullException(nameof(localizationEntries));

			if(node == null)
				throw new ArgumentNullException(nameof(node));

			foreach(var child in node.Nodes)
			{
				this.PopulateLocalizationEntries(localizationEntries, child, this.LocalizationPathResolver.Combine(parentPath ?? string.Empty, node.Name ?? string.Empty));
			}

			foreach(var entry in node.Entries)
			{
				var path = this.LocalizationPathResolver.Combine(parentPath ?? string.Empty, node.Name ?? string.Empty, entry.Key);

				localizationEntries[path] = entry.Value;
			}
		}

		protected internal virtual string ResolveLocalizedStringValue(string lookupValue, string value)
		{
			return value ?? lookupValue;
		}

		[SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code")]
		protected internal virtual void UpdateResourceContent(string path)
		{
			if(path == null)
				throw new ArgumentNullException(nameof(path));

			if(this.ResourceContentCache == null)
				return;

			var fileResource = this.ResourceProvider.FileResources.FirstOrDefault(item => string.Equals(item.Path, path, StringComparison.OrdinalIgnoreCase));

			if(fileResource == null)
			{
				this.Logger.LogWarningIfEnabled("The file-resource with path \"{0}\" could not be found.", path);
				return;
			}

			if(this.ResourceContentCache == null || !this.ResourceContentCache.ContainsKey(fileResource))
				return;

			lock(this.ResourceContentLock)
			{
				this.ResourceContentCache[fileResource] = fileResource.Read();
			}

			this.ClearLocalizationsCache();
		}

		#endregion

		#region Other members

		#region Finalizers

		~LocalizationProvider()
		{
			this.Settings.IncludeParentCulturesChanged -= this.OnIncludeParentCulturesChanged;
			this.Settings.AlphabeticalSortingChanged -= this.OnAlphabeticalSortingChanged;
		}

		#endregion

		#endregion
	}
}