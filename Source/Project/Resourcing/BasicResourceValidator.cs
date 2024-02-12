using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization.Resourcing
{
	public abstract class BasicResourceValidator : IResourceValidator
	{
		#region Constructors

		protected internal BasicResourceValidator(IFileSystem fileSystem)
		{
			this.FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
		}

		#endregion

		#region Properties

		protected internal virtual IFileSystem FileSystem { get; }

		#endregion

		#region Methods

		public virtual bool IsValidEmbeddedResource(IAssembly assembly, string name)
		{
			if(assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			// ReSharper disable ConvertIfStatementToReturnStatement
			if(!assembly.GetManifestResourceNames().Contains(name, StringComparer.OrdinalIgnoreCase))
				return false;
			// ReSharper restore ConvertIfStatementToReturnStatement

			return this.IsValidEmbeddedResourceInternal(assembly, name);
		}

		protected internal abstract bool IsValidEmbeddedResourceInternal(IAssembly assembly, string name);

		public virtual bool IsValidFileResource(IFileInfo file)
		{
			return file != null && file.Exists && this.IsValidFileResourceInternal(file);
		}

		[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
		public virtual bool IsValidFileResource(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				return false;

			try
			{
				return this.IsValidFileResource(this.FileSystem.FileInfo.New(path));
			}
			catch
			{
				return false;
			}
		}

		protected internal abstract bool IsValidFileResourceInternal(IFileInfo file);

		#endregion
	}
}