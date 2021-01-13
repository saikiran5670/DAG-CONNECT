using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.account
{
    public interface IAccountRepository
    {
        Task<Account> Create(Account account);
        Task<Account> Update(Account account);
        Task<bool> Delete(int accountId, int organizationId);
        Task<List<Account>> Get(AccountFilter filter);  
        Task<List<AccessRelationship>> GetAccessRelationship(AccessRelationshipFilter filter);    
    }
}
  