using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.organization.repository;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.audit.repository;  
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using net.atos.daf.ct2.group;
using AccountComponent = net.atos.daf.ct2.account;
using Identity = net.atos.daf.ct2.identity;
using AccountPreference = net.atos.daf.ct2.accountpreference;
using Subscription=net.atos.daf.ct2.subscription;
using net.atos.daf.ct2.subscription.repository;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;

namespace net.atos.daf.ct2.customerdataservice
{
    public class Startup
    {
        private readonly string swaggerBasePath = "customer-data";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
             services.AddControllers();
           var connectionString = Configuration.GetConnectionString("ConnectionString");
            IDataAccess dataAccess = new PgSQLDataAccess(connectionString);
            
            //services.AddControllers();
           // var connectionString="Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y97;Ssl Mode=Require;";
           // IDataAccess dataAccess = new PgSQLDataAccess(connectionString);           
           services.Configure<Identity.IdentityJsonConfiguration>(Configuration.GetSection("IdentityConfiguration")); 
           
            services.AddSingleton(dataAccess); 
            services.AddTransient<IAuditTraillib,AuditTraillib>(); 
            services.AddTransient<IAuditLogRepository, AuditLogRepository>(); 
            services.AddTransient<IOrganizationManager,OrganizationManager>();
            services.AddTransient<IOrganizationRepository, OrganizationRepository>();
            services.AddTransient<IPreferenceManager,PreferenceManager>();
            services.AddTransient<IAccountPreferenceRepository, AccountPreferenceRepository>();
            services.AddTransient<IVehicleRepository, VehicleRepository>();
            services.AddTransient<IVehicleManager,VehicleManager>();
            services.AddTransient<Identity.IAccountManager,Identity.AccountManager>();
            services.AddTransient<Identity.ITokenManager,Identity.TokenManager>();
            services.AddTransient<Identity.IAccountAuthenticator,Identity.AccountAuthenticator>();            
            services.AddTransient<AccountComponent.IAccountIdentityManager,AccountComponent.AccountIdentityManager>();            
            services.AddTransient<AccountPreference.IPreferenceManager,AccountPreference.PreferenceManager>();
            services.AddTransient<AccountPreference.IAccountPreferenceRepository, AccountPreference.AccountPreferenceRepository>();
            services.AddTransient<AccountComponent.IAccountRepository,AccountComponent.AccountRepository>();
            services.AddTransient<AccountComponent.IAccountManager,AccountComponent.AccountManager>();   
            services.AddTransient<Subscription.ISubscriptionManager,Subscription.SubscriptionManager>(); 
            services.AddTransient<ISubscriptionRepository,SubscriptionRepository>();  
            services.AddTransient<IGroupManager,GroupManager>();
            services.AddTransient<IGroupRepository, GroupRepository>();          
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            services.AddSwaggerGen(c =>
            {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer Data Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger(c =>
            {
                c.RouteTemplate = swaggerBasePath+"/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{swaggerBasePath}/swagger/v1/swagger.json", $"APP API - v1");
                c.RoutePrefix = $"{swaggerBasePath}/swagger";
            });
        }
    }
}
