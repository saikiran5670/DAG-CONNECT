using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.identitysession.entity;
using net.atos.daf.ct2.identitysession.repository;

namespace net.atos.daf.ct2.identitysession
{
    public class AccountAssertionManager : IAccountAssertionManager
    {
        readonly IAccountAssertionRepository _accountassertionRepository;
        public AccountAssertionManager(IAccountAssertionRepository accountassertionRepository)
        {
            this._accountassertionRepository = accountassertionRepository;
        }
        public async Task<int> InsertAssertion(AccountAssertion accountAssertion)
        {
            try
            {
                return await _accountassertionRepository.InsertAssertion(accountAssertion);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateAssertion(AccountAssertion accountAssertion)
        {
            try
            {
                return await _accountassertionRepository.UpdateAssertion(accountAssertion);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> DeleteAssertion(int accountId)
        {
            try
            {
                return await _accountassertionRepository.DeleteAssertion(accountId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> DeleteAssertionbySessionId(int sessionId)
        {
            try
            {
                return await _accountassertionRepository.DeleteAssertionbySessionId(sessionId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<AccountAssertion>> GetAssertion(int accountId)
        {
            try
            {
                return await _accountassertionRepository.GetAssertion(accountId);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
