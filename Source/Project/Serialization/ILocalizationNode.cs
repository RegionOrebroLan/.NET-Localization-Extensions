namespace RegionOrebroLan.Localization.Serialization
{
	public interface ILocalizationNode
	{
		#region Properties

		IDictionary<string, ILocalizationEntry> Entries { get; }
		string Name { get; }
		IEnumerable<ILocalizationNode> Nodes { get; }

		#endregion
	}
}