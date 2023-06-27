using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using RegionOrebroLan.Localization.Configuration;
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

		public static IServiceCollection AddPathBasedLocalization(this IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.AddLocalizationDependencies();

			services.AddResourcing();

			services.TryAddSingleton<IEmbeddedResourceAssembliesValidator, EmbeddedResourceAssembliesValidator>();
			services.TryAddSingleton<IFileResourcesDirectoryValidator, FileResourcesDirectoryValidator>();
			services.TryAddSingleton<ILocalizationOptionsResolver, LocalizationOptionsResolver>();
			services.TryAddSingleton<ILocalizationPathResolver, LocalizationPathResolver>();
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

		public static IServiceCollection AddPathBasedLocalization(this IServiceCollection services, IConfiguration configuration)
		{
			return services.AddPathBasedLocalization(configuration, LocalizationOptions.DefaultConfigurationPath);
		}

		public static IServiceCollection AddPathBasedLocalization(this IServiceCollection services, IConfiguration configuration, string configurationPath)
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

			services.AddPathBasedLocalization();

			return services;
		}

		public static IServiceCollection AddPathBasedLocalization(this IServiceCollection services, Action<LocalizationOptions> optionsAction)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.Configure(optionsAction);

			services.AddPathBasedLocalization();

			return services;
		}

		private static IServiceCollection AddResourcing(this IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.TryAddSingleton<IDynamicResourceProvider, DynamicCacheResourceProvider>();
			services.TryAddSingleton<IResourceLocator, ResourceLocator>();
			services.AddSingleton<IResourceResolver>(serviceProvider => new ResourceResolver(new JsonLocalizationParser(), new JsonResourceValidator(serviceProvider.GetService<IFileSystem>())));
			services.AddSingleton<IResourceResolver>(serviceProvider => new ResourceResolver(new XmlLocalizationParser(), new XmlResourceValidator(serviceProvider.GetService<IFileSystem>())));

			return services;
		}

		#endregion
	}
}