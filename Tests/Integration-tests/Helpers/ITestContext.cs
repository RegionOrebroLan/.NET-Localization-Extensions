namespace IntegrationTests.Helpers
{
	public interface ITestContext
	{
		#region Properties

		string ConfigurationDirectoryPath { get; }
		string ConfigurationDirectoryRelativePath { get; }
		string ConfigurationFilePath { get; }
		string ConfigurationFileRelativePath { get; }
		string ConfiguredFileResourcesDirectoryPath { get; }
		string ConfiguredFileResourcesDirectoryRelativePath { get; }
		string EmptyDirectoryPath { get; }
		string EmptyDirectoryRelativePath { get; }
		string FileResourcesDirectoryPath { get; }
		string FileResourcesDirectoryRelativePath { get; }
		string RootPath { get; }
		string TestDirectoryPath { get; }
		string TestDirectoryRelativePath { get; }

		#endregion
	}
}