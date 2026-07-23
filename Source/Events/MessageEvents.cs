using Gitbot2.Source.Commands;
using Gitbot2.Source.Core;
using Gitbot2.Source.Utils;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;
using System.Text;

namespace Gitbot2.Source.Events
{
    public class MessageCreateHandler(ILogger<MessageCreateHandler> logger,RestClient client) : IMessageCreateGatewayHandler
    {
        private CommandHandler comm;
        public async ValueTask HandleAsync(Message message)
        {
            try
            {


                if (message.Author.IsBot)
                {
                    return;
                }

                logger.LogInformation("Current User: {}", message.Author.Username);

                RoleStatus status = await Utility.isAllowed(client, message);

                

                if (status == RoleStatus.NotAllowed)
                {

                    await client.SendMessageAsync(message.ChannelId, $"{message.Author.Username} is not permitted");
                    return;
                } else if (status == RoleStatus.Error)
                {
                    await client.SendMessageAsync(message.ChannelId, "Failed to get user role,discontinuing"); // discontinue in an event of an error for safety
                    return;
                }

                

                logger.LogInformation($"{message.Author.Username} {message.Content}");

                string content = message.Content;

                if (content.EndsWith("/ignore", StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogInformation("Ignoring {}", content);
                    return;
                }

                if (content.Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    string help = @"
                    Commands:
                        list                    - lists all repositories
                        switch                  - switch to a repository
                        current                 - shows current repository
                        commit <message>        - commit changes with a message
                        merge <b1> <b2>         - merge branches
                        del <repo>              - deletes a repo
                        checkout <br>           - checksout branch
                ";

                    await client.SendMessageAsync(message.ChannelId, help);
                }
                else if (content.Equals("get-users", StringComparison.OrdinalIgnoreCase))
                {
                    await client.SendMessageAsync(message.ChannelId, $"Users Joined: {UserJoinedHandler.GetUsers()}");
                }
                else if (content.Equals("hi", StringComparison.OrdinalIgnoreCase) || content.Equals("hello", StringComparison.OrdinalIgnoreCase))
                {
                    await client.SendMessageAsync(message.ChannelId, $"Hey {message.Author.Username}!");

                }
                else if (content.Equals("list", StringComparison.OrdinalIgnoreCase))
                {
                    string[] repositories = FileSystem.GetRepositories();

                    string msg = (repositories != null) ? string.Join('\n', repositories).ToString() : string.Empty;

                    comm = new(content, client, message.ChannelId);

                    int success = await comm.ExecuteCommand();

                    if (success != 0)
                    {
                        await client.SendMessageAsync(message.ChannelId, "Something went wrong, try again later");
                    }

                    logger.LogInformation("Command exited with {}", success);
                }
                else if (content.Equals("shutdown", StringComparison.OrdinalIgnoreCase))
                {
                    await client.SendMessageAsync(message.ChannelId, "Good bye!");
                    Environment.Exit(0);
                } else if (content.Equals("current", StringComparison.OrdinalIgnoreCase))
                {
                    await client.SendMessageAsync(message.ChannelId, $"Current Repository: {FSOperations.GetCurrent(config: Services.CreateProvider().Services.GetService<IConfiguration>())}");
                } else if (content.StartsWith("switch", StringComparison.OrdinalIgnoreCase))
                {
                    if (content.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Count() < 2)
                    {
                        await client.SendMessageAsync(message.ChannelId, "missing repo name!");

                        return;
                    }


                    string repoName = content.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ElementAt(1);



                    if (FSOperations.SwitchRepo(repoName).Result == TaskStatus.RanToCompletion)
                    {
                        await client.SendMessageAsync(message.ChannelId, $"Repo Switched to {repoName}");


                    }
                    else if (FSOperations.SwitchRepo(repoName).Result == TaskStatus.Canceled)
                    {
                        await client.SendMessageAsync(message.Id, $"Failed to Switch Repo");
                    } else if (FSOperations.SwitchRepo(repoName).Result == TaskStatus.Faulted)
                    {
                        await client.SendMessageAsync(message.ChannelId,"Something Went wrong...");
                    }
                } else if (content.Equals("status", StringComparison.OrdinalIgnoreCase))
                {
                   string response = FSOperations.RepoStatus(Services.CreateProvider().Services.GetService<IConfiguration>());

                    await client.SendMessageAsync(message.ChannelId, response);
                } else if (content.StartsWith("commit", StringComparison.OrdinalIgnoreCase))
                {
                    Func<string, string[]> Tokenize = (line) =>
                    {
                        StringBuilder sb = new();
                        List<string> tokens = new();
                        bool inqoutes = false;

                        foreach (char c in line)
                        {

                            if (c == '"')
                            {
                                inqoutes = !inqoutes;
                                continue;
                            }

                            if(char.IsWhiteSpace(c) && !inqoutes)
                            {
                                tokens.Add(sb.ToString());
                                sb.Clear();
                                continue;
                            }

                            sb.Append(c);

                        }

                        if(sb.Length > 0)
                        {
                            tokens.Add(sb.ToString());
                        }

                        return tokens.ToArray();

                    };

                    string[] tokens = Tokenize(content);

                    if (tokens.Count() < 2)
                    {
                        await client.SendMessageAsync(message.ChannelId, "You forgot to enter a commit message");
                        return;
                    }

                   string response = FSOperations.CommitRepo(Services.CreateProvider().Services.GetService<IConfiguration>(), tokens[1]);

                    await client.SendMessageAsync(message.ChannelId, response);
                }
                else
                {
                    await client.SendMessageAsync(message.ChannelId, "I dont know what to say to that");
                }
            }catch(Exception ex)
            {
                logger.LogError(ex,"failed to send message");
            }

        }
    }

    public class MessageReactionHandler(ILogger<MessageReactionHandler> logger,RestClient client) : IMessageReactionAddGatewayHandler
    {
        public async ValueTask HandleAsync(MessageReactionAddEventArgs reaction)
        {
            User? user = reaction.User;

            await client.SendMessageAsync((ulong)Channels.Genchat, $"{user.Username} reacted with {reaction.Emoji.Name}");
        }
    }

}
