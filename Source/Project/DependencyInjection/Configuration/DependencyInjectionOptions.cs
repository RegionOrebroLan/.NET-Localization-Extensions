namespace RegionOrebroLan.Localization.DependencyInjection.Configuration
{
	public class DependencyInjectionOptions
	{
		#region Fields

		public const string DefaultConfigurationPath = "LocalizationDependencyInjection";

		#endregion

		#region Properties

		/// <summary>
		/// If set to true, the StaticCacheLocalizationProvider is registered as localization-provider instead of the DynamicCacheLocalizationProvider.
		/// </summary>
		public virtual bool StaticCache { get; set; }

		#endregion
	}
}