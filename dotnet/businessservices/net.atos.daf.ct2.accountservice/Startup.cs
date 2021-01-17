using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.group;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using Identity = net.atos.daf.ct2.identity;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;

namespace net.atos.daf.ct2.accountservice
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
            var connectionString = Configuration.GetConnectionString("ConnectionStringHardCode");
            IDataAccess dataAccess = new PgSQLDataAccess(connectionString);
            // Identity configuration
            services.AddSingleton(dataAccess);
            services.Configure<Identity.IdentityJsonConfiguration>(Configuration.GetSection("IdentityConfiguration")); 

            services.AddTransient<IAuditLogRepository,AuditLogRepository>();
            services.AddTransient<IAuditTraillib,AuditTraillib>();

            services.AddTransient<Identity.IAccountManager,Identity.AccountManager>();
            
            services.AddTransient<IGroupManager, GroupManager>();
            services.AddTransient<IGroupRepository, GroupRepository>();
            

            services.AddTransient<IAccountRepository,AccountRepository>();
            services.AddTransient<IAccountManager,AccountManager>();            
             services.AddTransient<IVehicleRepository, VehicleRepository>(); 
            services.AddTransient<IVehicleManager,VehicleManager>();

            //services.AddTransient<IGroupManager, GroupManager>();
            //services.AddTransient<IPreferenceManager, PreferenceManager>();
            services.AddTransient<IAccountPreferenceRepository, AccountPreferenceRepository>();
            services.AddTransient<IPreferenceManager, PreferenceManager>();
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
                endpoints.MapGrpcService<AccountManagementService>();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
