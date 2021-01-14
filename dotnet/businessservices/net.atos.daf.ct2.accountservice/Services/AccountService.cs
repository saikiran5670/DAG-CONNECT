using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using AccountComponent = net.atos.daf.ct2.account;
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.accountservice;



namespace net.atos.daf.ct2.accountservice
{
    public class AccountManagementService : AccountService.AccountServiceBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly AccountComponent.IAccountManager accountmanager;
        public AccountManagementService(ILogger<GreeterService> logger, AccountComponent.IAccountManager _accountmanager)
        {
            _logger = logger;
            accountmanager = _accountmanager;
        }

        public override Task<AccountData> Create(AccountRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.Account account = new AccountComponent.Account();
                account.Id = request.Id;
                account.EmailId = request.EmailId;
                account.Salutation = request.Salutation;
                account.FirstName = request.FirstName;
                account.LastName = request.LastName;                
                account.Dob = request.Dob.Seconds;
                account.AccountType = GetEnum((int) request.Type);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                account = accountmanager.Create(account).Result;

                // response 
                AccountData response = new AccountData();
                response.Code  = Responcecode.Success;
                response.Message = "Created";
                response.Account = request;

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountData
                {
                    Code = Responcecode.Failed,
                    Message = "Account Creation Faile due to - " + ex.Message,
                    Account = null
                });
            }
        }
        public override Task<AccountData> Update(AccountRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.Account account = new AccountComponent.Account();
                account.Id = request.Id;
                account.EmailId = request.EmailId;
                account.Salutation = request.Salutation;
                account.FirstName = request.FirstName;
                account.LastName = request.LastName;                
                account.Dob = request.Dob.Seconds;
                account.AccountType = GetEnum((int) request.Type);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                account = accountmanager.Update(account).Result;

                // response 
                AccountData response = new AccountData();
                response.Code  = Responcecode.Success;
                response.Message = "Updated";
                response.Account = request;

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountData
                {
                    Code = Responcecode.Failed,
                    Message = "Account Updation Faile due to - " + ex.Message,
                    Account = null
                });
            }
        }
        public override Task<AccountResponse> Delete(AccountRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.Account account = new AccountComponent.Account();
                account.Id = request.Id;
                account.EmailId = request.EmailId;
                account.Salutation = request.Salutation;
                account.FirstName = request.FirstName;
                account.LastName = request.LastName;                
                account.Dob = request.Dob.Seconds;
                account.AccountType = GetEnum((int) request.Type);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                var result = accountmanager.Delete(account).Result;

                // response 
                AccountResponse response = new AccountResponse();
                response.Code  = Responcecode.Success;
                response.Message = "Delete";
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Deletion Faile due to - " + ex.Message                    
                });
            }
        }
        public override Task<AccountResponse> ChangePassword(AccountRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.Account account = new AccountComponent.Account();
                account.Id = request.Id;
                account.EmailId = request.EmailId;
                account.Salutation = request.Salutation;
                account.FirstName = request.FirstName;
                account.LastName = request.LastName;
                account.Password = request.Password;                
                account.Dob = request.Dob.Seconds;
                account.AccountType = GetEnum((int) request.Type);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                var result = accountmanager.Delete(account).Result;

                // response 
                AccountResponse response = new AccountResponse();
                response.Code  = Responcecode.Success;
                response.Message = "Change Password";
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Change Password faile due to with reason : " + ex.Message                    
                });
            }
        }

         public override Task<AccountDataList> Get(AccountFilter request , ServerCallContext context)
        {
            try
            {
                AccountComponent.AccountFilter filter = new AccountComponent.AccountFilter();
                filter.Id = request.Id;
                filter.OrganizationId = request.OrganizationId;                
                filter.AccountType = GetEnum((int) request.AccountType);                
                filter.AccountIds = null;
                if(request.AccountIds != null && Convert.ToString(request.AccountIds).Length >0)
                {
                    filter.AccountIds = request.AccountIds;                    
                }                
                var result = accountmanager.Get(filter).Result;
                // response 
                AccountDataList response = new AccountDataList();
                foreach(AccountComponent.Account entity in result)
                {
                    response.Accounts.Add(MapToRequest(entity));
                }                
                response.Code  = Responcecode.Success;
                response.Message = "Get";
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountDataList
                {
                    Code = Responcecode.Failed,
                    Message = "Get faile due to with reason : " + ex.Message                                      
                });
            }
        }

        private AccountRequest MapToRequest(AccountComponent.Account account)
        {
            AccountRequest request = new AccountRequest();

            request.Id = account.Id;
            request.EmailId = account.EmailId ;
            request.Salutation = account.Salutation;
            request.FirstName = account.FirstName ;
            request.LastName = account.LastName;
            request.Dob = new Google.Protobuf.WellKnownTypes.Timestamp();
            if (account.Dob.HasValue) request.Dob.Seconds = account.Dob.Value;
            //request.Dob.Seconds  = account.Dob.HasValue ? account.Dob.Value : 0;
            request.Type = SetEnumAccountType(account.AccountType);
            request.OrganizationId = account.Organization_Id;            
            return request;
        }
        private AccountType SetEnumAccountType(AccountComponent.AccountType type)
        {
            AccountType accountType = AccountType.None;

            if ( type == AccountComponent.AccountType.None)
            {
                accountType = AccountType.None;
            }
            else if ( type == AccountComponent.AccountType.SystemAccount)
            {
                accountType = AccountType.SystemAccount;
            }
            else if ( type == AccountComponent.AccountType.PortalAccount)
            {
                accountType = AccountType.PortalAccount;
            }
            return accountType;
        }
        private AccountComponent.AccountType GetEnum(int value)
        {
            AccountComponent.AccountType accountType;
            switch(value)
            {
                case 0:
                accountType = AccountComponent.AccountType.None;
                break;
                case 1:
                accountType = AccountComponent.AccountType.SystemAccount;
                break;
                case 2:
                accountType = AccountComponent.AccountType.PortalAccount;
                break;
                default:
                 accountType = AccountComponent.AccountType.PortalAccount;
                 break;
            }
            return accountType;
        }
    }
}
