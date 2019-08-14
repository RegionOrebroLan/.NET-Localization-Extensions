using System;
using System.Collections.Generic;
using System.Linq;
using RegionOrebroLan.Localization.Collections.Extensions;
using RegionOrebroLan.Localization.Reflection;

namespace RegionOrebroLan.Localization.Validation
{
	public class EmbeddedResourceAssembliesValidator : IEmbeddedResourceAssembliesValidator
	{
		#region Fields

		public const string ExceptionMessagePrefix = "Embedded-resource-assemblies-exception: ";

		#endregion

		#region Methods

		protected internal virtual ArgumentException CreateDuplicatesNotAllowedException(string parameterName, IEnumerable<object> values)
		{
			return new ArgumentException(this.CreateExceptionMessage(parameterName, "contain duplicate values", values), parameterName);
		}

		protected internal virtual ArgumentException CreateEmptyValuesNotAllowedException(string parameterName, IEnumerable<object> values)
		{
			return new ArgumentException(this.CreateExceptionMessage(parameterName, "contain empty values", values), parameterName);
		}

		protected internal virtual string CreateExceptionMessage(string parameterName, string specifics, IEnumerable<object> values)
		{
			var message = $"{ExceptionMessagePrefix}The {parameterName}-collection can not {specifics}.";

			if(values != null)
				message += " Values: " + values.ToCommaSeparatedArgumentString();

			return message;
		}

		protected internal virtual ArgumentException CreateNullValuesNotAllowedException(string parameterName, IEnumerable<object> values)
		{
			return new ArgumentException(this.CreateExceptionMessage(parameterName, "contain null-values", values), parameterName);
		}

		protected internal virtual ArgumentException CreateWhiteSpaceOnlyValuesNotAllowedException(string parameterName, IEnumerable<object> values)
		{
			return new ArgumentException(this.CreateExceptionMessage(parameterName, "contain white-space-only values", values), parameterName);
		}

		public virtual void Validate(IEnumerable<IAssembly> assemblies)
		{
			assemblies = assemblies?.ToArray();

			if(assemblies == null)
				return;

			if(assemblies.Any(item => item == null))
				throw this.CreateNullValuesNotAllowedException(nameof(assemblies), assemblies);

			if(assemblies.Count() > assemblies.Distinct().Count())
				throw this.CreateDuplicatesNotAllowedException(nameof(assemblies), assemblies);
		}

		public virtual void Validate(IEnumerable<string> patterns)
		{
			patterns = patterns?.ToArray();

			if(patterns == null)
				return;

			if(patterns.Any(item => item == null))
				throw this.CreateNullValuesNotAllowedException(nameof(patterns), patterns);

			if(patterns.Any(string.IsNullOrEmpty))
				throw this.CreateEmptyValuesNotAllowedException(nameof(patterns), patterns);

			if(patterns.Any(string.IsNullOrWhiteSpace))
				throw this.CreateWhiteSpaceOnlyValuesNotAllowedException(nameof(patterns), patterns);

			if(patterns.Count() > patterns.Distinct().Count())
				throw this.CreateDuplicatesNotAllowedException(nameof(patterns), patterns);
		}

		#endregion
	}
}