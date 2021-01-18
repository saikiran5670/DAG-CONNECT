using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using AccountComponent = net.atos.daf.ct2.account;
using AccountEntity = net.atos.daf.ct2.account.entity;
using IdentityComponent = net.atos.daf.ct2.identity;
using IdentityEntity = net.atos.daf.ct2.identity.entity;
using AccountPreferenceComponent = net.atos.daf.ct2.accountpreference;

namespace net.atos.daf.ct2.authenticationservice
{
    public class AuthenticationService: AuthService.AuthServiceBase
    {
        private readonly ILogger logger;
        AccountComponent.IAccountIdentityManager accountIdentityManager;
        public AuthenticationService(AccountComponent.IAccountIdentityManager _accountIdentityManager,ILogger<AuthenticationService> _logger)
        {
            accountIdentityManager =_accountIdentityManager;
            logger=_logger;
        }
        public override Task<AccountIdentityResponse> Auth(IdentityRequest request, ServerCallContext context)
        {
            AccountIdentityResponse response = new AccountIdentityResponse();
            try
            {
                IdentityEntity.Identity account = new IdentityEntity.Identity();
                account.UserName = request.UserName;
                account.Password = request.Password;
                AccountEntity.AccountIdentity accIdentity = accountIdentityManager.Login(account).Result;
                // response 
                AccountPreference accPreference= new AccountPreference();
                // accPreference.Id = accIdentity.AccountPreference.Id == null ?0:accIdentity.AccountPreference.Id;
                // accPreference.Ref_Id =accIdentity.AccountPreference.Ref_Id;
                // accPreference.preferenceType =accIdentity.AccountPreference.PreferenceType;
                // accPreference.Language_Id =accIdentity.AccountPreference.Language_Id;
                // accPreference.Timezone_Id =accIdentity.AccountPreference.Timezone_Id;
                // accPreference.Currency_Type =accIdentity.AccountPreference.Currency_Type;
                // accPreference.Unit_Type = accIdentity.AccountPreference.Unit_Type;
                // accPreference.VehicleDisplay_Type =accIdentity.AccountPreference.VehicleDisplay_Type;
                // accPreference.DateFormat_Type =accIdentity.AccountPreference.DateFormat_Type;
                // accPreference.DriverId =accIdentity.AccountPreference.DriverId;
                // accPreference.Is_Active =accIdentity.AccountPreference.Is_Active;

                response.AccountPreference=accPreference;
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountIdentityResponse
                {
                    ///Code = Responsecode.Failed,
                    Message = " Authentication is failed due to - " + ex.Message,

                });
            }
        }
    }
}
