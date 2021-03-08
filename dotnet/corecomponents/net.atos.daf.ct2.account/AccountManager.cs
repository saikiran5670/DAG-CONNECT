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

                //Send account confirmation email
                await TriggerSendEmailRequest(account, EmailTemplateType.CreateAccount);
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

                        //Send account confirmation email
                        await TriggerSendEmailRequest(account, EmailTemplateType.CreateAccount);
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

        public async Task<Guid?> ResetPasswordInitiate(string emailId)
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
                    var tokenSecret = (Guid)identityresult.Result;
                    if (identityresult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //Save Reset Password Token to the database
                        var objToken = new ResetPasswordToken();
                        objToken.AccountId = account.Id;
                        objToken.TokenSecret = tokenSecret;
                        objToken.Status = ResetTokenStatus.New;
                        var now = DateTime.Now;
                        objToken.ExpiryAt = UTCHandling.GetUTCFromDateTime(now.AddMinutes(configuration.GetValue<double>("ResetPasswordTokenExpiryInMinutes")));
                        objToken.CreatedAt = UTCHandling.GetUTCFromDateTime(now);

                        //Create with status as New
                        await repository.Create(objToken);

                        //Send email
                        var isSent = TriggerSendEmailRequest(account, EmailTemplateType.ResetPassword);

                        if (isSent.Result)
                        {
                            //Update status to Issued
                            await repository.Update(objToken.Id, ResetTokenStatus.Issued);

                            return tokenSecret;
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
            var resetPasswordToken = await repository.GetIssuedResetToken(accountInfo.ResetToken.Value);
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

        #region Private Helper Methods
        private async Task<bool> TriggerSendEmailRequest(Account account, EmailTemplateType templateType)
        {
            //Send email
            var messageRequest = new MessageRequest();
            messageRequest.Configuration = emailConfiguration;
            messageRequest.ToAddressList = new Dictionary<string, string>()
            {
                { account.EmailId, account.LastName + ", " + account.FirstName }
            };

            FillEmailTemplate(messageRequest, templateType);           

            return await EmailHelper.SendEmail(messageRequest);
        }

        private void FillEmailTemplate(MessageRequest messageRequest, EmailTemplateType templateType)
        {
            StringBuilder sb = new StringBuilder();

            switch (templateType)
            {
                case EmailTemplateType.CreateAccount:
                    sb.Append("Your account has been successfully created on DAF portal.\n\n");

                    messageRequest.Subject = "DAF Account Confirmation";
                    messageRequest.ContentMimeType = MimeType.Text;
                    break;
                case EmailTemplateType.ResetPassword:
                    Uri baseUrl = new Uri(emailConfiguration.PortalServiceBaseUrl);
                    Uri resetUrl = new Uri(baseUrl, "account/resetpassword");
                    Uri resetInvalidateUrl = new Uri(baseUrl, "account/resetpasswordinvalidate");

                    sb.Append("A request has been received to reset the password fro your account.\n\n");                                       
                    sb.Append(resetUrl.AbsoluteUri + "\n\n\n");
                    sb.Append("Ïf you did not initiate this request, please click on the below link.\n\n");
                    sb.Append(resetInvalidateUrl.AbsoluteUri);

                    messageRequest.Subject = "Reset Password Confirmation";
                    messageRequest.ContentMimeType = MimeType.Text;
                    break;
                default:
                    messageRequest.Subject = string.Empty;
                    messageRequest.ContentMimeType = MimeType.Text;
                    break;
            }

            messageRequest.Content = sb.ToString();
        }

        #endregion
    }
}
