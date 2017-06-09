using System;
using System.Net.Configuration;
using NLog;

namespace WindowsServices.HW.Logging
{
    public class Logger : ILogger
    {
        private static ILogger _currentLogger;
        private static ILogger _codeRewritingLogger;
        private NLog.ILogger _actualLogger = LogManager.CreateNullLogger();
        private static bool _useCodeRewriting;


        public static ILogger Current
        {
            get { return  _currentLogger ?? (_currentLogger = new Logger()); }
        }

        public static ILogger CodeRewritingLogger
        {
            get
            {
                return _codeRewritingLogger ?? (_useCodeRewriting ? _currentLogger : _codeRewritingLogger = new Logger());
            }
        }

        public virtual void LogInfo(string message, params object[] args)
        {
            _actualLogger.Trace(message, args);
        }

        public virtual void LogError(string message, params object[] args)
        {
            _actualLogger.Error(message, args);
        }

        public virtual void LogError(Exception exception)
        {
            _actualLogger.Error(exception);
        }

        public virtual void SetActualLogger(object logger, bool useCodeRewriting = false)
        {
            _actualLogger = (NLog.ILogger)logger;
            _useCodeRewriting = useCodeRewriting;
        }
    }
}
