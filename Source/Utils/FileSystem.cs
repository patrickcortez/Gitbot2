using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Gitbot2.Source.Utils
{
    internal class Repositories
    {
        public List<string> Repos { get; set; } = [];

    }

    internal static class FileSystem
    {
        private static string[] _repos;
        private readonly static string jsonPath;
        private static ILogger _logger;

        private static void InitLogger()
        {
            _logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger("FileSystem");
        }

        static FileSystem()
        {
            InitLogger();

            jsonPath = Path.Combine(Environment.CurrentDirectory, "repos.json");

            if (!Path.Exists(jsonPath))
            {
                _logger.LogError(message: "repos.json doesnt exist: {}",args: jsonPath);
                return;
            }
            

            GetRepos();
        }

        private static void GetRepos()
        {
            try
            {
                string contents =  File.ReadAllText(jsonPath);
                Repositories repos = JsonSerializer.Deserialize<Repositories>(contents);

                _repos = repos!.Repos.ToArray();
            }catch(Exception ex)
            {
                _logger.LogError(ex,"failed to log repos.json");
            }
        }

        public static string[] GetRepositories()
        {
            return _repos;
        }
    }
}
