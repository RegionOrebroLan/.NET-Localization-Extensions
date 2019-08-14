namespace RegionOrebroLan.Localization.Serialization
{
	public interface ILocalizationEntry
	{
		#region Properties

		string Lookup { get; }
		string Value { get; }

		#endregion
	}
}