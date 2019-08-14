using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RegionOrebroLan.Localization.Json.Serialization
{
	public abstract class SerializableObject
	{
		#region Properties

		[JsonExtensionData]
		protected internal virtual IDictionary<string, JToken> InvalidProperties { get; } = new Dictionary<string, JToken>();

		#endregion

		#region Methods

		[OnDeserialized]
		protected internal void OnDeserialized(StreamingContext streamingContext)
		{
			this.OnDeserializedInternal(streamingContext);
		}

		protected internal virtual void OnDeserializedInternal(StreamingContext streamingContext)
		{
			if(this.InvalidProperties.Any())
				throw new JsonSerializationException($"The json for type \"{this.GetType()}\" contains the following invalid properties: {string.Join(", ", this.InvalidProperties.Keys)}.");
		}

		#endregion
	}
}