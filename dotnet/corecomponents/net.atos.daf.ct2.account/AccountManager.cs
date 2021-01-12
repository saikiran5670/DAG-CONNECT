using net.atos.daf.ct2.audit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using  net.atos.daf.ct2.audit.Enum;
//using net.atos.daf.ct2.identity;

namespace net.atos.daf.ct2.account
{
    public class AccountManager : IAccountManager
    {
        IAccountRepository repository;
        IAuditTraillib auditlog;
        public AccountManager(IAccountRepository _repository, IAuditTraillib _auditlog)
        {
            repository = _repository;
            auditlog = _auditlog;
        }
        public async Task<Account> Create(Account account)  
        {
            
            await repository.Create(account);

             return null;
        }
        public async Task<Account> Update(Account account)  
        {
            return await repository.Update(account);
        }
        public async Task<bool> Delete(int accountid,int organization_id)
        {
            return await repository.Delete(accountid,organization_id);
        }
        public async Task<IEnumerable<Account>> Get(AccountFilter filter)
        {
            return await repository.Get(filter);
        }
    }
}
