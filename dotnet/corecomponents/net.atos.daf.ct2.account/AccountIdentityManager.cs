using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.identity;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.account.entity;

namespace net.atos.daf.ct2.account
{
    public class AccountIdentityManager : IAccountIdentityManager
    {
        ITokenManager tokenManager;
        IAccountAuthenticator autheticator;
        IAccountManager accountManager;
        IAccountPreference accountPreference;        

        public AccountIdentityManager(ITokenManager _tokenManager, IAccountAuthenticator _autheticator,IAccountPreference _accountPreference)
        {
            autheticator = _autheticator;
            tokenManager = _tokenManager;
            accountPreference=_accountPreference;
        }
        public async Task<AccountIdentity> Login(Identity user)
        {
            AccountIdentity accIdentity = new AccountIdentity();

            Response idpResponse = await autheticator.AccessToken(user);
            if(idpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                IDPToken token = JsonConvert.DeserializeObject<IDPToken>(Convert.ToString(idpResponse.Result));
                AccountIDPClaim accIDPclaims= tokenManager.DecodeToken(token.access_token);

                AccountToken accToken = tokenManager.CreateToken(accIDPclaims);
                accIdentity.AccountToken=accToken;
                int accountId=GetAccountByEmail(user.UserName);
                if(accountId>0)
                {
                    AccountPreferenceFilter filter=new AccountPreferenceFilter();
                    filter.Ref_id=PreferenceType.Ref_id;
                    filter.PreferenceType=PreferenceType.Account;
                    // IEnumerable<AccountPreference> preferences=accountPreference.Get(filter);
                    // foreach(var pref in preferences) 
                    // {
                    //     accIdentity.AccountPreference=pref;
                    //     break; //get only first preference
                    // }
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
            filter.Name =email;
        //    filter.AccountType = AccountType.None;            
            IEnumerable<Account> result = accountManager.Get(filter).Result;
            foreach(var account in result) 
            {
                accountid=account.Id;
                //get only first account id
            }
            return accountid;
        }
    }
}
