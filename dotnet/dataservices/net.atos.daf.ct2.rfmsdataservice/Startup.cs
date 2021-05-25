using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.rfmsdataservice
{
    public class Startup
    {

        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
            public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .ConfigureApiBehaviorOptions(options => {
                options.InvalidModelStateResponseFactory = actionContext => {
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

            services.AddTransient<IAuditTraillib,AuditTraillib>(); 
            services.AddTransient<IAuditLogRepository, AuditLogRepository>(); 
            services.AddTransient<IVehicleManager, VehicleManager>();
            services.AddTransient<IVehicleRepository, VehicleRepository>();
            services.AddTransient<IOrganizationManager,OrganizationManager>();
            services.AddTransient<IOrganizationRepository, OrganizationRepository>();
            services.Configure<Identity.IdentityJsonConfiguration>(Configuration.GetSection("IdentityConfiguration")); 
            services.AddTransient<Identity.IAccountManager,Identity.AccountManager>();
            services.AddTransient<IAccountManager, AccountManager>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<Identity.ITokenManager,Identity.TokenManager>();
            services.AddTransient<Identity.IAccountAuthenticator,Identity.AccountAuthenticator>();
            services.AddTransient<IGroupManager,GroupManager>();
            services.AddTransient<IGroupRepository, GroupRepository>();
            services.AddTransient<Subscription.ISubscriptionManager, Subscription.SubscriptionManager>();
            services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();

            services.AddTransient<AccountComponent.IAccountIdentityManager,AccountComponent.AccountIdentityManager>();
            
            services.AddTransient<AccountPreference.IPreferenceManager,AccountPreference.PreferenceManager>();
            services.AddTransient<AccountPreference.IAccountPreferenceRepository, AccountPreference.AccountPreferenceRepository>();
            
            services.AddTransient<AccountComponent.IAccountRepository,AccountComponent.AccountRepository>();
            services.AddTransient<AccountComponent.IAccountManager,AccountComponent.AccountManager>();
            services.AddTransient<IdentitySessionComponent.IAccountSessionManager, IdentitySessionComponent.AccountSessionManager>();
            services.AddTransient<IdentitySessionComponent.IAccountTokenManager, IdentitySessionComponent.AccountTokenManager>();
            services.AddTransient<IdentitySessionComponent.repository.IAccountSessionRepository, IdentitySessionComponent.repository.AccountSessionRepository>();
            services.AddTransient<IdentitySessionComponent.repository.IAccountTokenRepository, IdentitySessionComponent.repository.AccountTokenRepository>();
            services.AddTransient<ITranslationRepository, TranslationRepository>();
            services.AddTransient<ITranslationManager, TranslationManager>();

            services.AddSingleton<IPostConfigureOptions<BasicAuthenticationOptions>, BasicAuthenticationPostConfigureOptions>();
            services.AddTransient<IBasicAuthenticationService, BasicAuthenticationService>();

            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(new ProducesAttribute("application/json"));
            //});

            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
            .AddBasic<BasicAuthenticationService>(options =>
            {
                options.ApplicationName = "DAFCT2.0";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    AccessPolicies.MainAccessPolicy,
                    policy => policy.RequireAuthenticatedUser()
                                    .Requirements.Add(new AuthorizeRequirement(AccessPolicies.MainAccessPolicy)));
                options.AddPolicy(
                    AccessPolicies.MainMileageAccessPolicy,
                    policy => policy.RequireAuthenticatedUser()
                                    .Requirements.Add(new AuthorizeRequirement(AccessPolicies.MainMileageAccessPolicy)));
                options.AddPolicy(
                   AccessPolicies.MainNamelistAccessPolicy,
                   policy => policy.RequireAuthenticatedUser()
                                   .Requirements.Add(new AuthorizeRequirement(AccessPolicies.MainNamelistAccessPolicy)));
            });

            services.AddSingleton<IAuthorizationHandler, AuthorizeHandler>();

            services.AddSwaggerGen(c =>
            {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vehicle Data Service", Version = "v1" });
            });

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
