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

namespace net.atos.daf.ct2.account
{
    public class AccountManager : IAccountManager
    {
        IAccountRepository repository;
        Identity.IAccountManager identity;
        IAuditTraillib auditlog;
        private readonly EmailConfiguration emailConfiguration;
        private readonly IConfiguration configuration;

        public AccountManager(IAccountRepository _repository, IAuditTraillib _auditlog, Identity.IAccountManager _identity, IConfiguration _configuration)
        {
            repository = _repository;
            auditlog = _auditlog;
            identity = _identity;
            configuration = _configuration;
            emailConfiguration = new EmailConfiguration();
            configuration.GetSection("EmailConfiguration").Bind(emailConfiguration);
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

            if (identityresult.StatusCode == System.Net.HttpStatusCode.Created)
            {
                // if this fails
                account = await repository.Create(account);

                account.isErrorInEmail = !(await SetPasswordViaEmail(account.EmailId));
            }
            else // there is issues and need delete user from IDP. 
            {
                // user already exits in IDP.
                if (identityresult.StatusCode == System.Net.HttpStatusCode.Conflict)
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

                        account.isErrorInEmail = !(await SetPasswordViaEmail(account.EmailId));
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
                else if (identityresult.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    account.isError = true;
                }
                else if (identityresult.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    account.isError = true;
                }
                //identityresult = await identity.DeleteUser(identityEntity);
                if (identityresult.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    // check to handle message
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

            if (identityresult.StatusCode == System.Net.HttpStatusCode.NoContent)
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
            if (identityresult.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                result = await repository.Delete(account.Id, account.Organization_Id);
            }
            else if (identityresult.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                    //TODO:  need to handle this -- is there in DB but not is IDP.
            }
            return result;
        }
        public async Task<bool> ChangePassword(Account account)
        {
            bool result = false;
            // create user in identity
            IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
            identityEntity.UserName = account.EmailId;
            //identityEntity.EmailId = account.EmailId;
            // identityEntity.FirstName = account.FirstName;
            // identityEntity.LastName = account.LastName;
            identityEntity.Password = account.Password;
            var identityresult = await identity.ChangeUserPassword(identityEntity);
            if (identityresult.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                result = true;

                //Send confirmation email
                await TriggerSendEmailRequest(account.EmailId, EmailTemplateType.ChangeResetPasswordSuccess);
            }
            return result;
        }
        public async Task<IEnumerable<Account>> Get(AccountFilter filter)
        {
            return await repository.Get(filter);
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
        public async Task<bool> DeleteAccessRelationship(int accountGroupId,int vehicleGroupId)
        {
            return await repository.DeleteAccessRelationship(accountGroupId,vehicleGroupId);
        }
        public async Task<List<AccessRelationship>> GetAccessRelationship(AccessRelationshipFilter filter)
        {
            return await repository.GetAccessRelationship(filter);
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

        public async Task<Guid?> ResetPasswordInitiate(string emailId, bool canSendEmail = true)
        {
            try
            {
                var accountResult = await repository.Get(new AccountFilter() { Email = emailId.ToLower() });
                var account = accountResult.SingleOrDefault();

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

                    var identityresult = await identity.ResetUserPasswordInitiate();
                    var processToken = (Guid)identityresult.Result;
                    if (identityresult.StatusCode == System.Net.HttpStatusCode.OK)
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
                            isSent = await TriggerSendEmailRequest(account.EmailId, EmailTemplateType.ResetPassword, processToken);

                        if ((canSendEmail && isSent) || !canSendEmail)
                        {
                            //Update status to Issued
                            await repository.Update(objToken.Id, ResetTokenStatus.Issued);

                            return processToken;
                        }
                        else
                            return null;
                    }
                }
            }
            catch (Exception ex)
            {
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Manager", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "Password Reset Initiate" + ex.Message, 1, 2, emailId);
                return null;
            }

            return Guid.Empty;
        }

        public async Task<bool> ResetPassword(Account accountInfo)
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

                    return false;
                }
                //Fetch Account corrosponding to it
                var accounts = await repository.Get(new AccountFilter() { Id = resetPasswordToken.AccountId });
                var account = accounts.SingleOrDefault();

                // create user in identity
                IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
                identityEntity.UserName = account.EmailId;
                identityEntity.Password = accountInfo.Password;
                var identityresult = await identity.ChangeUserPassword(identityEntity);
                if (identityresult.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    //Update status to Used
                    await repository.Update(resetPasswordToken.Id, ResetTokenStatus.Used);

                    //Send confirmation email
                    await TriggerSendEmailRequest(account.EmailId, EmailTemplateType.ChangeResetPasswordSuccess);

                    return true;
                }
            }
            return false;
        }
        public async Task<bool> ResetPasswordInvalidate(Guid ResetToken)
        {
            //Check if token record exists
            var resetPasswordToken = await repository.GetIssuedResetToken(ResetToken);

            if (resetPasswordToken != null)
            {
                //Update status to Invalidated
                await repository.Update(resetPasswordToken.Id, ResetTokenStatus.Invalidated);

                return true;
            }
            return false;
        }

        public async Task<IEnumerable<MenuFeatureDto>> GetMenuFeatures(int accountId, int roleId, int organizationId, string languageCode)
        {
            return await repository.GetMenuFeaturesList(accountId, roleId, organizationId, languageCode);
        }

        #region Private Helper Methods

        private async Task<bool> SetPasswordViaEmail(string emailId)
        {
            var tokenSecret = await ResetPasswordInitiate(emailId, false);

            if (!tokenSecret.HasValue)
                return false;
            else
            {
                //Send account confirmation email
                return await TriggerSendEmailRequest(emailId, EmailTemplateType.CreateAccount, tokenSecret);
            }
        }

        private async Task<bool> TriggerSendEmailRequest(string toEmailAddress, EmailTemplateType templateType, Guid? tokenSecret = null)
        {
            var messageRequest = new MessageRequest();
            messageRequest.Configuration = emailConfiguration;
            messageRequest.ToAddressList = new Dictionary<string, string>()
            {
                { toEmailAddress, null }
            };

            if(FillEmailTemplate(messageRequest, templateType, tokenSecret))
                return await EmailHelper.SendEmail(messageRequest);

            return false;
        }

        private bool FillEmailTemplate(MessageRequest messageRequest, EmailTemplateType templateType, Guid? tokenSecret = null)
        {
            StringBuilder sb = new StringBuilder();
            Uri baseUrl = new Uri(emailConfiguration.PortalServiceBaseUrl);
            //var templateString = EmailHelper.GetTemplateHtmlString(templateType);

            //if (string.IsNullOrEmpty(templateString))
            //    return false;

            switch (templateType)
            {
                case EmailTemplateType.CreateAccount:
                    Uri setUrl = new Uri(baseUrl, $"account/createpassword/{ tokenSecret }");

                    sb.Append("Your account has been created successfully on DAF portal.\n\n");
                    sb.Append("Please click the below button to set your new password.\n\n");
                    sb.Append(setUrl.AbsoluteUri + "\n\n\n");
                    messageRequest.Subject = "DAF Account Confirmation";
                    break;
                case EmailTemplateType.ResetPassword:
                    Uri resetUrl = new Uri(baseUrl, $"account/resetpassword/{ tokenSecret }");
                    Uri resetInvalidateUrl = new Uri(baseUrl, $"account/resetpasswordinvalidate/{ tokenSecret }");

                    sb.Append("A request has been received to reset the password from your account.\n\n");
                    sb.Append(resetUrl.AbsoluteUri + "\n\n\n");
                    sb.Append("If you did not initiate this request, please click on the below link.\n\n");
                    sb.Append(resetInvalidateUrl.AbsoluteUri);
                    //sb.Append(string.Format(templateString, resetUrl.AbsoluteUri, resetInvalidateUrl.AbsoluteUri));
                    messageRequest.Subject = "Reset Password Confirmation";
                    break;
                case EmailTemplateType.ChangeResetPasswordSuccess:
                    sb.Append("Your account password has been changed successfully.\n\n");

                    messageRequest.Subject = "Change Password Confirmation";
                    break;
                default:
                    messageRequest.Subject = string.Empty;
                    break;
            }

            messageRequest.Content = sb.ToString();
            messageRequest.ContentMimeType = MimeType.Html;

            return true;
        }

        #endregion
    }
}
