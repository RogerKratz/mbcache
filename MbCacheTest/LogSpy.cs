using System;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace MbCacheTest
{
	public class LogSpy : IDisposable
	{
		private readonly MemoryAppender _appender;
		private readonly Logger _logger;
		private readonly Level _prevLogLevel;

		public LogSpy(ILog log, Level level)
		{
			_logger = (Logger)log.Logger;

			// Change the log level to DEBUG and temporarily save the previous log level
			_prevLogLevel = _logger.Level;
			_logger.Level = level;

			// Add a new MemoryAppender to the logger.
			_appender = new MemoryAppender();
			_logger.AddAppender(_appender);
		}
		
		public string RenderedMessages()
		{
			var wholeMessage = new StringBuilder();
			foreach (var loggingEvent in _appender.GetEvents())
			{
				wholeMessage.AppendLine(loggingEvent.RenderedMessage);
			}
			return wholeMessage.ToString();
		}


		public void Dispose()
		{
			// Restore the previous log level of the SQL logger and remove the MemoryAppender
			_logger.Level = _prevLogLevel;
			_logger.RemoveAppender(_appender);
		}
	}
}