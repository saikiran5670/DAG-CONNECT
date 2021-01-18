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

            IdentityEntity.Response idpResponse = await autheticator.AccessToken(user);
            if(idpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                IdentityEntity.IDPToken token = JsonConvert.DeserializeObject<IdentityEntity.IDPToken>(Convert.ToString(idpResponse.Result));
                IdentityEntity.AccountIDPClaim accIDPclaims= tokenManager.DecodeToken(token.access_token);

                IdentityEntity.AccountToken accToken = tokenManager.CreateToken(accIDPclaims);
                accIdentity.AccountToken=accToken;
                int accountId= GetAccountByEmail(user.UserName);
                if(accountId>0)
                {
                    AccountPreferenceFilter filter=new AccountPreferenceFilter();
                    filter.Ref_Id=accountId;
                    //filter.Ref_Id =PreferenceType.Ref_id;
                    filter.PreferenceType=PreferenceType.Account;
                    IEnumerable<AccountPreference> preferences = preferenceManager.Get(filter).Result;
                    foreach(var pref in preferences) 
                    {
                        accIdentity.AccountPreference=pref;
                        break; //get only first preference
                    }
                }
            }
            return await Task.FromResult(accIdentity);
        }
        public Task<bool> ValidateToken(string token)
        {
            bool result=false;
            result= tokenManager.ValidateToken(token);
            return Task.FromResult(result);
        }
        private int GetAccountByEmail(string email)
        {
            int accountid=0;
            AccountFilter filter = new AccountFilter();     
            filter.Email =email;
            filter.AccountType = AccountType.PortalAccount;            
        //    filter.AccountType = AccountType.None;            
            IEnumerable<Account> result = accountManager.Get(filter).Result;
            foreach(var account in result) 
            {
                accountid=account.Id;
                break;//get only first account id
            }
            return accountid;
        }
    }
}
