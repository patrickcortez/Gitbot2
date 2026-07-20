using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gitbot2.Source.Utils
{
    internal static class ConsoleEvent
    {
        public static void RegisterConsoleEvents(IHost host,ILogger logger)
        {
             
            Console.CancelKeyPress += async (s, e) =>
            {

                logger.LogInformation("Stopping GITBOT...");


                await host.StopAsync();
                Environment.Exit(0);    

            };

            
        }
    }
}
