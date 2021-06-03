using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.termsandconditions.entity;
using net.atos.daf.ct2.termsandconditions.repository;

namespace net.atos.daf.ct2.termsandconditions
{
    public class TermsAndConditionsManager : ITermsAndConditionsManager
    {
        ITermsAndConditionsRepository _termsAndConditionsRepository;
        public TermsAndConditionsManager(ITermsAndConditionsRepository termsAndConditionsRepository)
        {
            this._termsAndConditionsRepository = termsAndConditionsRepository;
        }

        public async Task<AccountTermsCondition> AddUserAcceptedTermCondition(AccountTermsCondition accountTermsCondition)
        {
            try
            {
                return await _termsAndConditionsRepository.AddUserAcceptedTermCondition(accountTermsCondition);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<TermsAndConditions>> GetAcceptedTermConditionByUser(int AccountId, int OrganizationId)
        {
            return await _termsAndConditionsRepository.GetAcceptedTermConditionByUser(AccountId, OrganizationId);
        }

        public async Task<TermsAndConditions> GetLatestTermCondition(int AccountId, int OrganizationId)
        {
            return await _termsAndConditionsRepository.GetLatestTermCondition(AccountId, OrganizationId);
        }

        public async Task<List<TermsAndConditions>> GetTermConditionForVersionNo(string VersionNo, string LanguageCode)
        {
            return await _termsAndConditionsRepository.GetTermConditionForVersionNo(VersionNo, LanguageCode);
        }

        public async Task<TermsAndConditionResponseList> UploadTermsAndCondition(TermsandConFileDataList objTermsandConFileDataList)
        {
            return await _termsAndConditionsRepository.UploadTermsAndCondition(objTermsandConFileDataList);
        }
        public async Task<InactivateTandCStatusResponceList> InactivateTermsandCondition(InactivateTandCRequestList objInactivateTandCRequestList)
        {
            return await _termsAndConditionsRepository.InactivateTermsandCondition(objInactivateTandCRequestList);
        }
        public async Task<bool> CheckUserAcceptedTermCondition(int AccountId, int OrganizationId)
        {
            return await _termsAndConditionsRepository.CheckUserAcceptedTermCondition(AccountId, OrganizationId);
        }

        public async Task<List<string>> GetAllVersionNo(VersionByID objVersionByID)
        {
            return await _termsAndConditionsRepository.GetAllVersionNo(objVersionByID);
        }
    }
}
