using net.atos.daf.ct2.audit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.accountpreference
{
    public class PreferenceManager:IPreferenceManager
    {
        IAccountPreferenceRepository repository;
        IAuditLog auditlog;

        public PreferenceManager(IAccountPreferenceRepository _repository, IAuditLog _auditlog)
        {
            repository = _repository;
            auditlog = _auditlog;
        }
        public async Task<AccountPreference> Create(AccountPreference preference)
        {
            return await repository.Create(preference);
        }
        public async Task<AccountPreference> Update(AccountPreference preference)
        {
            return await repository.Update(preference);
        }
        public async  Task<bool> Delete(int preferenceId)
        {
            return await repository.Delete(preferenceId);
        }       
        public async Task<IEnumerable<AccountPreference>> Get(AccountPreferenceFilter filter)
        {
            return await repository.Get(filter);
        }
    }
}
