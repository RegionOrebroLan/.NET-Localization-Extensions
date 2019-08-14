namespace RegionOrebroLan.Localization.Configuration
{
	public interface ILocalizationOptionsResolver
	{
		#region Methods

		/// <summary>
		/// Should throw an exception if the localization-options can not be resolved.
		/// </summary>
		IResolvedLocalizationOptions Resolve(LocalizationOptions localizationOptions);

		#endregion
	}
}