using Gitbot2.Source.UserEvs;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;
using NetCord.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gitbot2.Source.Messages
{
    public class MessageCreateHandler(ILogger<MessageCreateHandler> logger,RestClient client) : IMessageCreateGatewayHandler
    {


        public async ValueTask HandleAsync(Message message)
        {

            char prefix = '/'; //CommandPrefix

            if (message.Author.IsBot)
            {
                return;
            }

            logger.LogInformation($"{message.Author.Username} {message.Content}");

            string content = message.Content;

            // Command Handler

            if (content.StartsWith("help", StringComparison.OrdinalIgnoreCase))
            {
                string help = @"
                    Commands:
                        list                - lists all repositories
                        switch              - switch to a repository
                        current             - shows current repository
                        commit <message>    - commit changes with a message
                        merge <b1> <b2>     - merge branches
                        del <repo>          - deletes a repo
                        checkout <br>       - checksout branch
                ";

                await client.SendMessageAsync(message.ChannelId, help);
            }
            else if (content.Equals("get-users", StringComparison.OrdinalIgnoreCase))
            {
                await client.SendMessageAsync(message.ChannelId, $"Users Joined: {UserJoinedHandler.GetUsers()}");
            }
            else if (content.Equals("hi", StringComparison.OrdinalIgnoreCase) || content.Equals("hello",StringComparison.OrdinalIgnoreCase))
            {
                await client.SendMessageAsync(message.ChannelId, $"Hey {message.Author.Username}!");
            }
            else
            {
                await client.SendMessageAsync(message.ChannelId, "I dont know what to say to that");
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
