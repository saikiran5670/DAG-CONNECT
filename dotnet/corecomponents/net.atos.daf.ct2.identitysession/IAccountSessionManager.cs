using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.identitysession.entity;

namespace net.atos.daf.ct2.identitysession
{
    public interface IAccountSessionManager
    {
         Task<string> InsertSession(AccountSession accountSession);
         Task<string> UpdateSession(AccountSession accountSession);
        Task<string> DeleteSession(AccountSession accountSession);
        Task<IEnumerable<AccountSession>> GetAccountSession(int AccountId);
    }
}
