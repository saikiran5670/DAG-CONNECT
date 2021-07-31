using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using net.atos.daf.ct2.accountservice;
using net.atos.daf.ct2.alertservice;
using net.atos.daf.ct2.auditservice;
using net.atos.daf.ct2.corridorservice;
using net.atos.daf.ct2.driverservice;
using net.atos.daf.ct2.featureservice;
using net.atos.daf.ct2.geofenceservice;
using net.atos.daf.ct2.mapservice;
using net.atos.daf.ct2.organizationservice;
using net.atos.daf.ct2.packageservice;
using net.atos.daf.ct2.poigeofences;
using net.atos.daf.ct2.poiservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.hubs;
using net.atos.daf.ct2.pushnotificationservice;
using net.atos.daf.ct2.reportschedulerservice;
using net.atos.daf.ct2.reportservice;
using net.atos.daf.ct2.roleservice;
using net.atos.daf.ct2.subscriptionservice;
using net.atos.daf.ct2.translationservice;
using net.atos.daf.ct2.vehicleservice;
using net.atos.daf.ct2.dashboardservice;
using net.atos.daf.ct2.notificationservice;

namespace net.atos.daf.ct2.portalservice
{
    public class Startup
    {
        private readonly string _swaggerBasePath = "portalservice";
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //gRPC service configuration
            var accountservice = Configuration["ServiceConfiguration:accountservice"];
            var packageservice = Configuration["ServiceConfiguration:packageservice"];
            var vehicleservice = Configuration["ServiceConfiguration:vehicleservice"];
            var translationservice = Configuration["ServiceConfiguration:translationservice"];
            var auditservice = Configuration["ServiceConfiguration:auditservice"];
            var featureservice = Configuration["ServiceConfiguration:featureservice"];
            var roleservice = Configuration["ServiceConfiguration:roleservice"];
            var organizationservice = Configuration["ServiceConfiguration:organizationservice"];
            var driverservice = Configuration["ServiceConfiguration:driverservice"];
            var subscriptionservice = Configuration["ServiceConfiguration:subscriptionservice"];
            var landmarkservice = Configuration["ServiceConfiguration:landmarkservice"];
            var alertservice = Configuration["ServiceConfiguration:alertservice"];
            var reportservice = Configuration["ServiceConfiguration:reportservice"];
            var mapservice = Configuration["ServiceConfiguration:mapservice"];
            var dashboarservice = Configuration["ServiceConfiguration:dashboardservice"];
            var reportschedulerservice = Configuration["ServiceConfiguration:reportschedulerservice"];
            string notificationservice = Configuration["ServiceConfiguration:notificationservice"];

            //Web Server Configuration
            var isdevelopmentenv = Configuration["WebServerConfiguration:isdevelopmentenv"];
            var cookiesexpireat = Configuration["WebServerConfiguration:cookiesexpireat"];
            var authcookiesexpireat = Configuration["WebServerConfiguration:authcookiesexpireat"];
            var headerstricttransportsecurity = Configuration["WebServerConfiguration:headerstricttransportsecurity"];
            var headeraccesscontrolalloworigin = Configuration["WebServerConfiguration:headeraccesscontrolalloworigin"];
            var headeraccesscontrolallowmethods = Configuration["WebServerConfiguration:headeraccesscontrolallowmethods"];
            var headerAccesscontrolallowheaders = Configuration["WebServerConfiguration:headeraccesscontrolallowheaders"];
            var httpsport = Configuration["WebServerConfiguration:httpsport"];
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // We are enforcing to call Insercure service             
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            services.Configure<PortalCacheConfiguration>(Configuration.GetSection("PortalCacheConfiguration"));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "Account";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = options.Cookie.SecurePolicy = string.IsNullOrEmpty(isdevelopmentenv) || isdevelopmentenv.Contains("Configuration") ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
                if (isdevelopmentenv.Contains("Configuration") || Convert.ToBoolean(isdevelopmentenv))
                {
                    options.Cookie.SameSite = SameSiteMode.None;
                }
                else
                {
                    options.Cookie.SameSite = SameSiteMode.Lax;
                }

                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(string.IsNullOrEmpty(authcookiesexpireat) || authcookiesexpireat.Contains("Configuration") ? 5184000 : Convert.ToDouble(authcookiesexpireat));
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
            services.AddDistributedMemoryCache();

            services.AddDataProtection()
                .SetApplicationName(_swaggerBasePath)
                .PersistKeysToFileSystem(new DirectoryInfo(@"/tmp/keys"));

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(string.IsNullOrEmpty(cookiesexpireat) || cookiesexpireat.Contains("Configuration") ? 5184000 : Convert.ToDouble(cookiesexpireat));
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                if (isdevelopmentenv.Contains("Configuration") || Convert.ToBoolean(isdevelopmentenv))
                {
                    options.Cookie.SameSite = SameSiteMode.None;
                }
                else
                {
                    options.Cookie.SameSite = SameSiteMode.Lax;
                }
                options.Cookie.SecurePolicy = string.IsNullOrEmpty(isdevelopmentenv) || isdevelopmentenv.Contains("Configuration") ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
            });
            services.AddAuthorization(options =>
            {
                //if (Environment.IsDevelopment())  //not working for dev0 environment
                if ((!isdevelopmentenv.Contains("Configuration")) && Convert.ToBoolean(isdevelopmentenv))
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAssertion(_ => true).Build();
                }
            });
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = string.IsNullOrEmpty(headerstricttransportsecurity) || headerstricttransportsecurity.Contains("Configuration") ? TimeSpan.FromHours(31536000) : TimeSpan.FromHours(Convert.ToDouble(headerstricttransportsecurity));
            });
            //services.AddHttpsRedirection(options =>
            //{
            //    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            //    options.HttpsPort = string.IsNullOrEmpty(httpsport) || httpsport.Contains("Configuration") ? 443 : Convert.ToInt32(httpsport);
            //});
            services.AddMemoryCache();
            services.AddControllers();
            services.AddTransient<AuditHelper, AuditHelper>();
            services.AddSingleton<SessionHelper>();
            services.AddTransient<AccountPrivilegeChecker, AccountPrivilegeChecker>();
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
            services.AddGrpcClient<CategoryService.CategoryServiceClient>(o =>
            {
                o.Address = new Uri(landmarkservice);
            });
            services.AddGrpcClient<DriverService.DriverServiceClient>(o =>
            {
                o.Address = new Uri(driverservice);
            });
            services.AddGrpcClient<SubscribeGRPCService.SubscribeGRPCServiceClient>(o =>
            {
                o.Address = new Uri(subscriptionservice);
            });
            services.AddGrpcClient<GeofenceService.GeofenceServiceClient>(o =>
            {
                o.Address = new Uri(landmarkservice);
            });
            services.AddGrpcClient<POIService.POIServiceClient>(o =>
            {
                o.Address = new Uri(landmarkservice);
            });
            services.AddGrpcClient<GroupService.GroupServiceClient>(o =>
            {
                o.Address = new Uri(landmarkservice);
            });
            services.AddGrpcClient<AlertService.AlertServiceClient>(o =>
            {
                o.Address = new Uri(alertservice);
            });
            services.AddGrpcClient<CorridorService.CorridorServiceClient>(o =>
            {
                o.Address = new Uri(landmarkservice);
            });
            services.AddGrpcClient<ReportService.ReportServiceClient>(o =>
            {
                o.Address = new Uri(reportservice);
            });
            services.AddGrpcClient<MapService.MapServiceClient>(o =>
            {
                o.Address = new Uri(mapservice);
            });
            services.AddGrpcClient<ReportSchedulerService.ReportSchedulerServiceClient>(o =>
            {
                o.Address = new Uri(reportschedulerservice);
            });
            services.AddGrpcClient<DashboardService.DashboardServiceClient>(o =>
            {
                o.Address = new Uri(dashboarservice);
            });

            services.AddGrpcClient<PushNotificationService.PushNotificationServiceClient>(o => o.Address = new Uri(notificationservice));
            services.AddGrpcClient<Greeter.GreeterClient>(o => o.Address = new Uri(notificationservice));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Portal Service", Version = "v1" });
            });
            services.AddCors(c =>
            {
                //This need to be change to orgin specific on UAT and prod
                c.AddPolicy("AllowOrigin",
                    options => options.AllowAnyOrigin()
                 );
            });

            services.AddSignalR(options => options.EnableDetailedErrors = true);

            services.AddControllers();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
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
            var headerAccesscontrolallowcredentials = Configuration["WebServerConfiguration:headeraccesscontrolallowcredentials"];
            var isdevelopmentenv = Configuration["WebServerConfiguration:isdevelopmentenv"];
            //if (Environment.IsDevelopment())  //not working for dev0 environment
            if (isdevelopmentenv.Contains("Configuration") || Convert.ToBoolean(isdevelopmentenv))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                //app.UseHttpsRedirection();
            }
            app.Use(async (context, next) =>
            {
                context.Response.Headers["Cache-Control"] = string.IsNullOrEmpty(headercachecontrol) || headercachecontrol.Contains("Configuration") ? "no-cache, no-store, must-revalidate" : headercachecontrol;
                context.Response.Headers["Expires"] = string.IsNullOrEmpty(headerexpires) || headerexpires.Contains("Configuration") ? "-1" : headerexpires;
                context.Response.Headers["Pragma"] = string.IsNullOrEmpty(headerpragma) || headerpragma.Contains("Configuration") ? "no-cache" : headerpragma;
                context.Response.Headers.Add("X-Frame-Options", string.IsNullOrEmpty(headerxframeoptions) || headerxframeoptions.Contains("Configuration") ? "DENY" : headerxframeoptions);
                context.Response.Headers.Add("X-Xss-Protection", string.IsNullOrEmpty(headerxxssprotection) || headerxxssprotection.Contains("Configuration") ? "1" : headerxxssprotection);
                ///////context.Response.Headers.Add("Content-Security-Policy", "script-src 'self' 'unsafe-eval' 'unsafe-inline'; navigate-to https://www.daf.com; connect-src 'self'; img-src 'self'; style-src 'self' 'unsafe-inline'");
                context.Response.Headers.Add("Strict-Transport-Security", string.IsNullOrEmpty(headerstricttransportsecurity) || headerstricttransportsecurity.Contains("Configuration") ? "31536000" : headerstricttransportsecurity);
                context.Response.Headers.Add("Access-Control-Allow-Origin", string.IsNullOrEmpty(headeraccesscontrolalloworigin) || headeraccesscontrolalloworigin.Contains("Configuration") ? "*" : headeraccesscontrolalloworigin);
                context.Response.Headers.Add("Access-Control-Allow-Credentials", string.IsNullOrEmpty(headerAccesscontrolallowcredentials) || headerAccesscontrolallowcredentials.Contains("Configuration") ? "true" : headerAccesscontrolallowcredentials);
                context.Response.Headers.Add("Access-Control-Allow-Methods", string.IsNullOrEmpty(headeraccesscontrolallowmethods) || headeraccesscontrolallowmethods.Contains("Configuration") ? "GET, POST, PUT, DELETE" : headeraccesscontrolallowmethods);
                context.Response.Headers.Add("Access-Control-Allow-Headers", string.IsNullOrEmpty(headerAccesscontrolallowheaders) || headerAccesscontrolallowheaders.Contains("Configuration") ? "Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With" : headerAccesscontrolallowheaders);
                context.Response.Headers.Remove("X-Powered-By");
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-AspNet-Version");
                context.Response.Headers.Remove("X-AspNetMvc-Version");
                await next();
            });
            app.UseRouting();
            //This need to be change to orgin specific on UAT and prod
            app.UseCors(builder =>
            {
                builder.WithOrigins(string.IsNullOrEmpty(headeraccesscontrolalloworigin) ? "*" : headeraccesscontrolalloworigin);
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });
            app.UseAuthorization();
            app.UseSession();
            //app.UseSessionValidator();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/NotificationHub");
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