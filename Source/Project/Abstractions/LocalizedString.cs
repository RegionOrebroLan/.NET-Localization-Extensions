using System.Globalization;

namespace RegionOrebroLan.Localization.Abstractions
{
	[CLSCompliant(false)]
	public class LocalizedString : Microsoft.Extensions.Localization.LocalizedString, ILocalizedString
	{
		#region Constructors

		public LocalizedString(string name, string value) : base(name, value) { }
		public LocalizedString(string name, string value, bool resourceNotFound) : base(name, value, resourceNotFound) { }
		public LocalizedString(string name, string value, bool resourceNotFound, string searchedLocation) : base(name, value, resourceNotFound, searchedLocation) { }

		#endregion

		#region Properties

		public virtual CultureInfo Culture { get; set; }

		#endregion
	}
}