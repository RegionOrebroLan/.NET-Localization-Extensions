using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Resourcing
{
	public class EmbeddedResource(IAssembly assembly, string name, ILocalizationParser parser) : BasicResource(assembly, parser), IEmbeddedResource
	{
		#region Properties

		public virtual string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

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

			return obj is IEmbeddedResource embeddedResource && this.Assembly.Equals(embeddedResource.Assembly) && this.Name.Equals(embeddedResource.Name, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return (this.Assembly + this.Name).ToUpperInvariant().GetHashCode();
		}

		public override string Read()
		{
			using(var stream = this.Assembly.GetManifestResourceStream(this.Name))
			{
				// ReSharper disable AssignNullToNotNullAttribute
				using(var streamReader = new StreamReader(stream))
				{
					return streamReader.ReadToEnd();
				}
				// ReSharper restore AssignNullToNotNullAttribute
			}
		}

		public override string ToString()
		{
			return "Embedded-resource: \"" + this.Assembly + " / " + this.Name + "\"";
		}

		#endregion
	}
}