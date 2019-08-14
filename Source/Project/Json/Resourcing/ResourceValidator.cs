using System;
using System.IO.Abstractions;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Resourcing;

namespace RegionOrebroLan.Localization.Json.Resourcing
{
	public class ResourceValidator : BasicResourceValidator
	{
		#region Fields

		private const string _validExtension = ".json";

		#endregion

		#region Constructors

		public ResourceValidator(IFileSystem fileSystem) : base(fileSystem) { }

		#endregion

		#region Properties

		protected internal virtual string ValidExtension => _validExtension;

		#endregion

		#region Methods

		protected internal override bool IsValidEmbeddedResourceInternal(IAssembly assembly, string name)
		{
			return this.IsValidExtension(this.FileSystem.Path.GetExtension(name));
		}

		protected internal virtual bool IsValidExtension(string extension)
		{
			return string.Equals(extension, this.ValidExtension, StringComparison.OrdinalIgnoreCase);
		}

		protected internal override bool IsValidFileResourceInternal(IFileInfo file)
		{
			return this.IsValidExtension(file?.Extension);
		}

		#endregion
	}
}