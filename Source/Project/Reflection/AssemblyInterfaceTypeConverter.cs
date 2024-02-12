using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace RegionOrebroLan.Localization.Reflection
{
	public class AssemblyInterfaceTypeConverter(IAssemblyHelper assemblyHelper) : TypeConverter
	{
		#region Fields

		private static readonly IAssemblyHelper _assemblyHelper = new AssemblyHelper(new RootNamespaceResolver());

		#endregion

		#region Constructors

		public AssemblyInterfaceTypeConverter() : this(_assemblyHelper) { }

		#endregion

		#region Properties

		protected internal virtual IAssemblyHelper AssemblyHelper { get; } = assemblyHelper ?? throw new ArgumentNullException(nameof(assemblyHelper));

		#endregion

		#region Methods

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if(sourceType == null)
				throw new ArgumentNullException(nameof(sourceType));

			return typeof(Assembly).IsAssignableFrom(sourceType) || typeof(IAssembly).IsAssignableFrom(sourceType) || sourceType == typeof(string);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(Assembly) || destinationType == typeof(AssemblyWrapper) || destinationType == typeof(IAssembly) || destinationType == typeof(string);
		}

		[SuppressMessage("Style", "IDE0010:Convert to conditional expression")]
		[SuppressMessage("Style", "IDE0046:Convert to conditional expression")]
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string assemblyName = null;

			switch(value)
			{
				case Assembly valueAsConcreteAssembly:
					assemblyName = valueAsConcreteAssembly.FullName;
					break;
				case IAssembly valueAsAssembly:
					assemblyName = valueAsAssembly.FullName;
					break;
				case string valueAsString:
					assemblyName = valueAsString;
					break;
			}

			// ReSharper disable All
			if(assemblyName != null)
			{
				if(assemblyName.Length == 0)
					return null;

				return this.AssemblyHelper.LoadByName(assemblyName);
			}
			// ReSharper restore All

			throw this.GetConvertFromException(value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if(destinationType == null)
				throw new ArgumentNullException(nameof(destinationType));

			// ReSharper disable InvertIf
			if(value is IAssembly assembly)
			{
				if(destinationType == typeof(Assembly))
					return Assembly.Load(new AssemblyName(assembly.FullName));

				if(destinationType == typeof(AssemblyWrapper))
					return (AssemblyWrapper)this.AssemblyHelper.LoadByName(assembly.FullName);

				if(destinationType == typeof(IAssembly))
					return this.AssemblyHelper.LoadByName(assembly.FullName);

				if(destinationType == typeof(string))
					return assembly.FullName;
			}
			// ReSharper restore InvertIf

			throw this.GetConvertToException(value, destinationType);
		}

		#endregion
	}
}