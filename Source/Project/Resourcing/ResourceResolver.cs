using System;
using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Resourcing
{
	public class ResourceResolver : IResourceResolver
	{
		#region Constructors

		public ResourceResolver(ILocalizationParser parser, IResourceValidator validator)
		{
			this.Parser = parser ?? throw new ArgumentNullException(nameof(parser));
			this.Validator = validator ?? throw new ArgumentNullException(nameof(validator));
		}

		#endregion

		#region Properties

		public virtual ILocalizationParser Parser { get; }
		public virtual IResourceValidator Validator { get; }

		#endregion
	}
}