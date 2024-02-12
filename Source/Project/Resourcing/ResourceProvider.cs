using System;
using System.IO.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RegionOrebroLan.Localization.Resourcing
{
	[Obsolete("Will be removed. Use DynamicCacheResourceProvider instead.")]
	public class ResourceProvider(IFileSystem fileSystem, IHostEnvironment hostEnvironment, ILoggerFactory loggerFactory, IResourceLocator resourceLocator, IDynamicLocalizationSettings settings) : DynamicCacheResourceProvider(fileSystem, hostEnvironment, loggerFactory, resourceLocator, settings) { }
}