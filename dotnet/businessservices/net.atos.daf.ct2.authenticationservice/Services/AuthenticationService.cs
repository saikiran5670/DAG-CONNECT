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
                    if(accIdentity.AccountId>0)
                    {
                       response.AccountId = accIdentity.AccountId;
                    }
                    if(accIdentity.AccountToken!=null)
                    {
                        AccountToken accToken = new AccountToken();
                        accToken.AccessToken=accIdentity.AccountToken.AccessToken;
                        accToken.ExpiresIn=accIdentity.AccountToken.ExpiresIn;
                        accToken.TokenType=accIdentity.AccountToken.TokenType;
                        accToken.SessionState=accIdentity.AccountToken.SessionState;
                        accToken.Scope=accIdentity.AccountToken.Scope;

                        response.AccountToken=accToken;
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
                    if(accIdentity.AccountPreference!=null)
                    {
                        AccountPreference accPreference= new AccountPreference();
                        accPreference.Id = accIdentity.AccountPreference.Id;
                        accPreference.RefId = accIdentity.AccountPreference.RefId;
                        accPreference.PreferenceType = (PreferenceType)Enum.Parse(typeof(PreferenceType), accIdentity.AccountPreference.PreferenceType.ToString());                 
                        accPreference.LanguageId =accIdentity.AccountPreference.LanguageId;
                        accPreference.TimezoneId =accIdentity.AccountPreference.TimezoneId;
                        accPreference.CurrencyId = accIdentity.AccountPreference.CurrencyId;                 
                        accPreference.UnitId = accIdentity.AccountPreference.UnitId;                 
                        accPreference.VehicleDisplayId= accIdentity.AccountPreference.VehicleDisplayId;                 
                        accPreference.DateFormatId= accIdentity.AccountPreference.DateFormatTypeId;                 
                        accPreference.TimeFormatId =accIdentity.AccountPreference.TimeFormatId;
                        accPreference.LandingPageDisplayId =accIdentity.AccountPreference.LandingPageDisplayId;
                        accPreference.DriverId =accIdentity.AccountPreference.DriverId;
                        accPreference.Active =accIdentity.AccountPreference.Active;

                        response.AccountPreference=accPreference;
                    }
                    if(accIdentity.AccountOrganization!=null && accIdentity.AccountOrganization.Count>0)
                    {   
                        AccountOrganization acctOrganization = new AccountOrganization();
                        foreach(var accOrg in accIdentity.AccountOrganization)
                        {
                            acctOrganization = new AccountOrganization();
                            acctOrganization.Id = accOrg.Id;
                            acctOrganization.Name = accOrg.Name;
                            response.AccountOrganizations.Add(acctOrganization);
                        }
                    }
                    if(accIdentity.AccountRole!=null && accIdentity.AccountRole.Count>0)
                    {   
                        AccountOrgRole accRole = new AccountOrgRole();
                        foreach(var accr in accIdentity.AccountRole)
                        {
                            accRole = new AccountOrgRole();
                            accRole.Id = accr.Id;
                            accRole.Name = accr.Name;
                            accRole.OrganizationId= accr.Organization_Id;
                            response.AccountRoles.Add(accRole);
                        }
                    }
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
