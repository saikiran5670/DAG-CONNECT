﻿using System;
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
using net.atos.daf.ct2.role.repository; 
using net.atos.daf.ct2.roleservice;
using net.atos.daf.ct2.role;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.repository;

namespace net.atos.daf.ct2.roleservice
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

            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));

            var connectionString = Configuration.GetConnectionString("ConnectionString");
            IDataAccess dataAccess = new PgSQLDataAccess(connectionString);
            services.AddSingleton(dataAccess); 
            services.AddTransient<IRoleManagement,RoleManagement>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IFeatureManager,FeatureManager>();
            services.AddTransient<IFeatureRepository,FeatureRepository>();

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
                endpoints.MapGrpcService<GreeterService>().EnableGrpcWeb()
                                                  .RequireCors("AllowAll");
                endpoints.MapGrpcService<RoleManagementService>().EnableGrpcWeb()
                                                  .RequireCors("AllowAll");

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
