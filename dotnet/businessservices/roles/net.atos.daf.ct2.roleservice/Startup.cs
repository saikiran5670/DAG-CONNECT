using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.rolerepository;
//using DAF.RoleManagementLogic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using net.atos.daf.ct2.role.repository;
using net.atos.daf.ct2.audit;

namespace net.atos.daf.ct2.roleservice
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

            var connectionString = Configuration.GetConnectionString("DevAzure");
            IDataAccess dataAccess = new PgSQLDataAccess(connectionString);
            services.AddSingleton(dataAccess); 
            services.AddTransient<IRoleManagement,RoleManagement>();
            services.AddTransient<IRoleRepository, RoleRepository>();
           // services.AddTransient<IFeature, Feature>();
            //services.AddTransient<IFeatureRepository, FeatureRepository>();
            services.AddTransient<IAuditLog, AuditLog>();
           // services.AddTransient<IAuditLogRepository,AuditLogRepository>();

            services.AddSwaggerGen(c =>
            {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "RoleManagement Service", Version = "v1" });
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

            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
               c.SwaggerEndpoint("/swagger/v1/swagger.json", "RoleManagement Service V1");
            });

        }
    }
}
