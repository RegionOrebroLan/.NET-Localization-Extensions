using System;
using System.Globalization;

namespace RegionOrebroLan.Localization.Extensions
{
	public static class StringExtension
	{
		#region Methods

		public static string FirstCharacterToLower(this string value)
		{
			return value.FirstCharacterToLower(CultureInfo.CurrentCulture);
		}

		public static string FirstCharacterToLower(this string value, CultureInfo culture)
		{
			if(value == null)
				throw new ArgumentNullException(nameof(value));

			if(value.Length == 0)
				return value;

			return char.ToLower(value[0], culture) + value.Substring(1);
		}

		public static string FirstCharacterToLowerInvariant(this string value)
		{
			return value.FirstCharacterToLower(CultureInfo.InvariantCulture);
		}

		#endregion
	}
}