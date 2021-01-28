using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.identitysession.entity;
namespace net.atos.daf.ct2.identitysession
{
    public interface IAccountAssertionManager
    {
        Task<int> InsertAssertion(AccountAssertion accountAssertion);
        Task<int> UpdateAssertion(AccountAssertion accountAssertion);
        Task<int> DeleteAssertion(int accountId);
        Task<string> DeleteAssertion(string sessionId);
        Task<IEnumerable<AccountAssertion>> GetAssertion(int accountId);
    }
}
