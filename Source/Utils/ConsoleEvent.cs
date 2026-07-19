using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gitbot2.Source.Utils
{
    internal static class ConsoleEvent
    {
        public static void RegisterConsoleEvents(IHost host)
        {
            Console.CancelKeyPress += async (s, e) =>
            {

                Console.WriteLine("Stopping GITBOT...");


                await host.StopAsync();
                Environment.Exit(0);    

            };
        }
    }
}
