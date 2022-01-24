using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Investigation.Localization
{
	public class InvestigatableResourceManagerWithCultureStringLocalizer : InvestigatableResourceManagerStringLocalizer
	{
		#region Constructors

		public InvestigatableResourceManagerWithCultureStringLocalizer(ResourceManager resourceManager, Assembly resourceAssembly, string baseName, IResourceNamesCache resourceNamesCache, CultureInfo culture, ILogger logger) : base(resourceManager, resourceAssembly, baseName, resourceNamesCache, logger)
		{
			this.Culture = culture ?? throw new ArgumentNullException(nameof(culture));
		}

		#endregion

		#region Properties

		public virtual CultureInfo Culture { get; }

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

		#endregion

		#region Methods

		[SuppressMessage("Globalization", "CA1304:Specify CultureInfo")]
		public override IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		{
			var allStrings = this.GetAllStrings(includeParentCultures, this.Culture).ToArray();

			return allStrings;
		}

		#endregion
	}
}