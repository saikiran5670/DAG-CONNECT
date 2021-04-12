using net.atos.daf.ct2.termsandconditions.entity;
using net.atos.daf.ct2.termsandconditions.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.termsandconditions
{
    public class TermsAndConditionsManager:ITermsAndConditionsManager
    {
        ITermsAndConditionsRepository termsAndConditionsRepository;
        public TermsAndConditionsManager(ITermsAndConditionsRepository _termsAndConditionsRepository)
        {
            termsAndConditionsRepository = _termsAndConditionsRepository;
        }

        public Task<AccountTermsCondition> AddUserAcceptedTermCondition(AccountTermsCondition accountTermsCondition)
        {
            throw new NotImplementedException();
        }

        public Task<TermsAndConditions> GetAcceptedTermConditionByUser(int AccountId)
        {
            throw new NotImplementedException();
        }
    }
}
