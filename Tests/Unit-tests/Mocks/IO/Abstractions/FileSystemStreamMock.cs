using System.IO.Abstractions;
using System.Reflection;

namespace UnitTests.Mocks.IO.Abstractions
{
	public class FileSystemStreamMock : FileSystemStream
	{
		#region Fields

		private static readonly FieldInfo _fileSystemStreamNameField = typeof(FileSystemStream).GetField("<Name>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

		#endregion

		#region Constructors

		public FileSystemStreamMock(Stream stream, string path, bool isAsync) : base(stream, string.IsNullOrEmpty(path) ? "-" : path, isAsync)
		{
			if(!string.IsNullOrEmpty(path))
				return;

			_fileSystemStreamNameField.SetValue(this, path);
		}

		#endregion
	}
}