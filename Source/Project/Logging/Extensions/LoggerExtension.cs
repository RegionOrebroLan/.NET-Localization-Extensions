using System;
using Microsoft.Extensions.Logging;

namespace RegionOrebroLan.Localization.Logging.Extensions
{
	[CLSCompliant(false)]
	public static class LoggerExtension
	{
		#region Methods

		public static void LogCriticalIfEnabled(this ILogger logger, string message, params object[] arguments)
		{
			logger.LogIfEnabled(LogLevel.Critical, null, message, arguments);
		}

		public static void LogCriticalIfEnabled(this ILogger logger, Exception exception, string message, params object[] arguments)
		{
			logger.LogIfEnabled(LogLevel.Critical, exception, message, arguments);
		}

		public static void LogDebugIfEnabled(this ILogger logger, string message, params object[] arguments)
		{
			logger.LogIfEnabled(LogLevel.Debug, null, message, arguments);
		}

		public static void LogErrorIfEnabled(this ILogger logger, string message, params object[] arguments)
		{
			logger.LogIfEnabled(LogLevel.Error, null, message, arguments);
		}

		public static void LogErrorIfEnabled(this ILogger logger, Exception exception, string message, params object[] arguments)
		{
			logger.LogIfEnabled(LogLevel.Error, exception, message, arguments);
		}

		private static void LogIfEnabled(this ILogger logger, LogLevel logLevel, Exception exception, string message, params object[] arguments)
		{
			if(logger == null)
				throw new ArgumentNullException(nameof(logger));

			if(!logger.IsEnabled(logLevel))
				return;

			logger.Log(logLevel, exception, message, arguments);
		}

		public static void LogInformationIfEnabled(this ILogger logger, string message, params object[] arguments)
		{
			logger.LogIfEnabled(LogLevel.Information, null, message, arguments);
		}

		public static void LogWarningIfEnabled(this ILogger logger, string message, params object[] arguments)
		{
			logger.LogIfEnabled(LogLevel.Warning, null, message, arguments);
		}

		#endregion
	}
}