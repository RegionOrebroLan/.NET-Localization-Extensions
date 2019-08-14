namespace RegionOrebroLan.Localization.IntegrationTests.Helpers
{
	public class TestContext : ITestContext
	{
		#region Properties

		public virtual string ConfigurationDirectoryPath { get; set; }
		public virtual string ConfigurationDirectoryRelativePath { get; set; }
		public virtual string ConfigurationFilePath { get; set; }
		public virtual string ConfigurationFileRelativePath { get; set; }
		public virtual string ConfiguredFileResourcesDirectoryPath { get; set; }
		public virtual string ConfiguredFileResourcesDirectoryRelativePath { get; set; }
		public virtual string EmptyDirectoryPath { get; set; }
		public virtual string EmptyDirectoryRelativePath { get; set; }
		public virtual string FileResourcesDirectoryPath { get; set; }
		public virtual string FileResourcesDirectoryRelativePath { get; set; }
		public virtual string RootPath { get; set; }
		public virtual string TestDirectoryPath { get; set; }
		public virtual string TestDirectoryRelativePath { get; set; }

		#endregion
	}
}