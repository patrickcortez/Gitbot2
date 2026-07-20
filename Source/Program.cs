using Gitbot2.Source.Core;
using Gitbot2.Source.Utils;
using Microsoft.Extensions.Logging;

namespace GitBot2.Source;

public static class Program{


    private static ILogger logger;

    static Program()
    {
        logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger("Program");
    }

    public static async Task<int> Main(params string[] args)
    {
        try
        {
            // Instantiate Bot
            Bot bot = new();
            // Run our Bot
            return await bot.RunAsync();

        }catch(Exception ex)
        {
            logger.LogError(ex,"An Error has occured");

            string fullex = ex.ToString(),
                   errP = Path.Combine(Environment.CurrentDirectory,"error.log")
                ;

            await File.AppendAllTextAsync(errP, $"[{DateTime.Now}]\n{fullex}\n{"-".PadRight(20)}\n");

            logger.LogInformation("Details written to {}",errP);

            return 1;
        }
    }

}
