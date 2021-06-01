using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account.entity;
using AccountComponent = net.atos.daf.ct2.account;

namespace net.atos.daf.ct2.singlesignonservice.Common
{
    public class BasicAuthenticationService : IBasicAuthenticationService
    {
        AccountComponent.IAccountIdentityManager _accountIdentityManager;
        public BasicAuthenticationService(AccountComponent.IAccountIdentityManager accountIdentityManager)
        {
            this._accountIdentityManager = accountIdentityManager;
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
