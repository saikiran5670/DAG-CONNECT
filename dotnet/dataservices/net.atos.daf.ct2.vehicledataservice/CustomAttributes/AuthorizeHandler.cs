using Dapper;
using Microsoft.AspNetCore.Authorization;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.vehicledataservice.CustomAttributes
{
    public class AuthorizeHandler :
          AuthorizationHandler<AuthorizeRequirement>
    {
        private readonly IAccountManager accountManager;
        public AuthorizeHandler(IAccountManager _accountManager)
        {
            accountManager = _accountManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, AuthorizeRequirement requirement)
        {
            if(!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            var emailClaim = context.User.Claims.Where(x => x.Type.Equals("email") || x.Type.Equals(ClaimTypes.Email)).FirstOrDefault();
            var emailAddress = emailClaim.Value;

            var isExists = await accountManager.CheckForFeatureAccessByEmailId(emailAddress, requirement.FeatureName);
            if (isExists)
                context.Succeed(requirement);
            else
                context.Fail();

            return;
        }
    }
}
