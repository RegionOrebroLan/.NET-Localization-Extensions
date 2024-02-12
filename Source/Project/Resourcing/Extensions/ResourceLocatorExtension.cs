using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Resourcing.Extensions
{
	public static class ResourceLocatorExtension
	{
		#region Methods

		public static IEnumerable<IFileResource> GetFileResources(this IResourceLocator resourceLocator, string directoryPath)
		{
			if(resourceLocator == null)
				throw new ArgumentNullException(nameof(resourceLocator));

			return resourceLocator.GetFileResources(directoryPath, true);
		}

		[SuppressMessage("Design", "CA1021:Avoid out parameters")]
		public static bool IsValidEmbeddedResource(this IResourceLocator resourceLocator, IAssembly assembly, string name, out ILocalizationParser parser)
		{
			if(resourceLocator == null)
				throw new ArgumentNullException(nameof(resourceLocator));

			parser = null;

			foreach(var resolver in resourceLocator.Resolvers)
			{
				if(!resolver.Validator.IsValidEmbeddedResource(assembly, name))
					continue;

				parser = resolver.Parser;
				return true;
			}

			return false;
		}

		public static bool IsValidFileResource(this IResourceLocator resourceLocator, string path)
		{
			if(resourceLocator == null)
				throw new ArgumentNullException(nameof(resourceLocator));

			return resourceLocator.Resolvers.Any(resolver => resolver.Validator.IsValidFileResource(path));
		}

		[SuppressMessage("Design", "CA1021:Avoid out parameters")]
		public static bool IsValidFileResource(this IResourceLocator resourceLocator, IFileInfo file, out ILocalizationParser parser)
		{
			if(resourceLocator == null)
				throw new ArgumentNullException(nameof(resourceLocator));

			parser = null;

			foreach(var resolver in resourceLocator.Resolvers)
			{
				if(!resolver.Validator.IsValidFileResource(file))
					continue;

				parser = resolver.Parser;
				return true;
			}

			return false;
		}

		#endregion
	}
}