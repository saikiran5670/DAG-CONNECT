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
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.email.Enum;
using System.Linq;

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
        private readonly IConfiguration configuration;
        private readonly IAuditTraillib auditlog;

        public AccountIdentityManager(IdentityComponent.ITokenManager _tokenManager, IdentityComponent.IAccountAuthenticator _autheticator,
                                    IAccountManager _accountManager, IAuditTraillib _auditlog,
                                    IdentitySessionComponent.IAccountSessionManager _accountSessionManager, IdentitySessionComponent.IAccountTokenManager _accountTokenManager,
                                    IdentityComponent.IAccountManager _identityAccountManager, IConfiguration _configuration)
        {
            autheticator = _autheticator;
            tokenManager = _tokenManager;
            accountManager = _accountManager;
            accountSessionManager = _accountSessionManager;
            accountTokenManager = _accountTokenManager;
            identityAccountManager = _identityAccountManager;
            configuration = _configuration;
            auditlog = _auditlog;
        }

        public async Task<AccountIdentity> Login(IdentityEntity.Identity user)
        {
            AccountIdentity accIdentity = new AccountIdentity();
            accIdentity.tokenIdentifier = string.Empty;
            accIdentity.ErrorMessage = "Account is not configured.";
            accIdentity.StatusCode = 1;
            IdentityEntity.AccountToken accToken = new IdentityEntity.AccountToken();
            Account account = GetAccountByEmail(user.UserName);
            if (account != null && account.Id > 0)
            {
                //Check if the user id blocked
                //1. Get Password Policy by Account Id
                var passwordPolicyAccount = await accountManager.GetPasswordPolicyAccount(account.Id);

                //2. Check isBlock = true, then return with error msg as contact to admin
                if (passwordPolicyAccount != null && passwordPolicyAccount.IsBlocked)
                {
                    accIdentity.ErrorMessage = AccountConstants.ERROR_ACCOUNT_BLOCKED;
                    accIdentity.StatusCode = 5;//TO Do: once fix the ResponceCode class we have change this accordingly. Applicable all the below lines in the method
                    return await Task.FromResult(accIdentity);
                }
                //3. Check if LockedUntil > Now date, Same as defual msg of failer
                if (passwordPolicyAccount != null && CheckLockedUntil(passwordPolicyAccount.LockedUntil))
                {
                    accIdentity.ErrorMessage = String.Format(AccountConstants.ERROR_ACCOUNT_LOCKED_INTERVAL, configuration["AccountPolicy:AccountUnlockDurationInMinutes"]);
                    accIdentity.StatusCode = 5;
                    return await Task.FromResult(accIdentity);
                }

                accToken = await PrepareSaveToken(user, account);
                if (accToken != null && accToken.statusCode == HttpStatusCode.OK)
                {
                    passwordPolicyAccount = await CaptureUserLastLogin(account);

                    IdentityEntity.AccountIDPClaim accIDPclaims = tokenManager.DecodeToken(accToken.AccessToken);

                    accIdentity.tokenIdentifier = accIDPclaims.Id;
                    accIdentity.accountInfo = account;
                    accIdentity.AccountOrganization = accountManager.GetAccountOrg(account.Id).Result;
                    accIdentity.AccountRole = accountManager.GetAccountRole(account.Id).Result;
                    #region commneted code
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
                    #endregion
                }//if check password max days expird
                else if (accToken?.statusCode == HttpStatusCode.BadRequest && CheckIsPasswordExpired(accToken?.message))
                {
                    //Generate Reset token with 302 response code
                    var identityResult = await GetResetToken(user);
                    accIdentity.StatusCode = (int)identityResult?.StatusCode;
                    if (identityResult?.StatusCode == HttpStatusCode.NotFound)
                    {
                        accIdentity.StatusCode = 3;
                        accIdentity.ErrorMessage = AccountConstants.ERROR_RESET_TOKEN_NOTFOUND;
                    }
                    if (identityResult?.StatusCode == HttpStatusCode.Redirect)
                        accIdentity.Token = new ExpiryToken(Convert.ToString(identityResult?.Result));
                }
                else
                {
                    //Implement Account lock logic
                    //Pre-Condition to Check, if PasswordPolicy not exist then upseting                    
                    passwordPolicyAccount = await accountManager.GetPasswordPolicyAccount(account.Id);
                    if (passwordPolicyAccount == null)
                    {
                        passwordPolicyAccount = new PasswordPolicyAccount();
                        passwordPolicyAccount.AccountId = account.Id;
                        await accountManager.UpsertPasswordPolicyAccount(passwordPolicyAccount);
                    }
                    //1. Increament to FailedLoginAttempts by one
                    passwordPolicyAccount.FailedLoginAttempts += 1;
                    //Calculate AccountLockAttempts

                    if (passwordPolicyAccount.FailedLoginAttempts % Convert.ToInt32(configuration["AccountPolicy:LoginAttemptThresholdLimit"]) == 0)
                    {
                        passwordPolicyAccount.LockedUntil = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(Convert.ToInt32(configuration["AccountPolicy:AccountUnlockDurationInMinutes"])));
                        passwordPolicyAccount.AccountLockAttempts += 1;
                        passwordPolicyAccount.FailedLoginAttempts = 0;
                        if (passwordPolicyAccount.AccountLockAttempts < Convert.ToInt32(configuration["AccountPolicy:AccountLockThresholdLimit"]))
                        {
                            await accountManager.UpsertPasswordPolicyAccount(passwordPolicyAccount);
                            await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Identity Component", "Account Identity Manager", AuditTrailEnum.Event_type.LOGIN, AuditTrailEnum.Event_status.FAILED, $"Incorrect login attempted - re-try after {configuration["AccountPolicy:AccountUnlockDurationInMinutes"]} minutes.-", 1, 2, account.EmailId);
                            accIdentity.ErrorMessage = String.Format(AccountConstants.ERROR_ACCOUNT_LOCKED_INTERVAL, configuration["AccountPolicy:AccountUnlockDurationInMinutes"]);
                            accIdentity.StatusCode = 5;
                            return await Task.FromResult(accIdentity);
                        }
                    }
                    // check in db to set proper not null - Not required

                    //2. Check if AccountLockAttempts < Config[AccountLockThresholdLimit]  
                    if (passwordPolicyAccount.AccountLockAttempts < Convert.ToInt32(configuration["AccountPolicy:AccountLockThresholdLimit"]))
                    {
                        await accountManager.UpsertPasswordPolicyAccount(passwordPolicyAccount);
                        accIdentity.ErrorMessage = AccountConstants.ERROR_ACCOUNT_LOGIN_FAILED;
                        await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Identity Component", "Account Identity Manager", AuditTrailEnum.Event_type.LOGIN, AuditTrailEnum.Event_status.FAILED, $"Incorrect login attempted count - {passwordPolicyAccount.FailedLoginAttempts} and Account Lock Attempts count - {passwordPolicyAccount.AccountLockAttempts}", 1, 2, account.EmailId);
                        accIdentity.StatusCode = 401;
                        return await Task.FromResult(accIdentity);
                    }
                    else
                    {
                        //Block
                        passwordPolicyAccount.IsBlocked = true;
                        await accountManager.UpsertPasswordPolicyAccount(passwordPolicyAccount);
                        await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Identity Component", "Account Identity Manager", AuditTrailEnum.Event_type.LOGIN, AuditTrailEnum.Event_status.FAILED, $"Incorrect login attempts, Account is locked.", 1, 2, account.EmailId);
                        accIdentity.ErrorMessage = AccountConstants.ERROR_ACCOUNT_BLOCKED;
                        accIdentity.StatusCode = 5;
                        return await Task.FromResult(accIdentity);
                    }
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
                accIdentity.accountInfo = account;
                IdentityEntity.Response idpResponse = await autheticator.AccessToken(user);
                if (idpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    IdentityEntity.IDPToken token = JsonConvert.DeserializeObject<IdentityEntity.IDPToken>(Convert.ToString(idpResponse.Result));
                    IdentityEntity.AccountIDPClaim accIDPclaims = tokenManager.DecodeToken(token.access_token);

                    accIDPclaims.TokenExpiresIn = token.expires_in;

                    IdentityEntity.AccountToken accToken = tokenManager.CreateToken(accIDPclaims);
                    accIdentity.AccountOrganization = accountManager.GetAccountOrg(account.Id).Result;
                    accIdentity.AccountRole = accountManager.GetAccountRole(account.Id).Result;
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
                accToken.message = "Account is not present";
            }
            return accToken;
        }
        public async Task<IdentityEntity.AccountToken> GenerateTokenGUID(IdentityEntity.Identity user)
        {
            string tokenIdentifier = string.Empty;
            IdentityEntity.AccountToken accToken = new IdentityEntity.AccountToken();
            Account account = GetAccountByEmail(user.UserName);
            if (account != null && account.Id > 0)
            {
                accToken = await PrepareSaveToken(user, account);
                if (accToken != null && accToken.statusCode == HttpStatusCode.OK)
                {
                    IdentityEntity.AccountIDPClaim accIDPclaims = tokenManager.DecodeToken(accToken.AccessToken);
                    //replacing jwt access token with guid based access token
                    accToken.AccessToken = accIDPclaims.Id;
                }
            }
            else
            {
                accToken.statusCode = System.Net.HttpStatusCode.NotFound;
                accToken.message = "Account is not present";
            }
            return accToken;
        }

        public async Task<bool> ValidateToken(string token)
        {
            bool result = false;
            result = await tokenManager.ValidateToken(token);
            if (result)
            {
                ValidTokenResponse response = await ValidateJwtToken(token);
                result = response.Valid;
            }
            return await Task.FromResult(result);
        }
        public async Task<ValidTokenResponse> ValidateTokenGuid(string token)
        {
            ValidTokenResponse response = await ValidateAndFetchTokenDetails(token);
            return response;
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
                        IdentityEntity.Identity identity = new IdentityEntity.Identity();
                        identity.UserName = accIDPclaims.Email;
                        IdentityEntity.Response response = await identityAccountManager.LogOut(identity);
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

                string emailid = await GetEmailByAccountId(accountId);
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
        private async Task<ValidTokenResponse> ValidateJwtToken(string token)
        {
            ValidTokenResponse response = new ValidTokenResponse();
            response.Valid = false;
            //decode token to extract token identifier, sessoin state and email
            IdentityEntity.AccountIDPClaim accIDPclaims = tokenManager.DecodeToken(token);
            if (accIDPclaims != null && !string.IsNullOrEmpty(accIDPclaims.Id))
            {
                response = await ValidateAndFetchTokenDetails(accIDPclaims.Id);
            }
            else
                response.Valid = false;
            return response;
        }
        private async Task<ValidTokenResponse> ValidateAndFetchTokenDetails(string tokenGuid)
        {
            ValidTokenResponse response = new ValidTokenResponse();
            response.Valid = false;
            int accountid = 0;
            int sessionid = 0;
            response.TokenIdentifier = tokenGuid;
            //check token is available in account token 
            IEnumerable<IdentitySessionComponent.entity.AccountToken> tokenlst = await accountTokenManager.GetTokenDetails(tokenGuid);
            foreach (var item in tokenlst)
            {
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(item.CreatedAt).ToLocalTime();
                DateTime exdateTime = dtDateTime.AddSeconds(item.ExpireIn);
                int result = DateTime.Compare(exdateTime, DateTime.Now);
                if (result > 0)
                {
                    accountid = item.AccountId;
                    response.AccountId = item.AccountId;
                }
                break;
            }            //check session is available in account session
            if (accountid > 0)
            {
                IEnumerable<IdentitySessionComponent.entity.AccountSession> sessionlst = await accountSessionManager.GetAccountSession(accountid);
                foreach (var item in tokenlst)
                {
                    sessionid = item.Session_Id;
                    response.SessionId = item.Session_Id;
                    break;
                }
                if (sessionid > 0)
                {
                    response.Valid = true;
                    response.Email = await GetEmailByAccountId(accountid);
                }
                else
                    response.Valid = false;
            }
            else
                response.Valid = false;

            return response;
        }
        private async Task LogoutFromIDP(int accountId)
        {
            string emailid = await GetEmailByAccountId(accountId);
            if (!string.IsNullOrEmpty(emailid))
            {   //sign out account from IDP using email
                IdentityEntity.Identity identity = new IdentityEntity.Identity();
                identity.UserName = emailid;
                IdentityEntity.Response response = await identityAccountManager.LogOut(identity);
            }
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
                        /* Assigning NULL as access token from db is not require after GUID implementation
                         * accTokenEntity.AccessToken = accToken.AccessToken;*/
                        accTokenEntity.AccessToken = null;

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
                        accTokenEntity.OrganizationId = account.Organization_Id.Value;
                        int tokenkey = await accountTokenManager.InsertToken(accTokenEntity);
                        //token generated successfully hence adding token info & updating session info
                        accSessionEntity.LastSessionRefresh = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                        accSessionEntity.SessionExpiredAt += unixTimeSecondsExpiresAt;
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

        private async Task<Response> GetResetToken(Identity user)
        {
            try
            {
                var result = await accountManager.ResetPasswordInitiate(user.UserName, 0, EmailEventType.PasswordExpiryNotification);
                result.StatusCode = result.StatusCode == HttpStatusCode.OK ? HttpStatusCode.Redirect : HttpStatusCode.NotFound;
                return result;
            }
            catch (Exception e)
            {
                return new Response { StatusCode = HttpStatusCode.NotFound };
            }
        }

        private bool CheckIsPasswordExpired(string message)
        {
            var identityResponseContent = JsonConvert.DeserializeObject<IdentityResponse>(message);
            return identityResponseContent.Error == AccountConstants.ERROR_INVALID_GRANT &&
                    identityResponseContent.Error_Description == AccountConstants.ERROR_PWD_EXPIRED;
        }

        private async Task<PasswordPolicyAccount> CaptureUserLastLogin(Account account)
        {
            PasswordPolicyAccount passwordPolicyAccount = new PasswordPolicyAccount();
            passwordPolicyAccount.AccountId = account.Id;
            passwordPolicyAccount.LastLogin = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            await accountManager.UpsertPasswordPolicyAccount(passwordPolicyAccount);
            return passwordPolicyAccount;
        }

        private bool CheckLockedUntil(long? lockedUntil)
        {
            if (lockedUntil != null && lockedUntil >= UTCHandling.GetUTCFromDateTime(DateTime.Now))
                return true;
            return false;
        }
        private async Task<string> GetEmailByAccountId(int accountId)
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
            return emailid;
        }


        #region Signle Sign On
        /// <summary>
        /// To generate valid SSO toke with session refresh operation
        /// </summary>
        /// <param name="Email">Current user email id</param>
        /// <returns>Token, TokenType, StatusCode, Message</returns>
        public async Task<SSOToken> GenerateSSOToken(TokenSSORequest request)
        {
            SSOToken ssoToken = new SSOToken();
            // Account account = GetAccountByEmail(request.Email);
            if (request.AccountID > 0)
            {
                IdentitySessionComponent.entity.AccountToken _latestToken = await GetAccountTokenDetails(request.AccountID);
                if (_latestToken?.Id > 0)
                {
                    int _recentTokenExpirIn = _latestToken.ExpireIn;
                    Guid _ssoGuid = Guid.NewGuid();

                    // Preparing SSO Token with new TokenId (GUID) and Token Type as 'S'
                    IdentitySessionComponent.entity.AccountToken _newSSOToken = _latestToken;
                    _newSSOToken.CreatedAt = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
                    _newSSOToken.TokenType = IdentitySessionComponent.ENUM.TokenType.SSO;
                    _newSSOToken.TokenId = Convert.ToString(_ssoGuid);
                    _newSSOToken.ExpireIn = _recentTokenExpirIn;
                    _newSSOToken.AccountId = request.AccountID;
                    _newSSOToken.OrganizationId = request.OrganizaitonID;

                    // Sotring toen details for future validation
                    int tokenkey = await accountTokenManager.InsertToken(_newSSOToken);

                    // Keeping session data updated with new expiryAt
                    IdentitySessionComponent.entity.AccountSession _latestSession = await GetAccountSessionDetails(_latestToken.Session_Id);
                    if (_latestSession?.Id > 0)
                    {
                        _latestSession.LastSessionRefresh = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                        _latestSession.SessionExpiredAt += _recentTokenExpirIn;
                        var session_Id = await accountSessionManager.UpdateSession(_latestSession);
                    }

                    // To Return valid SSO details 
                    ssoToken.token = Convert.ToString(_ssoGuid);
                    ssoToken.tokenType = IdentitySessionComponent.ENUM.TokenType.SSO.ToString();
                    ssoToken.statusCode = HttpStatusCode.Redirect;
                    ssoToken.message = "Request to redirected";
                    return ssoToken;
                }
                else
                {
                    ssoToken.statusCode = System.Net.HttpStatusCode.NotFound;
                    ssoToken.message = "Token is not found";
                }
            }
            else
            {
                ssoToken.statusCode = System.Net.HttpStatusCode.NotFound;
                ssoToken.message = "Account is not present";
            }
            return ssoToken;
        }

        /// <summary>
        /// Validate SSO token against user and its credentials
        /// </summary>
        /// <param name="tokenGuid">SSO GUID token shared with third party</param>
        /// <returns> 
        ///     "OrganizationID": "CRM_CUSTOMER_ID",
        ///     "OrganizationName": "CRM_CUSTOMER_NAME",
        ///     "AccountID": "ACCOUNT_ID",
        ///     "AccountName": "ACCOUNT_NAME",
        ///     "RoleID": "ROLE_CODE",
        ///     "TimeZone": "Europe/Amsterdam",
        ///     "DateFormat": "dd/mm/yy HH:MM",
        ///     "UnitDisplay": "metric",
        ///     "VehicleDisplay": "vin"
        ///  </returns>
        public async Task<SSOTokenResponse> ValidateSSOToken(string tokenGuid)
        {
            SSOTokenResponse _ssoTokenResponse = null;
            IEnumerable<IdentitySessionComponent.entity.AccountToken> _tokenlist = await accountTokenManager.GetTokenDetails(tokenGuid);
            // TODO: delete after testing
            //IdentitySessionComponent.entity.AccountToken _savedToeken = _tokenlist.FirstOrDefault();
            IdentitySessionComponent.entity.AccountToken _savedToeken = _tokenlist.Where(token => token.TokenType == IdentitySessionComponent.ENUM.TokenType.SSO).FirstOrDefault();
            if (_savedToeken?.AccountId > 0)
            {
                if (UtcDateCompare(_savedToeken.CreatedAt, _savedToeken.ExpireIn))
                {
                    IdentitySessionComponent.entity.AccountSession _latestSession = await GetAccountSessionDetails(_savedToeken.Session_Id);
                    if (_latestSession?.Id > 0)
                    {
                        // Get users SSO Details and return it back
                        _ssoTokenResponse = await accountManager.GetAccountSSODetails(_savedToeken.AccountId);
                    }
                }
            }
            return _ssoTokenResponse;
        }
        private async Task<identitysession.entity.AccountToken> GetAccountTokenDetails(int _accountID)
        {

            var accountTokens = await accountTokenManager.GetTokenDetails(_accountID);
            identitysession.entity.AccountToken lastestToken = accountTokens.OrderByDescending(__token => __token.CreatedAt).FirstOrDefault();

            return await Task.FromResult(lastestToken);
        }

        private async Task<identitysession.entity.AccountSession> GetAccountSessionDetails(int _sessionID)
        {

            identitysession.entity.AccountSession lastestSession = await accountSessionManager.GetAccountSessionById(_sessionID);

            return await Task.FromResult(lastestSession);
        }

        private bool UtcDateCompare(long UnixFromDate, long UnixToDate)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(UnixFromDate).ToLocalTime();
            DateTime exdateTime = dtDateTime.AddSeconds(UnixToDate);
            int result = DateTime.Compare(exdateTime, DateTime.Now);
            return result > 0 ? true : false;
        }

        #endregion

    }
}
