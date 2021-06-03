using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.identitysession.entity;

namespace net.atos.daf.ct2.identitysession.repository
{
    public interface IAccountTokenRepository
    {
        Task<int> InsertToken(AccountToken accountToken);
        Task<int> DeleteToken(List<string> token_Id);
        Task<int> DeleteTokenbySessionId(int sessionId);
        Task<IEnumerable<AccountToken>> GetTokenDetails(int AccountID);
        Task<IEnumerable<AccountToken>> GetTokenDetails(string AccessToken);
        Task<bool> ValidateToken(string TokenId);
        Task<int> DeleteTokenByTokenId(Guid tokenID);
        Task<int> GetTokenCount(int AccountID);
        Task<int> DeleteTokenbyAccountId(int sessionId);
    }
}