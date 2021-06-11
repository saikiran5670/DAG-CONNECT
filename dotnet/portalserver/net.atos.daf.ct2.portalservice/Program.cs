using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace net.atos.daf.ct2.portalservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.AddServerHeader = false;
                })
                .UseStartup<Startup>();
            }).ConfigureLogging(builder =>
                        {
                            builder.SetMinimumLevel(LogLevel.Trace);
                            builder.AddLog4Net("log4net.config");
                        });
    }
}
