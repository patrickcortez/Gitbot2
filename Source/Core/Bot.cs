using Gitbot2.Source.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gitbot2.Source.Core
{

    internal class Bot
    {
        private IHost _host;
        private GatewayClient client;
        private IConfiguration config;
        private ILogger logger;
        private RestClient Rclient;
        
        private bool isRunning = true;
        public static string CurrentRepo { get; set; }

        

        private int InitializeMembers()
        {
            try
            {
                // Initialize all our members
                _host = Services.CreateProvider();
                logger = _host.Services.GetService<ILogger>();
                client = _host.Services.GetService<GatewayClient>();
                config = _host.Services.GetService<IConfiguration>();
                Rclient = _host.Services.GetService<RestClient>();

                // Configure our events
                client.Connect += () => {
                    logger.LogInformation("GitBot connected at {}", DateTime.Now);

                    Rclient.SendMessageAsync(config.GetValue<ulong>("Discord:GeneralId"), "Hello Everyone!");

                    return ValueTask.CompletedTask;
                };

                return 0;
            }catch(Exception ex)
            {

                return 1;
            }
        } 
        

        public Bot() // Constructor
        {
            InitializeMembers();
            ConsoleEvent.RegisterConsoleEvents(_host,logger);
            
        }

        public async Task<int> RunAsync() // Start Bot
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
