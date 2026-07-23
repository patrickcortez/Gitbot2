using Gitbot2.Source.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace Gitbot2.Source.Events
{
    internal class UserJoinedHandler(ILogger<UserJoinedHandler> logger,RestClient client) : IGuildUserAddGatewayHandler
    {
        private static int Unum = 0;

        public ValueTask HandleAsync(GuildUser user)
        {
            IOptions<_Roles> roles = Services.CreateProvider().Services.GetService<IOptions<_Roles>>();

            object value = roles.Value.GenId;
            ulong Genchat = ulong.Parse(value as string);
            client.SendMessageAsync(Genchat, $"Greetings {user.Username}");

            Unum++;
            return ValueTask.CompletedTask;
        }

        public static int GetUsers()
        {
            return Unum;
        }

    }
}
