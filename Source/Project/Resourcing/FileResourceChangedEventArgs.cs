namespace RegionOrebroLan.Localization.Resourcing
{
	public class FileResourceChangedEventArgs(string path) : EventArgs
	{
		#region Properties

		public virtual string Path { get; } = path;

		#endregion
	}
}