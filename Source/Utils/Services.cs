using GitBot2.Source;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gitbot2.Source.Utils
{
    internal static class Services
    {

        public static IHost BuildHost()
        {
            var builder = new HostApplicationBuilder(); // Build our Host

            builder.Services
                .AddDiscordGateway(
                    option =>
                    {
                        option.Intents = GatewayIntents.GuildMessages
                        | GatewayIntents.DirectMessages
                        | GatewayIntents.MessageContent
                        | GatewayIntents.DirectMessageReactions
                        | GatewayIntents.GuildMessageReactions;

                    }
                )
                .AddGatewayHandlers(typeof(Program).Assembly)
                ;

            

            return builder.Build();
        }

        public static ServiceProvider CreateProvider()
        {

            // Stand by for now
            return new ServiceCollection()
                .BuildServiceProvider();
        }
    }
}
