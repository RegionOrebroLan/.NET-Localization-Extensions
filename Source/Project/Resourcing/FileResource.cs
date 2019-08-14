using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Resourcing
{
	public class FileResource : BasicResource, IFileResource
	{
		#region Constructors

		public FileResource(IAssembly assembly, IFileInfo file, ILocalizationParser parser) : base(assembly, parser)
		{
			this.File = file ?? throw new ArgumentNullException(nameof(file));
		}

		#endregion

		#region Properties

		public virtual IFileInfo File { get; }
		public virtual string Path => this.File.FullName;

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

			return obj is IFileResource fileResource && this.Assembly.Equals(fileResource.Assembly) && this.Path.Equals(fileResource.Path, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return (this.Assembly + this.Path).ToUpperInvariant().GetHashCode();
		}

		public override string Read()
		{
			using(var stream = this.File.OpenRead())
			{
				using(var streamReader = new StreamReader(stream))
				{
					return streamReader.ReadToEnd();
				}
			}
		}

		public override string ToString()
		{
			return "File-resource: \"" + this.Assembly + " / " + this.Path + "\"";
		}

		#endregion
	}
}