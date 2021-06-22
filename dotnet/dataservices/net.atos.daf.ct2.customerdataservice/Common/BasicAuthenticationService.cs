using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account.entity;
using AccountComponent = net.atos.daf.ct2.account;

namespace net.atos.daf.ct2.customerdataservice.Common
{
    public class BasicAuthenticationService : IBasicAuthenticationService
    {
        readonly AccountComponent.IAccountIdentityManager _accountIdentityManager;
        private readonly ILogger<BasicAuthenticationService> _logger;
        public BasicAuthenticationService(AccountComponent.IAccountIdentityManager accountIdentityManager, ILogger<BasicAuthenticationService> logger)
        {
            this._accountIdentityManager = accountIdentityManager;
            this._logger = logger;
        }
        public async Task<string> ValidateTokenGuid(string token)
        {
            ValidTokenResponse response = await _accountIdentityManager.ValidateTokenGuid(token);
            if (response != null && !string.IsNullOrEmpty(response.Email))
                return response.Email;
            else
                return string.Empty;
        }
    }
}
