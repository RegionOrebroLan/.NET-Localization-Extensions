using System.Collections.Concurrent;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RegionOrebroLan.Investigation.Extensions;

namespace RegionOrebroLan.Investigation.Localization
{
	public class InvestigatableResourceManagerStringLocalizerFactory : ResourceManagerStringLocalizerFactory
	{
		#region Constructors

		public InvestigatableResourceManagerStringLocalizerFactory(IOptions<LocalizationOptions> localizationOptions, ILoggerFactory loggerFactory) : base(localizationOptions, loggerFactory) { }

		#endregion

		#region Properties

		public virtual ConcurrentDictionary<string, ResourceManagerStringLocalizer> LocalizerCache
		{
			get
			{
				var localizerCache = this.GetFieldValue<ConcurrentDictionary<string, ResourceManagerStringLocalizer>>("_localizerCache");

				return localizerCache;
			}
		}

		public virtual ILoggerFactory LoggerFactory
		{
			get
			{
				var loggerFactory = this.GetFieldValue<ILoggerFactory>("_loggerFactory");

				return loggerFactory;
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

		public virtual string ResourcesRelativePath
		{
			get
			{
				var resourcesRelativePath = this.GetFieldValue<string>("_resourcesRelativePath");

				return resourcesRelativePath;
			}
		}

		#endregion

		#region Methods

		public virtual ResourceManagerStringLocalizer CreateProtectedResourceManagerStringLocalizer(Assembly assembly, string baseName)
		{
			return this.CreateResourceManagerStringLocalizer(assembly, baseName);
		}

		protected override ResourceManagerStringLocalizer CreateResourceManagerStringLocalizer(Assembly assembly, string baseName)
		{
			var investigatableResourceManagerStringLocalizer = new InvestigatableResourceManagerStringLocalizer(new ResourceManager(baseName, assembly), assembly, baseName, this.ResourceNamesCache, this.LoggerFactory.CreateLogger<ResourceManagerStringLocalizer>());

			return investigatableResourceManagerStringLocalizer;
		}

		protected internal virtual T GetFieldValue<T>(string fieldName)
		{
			return typeof(ResourceManagerStringLocalizerFactory).GetNonPublicInstanceFieldValue<T>(fieldName, this);
		}

		public virtual ResourceLocationAttribute GetProtectedResourceLocationAttribute(Assembly assembly)
		{
			return this.GetResourceLocationAttribute(assembly);
		}

		public virtual string GetProtectedResourcePrefix(TypeInfo typeInfo)
		{
			return this.GetResourcePrefix(typeInfo);
		}

		public virtual string GetProtectedResourcePrefix(TypeInfo typeInfo, string baseNamespace, string resourcesRelativePath)
		{
			return this.GetResourcePrefix(typeInfo, baseNamespace, resourcesRelativePath);
		}

		public virtual string GetProtectedResourcePrefix(string baseResourceName, string baseNamespace)
		{
			return this.GetResourcePrefix(baseResourceName, baseNamespace);
		}

		public virtual string GetProtectedResourcePrefix(string location, string baseName, string resourceLocation)
		{
			return this.GetResourcePrefix(location, baseName, resourceLocation);
		}

		public virtual RootNamespaceAttribute GetProtectedRootNamespaceAttribute(Assembly assembly)
		{
			return this.GetRootNamespaceAttribute(assembly);
		}

		protected override ResourceLocationAttribute GetResourceLocationAttribute(Assembly assembly)
		{
			var resourceLocationAttribute = base.GetResourceLocationAttribute(assembly);

			return resourceLocationAttribute;
		}

		protected override string GetResourcePrefix(TypeInfo typeInfo)
		{
			var resourcePrefix = base.GetResourcePrefix(typeInfo);

			return resourcePrefix;
		}

		protected override string GetResourcePrefix(TypeInfo typeInfo, string baseNamespace, string resourcesRelativePath)
		{
			var resourcePrefix = base.GetResourcePrefix(typeInfo, baseNamespace, resourcesRelativePath);

			return resourcePrefix;
		}

		protected override string GetResourcePrefix(string baseResourceName, string baseNamespace)
		{
			var resourcePrefix = base.GetResourcePrefix(baseResourceName, baseNamespace);

			return resourcePrefix;
		}

		protected override string GetResourcePrefix(string location, string baseName, string resourceLocation)
		{
			var resourcePrefix = base.GetResourcePrefix(location, baseName, resourceLocation);

			return resourcePrefix;
		}

		protected override RootNamespaceAttribute GetRootNamespaceAttribute(Assembly assembly)
		{
			var rootNamespaceAttribute = base.GetRootNamespaceAttribute(assembly);

			return rootNamespaceAttribute;
		}

		#endregion
	}
}