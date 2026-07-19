using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gitbot2.Source.Events
{
    internal class UserJoinedHandler(ILogger<UserJoinedHandler> logger,RestClient client) : IGuildUserAddGatewayHandler
    {
        private static int Unum = 0;

        public ValueTask HandleAsync(GuildUser user)
        {
            client.SendMessageAsync((ulong)Channels.Genchat, $"Greetings {user.Username}");

            Unum++;
            return ValueTask.CompletedTask;
        }

        public static int GetUsers()
        {
            return Unum;
        }

    }
}
