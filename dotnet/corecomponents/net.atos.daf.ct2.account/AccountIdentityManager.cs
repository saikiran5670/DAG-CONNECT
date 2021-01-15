using System;
using Newtonsoft.Json;
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

        public AccountIdentityManager(ITokenManager _tokenManager, IAccountAuthenticator _autheticator)
        {
            autheticator = _autheticator;
            tokenManager = _tokenManager;
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
                accIdentity.accountToken=accToken;
            }
            return await Task.FromResult(accIdentity);
        }
        public Task<bool> ValidateToken(string token)
        {
            bool result=false;
            result= tokenManager.ValidateToken(token);
            return Task.FromResult(result);
        }
    }
}
