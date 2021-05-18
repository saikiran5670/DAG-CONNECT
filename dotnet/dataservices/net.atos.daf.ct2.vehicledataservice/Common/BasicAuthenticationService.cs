using log4net;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
