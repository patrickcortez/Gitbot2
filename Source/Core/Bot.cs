using Gitbot2.Source.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gitbot2.Source.Core
{
    internal class Bot
    {
        private IHost _host;
        private bool isRunning = true;

        
        

        public Bot()
        {
            _host = Services.BuildHost();
            ConsoleEvent.RegisterConsoleEvents(_host);
            

        }

        public async Task<int> RunAsync()
        {
            try
            {


                await _host.StartAsync();
                

                await Task.Delay(-1);

                return 0;

            }catch(Exception ex)
            {
                Console.WriteLine($"An Error has Occurreed: {ex.Message}");

                string fullex = ex.ToString(),
                       errP = Path.Combine(Environment.CurrentDirectory, "error.log")
                    ;

                await File.AppendAllTextAsync(errP, $"[{DateTime.Now}]\n{fullex}\n{"-".PadRight(20)}\n");

                Console.WriteLine($"Details written to {errP}");

                return 1;
            }
        }
    }
}
