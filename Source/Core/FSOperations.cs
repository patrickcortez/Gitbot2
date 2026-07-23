using Gitbot2.Source.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gitbot2.Source.Core
{
    

    internal class DiscordConfig
    {

        public string Token { get; set; }
        public string GeneralId { get; set; }
        public string Current { get; set; }

    }

    class Conf
    {
        public DiscordConfig Discord { get; set; } = new();
    }
    internal static class FSOperations
    {
        private static ILogger logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger("FSOperations");
        public static async Task<TaskStatus> SwitchRepo(string newRepo)
        {
            try
            {
                if (!Path.Exists(newRepo) || !Path.IsPathRooted(newRepo))
                {
                    logger.LogWarning("Repo {} doesn't exist.", newRepo);
                    return TaskStatus.Canceled;
                }

                string path = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
                Stream read = File.OpenRead(path);

                Conf conf = await JsonSerializer.DeserializeAsync<Conf>(read);

                read.Close();

                if (conf is null)
                {
                    logger.LogWarning("Json is null");
                    return TaskStatus.Canceled;
                }

                conf.Discord.Current = newRepo;

                Stream write = new FileStream(path, FileMode.Create,FileAccess.Write,FileShare.None,4096,FileOptions.Asynchronous);

                await JsonSerializer.SerializeAsync(write, conf, new JsonSerializerOptions() { WriteIndented=true });

                write.Close();

                return TaskStatus.RanToCompletion;

               

            }catch(Exception ex)
            {
                logger.LogError(ex, "Json Serializing/Deserializing Error");
                return TaskStatus.Faulted;
            }

            
        }

        public static string GetCurrent(IConfiguration config)
        {
            string? repo = config.GetValue<string>("Discord:Current");
            

            return (!string.IsNullOrEmpty(repo)) ? repo : string.Empty ;
        }

        public static 
        
    }
}
