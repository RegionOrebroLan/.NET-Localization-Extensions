using Newtonsoft.Json;
using RegionOrebroLan.Localization.Resourcing;
using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Json.Serialization
{
	public class LocalizationParser : ILocalizationParser
	{
		#region Methods

		public virtual IEnumerable<ILocalization> Parse(IResource resource, string value)
		{
			if(resource == null)
				throw new ArgumentNullException(nameof(resource));

			if(string.IsNullOrEmpty(value))
				return Enumerable.Empty<ILocalization>();

			var localizations = (JsonConvert.DeserializeObject<SerializableLocalizationResource>(value) ?? new SerializableLocalizationResource())
				.Cultures.Select(item => item.Culture).ToArray();

			foreach(var localization in localizations)
			{
				localization.Resource = resource;
			}

			return localizations;
		}

		#endregion
	}
}