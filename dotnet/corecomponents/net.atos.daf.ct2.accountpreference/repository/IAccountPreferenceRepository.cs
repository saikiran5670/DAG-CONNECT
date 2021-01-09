using System;

namespace net.atos.daf.ct2.accountpreference
{
    public interface IAccountPreferenceRepository
    {
        Task<Accountpreference> Create(Accountpreference preference);
        Task<Accountpreference> Update(Accountpreference preference);
        Task<bool> Delete(long preferenceId);        
        Task<IEnumerable<Accountpreference>> Get(AccountpreferenceFilter filter);
        
    }
}
