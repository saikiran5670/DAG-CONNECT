using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.identitysession.entity;
using net.atos.daf.ct2.identitysession.repository;

namespace net.atos.daf.ct2.identitysession
{
    public class AccountAssertionManager : IAccountAssertionManager
    {
        IAccountAssertionRepository accountassertionRepository;
        public AccountAssertionManager(IAccountAssertionRepository _accountassertionRepository)
        {
            accountassertionRepository = _accountassertionRepository;
        }
        public async Task<int> InsertAssertion(AccountAssertion accountAssertion)
        {
            try
            {
                return await accountassertionRepository.InsertAssertion(accountAssertion);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> UpdateAssertion(AccountAssertion accountAssertion)
        {
             try
            {
                return await accountassertionRepository.UpdateAssertion(accountAssertion);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> DeleteAssertion(int accountId)
        {
             try
            {
                return await accountassertionRepository.DeleteAssertion(accountId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> DeleteAssertionbySessionId(int sessionId)
        {
             try
            {
                return await accountassertionRepository.DeleteAssertionbySessionId(sessionId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<AccountAssertion>> GetAssertion(int accountId)
        {
             try
            {
                return await accountassertionRepository.GetAssertion(accountId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
