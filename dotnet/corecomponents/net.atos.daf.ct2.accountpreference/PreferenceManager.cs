using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;

namespace net.atos.daf.ct2.accountpreference
{
    public class PreferenceManager : IPreferenceManager
    {
        IAccountPreferenceRepository repository;
        IAuditTraillib auditlog;
        public PreferenceManager(IAccountPreferenceRepository _repository, IAuditTraillib _auditlog)
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
        public async Task<bool> Delete(int preferenceId, PreferenceType preferenceType)
        {
            return await repository.Delete(preferenceId, preferenceType);
        }
        public async Task<IEnumerable<AccountPreference>> Get(AccountPreferenceFilter filter)
        {
            return await repository.Get(filter);
        }
    }
}
