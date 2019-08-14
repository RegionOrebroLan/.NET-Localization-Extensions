using System.Collections.Generic;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization.Validation
{
	public interface IEmbeddedResourceAssembliesValidator
	{
		#region Methods

		void Validate(IEnumerable<IAssembly> assemblies);
		void Validate(IEnumerable<string> patterns);

		#endregion
	}
}