using System.Globalization;

namespace RegionOrebroLan.Localization
{
	public interface ILocalizedString
	{
		#region Properties

		CultureInfo Culture { get; }
		string Name { get; }
		bool ResourceNotFound { get; }
		string SearchedLocation { get; }
		string Value { get; }

		#endregion
	}
}