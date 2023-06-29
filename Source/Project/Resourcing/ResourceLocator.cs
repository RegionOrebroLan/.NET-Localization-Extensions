using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Resourcing.Extensions;

namespace RegionOrebroLan.Localization.Resourcing
{
	public class ResourceLocator : IResourceLocator
	{
		#region Fields

		private const string _satelliteAssemblySearchPatternFormat = "{0}.resources{1}";

		#endregion

		#region Constructors

		public ResourceLocator(IAssemblyHelper assemblyHelper, IFileSystem fileSystem, ILoggerFactory loggerFactory, IEnumerable<IResourceResolver> resolvers)
		{
			this.AssemblyHelper = assemblyHelper ?? throw new ArgumentNullException(nameof(assemblyHelper));
			this.FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
			this.Resolvers = resolvers ?? throw new ArgumentNullException(nameof(resolvers));
		}

		#endregion

		#region Properties

		protected internal virtual IAssemblyHelper AssemblyHelper { get; }
		protected internal virtual IFileSystem FileSystem { get; }
		protected internal virtual ILogger Logger { get; }
		public virtual IEnumerable<IResourceResolver> Resolvers { get; }
		protected internal virtual string SatelliteAssemblySearchPatternFormat => _satelliteAssemblySearchPatternFormat;

		#endregion

		#region Methods

		public virtual IEnumerable<IEmbeddedResource> GetEmbeddedResources(IAssembly assembly)
		{
			if(assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			return this.GetEmbeddedResources(new[] { assembly }.Concat(this.GetSatelliteAssemblies(assembly)));
		}

		protected internal virtual IEnumerable<IEmbeddedResource> GetEmbeddedResources(IEnumerable<IAssembly> assemblies)
		{
			assemblies = assemblies?.ToArray();

			if(assemblies == null)
				throw new ArgumentNullException(nameof(assemblies));

			if(assemblies.Any(assemblyInformation => assemblyInformation == null))
				throw new ArgumentException("The assemblies collection can not contain null values.");

			var embeddedResources = new List<IEmbeddedResource>();

			foreach(var assembly in assemblies)
			{
				foreach(var resourceName in assembly.GetManifestResourceNames().OrderBy(resourceName => resourceName))
				{
					if(!this.IsValidEmbeddedResource(assembly, resourceName, out var parser))
						continue;

					embeddedResources.Add(new EmbeddedResource(assembly, resourceName, parser));
				}
			}

			return embeddedResources.ToArray();
		}

		public virtual IEnumerable<IFileResource> GetFileResources(string directoryPath, bool recursive)
		{
			var applicationAssembly = this.AssemblyHelper.ApplicationAssembly;

			var fileResources = new List<IFileResource>();

			foreach(var file in this.GetFiles(directoryPath, recursive))
			{
				if(!this.IsValidFileResource(file, out var parser))
					continue;

				fileResources.Add(new FileResource(applicationAssembly, file, parser));
			}

			return fileResources.ToArray();
		}

		protected internal virtual IEnumerable<IFileInfo> GetFiles(string directoryPath, bool recursive)
		{
			return this.GetFiles(directoryPath, recursive, "*");
		}

		protected internal virtual IEnumerable<IFileInfo> GetFiles(string directoryPath, bool recursive, string searchPattern)
		{
			/*
				We could have implemented it like this to:
				return this.FileSystem.DirectoryInfo.FromDirectoryName(directoryPath).EnumerateFiles(searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
				But then the result wouldn't have been in a correct order.
			*/

			if(recursive)
			{
				foreach(var subDirectoryPath in this.FileSystem.Directory.GetDirectories(directoryPath))
				{
					foreach(var file in this.GetFiles(subDirectoryPath, true, searchPattern))
					{
						yield return file;
					}
				}
			}

			foreach(var file in this.FileSystem.DirectoryInfo.New(directoryPath).EnumerateFiles(searchPattern))
			{
				yield return file;
			}
		}

		protected internal virtual IEnumerable<IAssembly> GetSatelliteAssemblies(IAssembly mainAssembly)
		{
			if(mainAssembly == null)
				throw new ArgumentNullException(nameof(mainAssembly));

			var directoryPath = this.FileSystem.Path.GetDirectoryName(mainAssembly.Location);
			var searchPattern = this.GetSatelliteAssemblySearchPattern(mainAssembly);

			return this.GetFiles(directoryPath, true, searchPattern)
				.Select(file => file.FullName)
				.Select(path => this.AssemblyHelper.LoadSatelliteAssembly(mainAssembly, path));
		}

		protected internal virtual string GetSatelliteAssemblySearchPattern(IAssembly assembly)
		{
			if(assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			var path = assembly.Location;

			return string.Format(CultureInfo.InvariantCulture, this.SatelliteAssemblySearchPatternFormat, this.FileSystem.Path.GetFileNameWithoutExtension(path), this.FileSystem.Path.GetExtension(path));
		}

		#endregion
	}
}