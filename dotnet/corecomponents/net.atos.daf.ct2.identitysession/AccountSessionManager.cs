using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using net.atos.daf.ct2.identitysession.entity;
using net.atos.daf.ct2.identitysession.repository;

namespace net.atos.daf.ct2.identitysession
{
    public class AccountSessionManager : IAccountSessionManager
    {
        IAccountSessionRepository sessionRepository;
        public AccountSessionManager(IAccountSessionRepository _sessionRepository)
        {
            sessionRepository = _sessionRepository;
        }
        public async Task<int> InsertSession(AccountSession accountSession)
        {
            try
            {
                return await sessionRepository.InsertSession(accountSession);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> UpdateSession(AccountSession accountSession)
        {
            try
            {
                return await sessionRepository.UpdateSession(accountSession);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> DeleteSession(string SessionId)
        {
            try
            {
                return await sessionRepository.DeleteSession(SessionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<AccountSession>> GetAccountSession(int AccountId)
        {
            try
            {
                return await sessionRepository.GetAccountSession(AccountId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> DeleteSessionByAccountId(int SessionId)
        {
            try
            {
                return await sessionRepository.DeleteSessionByAccountId(SessionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<AccountSession> GetAccountSessionById(int SessionId)
        {
            try
            {
                return await sessionRepository.GetAccountSessionById(SessionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

