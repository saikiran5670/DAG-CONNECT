using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofenceservice
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
                         });
    }
}
