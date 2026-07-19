using Gitbot2.Source.Commands;
using Gitbot2.Source.Utils;
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
                char prefix = '/'; //CommandPrefix
                bool isallowed = false;

               



                if (message.Author.IsBot)
                {
                    return;
                }

                if (Utility.isAllowed(client, message).Equals(RoleStatus.NotAllowed))
                {

                    await client.SendMessageAsync(message.ChannelId, $"{message.Author.Username} is not permitted");
                    return;
                } else if (Utility.isAllowed(client, message).Equals(RoleStatus.Error))
                {
                    await client.SendMessageAsync(message.ChannelId, "Failed to get user role,discontinuing"); // discontinue in an event of an error for safety
                    return;
                }

                

                logger.LogInformation($"{message.Author.Username} {message.Content}");

                string content = message.Content;

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

                    string msg = (repositories != null)? string.Join('\n', repositories).ToString(): string.Empty;

                    comm = new(content,client,message.ChannelId);

                    int success = await comm.ExecuteCommand();

                    if(success != 0)
                    {
                        await client.SendMessageAsync(message.ChannelId,"Something went wrong, try again later");
                    }

                    logger.LogInformation("Command exited with {}", success);
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
