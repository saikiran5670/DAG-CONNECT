using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.group;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.organization.repository;
using net.atos.daf.ct2.subscription.repository;
using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.translation.repository;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;
using AccountComponent = net.atos.daf.ct2.account;
using AccountPreference = net.atos.daf.ct2.accountpreference;
using Identity = net.atos.daf.ct2.identity;
using IdentitySessionComponent = net.atos.daf.ct2.identitysession;
using Subscription = net.atos.daf.ct2.subscription;
using net.atos.daf.ct2.fmsdataservice.Common;
using net.atos.daf.ct2.driver;
using net.atos.daf.ct2.fmsdataservice.CustomAttributes;

namespace net.atos.daf.ct2.fmsdataservice
{
    public class Startup
    {
        private readonly string _swaggerBasePath = "vehicle";
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
            services.AddTransient<IAuditLogRepository, AuditLogRepository>();

            services.AddAuthentication(BasicAuthenticationDefaults.AUTHENTICATION_SCHEME)
           .AddBasic<BasicAuthenticationService>(options =>
           {
               options.ApplicationName = "DAFCT2.0";
           });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    AccessPolicies.MAIN_ACCESS_POLICY,
                    policy => policy.RequireAuthenticatedUser()
                                    .Requirements.Add(new AuthorizeRequirement(AccessPolicies.MAIN_ACCESS_POLICY)));
                options.AddPolicy(
                    AccessPolicies.MAIN_MILEAGE_ACCESS_POLICY,
                    policy => policy.RequireAuthenticatedUser()
                                    .Requirements.Add(new AuthorizeRequirement(AccessPolicies.MAIN_MILEAGE_ACCESS_POLICY)));
                options.AddPolicy(
                   AccessPolicies.MAIN_NAMELIST_ACCESS_POLICY,
                   policy => policy.RequireAuthenticatedUser()
                                   .Requirements.Add(new AuthorizeRequirement(AccessPolicies.MAIN_NAMELIST_ACCESS_POLICY)));
            });

            services.AddSingleton<IAuthorizationHandler, AuthorizeHandler>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FMS Data Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "net.atos.daf.ct2.fmsdataservice v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
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
