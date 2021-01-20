using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.identitysession.entity;

namespace net.atos.daf.ct2.identitysession.repository
{
   public interface IAccountAssertionRepository
    {
        Task<int> InsertAssertion(AccountAssertion accountAssertion);
        Task<int> UpdateAssertion(AccountAssertion accountAssertion);
        Task<int> DeleteAssertion(int accountId);
        Task<IEnumerable<AccountAssertion>> GetAssertion(int accountId);
    }
}