namespace RegionOrebroLan.Localization.Json.Serialization
{
	public class SerializableLocalization : SerializableObject
	{
		#region Properties

		public virtual SerializableLocalizationInternal Culture { get; set; } = new SerializableLocalizationInternal();

		#endregion
	}
}