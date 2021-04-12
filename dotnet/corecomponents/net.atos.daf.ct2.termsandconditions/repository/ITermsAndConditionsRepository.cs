using net.atos.daf.ct2.termsandconditions.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.termsandconditions.repository
{
    public interface ITermsAndConditionsRepository
    {
        Task<AccountTermsCondition> AddUserAcceptedTermCondition(AccountTermsCondition accountTermsCondition);
        Task<TermsAndConditions> GetAcceptedTermConditionByUser(int AccountId);
    }
}
