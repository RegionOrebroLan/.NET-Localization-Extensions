using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Json.Serialization
{
	public class SerializableLocalizationEntry : SerializableObject, ILocalizationEntry
	{
		#region Properties

		public virtual string Lookup { get; set; }
		public virtual string Value { get; set; }

		#endregion
	}
}