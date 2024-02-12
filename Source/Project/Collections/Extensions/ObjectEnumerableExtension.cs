using RegionOrebroLan.Localization.Extensions;

namespace RegionOrebroLan.Localization.Collections.Extensions
{
	public static class ObjectEnumerableExtension
	{
		#region Methods

		public static string ToCommaSeparatedArgumentString(this IEnumerable<object> arguments)
		{
			return arguments.ToSeparatedArgumentString(", ");
		}

		public static string ToSeparatedArgumentString(this IEnumerable<object> arguments, string separator)
		{
			if(separator == null)
				throw new ArgumentNullException(nameof(separator));

			return arguments != null ? string.Join(separator, arguments.Select(argument => argument.ToArgumentString())) : "null-enumerable";
		}

		#endregion
	}
}