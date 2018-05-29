using Stylelabs.M.Sdk.WebApiClient.Logging;
using System;

namespace Stylelabs.Integration.Reference.ValidationTest
{
    public class ConsoleLogger : Logger
    {
        public override bool IsDebugEnabled => true;

        public override bool IsInfoEnabled => true;

        public override bool IsWarnEnabled => true;

        public override bool IsErrorEnabled => true;

        protected override void LogDebug(string message)
        {
            Console.WriteLine(message);
        }

        protected override void LogError(Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }

        protected override void LogError(string message, Exception exception)
        {
            Console.WriteLine(message);
            Console.WriteLine(exception.ToString());
        }

        protected override void LogInfo(string message)
        {
            Console.WriteLine(message);
        }

        protected override void LogWarn(string message)
        {
            Console.WriteLine(message);
        }
    }
}
