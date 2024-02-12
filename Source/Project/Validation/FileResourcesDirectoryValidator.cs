using System.IO.Abstractions;

namespace RegionOrebroLan.Localization.Validation
{
	public class FileResourcesDirectoryValidator : IFileResourcesDirectoryValidator
	{
		#region Methods

		public virtual void Validate(IDirectoryInfo fileResourcesDirectory)
		{
			if(fileResourcesDirectory == null)
				return;

			if(!fileResourcesDirectory.Exists)
				throw new DirectoryNotFoundException($"File-resources-directory-exception: The directory \"{fileResourcesDirectory.FullName}\" does not exist.");
		}

		#endregion
	}
}