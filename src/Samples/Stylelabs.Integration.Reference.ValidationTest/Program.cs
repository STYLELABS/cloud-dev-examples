using Stylelabs.M.Sdk.WebApiClient;
using System;

namespace Stylelabs.Integration.Reference.ValidationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MClient.Logger = new ConsoleLogger();

            try
            {
                Console.WriteLine("Please enter your email address");
                string email = Console.ReadLine();

                if (MConnector.VerifyConnection(email).Result)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Connection successful!");
                } else
                    throw new InvalidOperationException();
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Connection failed.");
            }
            finally
            {
                Console.ResetColor();
                Console.ReadKey();
            }
        }
    }
}
