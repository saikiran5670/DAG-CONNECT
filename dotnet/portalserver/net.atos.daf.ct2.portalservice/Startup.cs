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

using net.atos.daf.ct2.authenticationservice;
using net.atos.daf.ct2.accountservice;
using net.atos.daf.ct2.packageservice;

namespace net.atos.daf.ct2.portalservice
{
    public class Startup
    {
        private readonly string swaggerBasePath = "portalservice";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authservice = Configuration["ServiceConfiguration:authservice"];
            var accountservice = Configuration["ServiceConfiguration:accountservice"];
            var packageservice = Configuration["ServiceConfiguration:packageservice"];
            // We are enforcing to call Insercure service             
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            services.AddControllers();
            services.AddGrpcClient<Greeter.GreeterClient>(o =>
            {
                o.Address = new Uri(authservice);
            }); 
            services.AddGrpcClient<AuthService.AuthServiceClient>(o =>
            {
                o.Address = new Uri(authservice);
            });   
            services.AddGrpcClient<AccountService.AccountServiceClient>(o =>
            {
                o.Address = new Uri(accountservice);
            });
            services.AddGrpcClient<PackageService.PackageServiceClient>(o =>
            {
                o.Address = new Uri(packageservice);
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Portal Service", Version = "v1" });
            });
            services.AddCors(c =>
            {
                //This need to be change to orgin specific on UAT and prod
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
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
            //This need to be change to orgin specific on UAT and prod
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
