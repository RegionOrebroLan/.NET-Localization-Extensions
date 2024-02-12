using Microsoft.Extensions.Logging;

namespace IntegrationTests.Helpers
{
	public class TestLogger : ILogger
	{
		#region Properties

		public virtual IList<Tuple<LogLevel, EventId, object, Exception>> Logs { get; } = new List<Tuple<LogLevel, EventId, object, Exception>>();

		#endregion

		#region Methods

		public virtual IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}

		public virtual bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			this.Logs.Add(Tuple.Create<LogLevel, EventId, object, Exception>(logLevel, eventId, state, exception));
		}

		#endregion
	}
}