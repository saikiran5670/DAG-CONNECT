using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.notificationengine;
using net.atos.daf.ct2.notificationengine.repository;
using net.atos.daf.ct2.notificationservice.HostedServices;
using net.atos.daf.ct2.notificationservice.services;
using net.atos.daf.ct2.pushnotificationservice;

namespace net.atos.daf.ct2.notificationservice
{
    public class Program
    {
        public static ManualResetEvent Shutdown = new ManualResetEvent(false);
        public static void Main(string[] args)
        {
            //await CreateHostBuilder(args).Build().RunAsync();
            //var hostBuilder = new HostBuilder()
            // Add configuration, logging, ...
            //.ConfigureServices((hostContext, services) =>
            //{
            //    Server server = new Server
            //    {
            //        Services = { PushNotificationService.BindService(new PushNotificationManagementService()) }
            //    };
            //    services.AddSingleton<Server>(server);
            //    services.AddHostedService<NotificationHostedService>();
            //    services.AddSingleton<IHostedService, NotificationHostedService>();
            //    services.AddSingleton<INotificationIdentifierManager, NotificationIdentifierManager>();
            //    services.AddSingleton<INotificationIdentifierRepository, INotificationIdentifierRepository>();
            //});

            //await hostBuilder.RunConsoleAsync();
            CreateHostBuilder(args).Build().Run();
            //Server server = new Server
            //{
            //    Services = { PushNotificationService.BindService(new PushNotificationManagementService()) }
            //};
            //server.Start();

            //Shutdown.WaitOne(); // <--- Waits for ever or signal received

            //server.ShutdownAsync().Wait();
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
            //service.AddHostedService<NotificationHostedService>();
            //service.AddSingleton(typeof(INotificationIdentifierManager), typeof(NotificationIdentifierManager));
            //service.AddSingleton(typeof(INotificationIdentifierRepository), typeof(INotificationIdentifierRepository));
            Server server = new Server
            {
                //Services = { PushNotificationService.BindService(new PushNotificationManagementService().SayHello(h)).Result }
            };
            services.AddSingleton<Server>(server);
            services.AddHostedService<NotificationHostedService>();
            services.AddSingleton<IHostedService, NotificationHostedService>();
            //services.AddTransient<INotificationIdentifierManager, NotificationIdentifierManager>();
            //services.AddTransient<INotificationIdentifierRepository, NotificationIdentifierRepository>();
        });
    }
}
