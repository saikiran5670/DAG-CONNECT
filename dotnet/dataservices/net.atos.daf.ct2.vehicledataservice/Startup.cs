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
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.organization.repository;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.audit.repository;  
using net.atos.daf.ct2.audit;
using AccountComponent = net.atos.daf.ct2.account;
using Identity = net.atos.daf.ct2.identity;
using AccountPreference = net.atos.daf.ct2.accountpreference;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using net.atos.daf.ct2.group;
using Subscription = net.atos.daf.ct2.subscription;
using net.atos.daf.ct2.subscription.repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using net.atos.daf.ct2.vehicledataservice.CustomAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using IdentitySessionComponent = net.atos.daf.ct2.identitysession;

namespace net.atos.daf.ct2.vehicledataservice
{  
    public class Startup
    {
       
        private readonly string swaggerBasePath = "vehicle-data";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
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
            IDataAccess dataAccess = new PgSQLDataAccess(connectionString);
            IDataMartDataAccess dataMartdataAccess = new PgSQLDataMartDataAccess(DataMartconnectionString);
            services.AddSingleton(dataMartdataAccess);
            services.AddSingleton(dataAccess);

            services.AddTransient<IAuditTraillib,AuditTraillib>(); 
            services.AddTransient<IAuditLogRepository, AuditLogRepository>(); 
            services.AddTransient<IVehicleManager, VehicleManager>();
            services.AddTransient<IVehicleRepository, VehicleRepository>();
            services.AddTransient<IOrganizationManager,OrganizationManager>();
            services.AddTransient<IOrganizationRepository, OrganizationRepository>();
            services.Configure<Identity.IdentityJsonConfiguration>(Configuration.GetSection("IdentityConfiguration")); 
            services.AddTransient<Identity.IAccountManager,Identity.AccountManager>();
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


            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(new ProducesAttribute("application/json"));
            //});

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;

                RSA rsa = RSA.Create();
                rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(Configuration["IdentityConfiguration:RsaPublicKey"]), out _);
                SecurityKey key = new RsaSecurityKey(rsa)
                {
                    CryptoProviderFactory = new CryptoProviderFactory()
                    {
                        CacheSignatureProviders = false
                    }
                };
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["IdentityConfiguration:Issuer"],
                    IssuerSigningKey = key,
                    CryptoProviderFactory = new CryptoProviderFactory()
                    {
                        CacheSignatureProviders = false
                    }
                };

                x.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = context =>
                    {
                        context.HttpContext.User = context.Principal;
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    AccessPolicies.MainAccessPolicy,
                    policy => policy.RequireAuthenticatedUser()
                                    .Requirements.Add(new AuthorizeRequirement(AccessPolicies.MainAccessPolicy)));
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
