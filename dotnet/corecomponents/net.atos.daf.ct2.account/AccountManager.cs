using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.account.ENUM;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.email;
using net.atos.daf.ct2.email.Entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.utilities;
using Identity = net.atos.daf.ct2.identity;
using IdentityEntity = net.atos.daf.ct2.identity.entity;
using IdentitySessionEntity = net.atos.daf.ct2.identitysession.entity;

namespace net.atos.daf.ct2.account
{
    public class AccountManager : IAccountManager
    {
        IAccountRepository _repository;
        Identity.IAccountManager _identity;
        IAuditTraillib _auditlog;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IConfiguration _configuration;
        private readonly ITranslationManager _translationManager;

        public AccountManager(IAccountRepository Repository, IAuditTraillib Auditlog, Identity.IAccountManager Identity, IConfiguration Configuration, ITranslationManager TranslationManager)
        {
            this._repository = Repository;
            this._auditlog = Auditlog;
            this._identity = Identity;
            this._configuration = Configuration;
            _emailConfiguration = new EmailConfiguration();
            Configuration.GetSection("EmailConfiguration").Bind(_emailConfiguration);
            this._translationManager = TranslationManager;
        }
        public async Task<Account> Create(Account account)
        {
            // create user in identity
            IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
            identityEntity.UserName = account.EmailId;
            identityEntity.EmailId = account.EmailId;
            identityEntity.FirstName = account.FirstName;
            identityEntity.LastName = account.LastName;
            identityEntity.Password = account.Password;

            //TODO: If created in IDP, but have exception while create in DB.
            var identityresult = await _identity.CreateUser(identityEntity);

            if (identityresult.StatusCode == HttpStatusCode.Created)
            {
                // if this fails
                account = await _repository.Create(account);

                account.IsErrorInEmail = !(await SetPasswordViaEmail(account, EmailEventType.CreateAccount));
            }
            else // there is issues and need delete user from IDP. 
            {
                // user already exits in IDP.
                if (identityresult.StatusCode == HttpStatusCode.Conflict)
                {
                    // get account by email , if not exists in DB-- create it
                    AccountFilter filter = new AccountFilter();
                    filter.Email = account.EmailId;
                    filter.OrganizationId = account.Organization_Id.Value;
                    int Organization_Id = account.Organization_Id.Value;
                    filter.AccountType = AccountType.None;
                    filter.AccountIds = string.Empty;
                    filter.Name = string.Empty;
                    var accountGet = await _repository.Duplicate(filter);
                    if (accountGet == null)
                    {
                        account = await _repository.Create(account);
                        await _identity.UpdateUser(identityEntity);

                        account.IsErrorInEmail = !(await SetPasswordViaEmail(account, EmailEventType.CreateAccount));
                    }
                    else
                    {
                        account = accountGet;
                        account.IsDuplicate = true;
                        if (Organization_Id == account.Organization_Id)
                        {
                            account.IsDuplicateInOrg = true;
                        }
                    }
                }
                // inter server error in IDP.
                else if (identityresult.StatusCode == HttpStatusCode.InternalServerError)
                {
                    account.IsError = true;
                }
                else if (identityresult.StatusCode == HttpStatusCode.BadRequest)
                {
                    account.IsError = true;
                }
                //identityresult = await identity.DeleteUser(identityEntity);
                if (identityresult.StatusCode == HttpStatusCode.NoContent)
                {
                    // check to handle message
                    account.IsError = true;
                }
            }
            return account;
        }
        public async Task<Account> Update(Account account)
        {
            // create user in identity
            IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
            identityEntity.UserName = account.EmailId;
            //identityEntity.EmailId = account.EmailId;    
            identityEntity.FirstName = account.FirstName;
            identityEntity.LastName = account.LastName;
            var identityresult = await _identity.UpdateUser(identityEntity);

            if (identityresult.StatusCode == HttpStatusCode.NoContent)
            {
                account = await _repository.Update(account);
            }
            return account;
        }
        public async Task<bool> Delete(Account account)
        {
            bool result = false;
            // create user in identity
            IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
            identityEntity.UserName = account.EmailId;
            // identityEntity.EmailId = account.EmailId;
            // identityEntity.FirstName = account.FirstName;
            // identityEntity.LastName = account.LastName;
            var identityresult = await _identity.DeleteUser(identityEntity);
            if (identityresult.StatusCode == HttpStatusCode.NoContent)
            {
                result = await _repository.Delete(account.Id, account.Organization_Id.Value);
            }
            else if (identityresult.StatusCode == HttpStatusCode.NotFound)
            {
                //TODO:  need to handle this -- is there in DB but not is IDP.
            }
            return result;
        }
        public async Task<Response> ChangePassword(Account accountRequest)
        {
            var account = await _repository.GetAccountByEmailId(accountRequest.EmailId.ToLower());

            if (account != null)
            {
                if (await CheckForMinPasswordAge(account))
                {
                    // create user in identity
                    IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
                    identityEntity.UserName = account.EmailId;
                    identityEntity.Password = accountRequest.Password;
                    var identityResult = await _identity.ChangeUserPassword(identityEntity);
                    if (identityResult.StatusCode == HttpStatusCode.NoContent)
                    {
                        //Update password policy entry
                        await _repository.UpsertPasswordModifiedDate(account.Id, UTCHandling.GetUTCFromDateTime(DateTime.Now));

                        //Send confirmation email
                        account.Organization_Id = accountRequest.Organization_Id;
                        await TriggerSendEmailRequest(account, EmailEventType.ChangeResetPasswordSuccess);
                    }
                    return identityResult;
                }
                return new Response(HttpStatusCode.Forbidden);
            }
            return new Response(HttpStatusCode.NotFound);
        }

        public async Task<IEnumerable<Account>> Get(AccountFilter filter)
        {
            return await _repository.Get(filter);
        }
        public async Task<int> GetCount(int organization_id)
        {
            return await _repository.GetCount(organization_id);
        }
        public async Task<Account> AddAccountToOrg(Account account)
        {
            return await _repository.AddAccountToOrg(account);
        }

        public async Task<Account> GetAccountByEmailId(string emailId)
        {
            return await _repository.GetAccountByEmailId(emailId);
        }

        public async Task<AccountBlob> CreateBlob(AccountBlob accountBlob)
        {
            return await _repository.CreateBlob(accountBlob);
        }
        public async Task<AccountBlob> GetBlob(int blobId)
        {
            return await _repository.GetBlob(blobId);
        }
        #region AccessRelationship

        //public VehicleAccessRelationship CreateVehicleAccessRelationship(VehicleAccessRelationship entity)
        //{
        //    // 
        //    if (entity != null)
        //    {

        //            if (!entity.IsGroup)
        //            {
        //                // create vehicle group with vehicle
        //                int vehicleGroupId = 0;

        //                // check all account or account group
        //                foreach (var account in entity.AccountsAccountGroups)
        //                {
        //                    // create group type single
        //                    if (!account.IsGroup)
        //                    {
        //                        // create group for account

        //                        // 
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //    return entity;
        //}

        //public AccountAccessRelationship CreateAccountAccessRelationship(AccountAccessRelationship entity)
        //{
        //    // 
        //    if (entity != null)
        //    {

        //        if (!entity.IsGroup)
        //        {
        //            // create vehicle group with vehicle
        //            int vehicleGroupId = 0;

        //            // check all account or account group
        //            foreach (var account in entity.VehiclesVehicleGroups)
        //            {
        //                // create group type single
        //                if (!account.IsGroup)
        //                {
        //                    // create group for account

        //                    // 
        //                }
        //                else
        //                {

        //                }
        //            }
        //        }
        //    }
        //    return entity;
        //}
        public async Task<AccessRelationship> CreateAccessRelationship(AccessRelationship entity)
        {
            return await _repository.CreateAccessRelationship(entity);
        }
        public async Task<AccessRelationship> UpdateAccessRelationship(AccessRelationship entity)
        {
            return await _repository.UpdateAccessRelationship(entity);
        }
        public async Task<bool> DeleteAccessRelationship(int accountGroupId, int vehicleGroupId)
        {
            return await _repository.DeleteAccessRelationship(accountGroupId, vehicleGroupId);
        }
        public async Task<List<AccessRelationship>> GetAccessRelationship(AccessRelationshipFilter filter)
        {
            return await _repository.GetAccessRelationship(filter);
        }
        public async Task<List<AccountVehicleAccessRelationship>> GetAccountVehicleAccessRelationship(AccountVehicleAccessRelationshipFilter filter, bool is_vehicle)
        {
            return await _repository.GetAccountVehicleAccessRelationship(filter, is_vehicle);
        }
        public async Task<List<AccountVehicleEntity>> GetVehicle(AccountVehicleAccessRelationshipFilter filter, bool isVvehicle)
        {
            return await _repository.GetVehicle(filter, isVvehicle);
        }
        public async Task<List<AccountVehicleEntity>> GetAccount(AccountVehicleAccessRelationshipFilter filter, bool isAccount)
        {
            return await _repository.GetAccount(filter, isAccount);
        }
        public async Task<bool> DeleteVehicleAccessRelationship(int organizationId, int groupId, bool isVehicle)
        {
            return await _repository.DeleteVehicleAccessRelationship(organizationId, groupId, isVehicle);
        }
        #endregion
        public async Task<bool> AddRole(AccountRole accountRoles)
        {
            return await _repository.AddRole(accountRoles);
        }
        public async Task<bool> RemoveRole(AccountRole accountRoles)
        {
            return await _repository.RemoveRole(accountRoles);
        }
        public async Task<List<KeyValue>> GetRoles(AccountRole accountRoles)
        {
            return await _repository.GetRoles(accountRoles);
        }
        public async Task<List<int>> GetRoleAccounts(int roleId)
        {
            return await _repository.GetRoleAccounts(roleId);
        }

        public async Task<List<KeyValue>> GetAccountOrg(int accountId)
        {
            return await _repository.GetAccountOrg(accountId);
        }
        public async Task<List<AccountOrgRole>> GetAccountRole(int accountId)
        {
            return await _repository.GetAccountRole(accountId);
        }

        //This method is called for ResetPassword, SetupNewPassword and PassswordExpiry process
        public async Task<Response> ResetPasswordInitiate(string emailId, EmailEventType eventType = EmailEventType.ResetPassword)
        {
            var response = new Response(HttpStatusCode.NotFound);
            try
            {
                var account = await _repository.GetAccountByEmailId(emailId.ToLower());

                if (account != null)
                {
                    //Check if record already exists in ResetPasswordToken table with Issued status
                    var resetPasswordToken = await _repository.GetIssuedResetTokenByAccountId(account.Id);
                    if (resetPasswordToken != null)
                    {
                        //Check for Expiry of Reset Token
                        if (UTCHandling.GetUTCFromDateTime(DateTime.Now) > resetPasswordToken.ExpiryAt.Value)
                        {
                            //Update status to Expired
                            await _repository.Update(resetPasswordToken.Id, ResetTokenStatus.Expired);
                        }

                        //Update status to Invalidated
                        await _repository.Update(resetPasswordToken.Id, ResetTokenStatus.Invalidated);
                    }

                    var identityResult = await _identity.ResetUserPasswordInitiate();
                    var processToken = (Guid)identityResult.Result;
                    if (identityResult.StatusCode == HttpStatusCode.OK)
                    {
                        //Save Password Token to the database
                        var objToken = new ResetPasswordToken();
                        objToken.AccountId = account.Id;
                        objToken.ProcessToken = processToken;
                        objToken.Status = ResetTokenStatus.New;
                        var now = DateTime.Now;
                        var expiryAt = eventType == EmailEventType.CreateAccount
                                                    ? _configuration.GetValue<double>("CreatePasswordTokenExpiryInMinutes")
                                                    : _configuration.GetValue<double>("ResetPasswordTokenExpiryInMinutes");
                        objToken.ExpiryAt = UTCHandling.GetUTCFromDateTime(now.AddMinutes(expiryAt));
                        objToken.CreatedAt = UTCHandling.GetUTCFromDateTime(now);

                        //Create with status as New
                        await _repository.Create(objToken);

                        bool isSent = false;
                        //Send email for reset password flow only
                        //In cases like Create Password and Password Expiry, no need to send below email
                        if (eventType == EmailEventType.ResetPassword)
                        {
                            account.Organization_Id = (await _repository.GetAccountOrg(account.Id)).First().Id;
                            isSent = await TriggerSendEmailRequest(account, eventType, processToken);
                        }

                        if ((eventType == EmailEventType.ResetPassword && isSent) || eventType != EmailEventType.ResetPassword)
                        {
                            //Update status to Issued
                            await _repository.Update(objToken.Id, ResetTokenStatus.Issued);
                        }
                        else if (eventType == EmailEventType.ResetPassword && !isSent)
                        {
                            identityResult.StatusCode = HttpStatusCode.ExpectationFailed;
                        }
                    }
                    return identityResult;
                }
                return response;
            }
            catch (Exception ex)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Manager", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "Password Reset Initiate" + ex.Message, 1, 2, emailId);
                throw;
            }
        }

        public async Task<Response> GetResetPasswordTokenStatus(Guid processToken)
        {
            var response = new Response(HttpStatusCode.NotFound);
            try
            {
                //Check if token record exists, Fetch it and validate the status
                var resetPasswordToken = await _repository.GetIssuedResetToken(processToken);
                if (resetPasswordToken != null)
                {
                    //Check for Expiry of Reset Token
                    if (UTCHandling.GetUTCFromDateTime(DateTime.Now) > resetPasswordToken.ExpiryAt.Value)
                    {
                        //Update status to Expired
                        await _repository.Update(resetPasswordToken.Id, ResetTokenStatus.Expired);

                        return response;
                    }
                    return new Response(HttpStatusCode.OK);
                }
                return response;
            }
            catch (Exception ex)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Manager", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "Get Reset Password Token Status" + ex.Message, 1, 2, processToken.ToString());
                throw;
            }
        }

        public async Task<Response> ResetPassword(Account accountInfo)
        {
            var response = new Response(HttpStatusCode.NotFound);
            try
            {
                //Check if token record exists, Fetch it and validate the status
                var resetPasswordToken = await _repository.GetIssuedResetToken(accountInfo.ProcessToken.Value);
                if (resetPasswordToken != null)
                {
                    //Check for Expiry of Reset Token
                    if (UTCHandling.GetUTCFromDateTime(DateTime.Now) > resetPasswordToken.ExpiryAt.Value)
                    {
                        //Update status to Expired
                        await _repository.Update(resetPasswordToken.Id, ResetTokenStatus.Expired);

                        return response;
                    }
                    //Fetch Account corrosponding to it
                    var account = await _repository.GetAccountByAccountId(resetPasswordToken.AccountId);

                    // create user in identity
                    IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
                    identityEntity.UserName = account.EmailId;
                    identityEntity.Password = accountInfo.Password;
                    var identityresult = await _identity.ChangeUserPassword(identityEntity);
                    if (identityresult.StatusCode == HttpStatusCode.NoContent)
                    {
                        //Update password policy entry
                        await _repository.UpsertPasswordModifiedDate(account.Id, UTCHandling.GetUTCFromDateTime(DateTime.Now));

                        //Update status to Used
                        await _repository.Update(resetPasswordToken.Id, ResetTokenStatus.Used);

                        //Send confirmation email
                        account.Organization_Id = (await _repository.GetAccountOrg(account.Id)).First().Id;
                        await TriggerSendEmailRequest(account, EmailEventType.ChangeResetPasswordSuccess);
                    }
                    return identityresult;
                }
                return response;
            }
            catch (Exception ex)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Manager", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "Reset Password: " + ex.Message, 1, 2, accountInfo.ProcessToken.Value.ToString());
                throw;
            }
        }
        public async Task<Response> ResetPasswordInvalidate(Guid ResetToken)
        {
            var response = new Response(HttpStatusCode.NotFound);
            try
            {
                //Check if token record exists
                var resetPasswordToken = await _repository.GetIssuedResetToken(ResetToken);

                if (resetPasswordToken != null)
                {
                    //Update status to Invalidated
                    await _repository.Update(resetPasswordToken.Id, ResetTokenStatus.Invalidated);

                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }
            }
            catch (Exception ex)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Manager", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "Reset Password Invalidate: " + ex.Message, 1, 2, ResetToken.ToString());
                throw;
            }

            return response;
        }

        public async Task<IEnumerable<MenuFeatureDto>> GetMenuFeatures(MenuFeatureRquest request)
        {
            return await _repository.GetMenuFeaturesList(request);
        }

        public async Task<bool> CheckForFeatureAccessByEmailId(string emailId, string featureName)
        {
            return await _repository.CheckForFeatureAccessByEmailId(emailId, featureName);
        }

        public async Task<IEnumerable<EmailList>> SendEmailForPasswordExpiry(int noOfDays)
        {
            var emailSendList = new List<EmailList>();

            foreach (var account in await _repository.GetAccountOfPasswordExpiry(noOfDays))
            {
                try
                {
                    var isSuccuss = TriggerSendEmailRequest(account, EmailEventType.PasswordExpiryNotification).Result;

                    emailSendList.Add(new EmailList { AccountId = account.Id, Email = account.EmailId, IsSend = isSuccuss });
                    await _auditlog.AddLogs(new AuditTrail
                    {
                        Created_at = DateTime.Now,
                        Performed_at = DateTime.Now,
                        Performed_by = 2,
                        Component_name = "Email Notification Password Expiry",
                        Service_name = "Email Component",
                        Event_type = AuditTrailEnum.Event_type.Mail,
                        Event_status = isSuccuss ? AuditTrailEnum.Event_status.SUCCESS : AuditTrailEnum.Event_status.FAILED,
                        Message = isSuccuss ? $"Email send to {account.EmailId}" : $"Email is not send to {account.EmailId}",
                        Sourceobject_id = 0,
                        Targetobject_id = 0,
                        Updated_data = "EmailNotificationForPasswordExpiry"
                    });
                }
                catch (Exception ex)
                {
                    emailSendList.Add(new EmailList { AccountId = account.Id, Email = account.EmailId, IsSend = false });
                    await _auditlog.AddLogs(new AuditTrail
                    {
                        Created_at = DateTime.Now,
                        Performed_at = DateTime.Now,
                        Performed_by = 2,
                        Component_name = "Email Notification Password Expiry",
                        Service_name = "Email Component",
                        Event_type = AuditTrailEnum.Event_type.Mail,
                        Event_status = AuditTrailEnum.Event_status.FAILED,
                        Message = $"Email is not send to {account.EmailId} with error as {ex.Message}",
                        Sourceobject_id = 0,
                        Targetobject_id = 0,
                        Updated_data = "EmailNotificationForPasswordExpiry"
                    });
                }
            }
            foreach (var account in emailSendList)
            {
                try
                {
                    if (account.IsSend)
                        await _repository.UpdateIsReminderSent(account.AccountId);
                }
                catch (Exception ex)
                {
                    await _auditlog.AddLogs(new AuditTrail
                    {
                        Created_at = DateTime.Now,
                        Performed_at = DateTime.Now,
                        Performed_by = 2,
                        Component_name = "Email Notification Password Expiry",
                        Service_name = "Email Component",
                        Event_type = AuditTrailEnum.Event_type.UPDATE,
                        Event_status = AuditTrailEnum.Event_status.FAILED,
                        Message = $"Failed to update ISReminderSend for {account.Email} with error as {ex.Message}",
                        Sourceobject_id = 0,
                        Targetobject_id = 0,
                        Updated_data = "EmailNotificationForPasswordExpiry"
                    });
                }
            }
            return emailSendList.ToArray();
        }
        #region Private Helper Methods

        private async Task<bool> SetPasswordViaEmail(Account account, EmailEventType eventType)
        {
            var response = await ResetPasswordInitiate(account.EmailId, eventType);

            if (response.StatusCode != HttpStatusCode.OK)
                return false;
            else
            {
                var result = await _repository.GetAccountOrg(account.Id);
                account.OrgName = result.First().Name;
                //Send account confirmation email
                return await TriggerSendEmailRequest(account, eventType, (Guid)response.Result);
            }
        }

        private async Task<bool> TriggerSendEmailRequest(Account account, EmailEventType eventType, Guid? tokenSecret = null, EmailContentType contentType = EmailContentType.Html)
        {
            var messageRequest = new MessageRequest();
            if (eventType == EmailEventType.PasswordExpiryNotification)
            {
                messageRequest.RemainingDaysToExpire = Convert.ToInt32(_configuration["RemainingDaysToExpire"]);
            }
            messageRequest.AccountInfo = new AccountInfo
            {
                FullName = account.FullName,
                OrganizationName = account.OrgName
            };
            messageRequest.ToAddressList = new Dictionary<string, string>()
            {
                { account.EmailId, null }
            };
            messageRequest.Configuration = _emailConfiguration;
            messageRequest.TokenSecret = tokenSecret;
            try
            {
                var languageCode = await GetLanguageCodePreference(account.EmailId, account.Organization_Id);
                var emailTemplate = await _translationManager.GetEmailTemplateTranslations(eventType, contentType, languageCode);

                return await EmailHelper.SendEmail(messageRequest, emailTemplate);
            }
            catch (Exception ex)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Manager", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "Trigger Email: " + ex.Message, 1, 2, account.EmailId);
                return false;
            }
        }

        private async Task<bool> CheckForMinPasswordAge(Account account)
        {
            //Check for Min Password Age if policy enabled
            var isPolicyEnabled = Convert.ToBoolean(_configuration["AccountPolicy:MinPasswordAgePolicyEnabled"]);
            if (isPolicyEnabled)
            {
                var lastModifiedDate = await _repository.GetPasswordModifiedDate(account.Id);

                if (lastModifiedDate.HasValue)
                {
                    var minPasswordAge = Convert.ToInt32(_configuration["AccountPolicy:MinPasswordAgeInDays"]);

                    return (UTCHandling.GetUTCFromDateTime(DateTime.Now.AddDays(-minPasswordAge)) > lastModifiedDate);
                }
            }
            return true;
        }

        public async Task<PasswordPolicyAccount> GetPasswordPolicyAccount(int id)
        {
            return await _repository.GetPasswordPolicyAccount(id);
        }

        public async Task<int> UpsertPasswordPolicyAccount(PasswordPolicyAccount passwordPolicyAccount)
        {
            return await _repository.UpsertPasswordPolicyAccount(passwordPolicyAccount);
        }

        public async Task<string> GetLanguageCodePreference(string emailId, int? orgId)
        {
            return await _repository.GetLanguageCodePreference(emailId.ToLower(), orgId);
        }
        #endregion

        #region Account SSO Details

        public async Task<SSOTokenResponse> GetAccountSSODetails(IdentitySessionEntity.AccountToken account)
        {
            List<SSOTokenResponse> _responses = await _repository.GetAccountSSODetails(account);
            return _responses.FirstOrDefault();
        }

        #endregion
    }
}
