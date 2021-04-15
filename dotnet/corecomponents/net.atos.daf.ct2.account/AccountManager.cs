using net.atos.daf.ct2.audit;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Identity = net.atos.daf.ct2.identity;
using IdentityEntity = net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.account.ENUM;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.audit.Enum;
using System.Text;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.email;
using net.atos.daf.ct2.email.Entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.identity.entity;
using System.Net;
using net.atos.daf.ct2.translation;

namespace net.atos.daf.ct2.account
{
    public class AccountManager : IAccountManager
    {
        IAccountRepository repository;
        Identity.IAccountManager identity;
        IAuditTraillib auditlog;
        private readonly EmailConfiguration emailConfiguration;
        private readonly IConfiguration configuration;
        private readonly ITranslationManager translationManager;

        public AccountManager(IAccountRepository _repository, IAuditTraillib _auditlog, Identity.IAccountManager _identity, IConfiguration _configuration, ITranslationManager _translationManager)
        {
            repository = _repository;
            auditlog = _auditlog;
            identity = _identity;
            configuration = _configuration;
            emailConfiguration = new EmailConfiguration();
            configuration.GetSection("EmailConfiguration").Bind(emailConfiguration);
            translationManager = _translationManager;
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
            var identityresult = await identity.CreateUser(identityEntity);

            if (identityresult.StatusCode == HttpStatusCode.Created)
            {
                // if this fails
                account = await repository.Create(account);

                account.isErrorInEmail = !(await SetPasswordViaEmail(account));
            }
            else // there is issues and need delete user from IDP. 
            {
                // user already exits in IDP.
                if (identityresult.StatusCode == HttpStatusCode.Conflict)
                {
                    // get account by email , if not exists in DB-- create it
                    AccountFilter filter = new AccountFilter();
                    filter.Email = account.EmailId;
                    filter.OrganizationId = account.Organization_Id;
                    int Organization_Id = account.Organization_Id;
                    filter.AccountType = AccountType.None;
                    filter.AccountIds = string.Empty;
                    filter.Name = string.Empty;
                    var accountGet = await repository.Duplicate(filter);
                    if (accountGet == null)
                    {
                        account = await repository.Create(account);
                        await identity.UpdateUser(identityEntity);

                        account.isErrorInEmail = !(await SetPasswordViaEmail(account));
                    }
                    else
                    {
                        account = accountGet;
                        account.isDuplicate = true;
                        if (Organization_Id == account.Organization_Id)
                        {
                            account.isDuplicateInOrg = true;
                        }
                    }
                }
                // inter server error in IDP.
                else if (identityresult.StatusCode == HttpStatusCode.InternalServerError)
                {
                    account.isError = true;
                }
                else if (identityresult.StatusCode == HttpStatusCode.BadRequest)
                {
                    account.isError = true;
                }
                //identityresult = await identity.DeleteUser(identityEntity);
                if (identityresult.StatusCode == HttpStatusCode.NoContent)
                {
                    // check to handle message
                    account.isError = true;
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
            var identityresult = await identity.UpdateUser(identityEntity);

            if (identityresult.StatusCode == HttpStatusCode.NoContent)
            {
                account = await repository.Update(account);
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
            var identityresult = await identity.DeleteUser(identityEntity);
            if (identityresult.StatusCode == HttpStatusCode.NoContent)
            {
                result = await repository.Delete(account.Id, account.Organization_Id);
            }
            else if (identityresult.StatusCode == HttpStatusCode.NotFound)
            {
                //TODO:  need to handle this -- is there in DB but not is IDP.
            }
            return result;
        }
        public async Task<Response> ChangePassword(Account accountRequest)
        {
            var account = await repository.GetAccountByEmailId(accountRequest.EmailId.ToLower());

            if (account != null)
            {
                if (await CheckForMinPasswordAge(account))
                {
                    // create user in identity
                    IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
                    identityEntity.UserName = account.EmailId;
                    identityEntity.Password = accountRequest.Password;
                    var identityResult = await identity.ChangeUserPassword(identityEntity);
                    if (identityResult.StatusCode == HttpStatusCode.NoContent)
                    {
                        //Update password policy entry
                        await repository.UpsertPasswordModifiedDate(account.Id, UTCHandling.GetUTCFromDateTime(DateTime.Now));

                        //Send confirmation email
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
            return await repository.Get(filter);
        }
        public async Task<int> GetCount(int organization_id)
        {
            return await repository.GetCount(organization_id);
        }
        public async Task<Account> AddAccountToOrg(Account account)
        {
            return await repository.AddAccountToOrg(account);
        }
        public async Task<AccountBlob> CreateBlob(AccountBlob accountBlob)
        {
            return await repository.CreateBlob(accountBlob);
        }
        public async Task<AccountBlob> GetBlob(int blobId)
        {
            return await repository.GetBlob(blobId);
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
            return await repository.CreateAccessRelationship(entity);
        }
        public async Task<AccessRelationship> UpdateAccessRelationship(AccessRelationship entity)
        {
            return await repository.UpdateAccessRelationship(entity);
        }
        public async Task<bool> DeleteAccessRelationship(int accountGroupId, int vehicleGroupId)
        {
            return await repository.DeleteAccessRelationship(accountGroupId, vehicleGroupId);
        }
        public async Task<List<AccessRelationship>> GetAccessRelationship(AccessRelationshipFilter filter)
        {
            return await repository.GetAccessRelationship(filter);
        }
        public async Task<List<AccountVehicleAccessRelationship>> GetAccountVehicleAccessRelationship(AccountVehicleAccessRelationshipFilter filter, bool is_vehicle)
        {
            return await repository.GetAccountVehicleAccessRelationship(filter, is_vehicle);
        }
        public async Task<List<AccountVehicleEntity>> GetVehicle(AccountVehicleAccessRelationshipFilter filter, bool isVvehicle)
        {
            return await repository.GetVehicle(filter, isVvehicle);
        }
        public async Task<List<AccountVehicleEntity>> GetAccount(AccountVehicleAccessRelationshipFilter filter, bool isAccount)
        {
            return await repository.GetAccount(filter, isAccount);
        }
        public async Task<bool> DeleteVehicleAccessRelationship(int organizationId, int groupId, bool isVehicle)
        {
            return await repository.DeleteVehicleAccessRelationship(organizationId, groupId, isVehicle);
        }
        #endregion
        public async Task<bool> AddRole(AccountRole accountRoles)
        {
            return await repository.AddRole(accountRoles);
        }
        public async Task<bool> RemoveRole(AccountRole accountRoles)
        {
            return await repository.RemoveRole(accountRoles);
        }
        public async Task<List<KeyValue>> GetRoles(AccountRole accountRoles)
        {
            return await repository.GetRoles(accountRoles);
        }
        public async Task<List<int>> GetRoleAccounts(int roleId)
        {
            return await repository.GetRoleAccounts(roleId);
        }

        public async Task<List<KeyValue>> GetAccountOrg(int accountId)
        {
            return await repository.GetAccountOrg(accountId);
        }
        public async Task<List<AccountOrgRole>> GetAccountRole(int accountId)
        {
            return await repository.GetAccountRole(accountId);
        }

        public async Task<Response> ResetPasswordInitiate(string emailId, bool canSendEmail = true)
        {
            var response = new Response(HttpStatusCode.NotFound);
            try
            {
                var account = await repository.GetAccountByEmailId(emailId.ToLower());

                if (account != null)
                {
                    //Check if record already exists in ResetPasswordToken table with Issued status
                    var resetPasswordToken = await repository.GetIssuedResetTokenByAccountId(account.Id);
                    if (resetPasswordToken != null)
                    {
                        //Check for Expiry of Reset Token
                        if (UTCHandling.GetUTCFromDateTime(DateTime.Now) > resetPasswordToken.ExpiryAt.Value)
                        {
                            //Update status to Expired
                            await repository.Update(resetPasswordToken.Id, ResetTokenStatus.Expired);
                        }

                        //Update status to Invalidated
                        await repository.Update(resetPasswordToken.Id, ResetTokenStatus.Invalidated);
                    }

                    var identityResult = await identity.ResetUserPasswordInitiate();
                    var processToken = (Guid)identityResult.Result;
                    if (identityResult.StatusCode == HttpStatusCode.OK)
                    {
                        //Save Reset Password Token to the database
                        var objToken = new ResetPasswordToken();
                        objToken.AccountId = account.Id;
                        objToken.ProcessToken = processToken;
                        objToken.Status = ResetTokenStatus.New;
                        var now = DateTime.Now;
                        objToken.ExpiryAt = UTCHandling.GetUTCFromDateTime(now.AddMinutes(configuration.GetValue<double>("ResetPasswordTokenExpiryInMinutes")));
                        objToken.CreatedAt = UTCHandling.GetUTCFromDateTime(now);

                        //Create with status as New
                        await repository.Create(objToken);

                        bool isSent = false;
                        //Send activation email based on flag
                        if (canSendEmail)
                            isSent = await TriggerSendEmailRequest(account, EmailEventType.ResetPassword, processToken);

                        if ((canSendEmail && isSent) || !canSendEmail)
                        {
                            //Update status to Issued
                            await repository.Update(objToken.Id, ResetTokenStatus.Issued);
                        }
                    }
                    return identityResult;
                }
                return response;
            }
            catch (Exception ex)
            {
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Manager", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "Password Reset Initiate" + ex.Message, 1, 2, emailId);
                throw ex;
            }
        }

        public async Task<Response> ResetPassword(Account accountInfo)
        {
            var response = new Response(HttpStatusCode.NotFound);
            try
            {
                //Check if token record exists, Fetch it and validate the status
                var resetPasswordToken = await repository.GetIssuedResetToken(accountInfo.ProcessToken.Value);
                if (resetPasswordToken != null)
                {
                    //Check for Expiry of Reset Token
                    if (UTCHandling.GetUTCFromDateTime(DateTime.Now) > resetPasswordToken.ExpiryAt.Value)
                    {
                        //Update status to Expired
                        await repository.Update(resetPasswordToken.Id, ResetTokenStatus.Expired);

                        return response;
                    }
                    //Fetch Account corrosponding to it
                    var account = await repository.GetAccountByAccountId(resetPasswordToken.AccountId);

                    // create user in identity
                    IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
                    identityEntity.UserName = account.EmailId;
                    identityEntity.Password = accountInfo.Password;
                    var identityresult = await identity.ChangeUserPassword(identityEntity);
                    if (identityresult.StatusCode == HttpStatusCode.NoContent)
                    {
                        //Update password policy entry
                        await repository.UpsertPasswordModifiedDate(account.Id, UTCHandling.GetUTCFromDateTime(DateTime.Now));

                        //Update status to Used
                        await repository.Update(resetPasswordToken.Id, ResetTokenStatus.Used);

                        //Send confirmation email
                        await TriggerSendEmailRequest(account, EmailEventType.ChangeResetPasswordSuccess);
                    }
                    return identityresult;
                }
                return response;
            }
            catch (Exception ex)
            {
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Manager", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "Reset Password: " + ex.Message, 1, 2, accountInfo.ProcessToken.Value.ToString());
                throw ex;
            }
        }
        public async Task<Response> ResetPasswordInvalidate(Guid ResetToken)
        {
            var response = new Response(HttpStatusCode.NotFound);
            try
            {
                //Check if token record exists
                var resetPasswordToken = await repository.GetIssuedResetToken(ResetToken);

                if (resetPasswordToken != null)
                {
                    //Update status to Invalidated
                    await repository.Update(resetPasswordToken.Id, ResetTokenStatus.Invalidated);

                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }
            }
            catch (Exception ex)
            {
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Manager", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "Reset Password Invalidate: " + ex.Message, 1, 2, ResetToken.ToString());
                throw ex;
            }

            return response;
        }

        public async Task<IEnumerable<MenuFeatureDto>> GetMenuFeatures(int accountId, int roleId, int organizationId, string languageCode)
        {
            return await repository.GetMenuFeaturesList(accountId, roleId, organizationId, languageCode);
        }

        public async Task<bool> CheckForFeatureAccessByEmailId(string emailId, string featureName)
        {
            return await repository.CheckForFeatureAccessByEmailId(emailId, featureName);
        }

        #region Private Helper Methods

        private async Task<bool> SetPasswordViaEmail(Account account)
        {
            var response = await ResetPasswordInitiate(account.EmailId, false);

            if (response.StatusCode != HttpStatusCode.OK)
                return false;
            else
            {
                var result = await repository.GetAccountOrg(account.Id);
                account.OrgName = result.FirstOrDefault().Name;
                //Send account confirmation email
                return await TriggerSendEmailRequest(account, EmailEventType.CreateAccount, (Guid)response.Result);
            }
        }

        private async Task<bool> TriggerSendEmailRequest(Account account, EmailEventType templateType, Guid? tokenSecret = null, EmailContentType contentType = EmailContentType.Html)
        {
            var messageRequest = new MessageRequest();
            messageRequest.Configuration = emailConfiguration;
            messageRequest.ToAddressList = new Dictionary<string, string>()
            {
                { account.EmailId, null }
            };
            try
            {
                if (await FillEmailTemplate(account, messageRequest, templateType, contentType, tokenSecret))
                    return await EmailHelper.SendEmail(messageRequest);
            }
            catch (Exception ex)
            {
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Manager", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "Trigger Email: " + ex.Message, 1, 2, account.EmailId);
                return false;
            }
            return false;
        }

        private async Task<bool> FillEmailTemplate(Account account, MessageRequest messageRequest, EmailEventType eventType, EmailContentType contentType, Guid? tokenSecret)
        {
            var emailContent = string.Empty;
            try
            {
                Uri baseUrl = new Uri(emailConfiguration.PortalUIBaseUrl);
                Uri logoUrl = new Uri(baseUrl, "assets/logo.png");

                var languageCode = await GetLanguageCodePreference(account.EmailId);
                var emailTemplate = await translationManager.GetEmailTemplateTranslations(eventType, contentType, languageCode);
                var emailTemplateContent = EmailHelper.GetEmailContent(emailTemplate);

                if (string.IsNullOrEmpty(emailTemplateContent))
                    return false;

                switch (eventType)
                {
                    case EmailEventType.CreateAccount:
                        Uri setUrl = new Uri(baseUrl, $"#/auth/createpassword/{ tokenSecret }");

                        emailContent = string.Format(emailTemplateContent, logoUrl.AbsoluteUri, account.FullName, account.OrgName, setUrl.AbsoluteUri);
                        break;
                    case EmailEventType.ResetPassword:
                        Uri resetUrl = new Uri(baseUrl, $"#/auth/resetpassword/{ tokenSecret }");
                        Uri resetInvalidateUrl = new Uri(baseUrl, $"#/auth/resetpasswordinvalidate/{ tokenSecret }");

                        emailContent = string.Format(emailTemplateContent, logoUrl.AbsoluteUri, account.FullName, resetUrl.AbsoluteUri, resetInvalidateUrl.AbsoluteUri);
                        break;
                    case EmailEventType.ChangeResetPasswordSuccess:
                        emailContent = string.Format(emailTemplateContent, logoUrl.AbsoluteUri, account.FullName);
                        break;
                    default:
                        messageRequest.Subject = string.Empty;
                        break;
                }

                messageRequest.Subject = emailTemplate.TemplateLabels.Where(x => x.LabelKey.EndsWith("_Subject")).First().TranslatedValue;
                messageRequest.Content = emailContent;
                messageRequest.ContentMimeType = contentType == EmailContentType.Html ? MimeType.Html : MimeType.Text;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        private async Task<bool> CheckForMinPasswordAge(Account account)
        {
            //Check for Min Password Age if policy enabled
            var isPolicyEnabled = Convert.ToBoolean(configuration["AccountPolicy:MinPasswordAgePolicyEnabled"]);
            if (isPolicyEnabled)
            {
                var lastModifiedDate = await repository.GetPasswordModifiedDate(account.Id);

                if (lastModifiedDate.HasValue)
                {
                    var minPasswordAge = Convert.ToInt32(configuration["AccountPolicy:MinPasswordAgeInDays"]);

                    return (UTCHandling.GetUTCFromDateTime(DateTime.Now.AddDays(-minPasswordAge)) > lastModifiedDate);
                }
            }
            return true;
        }

        public async Task<PasswordPolicyAccount> GetPasswordPolicyAccount(int id)
        {
            return await repository.GetPasswordPolicyAccount(id);
        }

        public async Task<int> UpsertPasswordPolicyAccount(PasswordPolicyAccount passwordPolicyAccount)
        {
            return await repository.UpsertPasswordPolicyAccount(passwordPolicyAccount);
        }

        public async Task<string> GetLanguageCodePreference(string emailId)
        {
            return await repository.GetLanguageCodePreference(emailId.ToLower());
        }
        #endregion
    }
}
