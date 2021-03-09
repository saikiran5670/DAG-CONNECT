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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

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
            var accountservice = Configuration["ServiceConfiguration:accountservice"];
            var packageservice = Configuration["ServiceConfiguration:packageservice"];
            var vehicleservice = Configuration["ServiceConfiguration:vehicleservice"];
            var translationservice = Configuration["ServiceConfiguration:translationservice"];
            var auditservice = Configuration["ServiceConfiguration:auditservice"];
            var featureservice= Configuration["ServiceConfiguration:featureservice"];
            var roleservice = Configuration["ServiceConfiguration:roleservice"];
            var organizationservice = Configuration["ServiceConfiguration:organizationservice"];

            var isdevelopmentenv = Configuration["ServerConfiguration:isdevelopmentenv"];
            var cookiesexpireat = Configuration["ServerConfiguration:cookiesexpireat"];
            var authcookiesexpireat = Configuration["ServerConfiguration:authcookiesexpireat"];

            // We are enforcing to call Insercure service             
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            services.AddControllers();
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
                o.Address = new Uri(featureservice);
            });
            services.AddGrpcClient<TranslationService.TranslationServiceClient>(o =>
            {
                o.Address = new Uri(translationservice);
            });
            services.AddGrpcClient<AuditService.AuditServiceClient>(o =>
            {
                o.Address = new Uri(auditservice);
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                //options.Cookie.ExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToDouble(cookiesexpireat));
                options.Cookie.SecurePolicy = Convert.ToBoolean(isdevelopmentenv) ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToDouble(authcookiesexpireat));
            });
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute { Permanent = true });
                options.Filters.Add(new AuthorizeFilter());
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
            //This need to be change to orgin specific on UAT and prod
            app.UseCors(builder =>
            {
                builder.WithOrigins("*");
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            app.UseCookiePolicy();
            
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
