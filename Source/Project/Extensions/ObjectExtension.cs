namespace RegionOrebroLan.Localization.Extensions
{
	public static class ObjectExtension
	{
		#region Methods

		public static string ToArgumentString(this object argument)
		{
			return argument != null ? "\"" + argument + "\"" : "null";
		}

		#endregion
	}
}