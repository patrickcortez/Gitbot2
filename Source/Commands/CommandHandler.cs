using Gitbot2.Source.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCord.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gitbot2.Source.Commands
{
    internal class CommandHandler(string Command,RestClient client,ulong ChannelId, params object[] args)
    {
        private string CurrentCommand; // name of the current command
        private RestClient _client;
        private List<string> _Repos;
        private ILogger logger;
        private Dictionary<string, Func<object[],ValueTask>> Commands;


        public void Init()
        {
            _client = client;
            CurrentCommand = Command;
            logger = Services.CreateProvider("CommandHandler").Services.GetRequiredService<ILogger>();
            Commands = new Dictionary<string, Func<object[], ValueTask>>() {
                { "list", ListRepos}
            };
        }

        public async Task<int> ExecuteCommand()
        {
            try
            {
                Init();
                // Handle Commands
                

                string cleaned = CurrentCommand.Trim().ToLower();

                logger.LogInformation("recieved command: {}", cleaned);

                Delegate function = Commands[cleaned];

                await ((Func<object[], ValueTask>)function)(args);

                return 0;
            }catch(Exception ex)
            {
                logger.LogError(ex, "Failed to parse command");
                return 1;
            }
        }

        private async ValueTask ListRepos(params object[] args)
        {
            _Repos = FileSystem.GetRepositories().ToList();

            string msg = string.Join('\n', _Repos);

            await client.SendMessageAsync(ChannelId, $"List of Repositories:\n{msg}");

        }

    }
}
