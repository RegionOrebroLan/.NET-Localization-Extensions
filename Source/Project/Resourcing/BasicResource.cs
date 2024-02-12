using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Resourcing
{
	public abstract class BasicResource : IResource
	{
		#region Constructors

		protected BasicResource(IAssembly assembly, ILocalizationParser parser)
		{
			this.Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
			this.Parser = parser ?? throw new ArgumentNullException(nameof(parser));
		}

		#endregion

		#region Properties

		/// <inheritdoc cref="IResource" />
		public virtual IAssembly Assembly { get; }

		public virtual ILocalizationParser Parser { get; }

		#endregion

		#region Methods

		public abstract string Read();

		#endregion
	}
}