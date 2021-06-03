using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;

namespace net.atos.daf.ct2.accountpreference
{
    public class PreferenceManager : IPreferenceManager
    {
        IAccountPreferenceRepository _repository;
        IAuditTraillib _auditlog;
        public PreferenceManager(IAccountPreferenceRepository repository, IAuditTraillib auditlog)
        {
            _repository = repository;
            _auditlog = auditlog;
        }
        public async Task<AccountPreference> Create(AccountPreference preference)
        {
            return await _repository.Create(preference);
        }
        public async Task<AccountPreference> Update(AccountPreference preference)
        {
            return await _repository.Update(preference);
        }
        public async Task<bool> Delete(int preferenceId, PreferenceType preferenceType)
        {
            return await _repository.Delete(preferenceId, preferenceType);
        }
        public async Task<IEnumerable<AccountPreference>> Get(AccountPreferenceFilter filter)
        {
            return await _repository.Get(filter);
        }
    }
}
