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
                if(accIdentity !=null)
                {
                    AccountPreference accPreference= new AccountPreference();
                    // accPreference.Id = accIdentity.AccountPreference.Id == null ?0:accIdentity.AccountPreference.Id;
                    accPreference.RefId = accIdentity.AccountPreference.Ref_Id;
                    accPreference.PreferenceType = (PreferenceType)Enum.Parse(typeof(PreferenceType), accIdentity.AccountPreference.PreferenceType.ToString());                 
                    // accPreference.PreferenceType =accIdentity.AccountPreference.PreferenceType;
                    accPreference.LanguageId =accIdentity.AccountPreference.Language_Id;
                    accPreference.TimezoneId =accIdentity.AccountPreference.Timezone_Id;
                    accPreference.CurrencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), accIdentity.AccountPreference.Currency_Type.ToString());                 
                    accPreference.UnitType = (UnitType)Enum.Parse(typeof(UnitType), accIdentity.AccountPreference.Unit_Type.ToString());                 
                    accPreference.VehicleDisplayType= (VehicleDisplayType)Enum.Parse(typeof(VehicleDisplayType), accIdentity.AccountPreference.VehicleDisplay_Type.ToString());                 
                    accPreference.DateFormatType= (DateFormatDisplayType)Enum.Parse(typeof(DateFormatDisplayType), accIdentity.AccountPreference.DateFormat_Type.ToString());                 
                    accPreference.DriverId =accIdentity.AccountPreference.DriverId;
                    accPreference.IsActive =accIdentity.AccountPreference.Is_Active;

                    response.AccountPreference=accPreference;
                    return Task.FromResult(response);
                }
                else 
                {
                    return Task.FromResult(new AccountIdentityResponse
                    {
                        //Account not present  in IDP or IDP related error
                        Code = Responsecode.Failed,
                        Message = "Account is not configured.",
                    });
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountIdentityResponse
                {
                    Code = Responsecode.Failed,
                    Message = " Authentication is failed due to - " + ex.Message,
                });
            }
        }
        public override Task<ValidateResponse> Validate(ValidateRequest request, ServerCallContext context)
        {
            ValidateResponse response = new ValidateResponse();
            try
            {
                response.Valid=false;
                response.Valid = accountIdentityManager.ValidateToken(request.Token).Result;
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ValidateResponse
                {
                    Code = Responsecode.Failed,
                    Message = " Token is not valid - " + ex.Message,
                    Valid = false,
                });
            }
        }
    }
}
