using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Resourcing
{
	public interface IResourceResolver
	{
		#region Properties

		ILocalizationParser Parser { get; }
		IResourceValidator Validator { get; }

		#endregion
	}
}