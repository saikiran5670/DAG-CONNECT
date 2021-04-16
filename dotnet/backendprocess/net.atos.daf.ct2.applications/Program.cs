using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.email;
using net.atos.daf.ct2.email.Repository;

namespace net.atos.daf.ct2.applications
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var connectionString = hostContext.Configuration["Postgresql:ConnectionString"];
                    IDataAccess dataAccess = new PgSQLDataAccess(connectionString);
                    services.AddSingleton(dataAccess);
                    services.AddSingleton<IAuditTraillib, AuditTraillib>();
                    services.AddSingleton<IAuditLogRepository, AuditLogRepository>();
                    services.AddSingleton<IEmailNotificationPasswordExpiryManager, EmailNotificationPasswordExpiryManager>();
                    services.AddSingleton<IEmailNotificationPasswordExpiryRepository, EmailNotificationPasswordExpiryRepository>();
                    Console.WriteLine(args[0]);
                    if (args[0] == "PasswordExpiry")
                        services.AddHostedService<PasswordExpiryWorker>();
                });
    }
}
