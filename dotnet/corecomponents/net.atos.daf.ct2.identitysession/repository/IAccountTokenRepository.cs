using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.identitysession.entity;

namespace net.atos.daf.ct2.identitysession.repository
{
   public interface IAccountTokenRepository
    {
        Task<string> InsertToken(AccountToken accountToken);
        Task<int> DeleteToken(List<string> token_Id);
        Task<string> DeleteTokenbySessionId(string sessionId);
        Task<IEnumerable<AccountToken>> GetTokenDetails(int AccountID);
        Task<IEnumerable<AccountToken>> GetTokenDetails(string AccessToken);
        Task<bool> ValidateToken(string TokenId);
    }
}