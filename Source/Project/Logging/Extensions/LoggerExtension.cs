using System;
using Microsoft.Extensions.Logging;

namespace RegionOrebroLan.Localization.Logging.Extensions
{
	[CLSCompliant(false)]
	public static class LoggerExtension
	{
		#region Methods

		[Obsolete("Use logging-extensions from the NuGet-package \"RegionOrebroLan.Logging\" instead.", true)]
		public static void LogCriticalIfEnabled(this ILogger logger, string message, params object[] arguments)
		{
			RegionOrebroLan.Logging.Extensions.LoggerExtension.LogCriticalIfEnabled(logger, message, arguments);
		}

		[Obsolete("Use logging-extensions from the NuGet-package \"RegionOrebroLan.Logging\" instead.", true)]
		public static void LogCriticalIfEnabled(this ILogger logger, Exception exception, string message, params object[] arguments)
		{
			RegionOrebroLan.Logging.Extensions.LoggerExtension.LogCriticalIfEnabled(logger, exception, message, arguments);
		}

		[Obsolete("Use logging-extensions from the NuGet-package \"RegionOrebroLan.Logging\" instead.", true)]
		public static void LogDebugIfEnabled(this ILogger logger, string message, params object[] arguments)
		{
			RegionOrebroLan.Logging.Extensions.LoggerExtension.LogDebugIfEnabled(logger, message, arguments);
		}

		[Obsolete("Use logging-extensions from the NuGet-package \"RegionOrebroLan.Logging\" instead.", true)]
		public static void LogErrorIfEnabled(this ILogger logger, string message, params object[] arguments)
		{
			RegionOrebroLan.Logging.Extensions.LoggerExtension.LogErrorIfEnabled(logger, message, arguments);
		}

		[Obsolete("Use logging-extensions from the NuGet-package \"RegionOrebroLan.Logging\" instead.", true)]
		public static void LogErrorIfEnabled(this ILogger logger, Exception exception, string message, params object[] arguments)
		{
			RegionOrebroLan.Logging.Extensions.LoggerExtension.LogErrorIfEnabled(logger, exception, message, arguments);
		}

		[Obsolete("Use logging-extensions from the NuGet-package \"RegionOrebroLan.Logging\" instead.", true)]
		public static void LogInformationIfEnabled(this ILogger logger, string message, params object[] arguments)
		{
			RegionOrebroLan.Logging.Extensions.LoggerExtension.LogInformationIfEnabled(logger, message, arguments);
		}

		[Obsolete("Use logging-extensions from the NuGet-package \"RegionOrebroLan.Logging\" instead.", true)]
		public static void LogWarningIfEnabled(this ILogger logger, string message, params object[] arguments)
		{
			RegionOrebroLan.Logging.Extensions.LoggerExtension.LogWarningIfEnabled(logger, message, arguments);
		}

		#endregion
	}
}