﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.translation.repository;
using net.atos.daf.ct2.translationservice;
using Microsoft.Extensions.Configuration;

namespace net.atos.daf.ct2.translationservice
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
            // Enable CORS for service
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));
            //var connectionString = Configuration.GetConnectionString("ConnectionString");
            var connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y97;Ssl Mode=Require;";
            IDataAccess dataAccess = new PgSQLDataAccess(connectionString);
            // Identity configuration
            services.AddSingleton(dataAccess);
            
            services.AddTransient<ITranslationManager, TranslationManager>();
             services.AddTransient<ITranslationRepository, TranslationRepository>();

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
                endpoints.MapGrpcService<TranslationManagementService>().EnableGrpcWeb()
                                                  .RequireCors("AllowAll");
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
