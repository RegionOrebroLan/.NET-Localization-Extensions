using System;
using System.IO;
using System.IO.Abstractions;
using System.Xml;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Resourcing;

namespace RegionOrebroLan.Localization.Xml.Resourcing
{
	public class ResourceValidator : BasicResourceValidator
	{
		#region Constructors

		public ResourceValidator(IFileSystem fileSystem) : base(fileSystem) { }

		#endregion

		#region Methods

		protected internal override bool IsValidEmbeddedResourceInternal(IAssembly assembly, string name)
		{
			if(assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			return this.IsValidResource(assembly.GetManifestResourceStream(name));
		}

		protected internal override bool IsValidFileResourceInternal(IFileInfo file)
		{
			if(file == null)
				throw new ArgumentNullException(nameof(file));

			return this.IsValidResource(file.OpenRead());
		}

		protected internal virtual bool IsValidResource(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException(nameof(stream));

			try
			{
				using(var xmlTextReader = new XmlTextReader(stream))
				{
					xmlTextReader.DtdProcessing = DtdProcessing.Ignore;
					xmlTextReader.Read();
				}

				return true;
			}
			catch(XmlException)
			{
				return false;
			}
		}

		#endregion
	}
}