using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace net.atos.daf.ct2.account
{
    public interface IAccountManager
    {
        Task<Account> Create(Account account);
        Task<Account> Update(Account account);
        Task<bool> Delete(Account account);
        Task<bool> ChangePassword(Account account);
        Task<IEnumerable<Account>> Get(AccountFilter filter);  
        Task<List<AccessRelationship>> GetAccessRelationship(AccessRelationshipFilter filter);
    }
}
