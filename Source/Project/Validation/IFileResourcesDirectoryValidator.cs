using System.IO.Abstractions;

namespace RegionOrebroLan.Localization.Validation
{
	public interface IFileResourcesDirectoryValidator
	{
		#region Methods

		void Validate(IDirectoryInfo fileResourcesDirectory);

		#endregion
	}
}