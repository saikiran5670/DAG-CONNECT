using System.Reflection;
using System.Threading.Tasks;
using log4net;
using net.atos.daf.ct2.account.entity;
using AccountComponent = net.atos.daf.ct2.account;

namespace net.atos.daf.ct2.vehicledataservice.Common
{
    public class BasicAuthenticationService : IBasicAuthenticationService
    {
        AccountComponent.IAccountIdentityManager accountIdentityManager;
        private readonly ILog _logger;
        public BasicAuthenticationService(AccountComponent.IAccountIdentityManager _accountIdentityManager)
        {
            accountIdentityManager = _accountIdentityManager;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public async Task<string> ValidateTokenGuid(string token)
        {
            _logger.Info($"[VehicleDataService] Token received with request: {token}");
            ValidTokenResponse response = await accountIdentityManager.ValidateTokenGuid(token);
            _logger.Info($"[VehicleDataService] Is received token valid: {response.Valid}");
            if (response != null && !string.IsNullOrEmpty(response.Email))
                return response.Email;
            else
                return string.Empty;
        }
    }
}
