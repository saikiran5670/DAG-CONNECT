using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountComponent = net.atos.daf.ct2.account;

namespace net.atos.daf.ct2.vehicledataservice.Common
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
        public async Task<string> ValidatTokeneGuid(string token)
        {
            ValidTokenResponse response = await accountIdentityManager.ValidatTokeneGuid(token);
            if (response != null && !string.IsNullOrEmpty(response.Email))
                return response.Email;
            else
                return string.Empty;
        }
    }
}
