using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.identitysession.entity;
using net.atos.daf.ct2.identitysession.repository;

namespace net.atos.daf.ct2.identitysession
{
    public class AccountSessionManager : IAccountSessionManager
    {
        readonly IAccountSessionRepository _sessionRepository;
        public AccountSessionManager(IAccountSessionRepository sessionRepository)
        {
            this._sessionRepository = sessionRepository;
        }
        public async Task<int> InsertSession(AccountSession accountSession)
        {
            try
            {
                return await _sessionRepository.InsertSession(accountSession);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> UpdateSession(AccountSession accountSession)
        {
            try
            {
                return await _sessionRepository.UpdateSession(accountSession);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> DeleteSession(string SessionId)
        {
            try
            {
                return await _sessionRepository.DeleteSession(SessionId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<AccountSession>> GetAccountSession(int AccountId)
        {
            try
            {
                return await _sessionRepository.GetAccountSession(AccountId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> DeleteSessionByAccountId(int SessionId)
        {
            try
            {
                return await _sessionRepository.DeleteSessionByAccountId(SessionId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<AccountSession> GetAccountSessionById(int SessionId)
        {
            try
            {
                return await _sessionRepository.GetAccountSessionById(SessionId);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

