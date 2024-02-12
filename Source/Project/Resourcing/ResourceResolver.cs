using RegionOrebroLan.Localization.Serialization;

namespace RegionOrebroLan.Localization.Resourcing
{
	public class ResourceResolver(ILocalizationParser parser, IResourceValidator validator) : IResourceResolver
	{
		#region Properties

		public virtual ILocalizationParser Parser { get; } = parser ?? throw new ArgumentNullException(nameof(parser));
		public virtual IResourceValidator Validator { get; } = validator ?? throw new ArgumentNullException(nameof(validator));

		#endregion
	}
}