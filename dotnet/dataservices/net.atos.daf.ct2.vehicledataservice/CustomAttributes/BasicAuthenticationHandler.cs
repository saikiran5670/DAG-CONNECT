using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.vehicledataservice.Common;

namespace net.atos.daf.ct2.vehicledataservice.CustomAttributes
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private const string AuthorizationHeaderName = "Authorization";
        private const string AuthorizationHeaderType = "Bearer";
        private readonly IBasicAuthenticationService _authenticationService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IBasicAuthenticationService authenticationService)
            : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                //Authorization header not in request
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue headerValue))
            {
                //Invalid Authorization header
                return AuthenticateResult.NoResult();
            }
            if (!AuthorizationHeaderType.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                //Not a bearer authentication header type
                return AuthenticateResult.NoResult();
            }
            string email = string.Empty;
            try
            {
                string token = Convert.ToString(headerValue);
                token = token.Replace(AuthorizationHeaderType, "");
                email = await _authenticationService.ValidateTokenGuid(token);
            }
            catch (Exception)
            {
                return AuthenticateResult.Fail("");
            }
            if (string.IsNullOrEmpty(email))
            {
                return AuthenticateResult.Fail("");
            }
            else
            {
                var claims = new[] {
                                 new Claim(ClaimTypes.Email, email)
                                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
        }
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            //Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Options.Realm}\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}
