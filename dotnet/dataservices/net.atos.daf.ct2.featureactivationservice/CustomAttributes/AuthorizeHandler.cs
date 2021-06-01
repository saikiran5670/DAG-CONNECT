using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using net.atos.daf.ct2.account;

namespace net.atos.daf.ct2.featureactivationservice.CustomAttributes
{
    public class AuthorizeHandler :
          AuthorizationHandler<AuthorizeRequirement>
    {
        private readonly IAccountManager _accountManager;
        public AuthorizeHandler(IAccountManager accountManager)
        {
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
            }
            else
            {
                context.Fail();
                return;
            }

            try
            {
                var isExists = await _accountManager.CheckForFeatureAccessByEmailId(emailAddress, requirement.FeatureName);
                if (isExists)
                    context.Succeed(requirement);
                else
                    context.Fail();
                return;
            }
            catch (Exception)
            {
                context.Fail();
                return;
            }
        }
    }
}
