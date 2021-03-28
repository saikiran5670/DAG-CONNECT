using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityComponent = net.atos.daf.ct2.identity;
using IdentitySessionComponent = net.atos.daf.ct2.identitysession;
using IdentityEntity = net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.account.ENUM;
using net.atos.daf.ct2.utilities;
using System.Net;

namespace net.atos.daf.ct2.account
{
    public class AccountIdentityManager : IAccountIdentityManager
    {
        IdentityComponent.ITokenManager tokenManager;
        IdentityComponent.IAccountAuthenticator autheticator;
        IdentityComponent.IAccountManager identityAccountManager;
        IAccountManager accountManager;
        IdentitySessionComponent.IAccountSessionManager accountSessionManager;
        IdentitySessionComponent.IAccountTokenManager accountTokenManager;
        public AccountIdentityManager(IdentityComponent.ITokenManager _tokenManager, IdentityComponent.IAccountAuthenticator _autheticator, IAccountManager _accountManager, IdentitySessionComponent.IAccountSessionManager _accountSessionManager, IdentitySessionComponent.IAccountTokenManager _accountTokenManager, IdentityComponent.IAccountManager _identityAccountManager)
        {
            autheticator = _autheticator;
            tokenManager = _tokenManager;
            accountManager = _accountManager;
            accountSessionManager = _accountSessionManager;
            accountTokenManager = _accountTokenManager;
            identityAccountManager = _identityAccountManager;
        }
        public async Task<AccountIdentity> Login(IdentityEntity.Identity user)
        {
            AccountIdentity accIdentity = new AccountIdentity();
            accIdentity.tokenIdentifier = string.Empty;
            IdentityEntity.AccountToken accToken = new IdentityEntity.AccountToken();
            Account account = GetAccountByEmail(user.UserName);
            if (account != null && account.Id > 0)
            {
                accToken = await PrepareSaveToken(user, account);
                if (accToken != null && accToken.statusCode == HttpStatusCode.OK)
                {
                    IdentityEntity.AccountIDPClaim accIDPclaims = tokenManager.DecodeToken(accToken.AccessToken);

                    accIdentity.tokenIdentifier = accIDPclaims.Id;
                    accIdentity.accountInfo = account;
                    accIdentity.AccountOrganization = accountManager.GetAccountOrg(account.Id).Result;
                    accIdentity.AccountRole = accountManager.GetAccountRole(account.Id).Result;
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
                    // }
                }
            }
            return await Task.FromResult(accIdentity);
        }
        public async Task<AccountIdentity> LoginOld(IdentityEntity.Identity user)
        {
            AccountIdentity accIdentity = new AccountIdentity();
            accIdentity.tokenIdentifier = string.Empty;
            Account account = GetAccountByEmail(user.UserName);
            if (account != null && account.Id > 0)
            {
                //int accountId = account.Id;
                //accIdentity.AccountId= account.Id;
                accIdentity.accountInfo = account;
                IdentityEntity.Response idpResponse = await autheticator.AccessToken(user);
                if (idpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    IdentityEntity.IDPToken token = JsonConvert.DeserializeObject<IdentityEntity.IDPToken>(Convert.ToString(idpResponse.Result));
                    IdentityEntity.AccountIDPClaim accIDPclaims = tokenManager.DecodeToken(token.access_token);

                    accIDPclaims.TokenExpiresIn = token.expires_in;

                    IdentityEntity.AccountToken accToken = tokenManager.CreateToken(accIDPclaims);
                    //accIdentity.tokenIdentifier = true;
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
            Account account = GetAccountByEmail(user.UserName);
            if (account != null && account.Id > 0)
            {
                accToken = await PrepareSaveToken(user, account);
            }
            else
            {
                accToken.statusCode = System.Net.HttpStatusCode.NotFound;
                accToken.message = "Account is not present in database";
            }
            return accToken;
        }
        public async Task<bool> ValidateToken(string token)
        {
            bool result = false;
            result = await tokenManager.ValidateToken(token);
            if (result)
                result = await VerifyAccountToken(token);

            return await Task.FromResult(result);
        }        
        public async Task<bool> LogoutByJwtToken(string token)
        {
            bool isLogout = false;
            bool tokenValid = await ValidateToken(token);
            if (tokenValid)
            {
                //decode token to extract token identifier, sessoin state and email
                IdentityEntity.AccountIDPClaim accIDPclaims = tokenManager.DecodeToken(token);
                if (accIDPclaims != null && !string.IsNullOrEmpty(accIDPclaims.Id))
                {
                    //delete token by passing token identifier extracted from token
                    int accountid = await accountTokenManager.DeleteTokenByTokenId(Guid.Parse(accIDPclaims.Id));
                    //check if another token are availble for same account 
                    int tokencount = await accountTokenManager.GetTokenCount(accountid);
                    if (tokencount == 0)
                    {
                        //no token belong to this session hence delete the token
                        int sessionid = await accountSessionManager.DeleteSession(accIDPclaims.Sessionstate);
                        //sign out account from IDP by using username
                        IdentityEntity.Identity identity= new IdentityEntity.Identity();
                        identity.UserName = accIDPclaims.Email;
                        IdentityEntity.Response response=await identityAccountManager.LogOut(identity);
                    }
                    isLogout = true;
                }
            }
            return await Task.FromResult(isLogout);
        }
        public async Task<bool> LogoutByAccountId(int accountId)
        {
            bool isLogout = false;
            if (accountId > 0)
            {
                await accountTokenManager.DeleteTokenbyAccountId(accountId);
                await accountSessionManager.DeleteSessionByAccountId(accountId);

                string emailid = string.Empty;
                AccountFilter filter = new AccountFilter();
                filter.Id = accountId;
                IEnumerable<Account> accounts = await accountManager.Get(filter);
                foreach (var account in accounts)
                {
                    emailid = account.EmailId;
                    break;
                }
                if (!string.IsNullOrEmpty(emailid))
                {   //sign out account from IDP using email
                    IdentityEntity.Identity identity = new IdentityEntity.Identity();
                    identity.UserName = emailid;
                    IdentityEntity.Response response = await identityAccountManager.LogOut(identity);
                }
                isLogout = true;
            }
            return await Task.FromResult(isLogout);

        }
        public async Task<bool> LogoutByTokenId(string tokenid)
        {
            bool isLogout = false;
            //delete token by passing token identifier extracted from token
            int accountid = await accountTokenManager.DeleteTokenByTokenId(Guid.Parse(tokenid));
            if (accountid > 0)
            {
                //check if another token are availble for same account 
                int tokencount = await accountTokenManager.GetTokenCount(accountid);
                if (tokencount == 0)
                {
                    //no token belong to this session hence delete the token
                    int sessionid = await accountSessionManager.DeleteSessionByAccountId(accountid);
                    await LogoutFromIDP(accountid);
                }
                isLogout = true;
            }
            return await Task.FromResult(isLogout);
        }
        private async Task LogoutFromIDP(int accountId)
        {
            string emailid = string.Empty;
            AccountFilter filter = new AccountFilter();
            filter.Id = accountId;
            IEnumerable<Account> accounts = await accountManager.Get(filter);
            foreach (var account in accounts)
            {
                emailid = account.EmailId;
                break;
            }
            if (!string.IsNullOrEmpty(emailid))
            {   //sign out account from IDP using email
                IdentityEntity.Identity identity = new IdentityEntity.Identity();
                identity.UserName = emailid;
                IdentityEntity.Response response = await identityAccountManager.LogOut(identity);
            }
        }
        private async Task<bool> VerifyAccountToken(string token)
        {
            bool isAvailable = false;
            //decode token to extract token identifier, sessoin state and email
            IdentityEntity.AccountIDPClaim accIDPclaims = tokenManager.DecodeToken(token);
            if (accIDPclaims != null && !string.IsNullOrEmpty(accIDPclaims.Id))
            {
                int accountid = 0;
                int sessionid= 0;
                //check token is available in account token 
                IEnumerable <IdentitySessionComponent.entity.AccountToken> tokenlst = await accountTokenManager.GetTokenDetails(accIDPclaims.Id);
                foreach (var item in tokenlst)
                {
                    accountid = item.AccountId;
                    break;
                }
                //check session is available in account account
                if (accountid > 0)
                {
                    IEnumerable<IdentitySessionComponent.entity.AccountSession> sessionlst = await accountSessionManager.GetAccountSession(accountid);
                    foreach (var item in tokenlst)
                    {
                        sessionid = item.Session_Id;
                        break;
                    }
                    if (sessionid > 0)
                    {
                        isAvailable = true;
                    }
                    else
                        isAvailable = false;
                }
                else
                    isAvailable = false;
            }
            else
                isAvailable = false;
            return await Task.FromResult(isAvailable);
        }
        private async Task<IdentityEntity.AccountToken> PrepareSaveToken(IdentityEntity.Identity user, Account account)
        {
            IdentityEntity.AccountToken accToken = new IdentityEntity.AccountToken();
            //generate idp token 
            IdentityEntity.Response idpResponse = await autheticator.AccessToken(user);
            if (idpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                IdentityEntity.IDPToken token = JsonConvert.DeserializeObject<IdentityEntity.IDPToken>(Convert.ToString(idpResponse.Result));
                IdentityEntity.AccountIDPClaim accIDPclaims = tokenManager.DecodeToken(token.access_token);
                var now = DateTime.Now;
                long unixTimeSecondsIssueAt = new DateTimeOffset(now).ToUnixTimeSeconds();
                long unixTimeSecondsExpiresAt = 0;
                if (token.expires_in > 0)
                {
                    unixTimeSecondsExpiresAt = new DateTimeOffset(now.AddSeconds(token.expires_in)).ToUnixTimeSeconds();
                }
                int session_Id = 0;
                Guid sessionGuid = Guid.NewGuid();
                Guid tokenidentifier = Guid.NewGuid();
                IdentitySessionComponent.entity.AccountSession accSessionEntity = new IdentitySessionComponent.entity.AccountSession();
                IdentitySessionComponent.entity.AccountToken accTokenEntity = new IdentitySessionComponent.entity.AccountToken();
                //check if session is present for this account.
                IEnumerable<IdentitySessionComponent.entity.AccountSession> sessionlist = await accountSessionManager.GetAccountSession(account.Id);
                foreach (var item in sessionlist)
                {
                    //session for this account is already present.
                    session_Id = item.Id;
                    sessionGuid = item.Session_Id;
                    accSessionEntity = item;
                    break;
                }
                if (session_Id == 0)
                {
                    //create a new session entry as there is no session active for this account.                        
                    accSessionEntity.IpAddress = "0.0.0.0";//need to be discuss and implement 
                    accSessionEntity.AccountId = account.Id;
                    accSessionEntity.UserName = account.EmailId;
                    accSessionEntity.Session_Id = System.Guid.NewGuid();
                    accSessionEntity.SessionStartedAt = unixTimeSecondsIssueAt;
                    accSessionEntity.CreatedAt = unixTimeSecondsIssueAt;
                    session_Id = await accountSessionManager.InsertSession(accSessionEntity);
                }
                //account has a session now.
                if (session_Id > 0)
                {
                    foreach (var assertion in accIDPclaims.Assertions)
                    {
                        if (!String.IsNullOrEmpty(assertion.Value))
                        {
                            switch (assertion.Key.ToString())
                            {
                                //add session guid into account token response as session_state
                                case "session_state":
                                    accIDPclaims.Sessionstate = sessionGuid.ToString();
                                    break;
                            }
                        }
                    }
                    //set session guid to token for response value session_state
                    accIDPclaims.Sessionstate = sessionGuid.ToString();
                    accIDPclaims.TokenExpiresIn = token.expires_in;
                    accIDPclaims.IssuedAt = unixTimeSecondsIssueAt;
                    accIDPclaims.ValidTo = unixTimeSecondsExpiresAt;
                    accIDPclaims.Email = account.EmailId;
                    //set token identifier to account token
                    accIDPclaims.Id = tokenidentifier.ToString();
                    accToken = tokenManager.CreateToken(accIDPclaims);
                    if (accToken != null && !string.IsNullOrEmpty(accToken.AccessToken))
                    {
                        accTokenEntity = new IdentitySessionComponent.entity.AccountToken();
                        accTokenEntity.AccessToken = accToken.AccessToken;
                        accTokenEntity.AccountId = account.Id;
                        accTokenEntity.CreatedAt = unixTimeSecondsIssueAt;
                        accTokenEntity.ExpireIn = token.expires_in;
                        accTokenEntity.IdpType = IdentitySessionComponent.ENUM.IDPType.Keycloak;
                        accTokenEntity.TokenType = IdentitySessionComponent.ENUM.TokenType.Bearer;
                        accTokenEntity.Scope = accToken.Scope;
                        accTokenEntity.SessionState = accToken.SessionState;
                        accTokenEntity.Session_Id = session_Id;
                        accTokenEntity.TokenId = tokenidentifier.ToString();
                        accTokenEntity.UserId = account.Id;
                        accTokenEntity.UserName = account.EmailId;
                        int tokenkey = await accountTokenManager.InsertToken(accTokenEntity);
                        //token generated successfully hence adding token info & updating session info
                        accSessionEntity.LastSessionRefresh = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                        accSessionEntity.SessionExpiredAt = unixTimeSecondsExpiresAt;
                        session_Id = await accountSessionManager.UpdateSession(accSessionEntity);

                        accToken.statusCode = System.Net.HttpStatusCode.OK;
                        accToken.message = "Token is created and saved to databse.";
                    }
                    else
                    {
                        accToken.statusCode = System.Net.HttpStatusCode.Unauthorized;
                        accToken.message = "Custom token is not created";
                    }
                }
                else
                {
                    accToken.statusCode = System.Net.HttpStatusCode.InternalServerError;
                    accToken.message = "database session is not created";
                }
            }
            else
            {
                accToken.statusCode = idpResponse.StatusCode;
                accToken.message = Convert.ToString(idpResponse.Result);
            }

            return await Task.FromResult(accToken);
        }
        private Account GetAccountByEmail(string email)
        {
            Account account = new Account();

            AccountFilter filter = new AccountFilter();
            filter.Email = email;
            // filter.AccountType = AccountType.PortalAccount;            
            filter.AccountType = AccountType.None;
            IEnumerable<Account> result = accountManager.Get(filter).Result;
            foreach (var acc in result)
            {
                account = acc;
                break;//get only first account id
            }
            return account;
        }
    }
}
