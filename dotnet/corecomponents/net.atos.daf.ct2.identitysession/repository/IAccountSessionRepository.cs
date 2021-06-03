using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.identitysession.entity;


namespace net.atos.daf.ct2.identitysession.repository
{
    public interface IAccountSessionRepository
    {

        Task<int> InsertSession(AccountSession accountSession);
        Task<int> UpdateSession(AccountSession accountSession);
        Task<int> DeleteSession(string SessionId);
        Task<IEnumerable<AccountSession>> GetAccountSession(int AccountId);
        Task<int> DeleteSessionByAccountId(int AccountId);
        Task<AccountSession> GetAccountSessionById(int SessionId);

    }
}