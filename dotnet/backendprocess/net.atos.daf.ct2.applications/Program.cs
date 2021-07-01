using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
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
                            services.AddTransient<ITemplateManager, TemplateManager>();

                            //services.AddControllersWithViews();
                            //services.AddRazorPages();
                            //services.AddControllers();
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
