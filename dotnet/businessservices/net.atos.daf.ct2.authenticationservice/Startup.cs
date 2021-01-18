using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using AccountComponent = net.atos.daf.ct2.account;
using Identity = net.atos.daf.ct2.identity;
using AccountPreferenceComponent = net.atos.daf.ct2.accountpreference;

namespace net.atos.daf.ct2.authenticationservice
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
            services.AddGrpc();
            
            var connectionString = Configuration.GetConnectionString("ConnectionString");
            IDataAccess dataAccess = new PgSQLDataAccess(connectionString);
            // Identity configuration
            services.AddSingleton(dataAccess);
            services.Configure<Identity.IdentityJsonConfiguration>(Configuration.GetSection("IdentityConfiguration")); 
            
            services.AddTransient<IAuditLogRepository,AuditLogRepository>();
            services.AddTransient<IAuditTraillib,AuditTraillib>();

            services.AddTransient<Identity.IAccountManager,Identity.AccountManager>();
            services.AddTransient<Identity.ITokenManager,Identity.TokenManager>();
            services.AddTransient<Identity.IAccountAuthenticator,Identity.AccountAuthenticator>();
            
            services.AddTransient<AccountComponent.IAccountIdentityManager,AccountComponent.AccountIdentityManager>();
            
            services.AddTransient<AccountPreferenceComponent.IPreferenceManager,AccountPreferenceComponent.PreferenceManager>();
            services.AddTransient<AccountPreferenceComponent.IAccountPreferenceRepository, AccountPreferenceComponent.AccountPreferenceRepository>();
            
            // services.AddTransient<IGroupManager, GroupManager>();
            // services.AddTransient<IGroupRepository, GroupRepository>();
            
            services.AddTransient<AccountComponent.IAccountRepository,AccountComponent.AccountRepository>();
            services.AddTransient<AccountComponent.IAccountManager,AccountComponent.AccountManager>();            
         
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
                endpoints.MapGrpcService<AuthenticationService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
