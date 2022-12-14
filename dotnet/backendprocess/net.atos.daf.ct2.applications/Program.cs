using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.driver;
using net.atos.daf.ct2.map;
using net.atos.daf.ct2.map.repository;
using net.atos.daf.ct2.notification;
using net.atos.daf.ct2.notification.repository;
using net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reports.repository;
using net.atos.daf.ct2.reportscheduler;
using net.atos.daf.ct2.reportscheduler.report;
using net.atos.daf.ct2.reportscheduler.repository;
using net.atos.daf.ct2.template;
using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.translation.repository;
using net.atos.daf.ct2.unitconversion;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.visibility;
using net.atos.daf.ct2.visibility.repository;
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
                    services.AddMemoryCache();
                    services.AddSingleton<IAuditTraillib, AuditTraillib>();
                    services.AddSingleton<ITranslationRepository, TranslationRepository>();
                    services.AddSingleton<ITranslationManager, TranslationManager>();
                    services.AddSingleton<IAuditLogRepository, AuditLogRepository>();
                    if (args != null)
                    {
                        if (args[0] == "PasswordExpiry")
                        {
                            string dataMartconnectionString = hostContext.Configuration["ConnectionStrings:DataMartConnectionString"];

                            services.AddSingleton<IDataMartDataAccess, PgSQLDataMartDataAccess>((ctx) =>
                            {
                                return new PgSQLDataMartDataAccess(dataMartconnectionString);
                            });
                            services.AddSingleton<Identity.IAccountManager, Identity.AccountManager>();
                            services.AddSingleton<IDriverRepository, DriverRepository>();
                            services.AddSingleton<IDriverManager, DriverManager>();
                            services.AddSingleton<IAccountManager, AccountManager>();
                            services.AddSingleton<IAccountRepository, AccountRepository>();
                            services.AddTransient<IVehicleManager, VehicleManager>();
                            services.AddTransient<IVehicleRepository, VehicleRepository>();
                            services.AddTransient<ITemplateManager, TemplateManager>();
                            services.AddHostedService<PasswordExpiryWorker>();
                        }
                        else if (args[0] == "ReportCreationScheduler")
                        {
                            string dataMartconnectionString = hostContext.Configuration["ConnectionStrings:DataMartConnectionString"];

                            services.AddSingleton<IDataMartDataAccess, PgSQLDataMartDataAccess>((ctx) =>
                            {
                                return new PgSQLDataMartDataAccess(dataMartconnectionString);
                            });
                            services.AddSingleton<IReportCreationSchedulerManager, ReportCreationSchedulerManager>();
                            services.AddSingleton<IReportSchedulerManager, ReportSchedulerManager>();
                            services.AddSingleton<IReportSchedulerRepository, ReportSchedulerRepository>();
                            services.AddTransient<IReportCreator, ReportCreator>();
                            services.AddTransient<IReportManager, ReportManager>();
                            services.AddTransient<IReportRepository, ReportRepository>();
                            services.AddTransient<IVisibilityManager, VisibilityManager>();
                            services.AddTransient<IVisibilityRepository, VisibilityRepository>();
                            services.AddTransient<IVehicleManager, VehicleManager>();
                            services.AddTransient<IVehicleRepository, VehicleRepository>();
                            services.AddTransient<ITemplateManager, TemplateManager>();
                            services.AddTransient<IUnitConversionManager, UnitConversionManager>();
                            services.AddTransient<IUnitManager, UnitManager>();
                            services.AddTransient<IMapRepository, MapRepository>();
                            services.AddTransient<IMapManager, MapManager>();
                            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
                            services.AddHostedService<ReportCreationSchedulerWorker>();
                        }
                        else if (args[0] == "ReportEmailScheduler")
                        {
                            string dataMartconnectionString = hostContext.Configuration["ConnectionStrings:DataMartConnectionString"];

                            services.AddSingleton<IDataMartDataAccess, PgSQLDataMartDataAccess>((ctx) =>
                            {
                                return new PgSQLDataMartDataAccess(dataMartconnectionString);
                            });

                            services.AddSingleton<IReportSchedulerManager, ReportSchedulerManager>();
                            services.AddSingleton<IReportSchedulerRepository, ReportSchedulerRepository>();
                            services.AddTransient<IVisibilityManager, VisibilityManager>();
                            services.AddTransient<IVisibilityRepository, VisibilityRepository>();
                            services.AddTransient<IVehicleManager, VehicleManager>();
                            services.AddTransient<IVehicleRepository, VehicleRepository>();
                            services.AddSingleton<IReportEmailSchedulerManager, ReportEmailSchedulerManager>();
                            services.AddTransient<IEmailNotificationManager, EmailNotificationManager>();
                            services.AddTransient<IEmailRepository, EmailRepository>();
                            services.AddHostedService<ReportEmailSchedulerWorker>();

                        }
                    }
                    else
                    {
                        services.AddHostedService<NoWorker>();
                    }
                });
    }
}
