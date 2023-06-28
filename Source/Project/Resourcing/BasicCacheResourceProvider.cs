using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using RegionOrebroLan.Localization.Resourcing.Extensions;

namespace RegionOrebroLan.Localization.Resourcing
{
	public abstract class BasicCacheResourceProvider<TLocalizationSettings> : IResourceProvider where TLocalizationSettings : ILocalizationSettings
	{
		#region Constructors

		protected BasicCacheResourceProvider(ILoggerFactory loggerFactory, IResourceLocator resourceLocator, TLocalizationSettings settings)
		{
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
			this.ResourceLocator = resourceLocator ?? throw new ArgumentNullException(nameof(resourceLocator));
			this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
		}

		#endregion

		#region Properties

		public virtual IEnumerable<IEmbeddedResource> EmbeddedResources
		{
			get
			{
				// ReSharper disable InvertIf
				if(this.EmbeddedResourcesCache == null)
				{
					lock(this.EmbeddedResourcesLock)
					{
						if(this.EmbeddedResourcesCache == null)
						{
							var embeddedResources = new List<IEmbeddedResource>();

							foreach(var assembly in this.Settings.EmbeddedResourceAssemblies.Where(assembly => assembly != null).Distinct())
							{
								embeddedResources.AddRange(this.ResourceLocator.GetEmbeddedResources(assembly));
							}

							this.EmbeddedResourcesCache = embeddedResources.ToArray();
						}
					}
				}
				// ReSharper restore InvertIf

				return this.EmbeddedResourcesCache;
			}
		}

		protected internal virtual IEnumerable<IEmbeddedResource> EmbeddedResourcesCache { get; set; }
		protected internal virtual object EmbeddedResourcesLock { get; } = new();

		public virtual IEnumerable<IFileResource> FileResources
		{
			get
			{
				// ReSharper disable InvertIf
				if(this.FileResourcesCache == null)
				{
					lock(this.FileResourcesLock)
					{
						this.FileResourcesCache ??= (this.Settings.FileResourcesDirectory != null ? this.ResourceLocator.GetFileResources(this.Settings.FileResourcesDirectory.FullName) : Enumerable.Empty<IFileResource>()).ToArray();
					}
				}
				// ReSharper restore InvertIf

				return this.FileResourcesCache;
			}
		}

		protected internal virtual IEnumerable<IFileResource> FileResourcesCache { get; set; }
		protected internal virtual object FileResourcesLock { get; } = new();
		protected internal virtual ILogger Logger { get; }
		protected internal virtual IResourceLocator ResourceLocator { get; }
		protected internal virtual TLocalizationSettings Settings { get; }

		#endregion
	}
}