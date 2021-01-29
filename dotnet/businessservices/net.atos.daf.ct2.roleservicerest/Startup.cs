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
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.role.repository; 
using net.atos.daf.ct2.role;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.repository;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.audit;

namespace net.atos.daf.ct2.roleservicerest
{
    public class Startup
    {
        
        private readonly string swaggerBasePath = "role";
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

            services.AddTransient<IRoleManagement,RoleManagement>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IFeatureManager,FeatureManager>();
            services.AddTransient<IFeatureRepository,FeatureRepository>();
            
            services.AddTransient<IAuditLogRepository,AuditLogRepository>();
            services.AddTransient<IAuditTraillib,AuditTraillib>();

            // Identity configuration
            services.AddSingleton(dataAccess);

            services.AddCors(c =>  
            {  
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());  
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Role Service", Version = "v1" });
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

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
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
