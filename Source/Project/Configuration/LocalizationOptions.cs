namespace RegionOrebroLan.Localization.Configuration
{
	public class LocalizationOptions
	{
		#region Fields

		// ReSharper disable InconsistentNaming
		public const string DefaultConfigurationPath = "Localization";
		// ReSharper restore InconsistentNaming

		#endregion

		#region Properties

		public virtual bool AlphabeticalSorting { get; set; } = true;

		/// <summary>
		/// Items can be an assembly-name, assembly-name-pattern (MyAssembly*) or an assembly-fullname.
		/// </summary>
		public virtual IList<string> EmbeddedResourceAssemblies { get; } = new List<string>();

		/// <summary>
		/// Absolute or application-relative path to the directory to scan for file-resources. If empty, the application-root will be used. If null, file-resources are disabled.
		/// </summary>
		public virtual string FileResourcesDirectoryPath { get; set; }

		public virtual bool IncludeParentCultures { get; set; }
		public virtual bool ThrowErrors { get; set; }

		#endregion
	}
}