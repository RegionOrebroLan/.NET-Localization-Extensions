using RegionOrebroLan.Localization.Resourcing;

namespace RegionOrebroLan.Localization.Serialization
{
	public interface ILocalization : ILocalizationNode
	{
		#region Properties

		string CultureName { get; }
		int? Priority { get; }
		IResource Resource { get; }

		#endregion
	}
}