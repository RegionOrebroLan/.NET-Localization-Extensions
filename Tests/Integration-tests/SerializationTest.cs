using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using JsonFormatting = Newtonsoft.Json.Formatting;

namespace IntegrationTests
{
	public abstract class SerializationTest : IntegrationTest
	{
		#region Fields

		private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
		{
			Formatting = JsonFormatting.None,
			NullValueHandling = NullValueHandling.Ignore
		};

		private static XmlSerializerNamespaces _xmlSerializerNamespaces;
		private static readonly XmlWriterSettings _xmlWriterSettings = new() { OmitXmlDeclaration = true };

		#endregion

		#region Properties

		protected internal virtual JsonSerializerSettings JsonSerializerSettings => _jsonSerializerSettings;

		protected internal virtual XmlSerializerNamespaces XmlSerializerNamespaces
		{
			get
			{
				// ReSharper disable InvertIf
				if(_xmlSerializerNamespaces == null)
				{
					var xmlSerializerNamespaces = new XmlSerializerNamespaces();

					xmlSerializerNamespaces.Add(string.Empty, string.Empty);

					_xmlSerializerNamespaces = xmlSerializerNamespaces;
				}
				// ReSharper restore InvertIf

				return _xmlSerializerNamespaces;
			}
		}

		protected internal virtual XmlWriterSettings XmlWriterSettings => _xmlWriterSettings;

		#endregion

		#region Methods

		protected internal virtual Stream CreateStream(string value)
		{
			var memoryStream = new MemoryStream();
			var streamWriter = new StreamWriter(memoryStream);

			streamWriter.Write(value);
			streamWriter.Flush();

			memoryStream.Position = 0;

			return memoryStream;
		}

		protected internal virtual T JsonDeserialize<T>(string value)
		{
			return JsonConvert.DeserializeObject<T>(value);
		}

		protected internal virtual string JsonSerialize(object instance)
		{
			return JsonConvert.SerializeObject(instance, this.JsonSerializerSettings);
		}

		protected internal virtual T XmlDeserialize<T>(string value)
		{
			using(var stream = this.CreateStream(value))
			{
				using(var xmlReader = XmlReader.Create(stream, new XmlReaderSettings { IgnoreWhitespace = true }))
				{
					return (T)new XmlSerializer(typeof(T)).Deserialize(xmlReader);
				}
			}
		}

		protected internal virtual string XmlSerialize(object instance)
		{
			if(instance == null)
				throw new ArgumentNullException(nameof(instance));

			var xmlSerializer = new XmlSerializer(instance.GetType());
			var stringBuilder = new StringBuilder();

			using(var xmlWriter = XmlWriter.Create(stringBuilder, this.XmlWriterSettings))
			{
				xmlSerializer.Serialize(xmlWriter, instance, this.XmlSerializerNamespaces);
				return stringBuilder.ToString();
			}
		}

		#endregion
	}
}