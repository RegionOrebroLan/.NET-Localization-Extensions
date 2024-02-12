using Newtonsoft.Json;
using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Json.Serialization
{
	public class SerializableLocalizationNodeInternal : SerializableObject, ILocalizationNode
	{
		#region Properties

		IDictionary<string, ILocalizationEntry> ILocalizationNode.Entries => this.Entries.ToDictionary(item => item.Key, item => (ILocalizationEntry)item.Value);

		[JsonProperty(Order = 30)]
		public virtual IDictionary<string, SerializableLocalizationEntry> Entries { get; } = new Dictionary<string, SerializableLocalizationEntry>(StringComparer.OrdinalIgnoreCase);

		[JsonProperty(Order = 10)]
		public virtual string Name { get; set; }

		IEnumerable<ILocalizationNode> ILocalizationNode.Nodes => this.Nodes.Select(item => item.Node);

		[JsonProperty(Order = 20)]
		public virtual IList<SerializableLocalizationNode> Nodes { get; } = new List<SerializableLocalizationNode>();

		#endregion
	}
}