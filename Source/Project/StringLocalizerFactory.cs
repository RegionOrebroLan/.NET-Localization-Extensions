using System.Collections.Concurrent;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization
{
	public class StringLocalizerFactory(IAssemblyHelper assemblyHelper, ILocalizationProvider localizationProvider, ILoggerFactory loggerFactory) : IStringLocalizerFactory
	{
		#region Fields

		private const string _cacheKeyNullValue = "NULL ({bfdd0e5f-8414-4555-ad38-e9feaaba3afa})";

		#endregion

		#region Properties

		protected internal virtual IAssemblyHelper AssemblyHelper { get; } = assemblyHelper ?? throw new ArgumentNullException(nameof(assemblyHelper));
		protected internal virtual string CacheKeyNullValue => _cacheKeyNullValue;
		protected internal virtual ILocalizationProvider LocalizationProvider { get; } = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
		protected internal virtual ILoggerFactory LoggerFactory { get; } = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

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