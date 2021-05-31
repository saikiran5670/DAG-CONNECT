using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.termsandconditions.entity;

namespace net.atos.daf.ct2.termsandconditions
{
    public interface ITermsAndConditionsManager
    {
        Task<AccountTermsCondition> AddUserAcceptedTermCondition(AccountTermsCondition accountTermsCondition);
        Task<List<TermsAndConditions>> GetAcceptedTermConditionByUser(int AccountId, int OrganizationId);
        Task<TermsAndConditions> GetLatestTermCondition(int AccountId, int OrganizationId);
        Task<List<string>> GetAllVersionNo(VersionByID objVersionByID);
        Task<List<TermsAndConditions>> GetTermConditionForVersionNo(string VersionNo, string LanguageCode);
        Task<TermsAndConditionResponseList> UploadTermsAndCondition(TermsandConFileDataList objTermsandConFileDataList);

        Task<InactivateTandCStatusResponceList> InactivateTermsandCondition(InactivateTandCRequestList objInactivateTandCRequestList);

        Task<bool> CheckUserAcceptedTermCondition(int AccountId, int OrganizationId);
    }
}
