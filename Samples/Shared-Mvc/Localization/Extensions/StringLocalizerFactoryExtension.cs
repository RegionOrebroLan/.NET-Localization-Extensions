using Microsoft.Extensions.Localization;

namespace Application.Localization.Extensions
{
	public static class StringLocalizerFactoryExtension
	{
		#region Methods

		public static IStringLocalizer Create(this IStringLocalizerFactory stringLocalizerFactory, string typeFullName)
		{
			if(stringLocalizerFactory == null)
				throw new ArgumentNullException(nameof(stringLocalizerFactory));

			if(typeFullName == null)
				throw new ArgumentNullException(nameof(typeFullName));

			return stringLocalizerFactory.Create(typeFullName, "Application");
		}

		#endregion
	}
}