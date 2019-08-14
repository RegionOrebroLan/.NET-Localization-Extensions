using System;

namespace RegionOrebroLan.Localization.Resourcing
{
	public class FileResourceChangedEventArgs : EventArgs
	{
		#region Constructors

		public FileResourceChangedEventArgs(string path)
		{
			this.Path = path;
		}

		#endregion

		#region Properties

		public virtual string Path { get; }

		#endregion
	}
}