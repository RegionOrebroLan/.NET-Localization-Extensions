using System.Collections.Generic;

namespace Application.Configuration
{
	public class ExampleOptions
	{
		#region Properties

		/// <summary>
		/// The key is the base-name and the value is the location.
		/// </summary>
		public virtual IDictionary<string, string> Localizers { get; } = new Dictionary<string, string>();

		/// <summary>
		/// The key is the name and the value is an explanation.
		/// </summary>
		public virtual IDictionary<string, string> Names { get; } = new Dictionary<string, string>();

		#endregion
	}
}