using Microsoft.Azure.WebJobs.Host;
using Stylelabs.M.Sdk.WebApiClient.Logging;
using System;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Logging
{
    public class TraceWriterLogger : Logger
    {
        private readonly TraceWriter _logger;

        public TraceWriterLogger(TraceWriter logger)
        {
            _logger = logger;
        }
        
        public override bool IsDebugEnabled => false;

        public override bool IsInfoEnabled => false;

        public override bool IsWarnEnabled => true;

        public override bool IsErrorEnabled => true;

        protected override void LogDebug(string message)
        {
            _logger.Info(message);
        }

        protected override void LogError(Exception exception)
        {
            _logger.Info(exception.ToString());
        }

        protected override void LogError(string message, Exception exception)
        {
            _logger.Info(message);
            _logger.Info(exception.ToString());
        }

        protected override void LogInfo(string message)
        {
            _logger.Info(message);
        }

        protected override void LogWarn(string message)
        {
            _logger.Info(message);
        }
    }
}
