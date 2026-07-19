using Gitbot2.Source.Core;
using Gitbot2.Source.Utils;

namespace GitBot2.Source;

public static class Program{

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
            Console.WriteLine($"An Error has Occurreed: {ex.Message}");

            string fullex = ex.ToString(),
                   errP = Path.Combine(Environment.CurrentDirectory,"error.log")
                ;

            await File.AppendAllTextAsync(errP, $"[{DateTime.Now}]\n{fullex}\n{"-".PadRight(20)}\n");

            Console.WriteLine($"Details written to {errP}");

            return 1;
        }
    }

}
