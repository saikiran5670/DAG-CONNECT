using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace net.atos.daf.ct2.ecoscoredataservice.CustomAttributes
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private const string AUTHORIZATION_HEADER_NAME = "Authorization";
        private const string AUTHORIZATION_HEADER_TYPE = "Bearer";
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
            if (!Request.Headers.ContainsKey(AUTHORIZATION_HEADER_NAME))
            {
                //Authorization header not in request
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AUTHORIZATION_HEADER_NAME], out AuthenticationHeaderValue headerValue))
            {
                //Invalid Authorization header
                return AuthenticateResult.NoResult();
            }
            if (!AUTHORIZATION_HEADER_TYPE.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                //Not a bearer authentication header type
                return AuthenticateResult.NoResult();
            }
            string email;
            try
            {
                string token = Convert.ToString(headerValue);
                token = token.Replace(AUTHORIZATION_HEADER_TYPE, "");
                if (Guid.TryParse(token, out Guid result))
                {
                    email = await _authenticationService.ValidateTokenGuid(token);
                }
                else
                {
                    return AuthenticateResult.NoResult();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return AuthenticateResult.Fail(ex.Message);
            }
            if (string.IsNullOrEmpty(email))
            {
                return AuthenticateResult.Fail("Email address not found.");
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
