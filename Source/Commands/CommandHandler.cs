using Gitbot2.Source.Utils;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private IConfiguration config;
        private IOptions<_Roles> roles;



        public void Init()
        {
            _client = client;
            CurrentCommand = Command;
            logger = Services.CreateProvider("CommandHandler").Services.GetRequiredService<ILogger>();
            config = Services.CreateProvider("CommandHandler").Services.GetService<IConfiguration>();
            roles = Services.CreateProvider("CommandHandler").Services.GetRequiredService<IOptions<_Roles>>();
            Commands = new Dictionary<string, Func<object[], ValueTask>>() {
                { "list", ListRepos},
                { "merge",MergeRepo }
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

        private async ValueTask MergeRepo(params object[] args) // merge repo
        {
            string path = config.GetValue<string>("Discord:Current");
            string branch = string.Empty;
            MergeStatus final = new();

            if (args[0] is string _branch)
            {
                branch = _branch;
            }

            if(branch == string.Empty)
            {
                logger.LogError("Branch empty, warning sent");
                await client.SendMessageAsync(ulong.Parse(roles.Value.GenId), "Branch cannot be empty");
                return;
            }

            Repository repo = new(path);

            Branch target = repo.Branches[branch];

            Signature merger = new(repo.Config.Get<string>("user.name").ToString(),repo.Config.Get<string>("user.email").ToString(),DateTime.Now);

            var result = repo.Merge(target, merger);

            if(result.Status == MergeStatus.Conflicts)
            {
                await client.SendMessageAsync(ulong.Parse(roles.Value.GenId), $"Branch {target.FriendlyName} failed to merge due to conficts");
                return;
            }else if(result.Status == MergeStatus.UpToDate)
            {
                await client.SendMessageAsync(ulong.Parse(roles.Value.GenId), $"Branch {target.FriendlyName} is upto Date");
                return;
            }

            await client.SendMessageAsync(ulong.Parse(roles.Value.GenId), $"Branch {target.FriendlyName} at {DateTime.Now}");
        }

    }
}
