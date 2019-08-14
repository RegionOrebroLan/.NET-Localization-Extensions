using System;
using System.Globalization;
using RegionOrebroLan.Localization.Abstractions;

namespace RegionOrebroLan.Localization
{
	public class LocalizedStringFactory : ILocalizedStringFactory
	{
		#region Fields

		private const string _messageForMissingValueFormat = "[Value for name \"{0}\" and culture \"{1}\" is missing.]";

		#endregion

		#region Properties

		protected internal virtual string MessageForMissingValueFormat => _messageForMissingValueFormat;

		#endregion

		#region Methods

		public virtual ILocalizedString Create(CultureInfo culture, string information, string name, string value)
		{
			if(culture == null)
				throw new ArgumentNullException(nameof(culture));

			if(name == null)
				throw new ArgumentNullException(nameof(name));

			var resourceNotFound = value == null;

			if(value == null)
				value = this.CreateMessageForMissingValue(culture, name);

			return new LocalizedString(name, value, resourceNotFound, information) {Culture = culture};
		}

		protected internal virtual string CreateMessageForMissingValue(CultureInfo culture, string name)
		{
			return string.Format(CultureInfo.InvariantCulture, this.MessageForMissingValueFormat, name, culture);
		}

		#endregion
	}
}