using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.accountpreference
{
    public interface IAccountPreferenceRepository
    {
        Task<AccountPreference> Create(AccountPreference preference);
        Task<AccountPreference> Update(AccountPreference preference);
        Task<bool> Delete(int preferenceId, PreferenceType preferenceType);
        Task<IEnumerable<AccountPreference>> Get(AccountPreferenceFilter filter);

    }
}
