using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using AccountComponent = net.atos.daf.ct2.account;
using Identity = net.atos.daf.ct2.identity;
using AccountPreference = net.atos.daf.ct2.accountpreference;
namespace net.atos.daf.ct2.authenticationservicerest
{
    public class Startup
    {
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
            // Identity configuration
            services.AddSingleton(dataAccess);
            services.Configure<Identity.IdentityJsonConfiguration>(Configuration.GetSection("IdentityConfiguration")); 
            
            services.AddTransient<IAuditLogRepository,AuditLogRepository>();
            services.AddTransient<IAuditTraillib,AuditTraillib>();

            services.AddTransient<Identity.IAccountManager,Identity.AccountManager>();
            services.AddTransient<Identity.ITokenManager,Identity.TokenManager>();
            services.AddTransient<Identity.IAccountAuthenticator,Identity.AccountAuthenticator>();
            
            services.AddTransient<AccountComponent.IAccountIdentityManager,AccountComponent.AccountIdentityManager>();
            
            services.AddTransient<AccountPreference.IPreferenceManager,AccountPreference.PreferenceManager>();
            services.AddTransient<AccountPreference.IAccountPreferenceRepository, AccountPreference.AccountPreferenceRepository>();
            
            // services.AddTransient<IGroupManager, GroupManager>();
            // services.AddTransient<IGroupRepository, GroupRepository>();
            
            services.AddTransient<AccountComponent.IAccountRepository,AccountComponent.AccountRepository>();
            services.AddTransient<AccountComponent.IAccountManager,AccountComponent.AccountManager>();            

            services.AddCors(c =>  
            {  
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());  
            });

            services.AddSwaggerGen(c =>
            {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Authentication  Service", Version = "v1" });
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

            app.UseCors(builder => 
            {
                builder.WithOrigins("*");
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });  

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
                        app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
               c.SwaggerEndpoint("/swagger/v1/swagger.json", "Authentication Service");
            });

        }
    }
}
