using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Localization.Internal;
using Microsoft.Extensions.Logging;
using RegionOrebroLan.Investigation.Extensions;

namespace RegionOrebroLan.Investigation.Localization
{
	[CLSCompliant(false)]
	public class InvestigatableResourceManagerStringLocalizer : ResourceManagerStringLocalizer
	{
		#region Constructors

		public InvestigatableResourceManagerStringLocalizer(ResourceManager resourceManager, Assembly resourceAssembly, string baseName, IResourceNamesCache resourceNamesCache, ILogger logger) : base(resourceManager, resourceAssembly, baseName, resourceNamesCache, logger) { }

		#endregion

		#region Properties

		public override LocalizedString this[string name, params object[] arguments]
		{
			get
			{
				var localizedString = base[name, arguments];

				return localizedString;
			}
		}

		public override LocalizedString this[string name]
		{
			get
			{
				var localizedString = base[name];

				return localizedString;
			}
		}

		public virtual ILogger Logger
		{
			get
			{
				var logger = this.GetFieldValue<ILogger>("_logger");

				return logger;
			}
		}

		public virtual ConcurrentDictionary<string, object> MissingManifestCache
		{
			get
			{
				var missingManifestCache = this.GetFieldValue<ConcurrentDictionary<string, object>>("_missingManifestCache");

				return missingManifestCache;
			}
		}

		public virtual string ResourceBaseName
		{
			get
			{
				var resourceBaseName = this.GetFieldValue<string>("_resourceBaseName");

				return resourceBaseName;
			}
		}

		public virtual ResourceManager ResourceManager
		{
			get
			{
				var resourceManager = this.GetFieldValue<ResourceManager>("_resourceManager");

				return resourceManager;
			}
		}

		public virtual IResourceNamesCache ResourceNamesCache
		{
			get
			{
				var resourceNamesCache = this.GetFieldValue<IResourceNamesCache>("_resourceNamesCache");

				return resourceNamesCache;
			}
		}

		public virtual IResourceStringProvider ResourceStringProvider
		{
			get
			{
				var resourceStringProvider = this.GetFieldValue<IResourceStringProvider>("_resourceStringProvider");

				return resourceStringProvider;
			}
		}

		#endregion

		#region Methods

		public override IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		{
			var allStrings = this.GetProtectedAllStrings(includeParentCultures, CultureInfo.CurrentUICulture).ToArray();

			return allStrings;
		}

		protected internal virtual T GetFieldValue<T>(string fieldName)
		{
			return typeof(ResourceManagerStringLocalizer).GetNonPublicInstanceFieldValue<T>(fieldName, this);
		}

		public virtual IEnumerable<LocalizedString> GetProtectedAllStrings(bool includeParentCultures, CultureInfo culture)
		{
			var allStrings = this.GetAllStrings(includeParentCultures, culture).ToArray();

			return allStrings;
		}

		public virtual string GetProtectedStringSafely(string name, CultureInfo culture)
		{
			var value = this.GetStringSafely(name, culture);

			return value;
		}

		#endregion
	}
}