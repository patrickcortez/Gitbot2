using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gitbot2.Source.Utils
{
    internal static class Utility
    {
        private static ILogger logger;
        static Utility(){
            logger = Services.CreateProvider("Utility").Services.GetRequiredService<ILogger>();
        }
        public static async Task<RoleStatus> isAllowed(RestClient client,Message message)
        {
            try
            {

                var gUser = await client.GetGuildUserAsync(message.GuildId!.Value, message.Author.Id);

                if(gUser.RoleIds.Contains((ulong)Roles.Owner) || gUser.RoleIds.Contains((ulong)Roles.Developer))
                {
                    logger.LogInformation("User[{}] has been verified", gUser.Username);
                    return RoleStatus.Allowed;
                }

                return RoleStatus.NotAllowed;

            }catch(Exception ex)
            {
                logger.LogError(ex, "Failed to get users role");
                return RoleStatus.Error;
            }
            
        }

        
    }
}
