using NLog;
using NLog.Web;
using Microsoft.AspNetCore;
using TelegramBotBARS;

public static class Programm
{
    public static void Main(string[] args)
    {
        try
        {
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("init main");

            CreateWebHostBuilder(args)
                .ConfigureLogging(log =>
                {
                    log.ClearProviders();
                })
                .UseNLog()
                .Build().Run();
        }
        finally
        {
            LogManager.Shutdown();
        } 
    }
    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
}
