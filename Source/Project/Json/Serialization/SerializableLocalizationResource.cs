using System.Collections.Generic;

namespace RegionOrebroLan.Localization.Json.Serialization
{
	public class SerializableLocalizationResource : SerializableObject
	{
		#region Properties

		public virtual IList<SerializableLocalization> Cultures { get; } = new List<SerializableLocalization>();

		#endregion
	}
}