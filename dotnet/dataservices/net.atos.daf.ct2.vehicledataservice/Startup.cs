using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.driver;
using net.atos.daf.ct2.group;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.organization.repository;
using net.atos.daf.ct2.subscription.repository;
using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.translation.repository;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.vehicledataservice.Common;
using net.atos.daf.ct2.vehicledataservice.CustomAttributes;
using AccountComponent = net.atos.daf.ct2.account;
using AccountPreference = net.atos.daf.ct2.accountpreference;
using Identity = net.atos.daf.ct2.identity;
using IdentitySessionComponent = net.atos.daf.ct2.identitysession;
using Subscription = net.atos.daf.ct2.subscription;

namespace net.atos.daf.ct2.vehicledataservice
{
    public class Startup
    {
        private readonly string _swaggerBasePath = "vehicle-data";
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
            services.AddMemoryCache();
            services.AddTransient<IAuditTraillib, AuditTraillib>();
            services.AddTransient<IAuditLogRepository, AuditLogRepository>();
            services.AddTransient<IVehicleManager, VehicleManager>();
            services.AddTransient<IVehicleRepository, VehicleRepository>();
            services.AddTransient<IOrganizationManager, OrganizationManager>();
            services.AddTransient<IOrganizationRepository, OrganizationRepository>();
            services.Configure<Identity.IdentityJsonConfiguration>(Configuration.GetSection("IdentityConfiguration"));
            services.AddTransient<Identity.IAccountManager, Identity.AccountManager>();
            services.AddTransient<IAccountManager, AccountManager>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<Identity.ITokenManager, Identity.TokenManager>();
            services.AddTransient<Identity.IAccountAuthenticator, Identity.AccountAuthenticator>();
            services.AddTransient<IGroupManager, GroupManager>();
            services.AddTransient<IGroupRepository, GroupRepository>();
            services.AddTransient<Subscription.ISubscriptionManager, Subscription.SubscriptionManager>();
            services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();

            services.AddTransient<AccountComponent.IAccountIdentityManager, AccountComponent.AccountIdentityManager>();

            services.AddTransient<AccountPreference.IPreferenceManager, AccountPreference.PreferenceManager>();
            services.AddTransient<AccountPreference.IAccountPreferenceRepository, AccountPreference.AccountPreferenceRepository>();

            services.AddTransient<AccountComponent.IAccountRepository, AccountComponent.AccountRepository>();
            services.AddTransient<AccountComponent.IAccountManager, AccountComponent.AccountManager>();
            services.AddTransient<IdentitySessionComponent.IAccountSessionManager, IdentitySessionComponent.AccountSessionManager>();
            services.AddTransient<IdentitySessionComponent.IAccountTokenManager, IdentitySessionComponent.AccountTokenManager>();
            services.AddTransient<IdentitySessionComponent.repository.IAccountSessionRepository, IdentitySessionComponent.repository.AccountSessionRepository>();
            services.AddTransient<IdentitySessionComponent.repository.IAccountTokenRepository, IdentitySessionComponent.repository.AccountTokenRepository>();
            services.AddTransient<ITranslationRepository, TranslationRepository>();
            services.AddTransient<ITranslationManager, TranslationManager>();
            services.AddTransient<IDriverRepository, DriverRepository>();
            services.AddTransient<IDriverManager, DriverManager>();

            services.AddSingleton<IPostConfigureOptions<BasicAuthenticationOptions>, BasicAuthenticationPostConfigureOptions>();
            services.AddTransient<IBasicAuthenticationService, BasicAuthenticationService>();

            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(new ProducesAttribute("application/json"));
            //});

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
                c.RouteTemplate = _swaggerBasePath + "/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{_swaggerBasePath}/swagger/v1/swagger.json", $"APP API - v1");
                c.RoutePrefix = $"{_swaggerBasePath}/swagger";
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
