using System;
using System.Globalization;

namespace Shared.Globalization
{
	public class CustomCulture : CultureInfo
	{
		#region Constructors

		public CustomCulture(string baseName, string displayName, string englishName, string name, string nativeName, CultureInfo parent) : base(baseName)
		{
			var culture = new CultureInfo(name);

			this.DisplayName = displayName;
			this.EnglishName = englishName;
			this.LCID = culture.LCID;
			this.Name = culture.Name;
			this.NativeName = nativeName;
			this.Parent = parent ?? throw new ArgumentNullException(nameof(parent));
		}

		#endregion

		#region Properties

		public override string DisplayName { get; }
		public override string EnglishName { get; }
		public override int LCID { get; }
		public override string Name { get; }
		public override string NativeName { get; }
		public override CultureInfo Parent { get; }

		#endregion

		#region Methods

		public override string ToString()
		{
			return this.Name;
		}

		#endregion
	}
}