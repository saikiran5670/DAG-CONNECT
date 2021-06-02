using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
//using AccountPreference = net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.featureactivationservice.Common;
using net.atos.daf.ct2.featureactivationservice.CustomAttributes;
using net.atos.daf.ct2.subscription;
using net.atos.daf.ct2.subscription.repository;
using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.translation.repository;
using AccountComponent = net.atos.daf.ct2.account;
using Identity = net.atos.daf.ct2.identity;
using IdentitySessionComponent = net.atos.daf.ct2.identitysession;

namespace net.atos.daf.ct2.featureactivationservice
{
    public class Startup
    {
        private readonly string _swaggerBasePath = "featureactivation-data";
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
            services.AddTransient<IDataAccess, PgSQLDataAccess>((ctx) =>
            {
                return new PgSQLDataAccess(connectionString);
            });
            services.Configure<Identity.IdentityJsonConfiguration>(Configuration.GetSection("IdentityConfiguration"));
            services.AddTransient<IAuditTraillib, AuditTraillib>();
            services.AddTransient<IAuditLogRepository, AuditLogRepository>();
            services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
            services.AddTransient<ISubscriptionManager, SubscriptionManager>();
            services.AddTransient<IdentitySessionComponent.IAccountSessionManager, IdentitySessionComponent.AccountSessionManager>();
            services.AddTransient<IdentitySessionComponent.IAccountTokenManager, IdentitySessionComponent.AccountTokenManager>();
            services.AddTransient<IdentitySessionComponent.repository.IAccountSessionRepository, IdentitySessionComponent.repository.AccountSessionRepository>();
            services.AddTransient<IdentitySessionComponent.repository.IAccountTokenRepository, IdentitySessionComponent.repository.AccountTokenRepository>();

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
            });

            services.AddSingleton<IAuthorizationHandler, AuthorizeHandler>();

            services.AddTransient<Identity.IAccountManager, Identity.AccountManager>();
            services.AddTransient<Identity.ITokenManager, Identity.TokenManager>();
            services.AddTransient<Identity.IAccountAuthenticator, Identity.AccountAuthenticator>();
            services.AddTransient<AccountComponent.IAccountIdentityManager, AccountComponent.AccountIdentityManager>();
            //services.AddTransient<AccountPreference.IPreferenceManager, AccountPreference.PreferenceManager>();
            //services.AddTransient<AccountPreference.IAccountPreferenceRepository, AccountPreference.AccountPreferenceRepository>();
            services.AddTransient<AccountComponent.IAccountRepository, AccountComponent.AccountRepository>();
            services.AddTransient<AccountComponent.IAccountManager, AccountComponent.AccountManager>();
            services.AddTransient<ITranslationRepository, TranslationRepository>();
            services.AddTransient<ITranslationManager, TranslationManager>();
            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(new ProducesAttribute("application/json"));
            //});
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Feature Activation Data Service", Version = "v1" });
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
    }
}
