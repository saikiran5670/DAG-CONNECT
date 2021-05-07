using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.geofenceservice;
using net.atos.daf.ct2.poigeofence;
using net.atos.daf.ct2.poigeofence.repository;
using net.atos.daf.ct2.poigeofenceservice.Services;
using net.atos.daf.ct2.poigeofenservice;

namespace net.atos.daf.ct2.poigeofenceservice
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
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));


            string connectionString = Configuration.GetConnectionString("ConnectionString");
            string DataMartconnectionString = Configuration.GetConnectionString("DataMartConnectionString");
            services.AddTransient<IDataAccess, PgSQLDataAccess>((ctx) =>
            {
                return new PgSQLDataAccess(connectionString);
            });
            services.AddTransient<IDataMartDataAccess, PgSQLDataMartDataAccess>((ctx) =>
            {
                return new PgSQLDataMartDataAccess(DataMartconnectionString);
            });
            services.AddTransient<IPoiManager, PoiManager>();
            services.AddTransient<IPoiRepository, PoiRepository>();
            services.AddTransient<ICategoryManager, CategoryManager>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IGeofenceManager, GeofenceManager>();
            services.AddTransient<IGeofenceRepository, GeofenceRepository>();
            services.AddTransient<ILandmarkGroupManager, LandmarkGroupManager>();
            services.AddTransient<ILandmarkgroupRepository, LandmarkgroupRepository>();
            services.AddTransient<ICorridorManger, CorridorManger>();
            services.AddTransient<ICorridorRepository, CorridorRepository>();
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

                //endpoints.MapGrpcService<PoiGeofenceManagementService>().EnableGrpcWeb()
                //                                  .RequireCors("AllowAll");
                endpoints.MapGrpcService<POIManagementService>().EnableGrpcWeb()
                                                  .RequireCors("AllowAll");
                endpoints.MapGrpcService<CategoryManagementService>().EnableGrpcWeb()
                                                  .RequireCors("AllowAll");
                endpoints.MapGrpcService<GeofenceManagementService>().EnableGrpcWeb()
                                                  .RequireCors("AllowAll");
                endpoints.MapGrpcService<GroupManagementService>().EnableGrpcWeb()
                                                  .RequireCors("AllowAll");
                endpoints.MapGrpcService<CorridorManagementService>().EnableGrpcWeb()
                                                  .RequireCors("AllowAll");
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
