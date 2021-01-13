using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using AccountComponent = net.atos.daf.ct2.account;
//using net.atos.daf.ct2.accountpreference;
//using net.atos.daf.ct2.accountservice;

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

        public override Task<AccountResponse> Create(AccountRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.Account account = new AccountComponent.Account();
                account.Id = request.Id;
                account.EmailId = request.EmailId;
                account.Salutation = request.Salutation;
                account.FirstName = request.FirstName;
                account.LastName = request.LastName;                
                account.Dob = request.Dob.ToDateTime();
                account.AccountType = GetEnum((int) request.Type);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                account = accountmanager.Create(account).Result;
                return Task.FromResult(new AccountResponse
                {
                    Message = "Account Created:" + account.Id
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountResponse
                {
                    Message = "Account Creation Faile due to - " + ex.Message
                });
            }
        }
        // public override Task<AccountResponse> Update(AccountRequest request, ServerCallContext context)
        // {
        //     try
        //     {
        //         AccountComponent.Account account = new AccountComponent.Account();
        //         account.Id = request.Id;
        //         account.EmailId = request.EmailId;
        //         account.Salutation = request.Salutation;
        //         account.FirstName = request.FirstName;
        //         account.LastName = request.LastName;                
        //         account.Dob = request.Dob.ToDateTime();
        //         //account.AccountType =  AccountComponent.AccountType.PortalAccount;
        //         account.AccountType = GetEnum((int) request.Type);
        //         account.Organization_Id = request.OrganizationId;

        //         account.StartDate = DateTime.Now;
        //         account.EndDate = null;
        //         account = accountmanager.Create(account).Result;

        //         return Task.FromResult(new AccountResponse
        //         {
        //             Message = "Account Created " + account.Id
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return Task.FromResult(new AccountResponse
        //         {
        //             Message = "Exception " + ex.Message
        //         });
        //     }
        // }
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
