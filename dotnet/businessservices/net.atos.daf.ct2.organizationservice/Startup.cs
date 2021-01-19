using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.organization.repository;
using net.atos.daf.ct2.organizationservice.Services;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.audit.repository;  
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.vehicle;

namespace net.atos.daf.ct2.organizationservice
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

            string connectionString = Configuration.GetConnectionString("ConnectionString");
            IDataAccess dataAccess = new PgSQLDataAccess(connectionString);
          // var connectionString = Configuration.GetConnectionString("ConnectionString");
          // var connectionString="Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y\\97;Ssl Mode=Require;";
            //IDataAccess dataAccess = new PgSQLDataAccess(connectionString);           
            services.AddSingleton(dataAccess); 
             services.AddTransient<IAuditTraillib,AuditTraillib>(); 
             services.AddTransient<IAuditLogRepository, AuditLogRepository>(); 
            services.AddTransient<IOrganizationManager,OrganizationManager>();
            services.AddTransient<IOrganizationRepository, OrganizationRepository>();
            services.AddTransient<IPreferenceManager,PreferenceManager>();
            services.AddTransient<IAccountPreferenceRepository, AccountPreferenceRepository>();
             services.AddTransient<IVehicleManager,VehicleManager>();
            services.AddTransient<IVehicleRepository, VehicleRepository>();
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
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGrpcService<OrganizationManagementService>();             

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
