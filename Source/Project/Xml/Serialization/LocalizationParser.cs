using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using RegionOrebroLan.Localization.Resourcing;
using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Xml.Serialization
{
	public class LocalizationParser : ILocalizationParser
	{
		#region Methods

		[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Should be disposed by the caller.")]
		protected internal virtual Stream CreateStream(string value)
		{
			var memoryStream = new MemoryStream();
			var streamWriter = new StreamWriter(memoryStream);

			streamWriter.Write(value);
			streamWriter.Flush();

			memoryStream.Position = 0;

			return memoryStream;
		}

		public virtual IEnumerable<ILocalization> Parse(IResource resource, string value)
		{
			if(resource == null)
				throw new ArgumentNullException(nameof(resource));

			if(string.IsNullOrEmpty(value))
				return Enumerable.Empty<ILocalization>();

			using(var stream = this.CreateStream(value))
			{
				using(var xmlReader = XmlReader.Create(stream, new XmlReaderSettings {IgnoreWhitespace = true}))
				{
					var serializableLocalizationResource = (SerializableLocalizationResource) new XmlSerializer(typeof(SerializableLocalizationResource)).Deserialize(xmlReader);

					return serializableLocalizationResource.Localizations;
				}
			}
		}

		#endregion
	}
}