using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.reportschedulerservice.Services;
using net.atos.daf.ct2.reportscheduler;
using net.atos.daf.ct2.reportscheduler.repository;
using net.atos.daf.ct2.visibility.repository;
using net.atos.daf.ct2.visibility;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.notification;
using net.atos.daf.ct2.notification.repository;
using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.translation.repository;

namespace net.atos.daf.ct2.reportschedulerservice
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));

            string connectionString = Configuration.GetConnectionString("ConnectionString");
            services.AddTransient<IDataAccess, PgSQLDataAccess>((ctx) =>
            {
                return new PgSQLDataAccess(connectionString);
            });
            services.AddTransient<IReportSchedulerManager, ReportSchedulerManager>();
            services.AddTransient<IReportSchedulerRepository, ReportSchedulerRepository>();
            services.AddTransient<IVisibilityRepository, VisibilityRepository>();
            services.AddTransient<IVisibilityManager, VisibilityManager>();
            services.AddTransient<IAuditLogRepository, AuditLogRepository>();
            services.AddTransient<IAuditTraillib, AuditTraillib>();
            services.AddTransient<IEmailRepository, EmailRepository>();
            services.AddTransient<IEmailNotificationManager, EmailNotificationManager>();
            services.AddTransient<ITranslationRepository, TranslationRepository>();
            services.AddTransient<ITranslationManager, TranslationManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseGrpcWeb();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ReportSchedulerManagementService>().EnableGrpcWeb()
                                                  .RequireCors("AllowAll");

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
