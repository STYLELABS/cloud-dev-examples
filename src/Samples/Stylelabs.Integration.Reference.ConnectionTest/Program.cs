using Stylelabs.M.Sdk.WebApiClient;
using System;

namespace Stylelabs.Integration.Reference.ConnectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MClient.Logger = new ConsoleLogger();

            try
            {
                var entity = MConnector.Client.Entities.Get(1).Result;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Connection successful!");
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
