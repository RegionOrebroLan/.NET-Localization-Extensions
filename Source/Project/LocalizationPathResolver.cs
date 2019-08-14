using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization
{
	public class LocalizationPathResolver : ILocalizationPathResolver
	{
		#region Fields

		private const char _root = ':';
		private const char _separator = '.';
		private static readonly IEnumerable<char> _separatorsToReplace = new[] {Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar};

		#endregion

		#region Properties

		public virtual char Root => _root;
		public virtual char Separator => _separator;
		public virtual IEnumerable<char> SeparatorsToReplace => _separatorsToReplace;

		#endregion

		#region Methods

		public virtual string Combine(params string[] paths)
		{
			string combinedPaths = null;

			var resolvedPaths = (paths ?? Enumerable.Empty<string>()).Where(item => item != null).ToArray();

			// ReSharper disable InvertIf
			if(resolvedPaths.Any())
			{
				combinedPaths = string.Empty;

				resolvedPaths = resolvedPaths.Where(item => !string.IsNullOrEmpty(item)).ToArray();

				if(resolvedPaths.Any())
					combinedPaths = this.GetSeparatorResolvedPath(string.Join(this.Separator.ToString(CultureInfo.InvariantCulture), resolvedPaths));
			}
			// ReSharper restore InvertIf

			return combinedPaths;
		}

		protected internal virtual string GetSeparatorResolvedPath(string path)
		{
			// ReSharper disable All
			if(path != null)
			{
				foreach(var separatorToReplace in this.SeparatorsToReplace)
				{
					path = path.Replace(separatorToReplace, this.Separator);
				}
			}
			// ReSharper restore All

			return path;
		}

		public virtual bool NameIsRooted(string name)
		{
			return !string.IsNullOrWhiteSpace(name) && name[0] == this.Root;
		}

		protected internal virtual IEnumerable<string> Resolve(IAssembly assembly, string path)
		{
			var paths = new List<string>();

			// ReSharper disable InvertIf
			if(path != null)
			{
				path = this.GetSeparatorResolvedPath(path);

				paths.Add(path);

				if(!string.IsNullOrEmpty(path))
				{
					var rootNamespace = this.GetSeparatorResolvedPath(assembly?.RootNamespace);

					if(!string.IsNullOrEmpty(rootNamespace))
					{
						var prefix = rootNamespace + this.Separator;

						if(path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
							paths.Add(path.Substring(prefix.Length));
						else if(!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
							paths.Add(prefix + path);
					}
				}
			}
			// ReSharper restore InvertIf

			return paths.ToArray();
		}

		public virtual IEnumerable<string> Resolve(IAssembly assembly, string name, string path)
		{
			// ReSharper disable ConvertIfStatementToReturnStatement
			if(name != null && this.NameIsRooted(name))
				return this.Resolve(assembly, name.Substring(1));
			// ReSharper restore ConvertIfStatementToReturnStatement

			return this.Resolve(assembly, this.Combine(path, name));
		}

		#endregion
	}
}