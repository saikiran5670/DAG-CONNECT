using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.account.ENUM;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.utilities;
using Newtonsoft.Json;
using IdentityComponent = net.atos.daf.ct2.identity;
using IdentityEntity = net.atos.daf.ct2.identity.entity;
using IdentitySessionComponent = net.atos.daf.ct2.identitysession;

namespace net.atos.daf.ct2.account
{
    public class AccountIdentityManager : IAccountIdentityManager
    {
        readonly IdentityComponent.ITokenManager _tokenManager;
        readonly IdentityComponent.IAccountAuthenticator _autheticator;
        readonly IdentityComponent.IAccountManager _identityAccountManager;
        readonly IAccountManager _accountManager;
        readonly IdentitySessionComponent.IAccountSessionManager _accountSessionManager;
        readonly IdentitySessionComponent.IAccountTokenManager _accountTokenManager;
        private readonly IConfiguration _configuration;
        private readonly IAuditTraillib _auditlog;
        private readonly SSOConfiguration _ssoConfiguration;

        public AccountIdentityManager(IdentityComponent.ITokenManager TokenManager, IdentityComponent.IAccountAuthenticator Autheticator,
                                    IAccountManager AccountManager, IAuditTraillib Auditlog,
                                    IdentitySessionComponent.IAccountSessionManager AccountSessionManager, IdentitySessionComponent.IAccountTokenManager AccountTokenManager,
                                    IdentityComponent.IAccountManager IdentityAccountManager, IConfiguration Configuration)
        {
            this._autheticator = Autheticator;
            this._tokenManager = TokenManager;
            this._accountManager = AccountManager;
            this._accountSessionManager = AccountSessionManager;
            this._accountTokenManager = AccountTokenManager;
            this._identityAccountManager = IdentityAccountManager;
            this._configuration = Configuration;
            this._auditlog = Auditlog;
            _ssoConfiguration = new SSOConfiguration();
            Configuration.GetSection("SSOConfiguration").Bind(_ssoConfiguration);

        }

        public async Task<AccountIdentity> Login(IdentityEntity.Identity user)
        {
            int roleId = 0;
            AccountIdentity accIdentity = new AccountIdentity();
            accIdentity.TokenIdentifier = string.Empty;
            accIdentity.ErrorMessage = "Account is not configured.";
            accIdentity.StatusCode = 1;
            //IdentityEntity.AccountToken accToken = new IdentityEntity.AccountToken();
            Account account = GetAccountByEmail(user.UserName);
            if (account != null && account.Id > 0)
            {
                List<AccountOrgRole> accountOrgRoleList = await _accountManager.GetAccountRole(account.Id);
                foreach (AccountOrgRole aor in accountOrgRoleList)
                {
                    roleId = aor.Id;
                    break; //get only first role 
                }
                //Check if the user id blocked
                //1. Get Password Policy by Account Id
                var passwordPolicyAccount = await _accountManager.GetPasswordPolicyAccount(account.Id);

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
                    accIdentity.ErrorMessage = String.Format(AccountConstants.ERROR_ACCOUNT_LOCKED_INTERVAL, _configuration["AccountPolicy:AccountUnlockDurationInMinutes"]);
                    accIdentity.StatusCode = 5;
                    return await Task.FromResult(accIdentity);
                }

                IdentityEntity.AccountToken accToken = await PrepareSaveToken(user, account, roleId);
                if (accToken != null && accToken.StatusCode == HttpStatusCode.OK)
                {
                    await CaptureUserLastLogin(account);

                    IdentityEntity.AccountIDPClaim accIDPclaims = _tokenManager.DecodeToken(accToken.AccessToken);

                    accIdentity.TokenIdentifier = accIDPclaims.Id;
                    accIdentity.AccountInfo = account;
                    accIdentity.AccountOrganization = _accountManager.GetAccountOrg(account.Id).Result;
                    accIdentity.AccountRole = accountOrgRoleList;
                }//if check password max days expired
                else if (accToken?.StatusCode == HttpStatusCode.BadRequest && CheckIsPasswordExpired(accToken?.Message))
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
                    passwordPolicyAccount = await _accountManager.GetPasswordPolicyAccount(account.Id);
                    if (passwordPolicyAccount == null)
                    {
                        passwordPolicyAccount = new PasswordPolicyAccount();
                        passwordPolicyAccount.AccountId = account.Id;
                        await _accountManager.UpsertPasswordPolicyAccount(passwordPolicyAccount);
                    }
                    //1. Increament to FailedLoginAttempts by one
                    passwordPolicyAccount.FailedLoginAttempts += 1;
                    //Calculate AccountLockAttempts

                    if (passwordPolicyAccount.FailedLoginAttempts % Convert.ToInt32(_configuration["AccountPolicy:LoginAttemptThresholdLimit"]) == 0)
                    {
                        passwordPolicyAccount.LockedUntil = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["AccountPolicy:AccountUnlockDurationInMinutes"])));
                        passwordPolicyAccount.AccountLockAttempts += 1;
                        passwordPolicyAccount.FailedLoginAttempts = 0;
                        if (passwordPolicyAccount.AccountLockAttempts < Convert.ToInt32(_configuration["AccountPolicy:AccountLockThresholdLimit"]))
                        {
                            await _accountManager.UpsertPasswordPolicyAccount(passwordPolicyAccount);
                            await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Identity Component", "Account Identity Manager", AuditTrailEnum.Event_type.LOGIN, AuditTrailEnum.Event_status.FAILED, $"Incorrect login attempted - re-try after {_configuration["AccountPolicy:AccountUnlockDurationInMinutes"]} minutes.-", 1, 2, account.EmailId);
                            accIdentity.ErrorMessage = String.Format(AccountConstants.ERROR_ACCOUNT_LOCKED_INTERVAL, _configuration["AccountPolicy:AccountUnlockDurationInMinutes"]);
                            accIdentity.StatusCode = 5;
                            return await Task.FromResult(accIdentity);
                        }
                    }
                    // check in db to set proper not null - Not required

                    //2. Check if AccountLockAttempts < Config[AccountLockThresholdLimit]  
                    if (passwordPolicyAccount.AccountLockAttempts < Convert.ToInt32(_configuration["AccountPolicy:AccountLockThresholdLimit"]))
                    {
                        await _accountManager.UpsertPasswordPolicyAccount(passwordPolicyAccount);
                        accIdentity.ErrorMessage = AccountConstants.ERROR_ACCOUNT_LOGIN_FAILED;
                        await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Identity Component", "Account Identity Manager", AuditTrailEnum.Event_type.LOGIN, AuditTrailEnum.Event_status.FAILED, $"Incorrect login attempted count - {passwordPolicyAccount.FailedLoginAttempts} and Account Lock Attempts count - {passwordPolicyAccount.AccountLockAttempts}", 1, 2, account.EmailId);
                        accIdentity.StatusCode = 401;
                        return await Task.FromResult(accIdentity);
                    }
                    else
                    {
                        //Block
                        passwordPolicyAccount.IsBlocked = true;
                        await _accountManager.UpsertPasswordPolicyAccount(passwordPolicyAccount);
                        await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Identity Component", "Account Identity Manager", AuditTrailEnum.Event_type.LOGIN, AuditTrailEnum.Event_status.FAILED, $"Incorrect login attempts, Account is locked.", 1, 2, account.EmailId);
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
            accIdentity.TokenIdentifier = string.Empty;
            Account account = GetAccountByEmail(user.UserName);
            if (account != null && account.Id > 0)
            {
                accIdentity.AccountInfo = account;
                IdentityEntity.Response idpResponse = await _autheticator.AccessToken(user);
                if (idpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    IdentityEntity.IDPToken token = JsonConvert.DeserializeObject<IdentityEntity.IDPToken>(Convert.ToString(idpResponse.Result));
                    IdentityEntity.AccountIDPClaim accIDPclaims = _tokenManager.DecodeToken(token.Access_token);

                    accIDPclaims.TokenExpiresIn = token.Expires_in;

                    IdentityEntity.AccountToken accToken = _tokenManager.CreateToken(accIDPclaims);
                    accIdentity.AccountOrganization = _accountManager.GetAccountOrg(account.Id).Result;
                    accIdentity.AccountRole = _accountManager.GetAccountRole(account.Id).Result;
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
                int roleId = 0;
                List<AccountOrgRole> accountOrgRoleList = await _accountManager.GetAccountRole(account.Id);
                foreach (AccountOrgRole aor in accountOrgRoleList)
                {
                    roleId = aor.Id;
                    break; //get only first role 
                }
                accToken = await PrepareSaveToken(user, account, roleId);
            }
            else
            {
                accToken.StatusCode = System.Net.HttpStatusCode.NotFound;
                accToken.Message = "Account is not present";
            }
            return accToken;
        }
        public async Task<IdentityEntity.AccountToken> GenerateTokenGUID(IdentityEntity.Identity user)
        {
            IdentityEntity.AccountToken accToken = new IdentityEntity.AccountToken();
            Account account = GetAccountByEmail(user.UserName);
            if (account != null && account.Id > 0)
            {
                int roleId = 0;
                List<AccountOrgRole> accountOrgRoleList = await _accountManager.GetAccountRole(account.Id);
                foreach (AccountOrgRole aor in accountOrgRoleList)
                {
                    roleId = aor.Id;
                    break; //get only first role 
                }
                accToken = await PrepareSaveToken(user, account, roleId);
                if (accToken != null && accToken.StatusCode == HttpStatusCode.OK)
                {
                    IdentityEntity.AccountIDPClaim accIDPclaims = _tokenManager.DecodeToken(accToken.AccessToken);
                    //replacing jwt access token with guid based access token
                    accToken.AccessToken = accIDPclaims.Id;
                }
            }
            else
            {
                accToken.StatusCode = System.Net.HttpStatusCode.NotFound;
                accToken.Message = "Account is not present";
            }
            return accToken;
        }

        public async Task<bool> ValidateToken(string token)
        {
            bool result = await _tokenManager.ValidateToken(token);
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
                IdentityEntity.AccountIDPClaim accIDPclaims = _tokenManager.DecodeToken(token);
                if (accIDPclaims != null && !string.IsNullOrEmpty(accIDPclaims.Id))
                {
                    //delete token by passing token identifier extracted from token
                    int accountid = await _accountTokenManager.DeleteTokenByTokenId(Guid.Parse(accIDPclaims.Id));
                    //check if another token are availble for same account 
                    int tokencount = await _accountTokenManager.GetTokenCount(accountid);
                    if (tokencount == 0)
                    {
                        //no token belong to this session hence delete the token
                        int sessionid = await _accountSessionManager.DeleteSession(accIDPclaims.Sessionstate);
                        //sign out account from IDP by using username
                        IdentityEntity.Identity identity = new IdentityEntity.Identity();
                        identity.UserName = accIDPclaims.Email;
                        IdentityEntity.Response response = await _identityAccountManager.LogOut(identity);
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
                await _accountTokenManager.DeleteTokenbyAccountId(accountId);
                await _accountSessionManager.DeleteSessionByAccountId(accountId);

                string emailid = await GetEmailByAccountId(accountId);
                if (!string.IsNullOrEmpty(emailid))
                {   //sign out account from IDP using email
                    IdentityEntity.Identity identity = new IdentityEntity.Identity();
                    identity.UserName = emailid;
                    IdentityEntity.Response response = await _identityAccountManager.LogOut(identity);
                }
                isLogout = true;
            }
            return await Task.FromResult(isLogout);
        }

        public async Task<bool> LogoutByTokenId(string tokenid)
        {
            bool isLogout = false;
            //delete token by passing token identifier extracted from token
            int accountid = await _accountTokenManager.DeleteTokenByTokenId(Guid.Parse(tokenid));
            if (accountid > 0)
            {
                //check if another token are availble for same account 
                int tokencount = await _accountTokenManager.GetTokenCount(accountid);
                if (tokencount == 0)
                {
                    //no token belong to this session hence delete the token
                    int sessionid = await _accountSessionManager.DeleteSessionByAccountId(accountid);
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
            IdentityEntity.AccountIDPClaim accIDPclaims = _tokenManager.DecodeToken(token);
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
            IEnumerable<IdentitySessionComponent.entity.AccountToken> tokenlst = await _accountTokenManager.GetTokenDetails(tokenGuid);
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
                IEnumerable<IdentitySessionComponent.entity.AccountSession> sessionlst = await _accountSessionManager.GetAccountSession(accountid);
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
                IdentityEntity.Response response = await _identityAccountManager.LogOut(identity);
            }
        }
        private async Task<IdentityEntity.AccountToken> PrepareSaveToken(IdentityEntity.Identity user, Account account, int roleId)
        {
            IdentityEntity.AccountToken accToken = new IdentityEntity.AccountToken();
            //generate idp token 
            IdentityEntity.Response idpResponse = await _autheticator.AccessToken(user);
            if (idpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                IdentityEntity.IDPToken token = JsonConvert.DeserializeObject<IdentityEntity.IDPToken>(Convert.ToString(idpResponse.Result));
                IdentityEntity.AccountIDPClaim accIDPclaims = _tokenManager.DecodeToken(token.Access_token);
                var now = DateTime.Now;
                long unixTimeSecondsIssueAt = new DateTimeOffset(now).ToUnixTimeSeconds();
                long unixTimeSecondsExpiresAt = 0;
                if (token.Expires_in > 0)
                {
                    unixTimeSecondsExpiresAt = new DateTimeOffset(now.AddSeconds(token.Expires_in)).ToUnixTimeSeconds();
                }
                int session_Id = 0;
                Guid sessionGuid = Guid.NewGuid();
                Guid tokenidentifier = Guid.NewGuid();
                IdentitySessionComponent.entity.AccountSession accSessionEntity = new IdentitySessionComponent.entity.AccountSession();
                //IdentitySessionComponent.entity.AccountToken accTokenEntity = new IdentitySessionComponent.entity.AccountToken();
                //check if session is present for this account.
                IEnumerable<IdentitySessionComponent.entity.AccountSession> sessionlist = await _accountSessionManager.GetAccountSession(account.Id);
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
                    session_Id = await _accountSessionManager.InsertSession(accSessionEntity);
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
                    accIDPclaims.TokenExpiresIn = token.Expires_in;
                    accIDPclaims.IssuedAt = unixTimeSecondsIssueAt;
                    accIDPclaims.ValidTo = unixTimeSecondsExpiresAt;
                    accIDPclaims.Email = account.EmailId;
                    //set token identifier to account token
                    accIDPclaims.Id = tokenidentifier.ToString();
                    accToken = _tokenManager.CreateToken(accIDPclaims);
                    if (accToken != null && !string.IsNullOrEmpty(accToken.AccessToken))
                    {
                        IdentitySessionComponent.entity.AccountToken accTokenEntity = new IdentitySessionComponent.entity.AccountToken();
                        /* Assigning NULL as access token from db is not require after GUID implementation
                         * accTokenEntity.AccessToken = accToken.AccessToken;*/
                        accTokenEntity.AccessToken = string.Empty;

                        accTokenEntity.AccountId = account.Id;
                        accTokenEntity.CreatedAt = unixTimeSecondsIssueAt;
                        accTokenEntity.ExpireIn = token.Expires_in;
                        accTokenEntity.IdpType = IdentitySessionComponent.ENUM.IDPType.Keycloak;
                        accTokenEntity.TokenType = IdentitySessionComponent.ENUM.TokenType.Bearer;
                        accTokenEntity.Scope = accToken.Scope;
                        accTokenEntity.SessionState = accToken.SessionState;
                        accTokenEntity.Session_Id = session_Id;
                        accTokenEntity.TokenId = tokenidentifier.ToString();
                        accTokenEntity.UserId = account.Id;
                        accTokenEntity.UserName = account.EmailId;
                        accTokenEntity.RoleId = roleId;
                        accTokenEntity.OrganizationId = account.Organization_Id.Value;
                        int tokenkey = await _accountTokenManager.InsertToken(accTokenEntity);
                        //token generated successfully hence adding token info & updating session info
                        accSessionEntity.LastSessionRefresh = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                        accSessionEntity.SessionExpiredAt += unixTimeSecondsExpiresAt;
                        await _accountSessionManager.UpdateSession(accSessionEntity);

                        accToken.StatusCode = System.Net.HttpStatusCode.OK;
                        accToken.Message = "Token is created and saved to databse.";
                    }
                    else
                    {
                        accToken.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                        accToken.Message = "Custom token is not created";
                    }
                }
                else
                {
                    accToken.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                    accToken.Message = "database session is not created";
                }
            }
            else
            {
                accToken.StatusCode = idpResponse.StatusCode;
                accToken.Message = Convert.ToString(idpResponse.Result);
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
            IEnumerable<Account> result = _accountManager.Get(filter).Result;
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
                var result = await _accountManager.ResetPasswordInitiate(user.UserName, EmailEventType.PasswordExpiryNotification);
                result.StatusCode = result.StatusCode == HttpStatusCode.OK ? HttpStatusCode.Redirect : HttpStatusCode.NotFound;
                return result;
            }
            catch (Exception)
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
            await _accountManager.UpsertPasswordPolicyAccount(passwordPolicyAccount);
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
            IEnumerable<Account> accounts = await _accountManager.Get(filter);
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
            if (request?.AccountID > 0)
            {
                IdentitySessionComponent.entity.AccountToken latestToken = await GetAccountTokenDetails(request.AccountID);
                if (latestToken?.Id > 0)
                {
                    int recentTokenExpirIn = latestToken.ExpireIn;
                    Guid ssoGuid = Guid.NewGuid();

                    // Preparing SSO Token with new TokenId (GUID) and Token Type as 'S'
                    IdentitySessionComponent.entity.AccountToken newSSOToken = latestToken;
                    newSSOToken.AccessToken = string.Empty;
                    newSSOToken.CreatedAt = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
                    newSSOToken.TokenType = IdentitySessionComponent.ENUM.TokenType.SSO;
                    newSSOToken.TokenId = Convert.ToString(ssoGuid);
                    newSSOToken.ExpireIn = recentTokenExpirIn;
                    newSSOToken.AccountId = request.AccountID;
                    newSSOToken.OrganizationId = request.OrganizaitonID;
                    newSSOToken.RoleId = request.RoleID;

                    // Sotring toen details for future validation
                    int tokenkey = await _accountTokenManager.InsertToken(newSSOToken);

                    // Keeping session data updated with new expiryAt
                    IdentitySessionComponent.entity.AccountSession latestSession = await GetAccountSessionDetails(latestToken.Session_Id);
                    if (latestSession?.Id > 0)
                    {
                        latestSession.LastSessionRefresh = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                        latestSession.SessionExpiredAt += recentTokenExpirIn;
                        var session_Id = await _accountSessionManager.UpdateSession(latestSession);
                    }

                    // To Return valid SSO details 
                    ssoToken.Token = _ssoConfiguration.ZuoraBaseUrl + "=" + Convert.ToString(ssoGuid);
                    ssoToken.TokenType = IdentitySessionComponent.ENUM.TokenType.SSO.ToString();
                    ssoToken.StatusCode = HttpStatusCode.OK;
                    ssoToken.Message = "Request to redirect";
                    return ssoToken;
                }
                else
                {
                    ssoToken.StatusCode = System.Net.HttpStatusCode.NotFound;
                    ssoToken.Message = "User or User Token not found";
                }
            }
            else
            {
                ssoToken.StatusCode = System.Net.HttpStatusCode.NotFound;
                ssoToken.Message = "Invalid Account";
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
        public async Task<SSOResponse> ValidateSSOToken(string tokenGuid)
        {
            SSOResponse ssoResponse = new SSOResponse();
            IEnumerable<IdentitySessionComponent.entity.AccountToken> tokenList = await _accountTokenManager.GetTokenDetails(tokenGuid);
            // TODO: delete after testing
            //IdentitySessionComponent.entity.AccountToken _savedToeken = _tokenlist.FirstOrDefault();
            IdentitySessionComponent.entity.AccountToken savedToken = tokenList.Where(token => token.TokenType == IdentitySessionComponent.ENUM.TokenType.SSO).FirstOrDefault();
            if (savedToken?.AccountId > 0)
            {
                if (UtcDateCompare(savedToken.CreatedAt, savedToken.ExpireIn))
                {
                    IdentitySessionComponent.entity.AccountSession latestSession = await GetAccountSessionDetails(savedToken.Session_Id);
                    if (latestSession?.Id > 0)
                    {
                        // Get users SSO Details and return it back
                        ssoResponse.Details = await _accountManager.GetAccountSSODetails(savedToken);
                    }
                }
                else
                {
                    ssoResponse.StatusCode = HttpStatusCode.NotFound;
                    ssoResponse.Message = "TOKEN_EXPIRED";
                    ssoResponse.Value = tokenGuid;
                }
            }
            else
            {
                ssoResponse.StatusCode = HttpStatusCode.NotFound;
                ssoResponse.Message = "INVALID_TOKEN";
                ssoResponse.Value = tokenGuid;
            }
            return ssoResponse;
        }
        private async Task<identitysession.entity.AccountToken> GetAccountTokenDetails(int accountID)
        {

            var accountTokens = await _accountTokenManager.GetTokenDetails(accountID);
            identitysession.entity.AccountToken lastestToken = accountTokens.OrderByDescending(token => token.CreatedAt).FirstOrDefault();

            return await Task.FromResult(lastestToken);
        }

        private async Task<identitysession.entity.AccountSession> GetAccountSessionDetails(int sessionID)
        {

            identitysession.entity.AccountSession lastestSession = await _accountSessionManager.GetAccountSessionById(sessionID);

            return await Task.FromResult(lastestSession);
        }

        private bool UtcDateCompare(long unixFromDate, long unixToDate)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixFromDate).ToLocalTime();
            DateTime exdateTime = dtDateTime.AddSeconds(unixToDate);
            int result = DateTime.Compare(exdateTime, DateTime.Now);
            return result > 0 ? true : false;
        }

        #endregion

    }
}
