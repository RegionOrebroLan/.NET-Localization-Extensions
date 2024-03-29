using System.IO.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using RegionOrebroLan.Localization.Configuration;
using RegionOrebroLan.Localization.DependencyInjection.Configuration;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Resourcing;
using RegionOrebroLan.Localization.Validation;
using JsonLocalizationParser = RegionOrebroLan.Localization.Json.Serialization.LocalizationParser;
using JsonResourceValidator = RegionOrebroLan.Localization.Json.Resourcing.ResourceValidator;
using LocalizationOptions = RegionOrebroLan.Localization.Configuration.LocalizationOptions;
using XmlLocalizationParser = RegionOrebroLan.Localization.Xml.Serialization.LocalizationParser;
using XmlResourceValidator = RegionOrebroLan.Localization.Xml.Resourcing.ResourceValidator;

namespace RegionOrebroLan.Localization.DependencyInjection.Extensions
{
	public static class ServiceCollectionExtension
	{
		#region Methods

		private static IServiceCollection AddLocalizationDependencies(this IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.TryAddSingleton<IAssemblyHelper, AssemblyHelper>();
			services.TryAddSingleton<IFileSystem, FileSystem>();

			return services;
		}

		/// <summary>
		/// If the staticCache parameter is set to true, the StaticCacheLocalizationProvider is registered as localization-provider instead of the DynamicCacheLocalizationProvider.
		/// </summary>
		public static IServiceCollection AddPathBasedLocalization(this IServiceCollection services, bool staticCache = false)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.AddLocalizationDependencies();

			services.AddResourcing();

			services.TryAddSingleton<IDynamicLocalizationSettings, DynamicLocalizationSettings>();
			services.TryAddSingleton<IEmbeddedResourceAssembliesValidator, EmbeddedResourceAssembliesValidator>();
			services.TryAddSingleton<IFileResourcesDirectoryValidator, FileResourcesDirectoryValidator>();
			services.TryAddSingleton<ILocalizationOptionsResolver, LocalizationOptionsResolver>();
			services.TryAddSingleton<ILocalizationPathResolver, LocalizationPathResolver>();

			if(staticCache)
				services.TryAddSingleton<ILocalizationProvider, StaticCacheLocalizationProvider>();
			else
				services.TryAddSingleton<ILocalizationProvider, DynamicCacheLocalizationProvider>();

			services.TryAddSingleton<ILocalizationSettings, LocalizationSettings>();
			services.TryAddSingleton<ILocalizedStringFactory, LocalizedStringFactory>();
			services.TryAddSingleton<IRootNamespaceResolver, RootNamespaceResolver>();

			// The following two have to be registered even if they already exist. Otherwise ResourceManagerStringLocalizerFactory/ResourceManagerStringLocalizer may be used.
			services.AddSingleton<IStringLocalizerFactory, StringLocalizerFactory>();
			services.AddSingleton(serviceProvider => serviceProvider.GetService<IStringLocalizerFactory>().Create(null, null));

			services.AddLocalization();

			return services;
		}

		public static IServiceCollection AddPathBasedLocalization(this IServiceCollection services, IConfiguration configuration, string configurationPath = LocalizationOptions.DefaultConfigurationPath, string dependencyInjectionConfigurationPath = DependencyInjectionOptions.DefaultConfigurationPath)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			services.Configure<LocalizationOptions>(configuration.GetSection(configurationPath));

			services.PostConfigure<LocalizationOptions>(localizationOptions =>
			{
				var embeddedResourceAssemblies = new List<string>();
				configuration.GetSection(configurationPath).Bind("EmbeddedResourceAssemblies", embeddedResourceAssemblies);

				if(embeddedResourceAssemblies.Any(string.IsNullOrEmpty))
					throw new InvalidOperationException("Localization configuration-error: The embedded-resource-assemblies collection can not contain null-values or empty values.");

				if(embeddedResourceAssemblies.Count > embeddedResourceAssemblies.Distinct(StringComparer.OrdinalIgnoreCase).Count())
					throw new InvalidOperationException($"Localization configuration-error: The embedded-resource-assemblies collection can not contain duplicates. Embedded-resource-assemblies: \"{string.Join("\", \"", embeddedResourceAssemblies)}\"");
			});

			var dependencyInjection = new DependencyInjectionOptions();
			configuration.GetSection(dependencyInjectionConfigurationPath).Bind(dependencyInjection);

			services.AddPathBasedLocalization(dependencyInjection.StaticCache);

			return services;
		}

		public static IServiceCollection AddPathBasedLocalization(this IServiceCollection services, Action<LocalizationOptions> optionsAction, bool staticCache = false)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.Configure(optionsAction);

			services.AddPathBasedLocalization(staticCache);

			return services;
		}

		private static IServiceCollection AddResourcing(this IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.TryAddSingleton<IDynamicResourceProvider, DynamicCacheResourceProvider>();
			services.TryAddSingleton<IResourceLocator, ResourceLocator>();
			services.TryAddSingleton<IResourceProvider, StaticCacheResourceProvider>();
			services.AddSingleton<IResourceResolver>(serviceProvider => new ResourceResolver(new JsonLocalizationParser(), new JsonResourceValidator(serviceProvider.GetService<IFileSystem>())));
			services.AddSingleton<IResourceResolver>(serviceProvider => new ResourceResolver(new XmlLocalizationParser(), new XmlResourceValidator(serviceProvider.GetService<IFileSystem>())));

			return services;
		}

		#endregion
	}
}