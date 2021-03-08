using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityComponent= net.atos.daf.ct2.identity;
using IdentityEntity=net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.account.ENUM;

namespace net.atos.daf.ct2.account
{
    public class AccountIdentityManager : IAccountIdentityManager
    {
        IdentityComponent.ITokenManager tokenManager;
        IdentityComponent.IAccountAuthenticator autheticator;
        IAccountManager accountManager;
        IPreferenceManager preferenceManager;        
        
       public AccountIdentityManager(IdentityComponent.ITokenManager _tokenManager, IdentityComponent.IAccountAuthenticator _autheticator,IPreferenceManager _preferenceManager,IAccountManager _accountManager)
        {
            autheticator = _autheticator;
            tokenManager = _tokenManager;
            preferenceManager = _preferenceManager;
            accountManager=_accountManager;
        }
        public async Task<AccountIdentity> Login(IdentityEntity.Identity user)
        {
            AccountIdentity accIdentity = new AccountIdentity();
            accIdentity.Authenticated = false;
            Account account = GetAccountByEmail(user.UserName);
            if(account !=null && account.Id > 0)
            {
                //int accountId = account.Id;
                //accIdentity.AccountId= account.Id;
                accIdentity.accountInfo = account;
                IdentityEntity.Response idpResponse = await autheticator.AccessToken(user);
                if(idpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    IdentityEntity.IDPToken token = JsonConvert.DeserializeObject<IdentityEntity.IDPToken>(Convert.ToString(idpResponse.Result));
                    IdentityEntity.AccountIDPClaim accIDPclaims= tokenManager.DecodeToken(token.access_token);

                    accIDPclaims.TokenExpiresIn=token.expires_in;

                    IdentityEntity.AccountToken accToken = tokenManager.CreateToken(accIDPclaims);
                    accIdentity.Authenticated = true;
                    //accIdentity.AccountToken=accToken;
                    // int accountId= GetAccountByEmail(user.UserName);
                    // if(accountId>0)
                    // {
                    //AccountPreferenceFilter filter=new AccountPreferenceFilter();
                    //filter.Ref_Id=account.Id;
                    ////filter.Ref_Id =PreferenceType.Ref_id;
                    //filter.PreferenceType=PreferenceType.Account;
                    //IEnumerable<AccountPreference> preferences = preferenceManager.Get(filter).Result;
                    //foreach(var pref in preferences) 
                    //{
                    //    accIdentity.AccountPreference=pref;
                    //    break; //get only first preference
                    //}
                    accIdentity.AccountOrganization = accountManager.GetAccountOrg(account.Id).Result;
                    accIdentity.AccountRole = accountManager.GetAccountRole(account.Id).Result;
                    // }
                }
            }
            return await Task.FromResult(accIdentity);
        }
        public async Task<IdentityEntity.AccountToken> GenerateToken(IdentityEntity.Identity user)
        {
            IdentityEntity.AccountToken accToken = new IdentityEntity.AccountToken();

            IdentityEntity.Response idpResponse = await autheticator.AccessToken(user);
            if(idpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                IdentityEntity.IDPToken token = JsonConvert.DeserializeObject<IdentityEntity.IDPToken>(Convert.ToString(idpResponse.Result));
                IdentityEntity.AccountIDPClaim accIDPclaims= tokenManager.DecodeToken(token.access_token);
                accIDPclaims.TokenExpiresIn=token.expires_in;
                accToken = tokenManager.CreateToken(accIDPclaims);
            }
            return await Task.FromResult(accToken);
        }
        public Task<bool> ValidateToken(string token)
        {
            bool result=false;
            result= tokenManager.ValidateToken(token);
            return Task.FromResult(result);
        }
        private Account GetAccountByEmail(string email)
        {
            Account account = new Account();

            AccountFilter filter = new AccountFilter();     
            filter.Email =email;
            // filter.AccountType = AccountType.PortalAccount;            
            filter.AccountType = AccountType.None;            
            IEnumerable<Account> result = accountManager.Get(filter).Result;
            foreach(var acc in result) 
            {
                account = acc;
                break;//get only first account id
            }
            return account;
        }
    }
}
