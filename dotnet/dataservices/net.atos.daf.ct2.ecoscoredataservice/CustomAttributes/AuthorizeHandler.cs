using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authorization;
using net.atos.daf.ct2.account;

namespace net.atos.daf.ct2.ecoscoredataservice.CustomAttributes
{
    public class AuthorizeHandler :
          AuthorizationHandler<AuthorizeRequirement>
    {
        private readonly IAccountManager _accountManager;
        private readonly ILog _logger;

        public AuthorizeHandler(IAccountManager accountManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            this._accountManager = accountManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, AuthorizeRequirement requirement)
        {
            string emailAddress = string.Empty;
            var emailClaim = context.User.Claims.Where(x => x.Type.Equals("email") || x.Type.Equals(ClaimTypes.Email)).FirstOrDefault();

            if (emailClaim != null && !string.IsNullOrEmpty(emailClaim.Value))
            {
                emailAddress = emailClaim.Value;
                _logger.Info($"[VehicleDataService] Email claim received: {emailAddress}");
            }
            else
            {
                context.Fail();
                return;
            }

            try
            {
                var isExists = await _accountManager.CheckForFeatureAccessByEmailId(emailAddress, requirement.FeatureName);
                _logger.Info($"[VehicleDataService] Is user authorized: {isExists}");
                if (isExists)
                    context.Succeed(requirement);
                else
                    context.Fail();
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("[VehicleDataService] Error occurred while authorizing the request", ex);
                context.Fail();
                return;
            }
        }
    }
}
