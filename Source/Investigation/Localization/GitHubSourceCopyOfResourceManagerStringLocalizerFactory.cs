using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RegionOrebroLan.Investigation.Localization
{
	/// <summary>
	/// https://github.com/aspnet/Localization/blob/master/src/Microsoft.Extensions.Localization/ResourceManagerStringLocalizerFactory.cs
	/// </summary>
	public class GitHubSourceCopyOfResourceManagerStringLocalizerFactory : IStringLocalizerFactory
	{
		#region Fields

		private IOptions<LocalizationOptions> _localizationOptions;

		#endregion

		#region Constructors

		public GitHubSourceCopyOfResourceManagerStringLocalizerFactory(IOptions<LocalizationOptions> localizationOptions, ILoggerFactory loggerFactory)
		{
			this._localizationOptions = localizationOptions ?? throw new ArgumentNullException(nameof(localizationOptions));
			this.LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
		}

		#endregion

		#region Properties

		public virtual IOptions<LocalizationOptions> LocalizationOptions
		{
			get => this._localizationOptions;
			set => this._localizationOptions = value;
		}

		public virtual ConcurrentDictionary<string, ResourceManagerStringLocalizer> LocalizerCache { get; } = new ConcurrentDictionary<string, ResourceManagerStringLocalizer>();
		public virtual ILoggerFactory LoggerFactory { get; }
		public virtual IResourceNamesCache ResourceNamesCache { get; } = new ResourceNamesCache();

		public virtual string ResourcesRelativePath
		{
			get
			{
				var resourcesRelativePath = this.LocalizationOptions.Value.ResourcesPath ?? string.Empty;

				if(!string.IsNullOrEmpty(resourcesRelativePath))
					resourcesRelativePath = resourcesRelativePath.Replace(Path.AltDirectorySeparatorChar, '.').Replace(Path.DirectorySeparatorChar, '.') + ".";

				return resourcesRelativePath;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Creates a <see cref="ResourceManagerStringLocalizer"/> using the <see cref="Assembly"/> and
		/// <see cref="Type.FullName"/> of the specified <see cref="Type"/>.
		/// </summary>
		/// <param name="resourceSource">The <see cref="Type"/>.</param>
		/// <returns>The <see cref="ResourceManagerStringLocalizer"/>.</returns>
		public IStringLocalizer Create(Type resourceSource)
		{
			if(resourceSource == null)
				throw new ArgumentNullException(nameof(resourceSource));

			var typeInfo = resourceSource.GetTypeInfo();

			var baseName = this.GetResourcePrefix(typeInfo);

			var assembly = typeInfo.Assembly;

			// Lets skip caching for a moment
			var stringLocalizer = this.CreateResourceManagerStringLocalizer(assembly, baseName);
			return stringLocalizer;
			//return this.LocalizerCache.GetOrAdd(baseName, _ => this.CreateResourceManagerStringLocalizer(assembly, baseName));
		}

		/// <summary>
		/// Creates a <see cref="ResourceManagerStringLocalizer"/>.
		/// </summary>
		/// <param name="baseName">The base name of the resource to load strings from.</param>
		/// <param name="location">The location to load resources from.</param>
		/// <returns>The <see cref="ResourceManagerStringLocalizer"/>.</returns>
		public IStringLocalizer Create(string baseName, string location)
		{
			if(baseName == null)
				throw new ArgumentNullException(nameof(baseName));

			if(location == null)
				throw new ArgumentNullException(nameof(location));

			// Lets skip caching for a moment
			var assemblyName = new AssemblyName(location);
			var assembly = Assembly.Load(assemblyName);
			baseName = this.GetResourcePrefix(baseName, location);

			var stringLocalizer = this.CreateResourceManagerStringLocalizer(assembly, baseName);

			return stringLocalizer;

			//return this.LocalizerCache.GetOrAdd($"B={baseName},L={location}", _ =>
			//{
			//	var assemblyName = new AssemblyName(location);
			//	var assembly = Assembly.Load(assemblyName);
			//	baseName = this.GetResourcePrefix(baseName, location);

			//	return this.CreateResourceManagerStringLocalizer(assembly, baseName);
			//});
		}

		/// <summary>Creates a <see cref="ResourceManagerStringLocalizer"/> for the given input.</summary>
		/// <param name="assembly">The assembly to create a <see cref="ResourceManagerStringLocalizer"/> for.</param>
		/// <param name="baseName">The base name of the resource to search for.</param>
		/// <returns>A <see cref="ResourceManagerStringLocalizer"/> for the given <paramref name="assembly"/> and <paramref name="baseName"/>.</returns>
		/// <remarks>This method is virtual for testing purposes only.</remarks>
		protected virtual ResourceManagerStringLocalizer CreateResourceManagerStringLocalizer(
			Assembly assembly,
			string baseName)
		{
			return new ResourceManagerStringLocalizer(
				new ResourceManager(baseName, assembly),
				assembly,
				baseName,
				this.ResourceNamesCache,
				this.LoggerFactory.CreateLogger<ResourceManagerStringLocalizer>());
		}

		/// <summary>Gets a <see cref="ResourceLocationAttribute"/> from the provided <see cref="Assembly"/>.</summary>
		/// <param name="assembly">The assembly to get a <see cref="ResourceLocationAttribute"/> from.</param>
		/// <returns>The <see cref="ResourceLocationAttribute"/> associated with the given <see cref="Assembly"/>.</returns>
		/// <remarks>This method is protected and virtual for testing purposes only.</remarks>
		protected virtual ResourceLocationAttribute GetResourceLocationAttribute(Assembly assembly)
		{
			return assembly.GetCustomAttribute<ResourceLocationAttribute>();
		}

		private string GetResourcePath(Assembly assembly)
		{
			var resourceLocationAttribute = this.GetResourceLocationAttribute(assembly);

			// If we don't have an attribute assume all assemblies use the same resource location.
			var resourceLocation = resourceLocationAttribute == null
				? this.ResourcesRelativePath
				: resourceLocationAttribute.ResourceLocation + ".";
			resourceLocation = resourceLocation
				.Replace(Path.DirectorySeparatorChar, '.')
				.Replace(Path.AltDirectorySeparatorChar, '.');

			return resourceLocation;
		}

		/// <summary>
		/// Gets the resource prefix used to look up the resource.
		/// </summary>
		/// <param name="typeInfo">The type of the resource to be looked up.</param>
		/// <returns>The prefix for resource lookup.</returns>
		protected virtual string GetResourcePrefix(TypeInfo typeInfo)
		{
			if(typeInfo == null)
				throw new ArgumentNullException(nameof(typeInfo));

			var resourcePrefix = this.GetResourcePrefix(typeInfo, this.GetRootNamespace(typeInfo.Assembly), this.GetResourcePath(typeInfo.Assembly));

			return resourcePrefix;
		}

		/// <summary>
		/// Gets the resource prefix used to look up the resource.
		/// </summary>
		/// <param name="typeInfo">The type of the resource to be looked up.</param>
		/// <param name="baseNamespace">The base namespace of the application.</param>
		/// <param name="resourcesRelativePath">The folder containing all resources.</param>
		/// <returns>The prefix for resource lookup.</returns>
		/// <remarks>
		/// For the type "Sample.Controllers.Home" if there's a resourceRelativePath return
		/// "Sample.Resourcepath.Controllers.Home" if there isn't one then it would return "Sample.Controllers.Home".
		/// </remarks>
		protected virtual string GetResourcePrefix(TypeInfo typeInfo, string baseNamespace, string resourcesRelativePath)
		{
			if(typeInfo == null)
				throw new ArgumentNullException(nameof(typeInfo));

			if(string.IsNullOrEmpty(baseNamespace))
				throw new ArgumentNullException(nameof(baseNamespace));

			if(string.IsNullOrEmpty(resourcesRelativePath))
				return typeInfo.FullName;

			// This expectation is defined by dotnet's automatic resource storage.
			// We have to conform to "{RootNamespace}.{ResourceLocation}.{FullTypeName - AssemblyName}".
			var assemblyName = new AssemblyName(typeInfo.Assembly.FullName).Name;
			return baseNamespace + "." + resourcesRelativePath + TrimPrefix(typeInfo.FullName, assemblyName + ".");
		}

		/// <summary>
		/// Gets the resource prefix used to look up the resource.
		/// </summary>
		/// <param name="baseResourceName">The name of the resource to be looked up</param>
		/// <param name="baseNamespace">The base namespace of the application.</param>
		/// <returns>The prefix for resource lookup.</returns>
		[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
		protected virtual string GetResourcePrefix(string baseResourceName, string baseNamespace)
		{
			if(string.IsNullOrEmpty(baseResourceName))
				throw new ArgumentNullException(nameof(baseResourceName));

			if(string.IsNullOrEmpty(baseNamespace))
				throw new ArgumentNullException(nameof(baseNamespace));

			var assemblyName = new AssemblyName(baseNamespace);
			var assembly = Assembly.Load(assemblyName);
			var rootNamespace = this.GetRootNamespace(assembly);
			var resourceLocation = this.GetResourcePath(assembly);
			var locationPath = rootNamespace + "." + resourceLocation;

			baseResourceName = locationPath + TrimPrefix(baseResourceName, baseNamespace + ".");

			return baseResourceName;
		}

		/// <summary>
		/// Gets the resource prefix used to look up the resource.
		/// </summary>
		/// <param name="location">The general location of the resource.</param>
		/// <param name="baseName">The base name of the resource.</param>
		/// <param name="resourceLocation">The location of the resource within <paramref name="location"/>.</param>
		/// <returns>The resource prefix used to look up the resource.</returns>
		protected virtual string GetResourcePrefix(string location, string baseName, string resourceLocation)
		{
			// Re-root the base name if a resources path is set
			return location + "." + resourceLocation + TrimPrefix(baseName, location + ".");
		}

		private string GetRootNamespace(Assembly assembly)
		{
			var rootNamespaceAttribute = this.GetRootNamespaceAttribute(assembly);

			var rootNamespace = rootNamespaceAttribute?.RootNamespace ?? new AssemblyName(assembly.FullName).Name;

			return rootNamespace;
		}

		/// <summary>Gets a <see cref="RootNamespaceAttribute"/> from the provided <see cref="Assembly"/>.</summary>
		/// <param name="assembly">The assembly to get a <see cref="RootNamespaceAttribute"/> from.</param>
		/// <returns>The <see cref="RootNamespaceAttribute"/> associated with the given <see cref="Assembly"/>.</returns>
		/// <remarks>This method is protected and virtual for testing purposes only.</remarks>
		protected virtual RootNamespaceAttribute GetRootNamespaceAttribute(Assembly assembly)
		{
			var rootNamespaceAttribute = assembly.GetCustomAttribute<RootNamespaceAttribute>();

			return rootNamespaceAttribute;
		}

		private static string TrimPrefix(string name, string prefix)
		{
			// ReSharper disable ConvertIfStatementToReturnStatement
			if(name != null && name.StartsWith(prefix, StringComparison.Ordinal))
				return name.Substring(prefix.Length);
			// ReSharper restore ConvertIfStatementToReturnStatement

			return name;
		}

		#endregion
	}
}