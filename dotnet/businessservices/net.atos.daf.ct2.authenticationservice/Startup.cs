using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using AccountComponent = net.atos.daf.ct2.account;
using IdentitySessionComponent = net.atos.daf.ct2.identitysession;
using Identity = net.atos.daf.ct2.identity;
using AccountPreferenceComponent = net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.translation.repository;
using net.atos.daf.ct2.translation;

namespace net.atos.daf.ct2.authenticationservice
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
            var connectionString = Configuration.GetConnectionString("ConnectionString");
            IDataAccess dataAccess = new PgSQLDataAccess(connectionString);
            // Identity configuration
            services.AddSingleton(dataAccess);
            services.Configure<Identity.IdentityJsonConfiguration>(Configuration.GetSection("IdentityConfiguration")); 
            
            services.AddTransient<IAuditLogRepository,AuditLogRepository>();
            services.AddTransient<IAuditTraillib,AuditTraillib>();

            services.AddTransient<Identity.IAccountManager,Identity.AccountManager>();
            services.AddTransient<Identity.ITokenManager,Identity.TokenManager>();
            services.AddTransient<Identity.IAccountAuthenticator,Identity.AccountAuthenticator>();
            
            services.AddTransient<AccountComponent.IAccountIdentityManager,AccountComponent.AccountIdentityManager>();
            services.AddTransient<IdentitySessionComponent.IAccountSessionManager, IdentitySessionComponent.AccountSessionManager>();
            services.AddTransient<IdentitySessionComponent.repository.IAccountSessionRepository, IdentitySessionComponent.repository.AccountSessionRepository>();
            services.AddTransient<IdentitySessionComponent.IAccountTokenManager, IdentitySessionComponent.AccountTokenManager>();
            services.AddTransient<IdentitySessionComponent.repository.IAccountTokenRepository, IdentitySessionComponent.repository.AccountTokenRepository>();

            services.AddTransient<AccountPreferenceComponent.IPreferenceManager,AccountPreferenceComponent.PreferenceManager>();
            services.AddTransient<AccountPreferenceComponent.IAccountPreferenceRepository, AccountPreferenceComponent.AccountPreferenceRepository>();
            
            // services.AddTransient<IGroupManager, GroupManager>();
            // services.AddTransient<IGroupRepository, GroupRepository>();
            
            services.AddTransient<AccountComponent.IAccountRepository,AccountComponent.AccountRepository>();
            services.AddTransient<AccountComponent.IAccountManager,AccountComponent.AccountManager>();
            services.AddTransient<ITranslationRepository, TranslationRepository>();
            services.AddTransient<ITranslationManager, TranslationManager>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Added for turn off caching of signature providers to avoid memory leak IDP
            CryptoProviderFactory.DefaultCacheSignatureProviders =false;

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
                                                  .RequireCors("AllowAll");;
                endpoints.MapGrpcService<AuthenticationService>().EnableGrpcWeb()
                                                  .RequireCors("AllowAll");;

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
