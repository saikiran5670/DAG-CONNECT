using System;
using AccountComponent = net.atos.daf.ct2.account;
using Preference = net.atos.daf.ct2.accountpreference;
using Group = net.atos.daf.ct2.group;
namespace net.atos.daf.ct2.accountservice
{
    public class Mapper
    {
        public AccountComponent.entity.Account ToAccountEntity(AccountRequest request)
        {
            var account = new AccountComponent.entity.Account();
            account.Id = request.Id;
            account.EmailId = request.EmailId;
            account.Salutation = request.Salutation;
            account.FirstName = request.FirstName;
            account.LastName = request.LastName;
            account.Password = request.Password;
            account.Organization_Id = request.OrganizationId;
            account.StartDate = DateTime.Now;
            account.EndDate = null;
            return account;
        }
        public AccountRequest ToAccountDetail(AccountComponent.entity.Account account)
        {
            AccountRequest response = new AccountRequest();

            response.Id = account.Id;
            response.EmailId = account.EmailId;
            response.Salutation = account.Salutation;
            response.FirstName = account.FirstName;
            response.LastName = account.LastName;
            response.OrganizationId = account.Organization_Id;
            return response;
        }
    }
}
