using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyModel;

namespace RegionOrebroLan.Localization.Reflection
{
	public class AssemblyHelper(IRootNamespaceResolver rootNamespaceResolver) : IAssemblyHelper
	{
		#region Fields

		private static IEnumerable<IAssembly> _runtimeAssemblies;
		private const char _wildcardCharacter = '*';

		#endregion

		#region Properties

		public virtual IAssembly ApplicationAssembly => this.Wrap(Assembly.GetEntryAssembly());
		protected internal virtual IRootNamespaceResolver RootNamespaceResolver { get; } = rootNamespaceResolver ?? throw new ArgumentNullException(nameof(rootNamespaceResolver));

		public virtual IEnumerable<IAssembly> RuntimeAssemblies
		{
			get
			{
				// Caching, guess that is no problem.
				// ReSharper disable InvertIf
				if(_runtimeAssemblies == null)
				{
					var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

					foreach(var assemblyName in this.GetRuntimeAssemblyNames())
					{
						if(!assemblies.Any(assembly => string.Equals(assembly.GetName().Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase)))
						{
							Assembly assembly;

							try
							{
								assembly = Assembly.Load(assemblyName);
							}
							catch(BadImageFormatException badImageFormatException)
							{
								// We write to the console because we have no logger in this class. Guess that is better than nothing. And it will be hard to get a logger in because AssemblyInterfaceTypeConverter instantiates this class in a static variable.
								Console.WriteLine($"Skipping the assembly \"{assemblyName}\". {nameof(BadImageFormatException)} thrown when loading assembly \"{assemblyName}\". {badImageFormatException}");

								continue;
							}

							if(assembly == null)
								throw new InvalidOperationException($"The assembly \"{assemblyName}\" was loaded but is null.");

							assemblies.Add(assembly);
						}
					}

					_runtimeAssemblies = assemblies.Select(this.Wrap).ToArray();
				}
				// ReSharper restore InvertIf

				return _runtimeAssemblies;
			}
		}

		protected internal virtual char WildcardCharacter => _wildcardCharacter;

		#endregion

		#region Methods

		public virtual IEnumerable<IAssembly> Find(string pattern)
		{
			if(pattern == null)
				throw new ArgumentNullException(nameof(pattern));

			var assemblies = new List<IAssembly>();

			if(pattern.Contains(this.WildcardCharacter))
			{
				foreach(var assembly in this.RuntimeAssemblies)
				{
					var regularExpressionPattern = pattern.Replace(this.WildcardCharacter.ToString(CultureInfo.InvariantCulture), "*");
					regularExpressionPattern = "^" + Regex.Escape(regularExpressionPattern).Replace("\\*", ".*") + "$";

					if(Regex.IsMatch(assembly.Name, regularExpressionPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase) && !assemblies.Contains(assembly))
						assemblies.Add(assembly);

					if(Regex.IsMatch(assembly.FullName, regularExpressionPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase) && !assemblies.Contains(assembly))
						assemblies.Add(assembly);
				}
			}
			else
			{
				var assembly = this.RuntimeAssemblies.FirstOrDefault(item => string.Equals(pattern, item.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(pattern, item.FullName, StringComparison.OrdinalIgnoreCase));

				if(assembly != null)
					assemblies.Add(assembly);
				else
					throw new InvalidOperationException($"The assembly \"{pattern}\" is not loaded at runtime.");
			}

			return assemblies.ToArray();
		}

		protected internal virtual IEnumerable<AssemblyName> GetRuntimeAssemblyNames()
		{
			return DependencyContext.Default.GetRuntimeAssemblyNames(RuntimeEnvironment.GetRuntimeIdentifier());
		}

		public virtual IAssembly Load(string path)
		{
			return this.Wrap(this.LoadInternal(path));
		}

		public virtual IAssembly LoadByName(string name)
		{
			return this.Wrap(Assembly.Load(new AssemblyName(name)));
		}

		protected internal virtual Assembly LoadInternal(string path)
		{
			return Assembly.LoadFile(path);
		}

		public virtual IAssembly LoadSatelliteAssembly(IAssembly mainAssembly, string path)
		{
			return this.Wrap(mainAssembly, this.LoadInternal(path));
		}

		public virtual IAssembly Wrap(Assembly assembly)
		{
			return assembly != null ? new AssemblyWrapper(assembly, this.RootNamespaceResolver) : null;
		}

		public virtual IAssembly Wrap(IAssembly mainAssembly, Assembly satelliteAssembly)
		{
			if(mainAssembly == null)
				throw new ArgumentNullException(nameof(mainAssembly));

			if(satelliteAssembly == null)
				throw new ArgumentNullException(nameof(satelliteAssembly));

			return new AssemblyWrapper(satelliteAssembly, mainAssembly, this.RootNamespaceResolver);
		}

		#endregion
	}
}