using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.schedularservice.ServiceSchedular;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
namespace net.atos.daf.ct2.schedularservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddLog4Net("log4net.config");
                }).ConfigureServices((hostcontext, services) =>
                {
                    Server server = new Server
                    {
                        //Services = { PushNotificationService.BindService(new PushNotificationManagementService().SayHello(h)).Result }
                    };
                    services.AddSingleton<Server>(server);
                    services.AddHostedService<DataCleanupHostedService>();
                    services.AddSingleton<IHostedService, DataCleanupHostedService>();
                });
    }
}
