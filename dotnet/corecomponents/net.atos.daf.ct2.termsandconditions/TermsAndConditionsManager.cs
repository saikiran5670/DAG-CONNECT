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

        public async Task<AccountTermsCondition> AddUserAcceptedTermCondition(AccountTermsCondition accountTermsCondition)
        {
            try
            {
                return await termsAndConditionsRepository.AddUserAcceptedTermCondition(accountTermsCondition);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<TermsAndConditions>> GetAcceptedTermConditionByUser(int AccountId, int OrganizationId)
        {
            return await termsAndConditionsRepository.GetAcceptedTermConditionByUser(AccountId, OrganizationId);
        }

        public async Task<List<string>> GetAllVersionNo()
        {
            return await termsAndConditionsRepository.GetAllVersionNo();
        }

        public async Task<TermsAndConditions> GetLatestTermCondition(int AccountId, int OrganizationId)
        {
            return await termsAndConditionsRepository.GetLatestTermCondition(AccountId, OrganizationId);
        }

        public async Task<List<TermsAndConditions>> GetTermConditionForVersionNo(string VersionNo, string LanguageCode)
        {
            return await termsAndConditionsRepository.GetTermConditionForVersionNo(VersionNo,LanguageCode);
        }


    }
}
