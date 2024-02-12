using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace RegionOrebroLan.Localization.Reflection
{
	public class AssemblyWrapper(Assembly assembly, IAssembly mainAssembly, IRootNamespaceResolver rootNamespaceResolver) : IAssembly
	{
		#region Fields

		private Lazy<CultureInfo> _culture;
		private Lazy<string> _name;
		private Lazy<string> _rootNamespace;

		#endregion

		#region Constructors

		public AssemblyWrapper(Assembly assembly, IRootNamespaceResolver rootNamespaceResolver) : this(assembly, null, rootNamespaceResolver) { }

		#endregion

		#region Properties

		protected internal virtual Assembly Assembly { get; } = assembly ?? throw new ArgumentNullException(nameof(assembly));

		public virtual CultureInfo Culture
		{
			get
			{
				this._culture ??= new Lazy<CultureInfo>(() => this.Assembly.GetName().CultureInfo);

				return this._culture.Value;
			}
		}

		public virtual string FullName => this.Assembly.FullName;
		public virtual bool IsSatelliteAssembly => this.MainAssembly != null;
		public virtual string Location => this.Assembly.Location;
		protected internal virtual IAssembly MainAssembly { get; } = mainAssembly;

		public virtual string Name
		{
			get
			{
				this._name ??= new Lazy<string>(() => this.Assembly.GetName().Name);

				return this._name.Value;
			}
		}

		public virtual string RootNamespace
		{
			get
			{
				this._rootNamespace ??= new Lazy<string>(() =>
				{
					var rootNamespace = this.RootNamespaceResolver.GetRootNamespace(this.Assembly);

					return rootNamespace?.Name ?? (this.MainAssembly == null ? this.Assembly.GetName().Name : this.MainAssembly.RootNamespace);
				});

				return this._rootNamespace.Value;
			}
		}

		protected internal virtual IRootNamespaceResolver RootNamespaceResolver { get; } = rootNamespaceResolver ?? throw new ArgumentNullException(nameof(rootNamespaceResolver));

		#endregion

		#region Methods

		[SuppressMessage("Style", "IDE0041:Use 'is null' check")]
		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj))
				return false;

			// ReSharper disable ConvertIfStatementToReturnStatement
			if(ReferenceEquals(this, obj))
				return true;
			// ReSharper restore ConvertIfStatementToReturnStatement

			return obj is IAssembly assembly && this.FullName.Equals(assembly.FullName, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return this.FullName.ToUpperInvariant().GetHashCode();
		}

		public virtual IEnumerable<string> GetManifestResourceNames()
		{
			return this.Assembly.GetManifestResourceNames();
		}

		public virtual Stream GetManifestResourceStream(string name)
		{
			return this.Assembly.GetManifestResourceStream(name);
		}

		public override string ToString()
		{
			return this.FullName;
		}

		#endregion
	}
}