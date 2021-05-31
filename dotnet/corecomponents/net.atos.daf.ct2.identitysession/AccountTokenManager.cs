using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.identitysession.entity;
using net.atos.daf.ct2.identitysession.repository;

namespace net.atos.daf.ct2.identitysession
{
    public class AccountTokenManager : IAccountTokenManager
    {
        IAccountTokenRepository tokenRepository;
        public AccountTokenManager(IAccountTokenRepository _tokenRepository)
        {
            tokenRepository = _tokenRepository;
        }
        public async Task<int> InsertToken(AccountToken accountToken)
        {
            try
            {
                return await tokenRepository.InsertToken(accountToken);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> DeleteToken(List<string> token_Id)
        {
            try
            {
                return await tokenRepository.DeleteToken(token_Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> DeleteTokenbySessionId(int sessionID)
        {
            try
            {
                return await tokenRepository.DeleteTokenbySessionId(sessionID);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<AccountToken>> GetTokenDetails(int AccountID)
        {
            try
            {
                return await tokenRepository.GetTokenDetails(AccountID);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<AccountToken>> GetTokenDetails(string TokenId)
        {
            try
            {
                return await tokenRepository.GetTokenDetails(TokenId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> ValidateToken(string TokenId)
        {
            try
            {
                return await tokenRepository.ValidateToken(TokenId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> DeleteTokenByTokenId(Guid tokenID)
        {
            try
            {
                return await tokenRepository.DeleteTokenByTokenId(tokenID);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> GetTokenCount(int AccountID)
        {
            try
            {
                return await tokenRepository.GetTokenCount(AccountID);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> DeleteTokenbyAccountId(int sessionID)
        {
            try
            {
                return await tokenRepository.DeleteTokenbySessionId(sessionID);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
