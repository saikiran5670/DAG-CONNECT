using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account.entity;
using AccountComponent = net.atos.daf.ct2.account;

namespace net.atos.daf.ct2.customerdataservice.Common
{
    public class BasicAuthenticationService : IBasicAuthenticationService
    {
        AccountComponent.IAccountIdentityManager accountIdentityManager;
        private readonly ILogger<BasicAuthenticationService> logger;
        public BasicAuthenticationService(AccountComponent.IAccountIdentityManager _accountIdentityManager, ILogger<BasicAuthenticationService> _logger)
        {
            accountIdentityManager = _accountIdentityManager;
            logger = _logger;
        }
        public async Task<string> ValidateTokenGuid(string token)
        {
            ValidTokenResponse response = await accountIdentityManager.ValidateTokenGuid(token);
            if (response != null && !string.IsNullOrEmpty(response.Email))
                return response.Email;
            else
                return string.Empty;
        }
    }
}
