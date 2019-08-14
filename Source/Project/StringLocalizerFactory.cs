using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization
{
	public class StringLocalizerFactory : IStringLocalizerFactory
	{
		#region Fields

		private const string _cacheKeyNullValue = "NULL ({bfdd0e5f-8414-4555-ad38-e9feaaba3afa})";

		#endregion

		#region Constructors

		[CLSCompliant(false)]
		public StringLocalizerFactory(IAssemblyHelper assemblyHelper, ILocalizationProvider localizationProvider, ILoggerFactory loggerFactory)
		{
			this.AssemblyHelper = assemblyHelper ?? throw new ArgumentNullException(nameof(assemblyHelper));
			this.LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
			this.LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
		}

		#endregion

		#region Properties

		protected internal virtual IAssemblyHelper AssemblyHelper { get; }
		protected internal virtual string CacheKeyNullValue => _cacheKeyNullValue;

		[CLSCompliant(false)]
		protected internal virtual ILocalizationProvider LocalizationProvider { get; }

		[CLSCompliant(false)]
		protected internal virtual ILoggerFactory LoggerFactory { get; }

		[CLSCompliant(false)]
		protected internal virtual ConcurrentDictionary<string, IStringLocalizer> StringLocalizerInstanceCache { get; } = new ConcurrentDictionary<string, IStringLocalizer>(StringComparer.OrdinalIgnoreCase);

		#endregion

		#region Methods

		[CLSCompliant(false)]
		public virtual IStringLocalizer Create(Type resourceSource)
		{
			if(resourceSource == null)
				throw new ArgumentNullException(nameof(resourceSource));

			return this.Create(this.AssemblyHelper.Wrap(resourceSource.Assembly), resourceSource.FullName);
		}

		[CLSCompliant(false)]
		public virtual IStringLocalizer Create(string baseName, string location)
		{
			IAssembly assembly = null;

			if(location != null)
				assembly = this.AssemblyHelper.LoadByName(location);

			return this.Create(assembly, baseName);
		}

		[CLSCompliant(false)]
		protected internal virtual IStringLocalizer Create(IAssembly assembly, string baseName)
		{
			var key = $"Assembly={(assembly == null ? this.CacheKeyNullValue : assembly.FullName)}&BaseName={baseName ?? this.CacheKeyNullValue}";

			return this.StringLocalizerInstanceCache.GetOrAdd(key, _ => new StringLocalizer(this.ResolveAssembly(assembly), null, this.LocalizationProvider, this.LoggerFactory.CreateLogger<StringLocalizer>(), baseName));
		}

		protected internal virtual IAssembly ResolveAssembly(IAssembly assembly)
		{
			return assembly ?? this.AssemblyHelper.ApplicationAssembly;
		}

		#endregion
	}
}