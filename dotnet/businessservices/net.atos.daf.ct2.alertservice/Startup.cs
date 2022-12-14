using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.atos.daf.ct2.alert;
using net.atos.daf.ct2.alert.repository;
using net.atos.daf.ct2.alertservice.Services;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.kafkacdc;
using net.atos.daf.ct2.kafkacdc.repository;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.visibility;
using net.atos.daf.ct2.visibility.repository;

namespace net.atos.daf.ct2.alertservice
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
            services.AddGrpc(options =>
            {
                options.MaxReceiveMessageSize = null;
                options.MaxSendMessageSize = null;
                options.ResponseCompressionLevel = CompressionLevel.Optimal;
                options.ResponseCompressionAlgorithm = "gzip";
            });
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));

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
            services.AddMemoryCache();
            services.AddTransient<IAlertManager, AlertManager>();
            services.AddTransient<IAlertRepository, AlertRepository>();
            services.AddTransient<IVisibilityRepository, VisibilityRepository>();
            services.AddTransient<IVisibilityManager, VisibilityManager>();
            services.AddTransient<IAlertMgmAlertCdcManager, AlertMgmAlertCdcManager>();
            services.AddTransient<IAlertMgmAlertCdcRepository, AlertMgmAlertCdcRepository>();
            services.AddTransient<IVehicleManager, VehicleManager>();
            services.AddTransient<IVehicleRepository, VehicleRepository>();
            //services.AddTransient<IAuditLogRepository, AuditLogRepository>();
            //services.AddTransient<IAuditTraillib, AuditTraillib>();
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
                endpoints.MapGrpcService<AlertManagementService>().EnableGrpcWeb()
                                                  .RequireCors("AllowAll");

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
