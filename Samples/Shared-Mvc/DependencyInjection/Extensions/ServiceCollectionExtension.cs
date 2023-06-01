using System;
using Application.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection.Extensions
{
	public static class ServiceCollectionExtension
	{
		#region Methods

		public static IServiceCollection ConfigureExample(this IServiceCollection services, IConfiguration configuration)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			return services.Configure<ExampleOptions>(configuration.GetSection("Example"));
		}

		#endregion
	}
}