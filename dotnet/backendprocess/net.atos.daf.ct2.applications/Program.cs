using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.translation.repository;
using Identity = net.atos.daf.ct2.identity;

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
                    var connectionString = hostContext.Configuration["ConnectionStrings:ConnectionString"];
                    IDataAccess dataAccess = new PgSQLDataAccess(connectionString);
                    services.AddSingleton(dataAccess);
                    services.AddSingleton<IAuditTraillib, AuditTraillib>();
                    services.AddSingleton<ITranslationRepository, TranslationRepository>();
                    services.AddSingleton<ITranslationManager, TranslationManager>();
                    services.AddSingleton<Identity.IAccountManager, Identity.AccountManager>();
                    services.AddSingleton<IAuditLogRepository, AuditLogRepository>();
                    services.AddSingleton<IAccountManager, AccountManager>();
                    services.AddSingleton<IAccountRepository, AccountRepository>();
                    if (args != null)
                    {
                        if (args[0] == "PasswordExpiry")
                            services.AddHostedService<PasswordExpiryWorker>();
                    }
                    else
                    {
                        services.AddHostedService<NoWorker>();
                    }
                });
    }
}
