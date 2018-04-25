using Stylelabs.M.Sdk.WebApiClient.Logging;
using System;

namespace Stylelabs.Integration.Reference.Training.Logging
{
    public class ConsoleLogger : Logger
    {
        public override bool IsDebugEnabled => false;

        public override bool IsInfoEnabled => false;

        public override bool IsWarnEnabled => true;

        public override bool IsErrorEnabled => true;

        protected override void LogDebug(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        protected override void LogError(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exception.ToString());
            Console.ResetColor();
        }

        protected override void LogError(string message, Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.WriteLine(exception.ToString());
            Console.ResetColor();
        }

        protected override void LogInfo(string message)
        {
            Console.WriteLine(message);
        }

        protected override void LogWarn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
