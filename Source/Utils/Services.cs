using GitBot2.Source;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace Gitbot2.Source.Utils
{
    internal static class Services
    {
        /* Unused

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

        */

        public static IHost CreateProvider(string categoryname = "")
        {

            var builder = new HostApplicationBuilder(); // Build our Host

            builder.Services
                .AddDiscordGateway(

                    option =>
                    {
                        option.Token = new BotToken(builder.Configuration["Discord:Token"]).RawToken;
                        option.Intents = GatewayIntents.GuildMessages
                        | GatewayIntents.DirectMessages
                        | GatewayIntents.MessageContent
                        | GatewayIntents.DirectMessageReactions
                        | GatewayIntents.GuildMessageReactions;

                       

                    }
                )
                .AddGatewayHandlers(typeof(Program).Assembly)
                .AddSingleton<ILogger>(LoggerFactory.Create(c => c.AddConsole()).CreateLogger(categoryname))
                .AddLogging()
                ;



            return builder.Build();
        }
    }
}
