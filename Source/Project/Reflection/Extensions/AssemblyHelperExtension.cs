namespace RegionOrebroLan.Localization.Reflection.Extensions
{
	public static class AssemblyHelperExtension
	{
		#region Methods

		public static IEnumerable<IAssembly> Find(this IAssemblyHelper assemblyHelper, IEnumerable<string> patterns)
		{
			if(assemblyHelper == null)
				throw new ArgumentNullException(nameof(assemblyHelper));

			if(patterns == null)
				throw new ArgumentNullException(nameof(patterns));

			var assemblies = new List<IAssembly>();

			foreach(var pattern in patterns)
			{
				assemblies.AddRange(assemblyHelper.Find(pattern));
			}

			return assemblies.ToArray();
		}

		#endregion
	}
}