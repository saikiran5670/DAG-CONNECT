using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.rfmsdataservice
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
                                 serverOptions.AllowSynchronousIO = true;
                             })
                             .UseStartup<Startup>();
                         }).ConfigureLogging(builder =>
                         {
                             builder.SetMinimumLevel(LogLevel.Trace);
                             builder.AddLog4Net("log4net.config");
                         });

    }
}
