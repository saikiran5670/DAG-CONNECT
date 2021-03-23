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
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using net.atos.daf.ct2.accountservice;
using net.atos.daf.ct2.packageservice;
using net.atos.daf.ct2.vehicleservice;
using net.atos.daf.ct2.featureservice;
using net.atos.daf.ct2.translationservice;
using net.atos.daf.ct2.auditservice;
using net.atos.daf.ct2.roleservice;

using net.atos.daf.ct2.organizationservice;
using net.atos.daf.ct2.driverservice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.subscriptionservice;

namespace net.atos.daf.ct2.portalservice
{
    public class Startup
    {
        private readonly string swaggerBasePath = "portalservice";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //gRPC service configuration
            var accountservice = Configuration["ServiceConfiguration:accountservice"];
            var packageservice = Configuration["ServiceConfiguration:packageservice"];
            var vehicleservice = Configuration["ServiceConfiguration:vehicleservice"];
            var translationservice = Configuration["ServiceConfiguration:translationservice"];
            var auditservice = Configuration["ServiceConfiguration:auditservice"];
            var featureservice= Configuration["ServiceConfiguration:featureservice"];
            var roleservice = Configuration["ServiceConfiguration:roleservice"];
            var organizationservice = Configuration["ServiceConfiguration:organizationservice"];
            var driverservice = Configuration["ServiceConfiguration:driverservice"];
            var subscriptionservice = Configuration["ServiceConfiguration:subscriptionservice"];

            //Web Server Configuration
            var isdevelopmentenv = Configuration["WebServerConfiguration:isdevelopmentenv"];
            var cookiesexpireat = Configuration["WebServerConfiguration:cookiesexpireat"];
            var authcookiesexpireat = Configuration["WebServerConfiguration:authcookiesexpireat"];
            var headerstricttransportsecurity = Configuration["WebServerConfiguration:headerstricttransportsecurity"];
           // var httpsport = Configuration["WebServerConfiguration:httpsport"];

            // We are enforcing to call Insercure service             
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            services.Configure<PortalCacheConfiguration>(Configuration.GetSection("PortalCacheConfiguration"));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = "Account";
                    options.Cookie.HttpOnly = false;
                    //options.Cookie.Expiration = TimeSpan.FromMinutes(Convert.ToDouble(cookiesexpireat));
                    //options.Cookie.SecurePolicy = string.IsNullOrEmpty(isdevelopmentenv)? CookieSecurePolicy.Always : Convert.ToBoolean(isdevelopmentenv) ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(string.IsNullOrEmpty(authcookiesexpireat)? 5184000 : Convert.ToDouble(authcookiesexpireat));
                    options.Events = new CookieAuthenticationEvents
                    {                          
                        OnRedirectToLogin = redirectContext =>
                        {
                            redirectContext.HttpContext.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        },
                        OnRedirectToAccessDenied = context => 
                        { 
                            context.Response.StatusCode = 403; 
                            return Task.CompletedTask; 
                        }
                    };                
            });

            /*   services.AddHsts(options =>
               {
                   options.Preload = true;
                   options.IncludeSubDomains = true;
                   options.MaxAge = TimeSpan.FromHours(Convert.ToInt32(headerstricttransportsecurity));
               });
               services.AddHttpsRedirection(options =>
               {
                   options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                   options.HttpsPort = string.IsNullOrEmpty(httpsport)? 443 : Convert.ToInt32(httpsport);
               }); */

            services.AddMemoryCache();

            services.AddControllers();

            services.AddDistributedMemoryCache();

            services.AddScoped<IMemoryCacheExtensions, MemoryCacheExtensions>();
            services.AddScoped<IMemoryCacheProvider, MemoryCacheProvider>();

            services.AddGrpcClient<AccountService.AccountServiceClient>(o =>
            {
                o.Address = new Uri(accountservice);
            });
            services.AddGrpcClient<PackageService.PackageServiceClient>(o =>
            {
                o.Address = new Uri(packageservice);
            });
            services.AddGrpcClient<VehicleService.VehicleServiceClient>(o =>
            {
                o.Address = new Uri(vehicleservice);
            });
          
            services.AddGrpcClient<FeatureService.FeatureServiceClient>(o =>
            {
                o.Address = new Uri(featureservice);
            });
             services.AddGrpcClient<FeatureService.FeatureServiceClient>(o =>
            {
                o.Address = new Uri(featureservice);
            });
            services.AddGrpcClient<RoleService.RoleServiceClient>(o =>
            {
                o.Address = new Uri(roleservice);
            });
            services.AddGrpcClient<OrganizationService.OrganizationServiceClient>(o =>
            {
                o.Address = new Uri(organizationservice);
            });
            services.AddGrpcClient<TranslationService.TranslationServiceClient>(o =>
            {
                o.Address = new Uri(translationservice);
            });
            services.AddGrpcClient<AuditService.AuditServiceClient>(o =>
            {
                o.Address = new Uri(auditservice);
            });
             services.AddGrpcClient<DriverService.DriverServiceClient>(o =>
            {
                o.Address = new Uri(driverservice);
            });
            services.AddGrpcClient<SubscribeGRPCService.SubscribeGRPCServiceClient>(o =>
            {
                o.Address = new Uri(subscriptionservice);
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Portal Service", Version = "v1" });
             });
            services.AddCors(c =>
            {
                //This need to be change to orgin specific on UAT and prod
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           // else
           // {
           //     app.UseHsts();
           // }
            //Web Server Configuration
            var headercachecontrol = Configuration["WebServerConfiguration:headercachecontrol"];
            var headerexpires = Configuration["WebServerConfiguration:headerexpires"];
            var headerpragma = Configuration["WebServerConfiguration:headerpragma"];
            var headerxframeoptions = Configuration["WebServerConfiguration:headerxframeoptions"];
            var headerxxssprotection = Configuration["WebServerConfiguration:headerxxssprotection"];
            var headerstricttransportsecurity = Configuration["WebServerConfiguration:headerstricttransportsecurity"];
            var headeraccesscontrolalloworigin = Configuration["WebServerConfiguration:headeraccesscontrolalloworigin"];
            var headeraccesscontrolallowmethods = Configuration["WebServerConfiguration:headeraccesscontrolallowmethods"];
            var headerAccesscontrolallowheaders = Configuration["WebServerConfiguration:headeraccesscontrolallowheaders"];

            app.Use(async (context, next) =>
            {
                //context.Response.Headers["Cache-Control"] = string.IsNullOrEmpty(headercachecontrol)? "no-cache, no-store, must-revalidate" : headercachecontrol;
                ////context.Response.Headers["Expires"] = string.IsNullOrEmpty(headerexpires) ? "-1" : headerexpires;
                //context.Response.Headers["Pragma"] = string.IsNullOrEmpty(headerpragma) ? "no-cache" : headerpragma;                
                //context.Response.Headers.Add("X-Frame-Options", string.IsNullOrEmpty(headerxframeoptions) ? "DENY" : headerxframeoptions);
                //context.Response.Headers.Add("X-Xss-Protection", string.IsNullOrEmpty(headerxxssprotection) ? "1" : headerxxssprotection);
                ////context.Response.Headers.Add("Content-Security-Policy", "script-src 'self' 'unsafe-eval' 'unsafe-inline'; navigate-to https://www.daf.com; connect-src 'self'; img-src 'self'; style-src 'self' 'unsafe-inline'");
                //context.Response.Headers.Add("Strict-Transport-Security", string.IsNullOrEmpty(headerstricttransportsecurity) ? "31536000" : headerstricttransportsecurity);
                context.Response.Headers.Add("Access-Control-Allow-Origin", string.IsNullOrEmpty(headeraccesscontrolalloworigin) ? "*" : headeraccesscontrolalloworigin);
                //context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
                context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                //context.Response.Headers.Add("Access-Control-Allow-Methods", string.IsNullOrEmpty(headeraccesscontrolallowmethods) ? "GET, POST, PUT, DELETE" : headeraccesscontrolallowmethods);
                //context.Response.Headers.Add("Access-Control-Allow-Headers", string.IsNullOrEmpty(headerAccesscontrolallowheaders) ? "Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With" : headerAccesscontrolallowheaders);
                
                //context.Response.Headers.Remove("X-Powered-By");
                //context.Response.Headers.Remove("Server");
                //context.Response.Headers.Remove("X-AspNet-Version");
                //context.Response.Headers.Remove("X-AspNetMvc-Version");

                await next();
            });

            //app.UseHttpsRedirection();

            app.UseRouting();
            //This need to be change to orgin specific on UAT and prod
            app.UseCors(builder =>
            {
                builder.WithOrigins(string.IsNullOrEmpty(headeraccesscontrolalloworigin) ? "*" : headeraccesscontrolalloworigin);
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            //app.UseCookiePolicy();
            
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
             app.UseSwagger(c =>
            {
                c.RouteTemplate = swaggerBasePath+"/swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{swaggerBasePath}/swagger/v1/swagger.json", $"APP API - v1");
                c.RoutePrefix = $"{swaggerBasePath}/swagger";
            });
        }
    }
}