using System.Reflection;
using System.Threading.Tasks;
using log4net;
using net.atos.daf.ct2.account.entity;
using AccountComponent = net.atos.daf.ct2.account;

namespace net.atos.daf.ct2.rfmsdataservice.Common
{
    public class BasicAuthenticationService : IBasicAuthenticationService
    {
        private readonly AccountComponent.IAccountIdentityManager _accountIdentityManager;
        private readonly ILog _logger;
        public BasicAuthenticationService(AccountComponent.IAccountIdentityManager accountIdentityManager)
        {
            this._accountIdentityManager = accountIdentityManager;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public async Task<string> ValidateToken(string token)
        {
            _logger.Info($"[VehicleDataService] Token received with request: {token}");
            //To-do change below call to ValidateToken to validate token in JWT format
            //This call change needs to be done in _accountIdentityManager which is currently in discussion
            ValidTokenResponse response = await _accountIdentityManager.ValidateTokenGuid(token);
            _logger.Info($"[VehicleDataService] Is received token valid: {response.Valid}");
            if (response != null && !string.IsNullOrEmpty(response.Email))
                return response.Email;
            else
                return string.Empty;
        }
    }
}
