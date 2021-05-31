using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.termsandconditions.entity;
using net.atos.daf.ct2.termsandconditions.repository;

namespace net.atos.daf.ct2.termsandconditions
{
    public class TermsAndConditionsManager : ITermsAndConditionsManager
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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<TermsAndConditions>> GetAcceptedTermConditionByUser(int AccountId, int OrganizationId)
        {
            return await termsAndConditionsRepository.GetAcceptedTermConditionByUser(AccountId, OrganizationId);
        }

        public async Task<TermsAndConditions> GetLatestTermCondition(int AccountId, int OrganizationId)
        {
            return await termsAndConditionsRepository.GetLatestTermCondition(AccountId, OrganizationId);
        }

        public async Task<List<TermsAndConditions>> GetTermConditionForVersionNo(string VersionNo, string LanguageCode)
        {
            return await termsAndConditionsRepository.GetTermConditionForVersionNo(VersionNo, LanguageCode);
        }

        public async Task<TermsAndConditionResponseList> UploadTermsAndCondition(TermsandConFileDataList objTermsandConFileDataList)
        {
            return await termsAndConditionsRepository.UploadTermsAndCondition(objTermsandConFileDataList);
        }
        public async Task<InactivateTandCStatusResponceList> InactivateTermsandCondition(InactivateTandCRequestList objInactivateTandCRequestList)
        {
            return await termsAndConditionsRepository.InactivateTermsandCondition(objInactivateTandCRequestList);
        }
        public async Task<bool> CheckUserAcceptedTermCondition(int AccountId, int OrganizationId)
        {
            return await termsAndConditionsRepository.CheckUserAcceptedTermCondition(AccountId, OrganizationId);
        }

        public async Task<List<string>> GetAllVersionNo(VersionByID objVersionByID)
        {
            return await termsAndConditionsRepository.GetAllVersionNo(objVersionByID);
        }
    }
}
