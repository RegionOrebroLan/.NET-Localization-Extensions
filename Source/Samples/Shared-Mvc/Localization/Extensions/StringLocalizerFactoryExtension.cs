using System;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace Company.WebApplication.Localization.Extensions
{
	public static class StringLocalizerFactoryExtension
	{
		#region Fields

		private const char _namespaceSeparator = '.';

		#endregion

		#region Methods

		public static IStringLocalizer Create(this IStringLocalizerFactory stringLocalizerFactory, string typeFullName)
		{
			return stringLocalizerFactory.Create(2, typeFullName);
		}

		public static IStringLocalizer Create(this IStringLocalizerFactory stringLocalizerFactory, int numberOfPartsForLocation, string typeFullName)
		{
			if(stringLocalizerFactory == null)
				throw new ArgumentNullException(nameof(stringLocalizerFactory));

			if(typeFullName == null)
				throw new ArgumentNullException(nameof(typeFullName));

			return stringLocalizerFactory.Create(typeFullName, string.Join(_namespaceSeparator.ToString(CultureInfo.InvariantCulture), typeFullName.Split(_namespaceSeparator).Take(numberOfPartsForLocation)));
		}

		#endregion
	}
}