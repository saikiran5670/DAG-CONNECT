using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.notificationengine;
using net.atos.daf.ct2.notificationengine.repository;
using net.atos.daf.ct2.notificationservice.services;
using net.atos.daf.ct2.notification;
using net.atos.daf.ct2.notification.repository;
using net.atos.daf.ct2.translation.repository;
using net.atos.daf.ct2.translation;

namespace net.atos.daf.ct2.notificationservice
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("ConnectionString");
            string dataMartconnectionString = Configuration.GetConnectionString("DataMartConnectionString");
            services.AddTransient<IDataAccess, PgSQLDataAccess>((ctx) =>
            {
                return new PgSQLDataAccess(connectionString);
            });
            services.AddTransient<IDataMartDataAccess, PgSQLDataMartDataAccess>((ctx) =>
            {
                return new PgSQLDataMartDataAccess(dataMartconnectionString);
            });

            services.AddTransient<ITripAlertManager, TripAlertManager>();
            services.AddTransient<ITripAlertRepository, TripAlertRepository>();
            services.AddTransient<INotificationIdentifierManager, NotificationIdentifierManager>();
            services.AddTransient<INotificationIdentifierRepository, NotificationIdentifierRepository>();
            services.AddTransient<IEmailNotificationManager, EmailNotificationManager>();
            services.AddTransient<IEmailRepository, EmailRepository>();
            services.AddTransient<ITranslationManager, TranslationManager>();
            services.AddTransient<ITranslationRepository, TranslationRepository>();

            services.AddGrpc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<PushNotificationManagementService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
