using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RegionOrebroLan.Localization.Resourcing;
using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Json.Serialization
{
	public class SerializableLocalizationInternal : SerializableObject, ILocalization
	{
		#region Properties

		[JsonProperty("Name", Order = 10)]
		public virtual string CultureName { get; set; }

		IDictionary<string, ILocalizationEntry> ILocalizationNode.Entries => this.Entries.ToDictionary(item => item.Key, item => (ILocalizationEntry) item.Value);

		[JsonProperty(Order = 40)]
		public virtual IDictionary<string, SerializableLocalizationEntry> Entries { get; } = new Dictionary<string, SerializableLocalizationEntry>(StringComparer.OrdinalIgnoreCase);

		[JsonIgnore]
		public virtual string Name => null;

		IEnumerable<ILocalizationNode> ILocalizationNode.Nodes => this.Nodes.Select(item => item.Node);

		[JsonProperty(Order = 30)]
		public virtual IList<SerializableLocalizationNode> Nodes { get; } = new List<SerializableLocalizationNode>();

		[JsonProperty(Order = 20)]
		public virtual int? Priority { get; set; }

		[JsonIgnore]
		public virtual IResource Resource { get; set; }

		#endregion
	}
}