using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.rfms;
using net.atos.daf.ct2.rfms.repository;
using net.atos.daf.ct2.vehicle.repository;

namespace net.atos.daf.ct2.rfmsdataservice
{


    public class Startup
    {

        private readonly string swaggerBasePath = "rfms-data";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    return CustomErrorResponse(actionContext);
                };
            });
            var connectionString = Configuration.GetConnectionString("ConnectionString");
            var DataMartconnectionString = Configuration.GetConnectionString("DataMartConnectionString");
            services.AddTransient<IDataAccess, PgSQLDataAccess>((ctx) =>
            {
                return new PgSQLDataAccess(connectionString);
            });
            services.AddTransient<IDataMartDataAccess, PgSQLDataMartDataAccess>((ctx) =>
            {
                return new PgSQLDataMartDataAccess(DataMartconnectionString);
            });

            services.AddTransient<IAuditTraillib, AuditTraillib>();
            services.AddTransient<IRfmsRepository, RfmsRepository>();
            services.AddTransient<IRfmsManager, RfmsManager>();
            services.AddTransient<IVehicleRepository, VehicleRepository>();


            //   services.AddSingleton<IPostConfigureOptions<BasicAuthenticationOptions>, BasicAuthenticationPostConfigureOptions>();
            // services.AddTransient<IBasicAuthenticationService, BasicAuthenticationService>();

            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(new ProducesAttribute("application/json"));
            //});

            //services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
            //.AddBasic<BasicAuthenticationService>(options =>
            //{
            //    options.ApplicationName = "DAFCT2.0";
            //});

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(
            //        AccessPolicies.MainAccessPolicy,
            //        policy => policy.RequireAuthenticatedUser()
            //                        .Requirements.Add(new AuthorizeRequirement(AccessPolicies.MainAccessPolicy)));
            //    options.AddPolicy(
            //        AccessPolicies.MainMileageAccessPolicy,
            //        policy => policy.RequireAuthenticatedUser()
            //                        .Requirements.Add(new AuthorizeRequirement(AccessPolicies.MainMileageAccessPolicy)));
            //    options.AddPolicy(
            //       AccessPolicies.MainNamelistAccessPolicy,
            //       policy => policy.RequireAuthenticatedUser()
            //                       .Requirements.Add(new AuthorizeRequirement(AccessPolicies.MainNamelistAccessPolicy)));
            //});

            // services.AddSingleton<IAuthorizationHandler, AuthorizeHandler>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vehicle Data Service", Version = "v1" });
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

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger(c =>
            {
                c.RouteTemplate = swaggerBasePath + "/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{swaggerBasePath}/swagger/v1/swagger.json", $"APP API - v1");
                c.RoutePrefix = $"{swaggerBasePath}/swagger";
            });

        }

        private BadRequestObjectResult CustomErrorResponse(ActionContext actionContext)
        {
            return new BadRequestObjectResult(actionContext.ModelState
            .Where(modelError => modelError.Value.Errors.Count > 0)
            .Select(modelError => string.Empty).FirstOrDefault());
        }

    }
}

