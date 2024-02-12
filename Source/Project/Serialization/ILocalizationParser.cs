using RegionOrebroLan.Localization.Resourcing;

namespace RegionOrebroLan.Localization.Serialization
{
	public interface ILocalizationParser
	{
		#region Methods

		IEnumerable<ILocalization> Parse(IResource resource, string value);

		#endregion
	}
}