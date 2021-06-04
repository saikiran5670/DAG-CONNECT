using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class SessionValidateMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.Value.Contains("portalservice"))
            {
                var serviceCalls = Assembly.GetExecutingAssembly().GetTypes()
                                  .SelectMany(t => t.GetMethods())
                                  .Where(m => m.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Length > 0)
                                  .Select(x => x.CustomAttributes.Where(y => y.AttributeType == typeof(RouteAttribute)))
                                  .Select(x => x.First().ConstructorArguments.First().Value.ToString().Replace("~", "").Replace("/", "").ToLower())
                                  .ToList();
                var requestedPath = context.Request.Path.Value.Replace("~","").Replace("/", "").ToLower();
                if (!serviceCalls.Any(x => requestedPath.Contains(x)))
                {
                    if(context.Request.Path.Value.Contains("setuserselection"))
                    {
                        if (!context.Session.Keys.Any(x => x.Equals(SessionConstants.AccountKey)))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            await context.Response.WriteAsync("Session not found. Please login first to perform the operation.");
                        }
                        else
                        {
                            await _next(context);
                        }
                    }
                    else
                    {
                        var keys = new string[] { SessionConstants.AccountKey, SessionConstants.RoleKey, SessionConstants.OrgKey, SessionConstants.ContextOrgKey };
                        if (!context.Session.Keys.Any(x => keys.Contains(x)))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            await context.Response.WriteAsync("Session not found. Please login first to perform the operation.");
                        }
                        else
                        {
                            await _next(context);
                        }
                    }                    
                }
                else
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }

    public static class SessionValidateMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionValidator(
            this IApplicationBuilder builder) => builder.UseMiddleware<SessionValidateMiddleware>();
    }
}
