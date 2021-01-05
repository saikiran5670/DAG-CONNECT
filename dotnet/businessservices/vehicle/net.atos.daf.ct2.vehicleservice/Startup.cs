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
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.vehiclerepository;
//using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.data;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;

namespace net.atos.daf.ct2.vehicleservice
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
            services.AddTransient<IVehicleManagement,VehicleManagement>();
            services.AddTransient<IVehicleRepository, VehicleRepository>();
            services.AddSwaggerGen(c =>
            {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vehicle Management Service", Version = "v1" });
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
               c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vehicle Management Service V1");
            });
        }
    }
}
