using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Gitbot2.Source.Commands
{
    public class CommandModule : ApplicationCommandModule<ApplicationCommandContext>
    {
        [SlashCommand("ping","Get a message of pong!")]
        public Task<string> Pong() => Task.FromResult("Pong!");

        [SlashCommand("ignore", "Enable/Disable to Parse messages")]
        public string Ignore(bool result)
        {
                if (result)
                {
                    MessageToggle.Ignore = true;
                    return "Ignoring Messages";
                }
                else
                {
                    MessageToggle.Ignore = false;
                    return "Parsing Messages";
                }
        }

    }


}
