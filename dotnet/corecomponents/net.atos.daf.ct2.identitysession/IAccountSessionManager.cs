using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.identitysession.entity;

namespace net.atos.daf.ct2.identitysession
{
    public interface IAccountSessionManager
    {
         Task<int> InsertSession(AccountSession accountSession);
         Task<int> UpdateSession(AccountSession accountSession);
        Task<int> DeleteSession(AccountSession accountSession);
        Task<IEnumerable<AccountSession>> GetAccountSession(int AccountId);
    }
}
