using System.Reflection;
using System.Threading.Tasks;
using log4net;
using net.atos.daf.ct2.account.entity;
using AccountComponent = net.atos.daf.ct2.account;

namespace net.atos.daf.ct2.ecoscoredataservice
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

        public async Task<string> ValidateTokenGuid(string token)
        {
            _logger.Info($"[EcoScoreDataService] Token received with request: {token}");
            ValidTokenResponse response = await _accountIdentityManager.ValidateTokenGuid(token);
            _logger.Info($"[EcoScoreDataService] Is received token valid: {response.Valid}");
            if (response != null && !string.IsNullOrEmpty(response.Email))
                return response.Email;
            else
                return string.Empty;
        }
    }
}
