using System;
using Google.Protobuf;
using net.atos.daf.ct2.translationservice;

namespace net.atos.daf.ct2.portalservice.Entity.Translation
{
    public class Mapper
    {
        public dropdownarrayRequest MapDropdown(DropdownRequest request)
        {
            dropdownarrayRequest response = new dropdownarrayRequest();
            if (request == null) return response;
            if (request != null && request.Dropdownname != null && Convert.ToInt16(request.Dropdownname.Count) > 0)
            {
                foreach (var item in request.Dropdownname)
                {
                    if (item != null)
                    {
                        response.Dropdownname.Add(new DropdownName() { Dropdownname = item });
                    }
                }
            }
            response.Languagecode = request.Languagecode;

            return response;
        }
        public TranslationUploadRequest MapFileDetailRequest(FileUploadRequest request)
        {
            TranslationUploadRequest response = new TranslationUploadRequest();
            if (request == null) return response;
            if (request != null && request.File != null)
            {
                foreach (var item in request.File)
                {
                    response.File.Add(new TranslationData() { Code = item.Code, Type = item.Type, Name = item.Name, Value = item.Value });
                }
            }
            response.FileName = request.File_name;
            response.Description = request.Description;
            response.FileSize = request.File_size;
            response.FailureCount = request.Failure_count;
            response.AddedCount = request.Added_count;
            response.UpdatedCount = request.Updated_count;

            return response;

        }
        public TranslationsRequest MapGetTranslations(TranslationRequest request)
        {
            TranslationsRequest response = new TranslationsRequest();
            if (request == null) return response;
            if (request != null)
            {
                response.Id = request.Id;
                response.Code = request.Code;
                response.Type = request.Type;
                response.Name = request.Name;
                response.Value = request.Value;
                response.Filter = request.Filter;
                response.MenuId = request.MenuId;
            }
            return response;
        }

        public WarningDataRequest ToImportDTCWarning(DTCWarningImportRequest request)
        {

            var dtcRequests = new WarningDataRequest();
            //id,code , type,veh_type,class,number,description,advice, expires_at,icon_id,created_by,
            foreach (var x in request.DtcWarningToImport)
            {
                var dtcRequest = new dtcwarning()
                {
                    Code = x.Code,
                    Type = x.Type,
                    VehType = x.Veh_type ?? string.Empty,
                    WarningClass = x.Warning_class,
                    Number = x.Number,
                    Description = x.Description,
                    Advice = x.Advice,
                    ExpiresAt = x.Expires_at,
                    IconId = x.Icon_id,
                    CreatedBy = x.Created_by
                };
                dtcRequests.DtcData.Add(dtcRequest);

            }
            return dtcRequests;

        }

        public AcceptedTermConditionRequest ToAcceptedTermConditionRequestEntity(AccountTermsCondition request)
        {
            AcceptedTermConditionRequest acceptedTermConditionRequest = new AcceptedTermConditionRequest();
            acceptedTermConditionRequest.Id = request.Id;
            acceptedTermConditionRequest.AccountId = request.Account_Id;
            acceptedTermConditionRequest.OrganizationId = request.Organization_Id;
            acceptedTermConditionRequest.VersionNo = request.Version_no;
            acceptedTermConditionRequest.TermsAndConditionId = request.Terms_And_Condition_Id;
            return acceptedTermConditionRequest;
        }

        public IconUpdateRequest ToImportDTCWarningIcon(DTCWarningIconUpdateRequest request)
        {

            var dtcRequests = new IconUpdateRequest();

            foreach (var x in request.DtcWarningUpdateIcon)
            {
                if (x.Name != null || x.Name != "" || x.Icon != null)
                {

                    var dtcRequest = new dtcIconUpdate()
                    {
                        Name = x.Name,
                        Icon = ByteString.CopyFrom(x.Icon),
                        ModifiedBy = x.ModifiedBy
                    };
                    dtcRequests.IconData.Add(dtcRequest);
                }
            }
            return dtcRequests;

        }

        public string MapDTCTLanguageCode(string LanguageCode)
        {
            string code;

            switch (LanguageCode)
            {
                case "BG":
                    code = "bg-BG";
                    break;
                case "CS":
                    code = "cs-CZ";
                    break;
                case "DA":
                    code = "da-DK";
                    break;
                case "DE":
                    code = "de-DE";
                    break;
                case "EL":
                    code = "EL";
                    break;
                case "EN":
                    code = "EN-GB";
                    break;
                case "ES":
                    code = "es-ES";
                    break;
                case "ET":
                    code = "et-EE";
                    break;
                case "FI":
                    code = "fi-FI";
                    break;
                case "FR":
                    code = "fr-FR";
                    break;
                case "HR":
                    code = "hr-HR";
                    break;
                case "HU":
                    code = "hu-HU";
                    break;
                case "IT":
                    code = "it-IT";
                    break;
                case "LT":
                    code = "lt-LT";
                    break;
                case "LV":
                    code = "lv-LV";
                    break;
                case "NL":
                    code = "nl-NL";
                    break;
                case "NO":
                    code = "nb-NO";
                    break;
                case "PL":
                    code = "pl-PL";
                    break;
                case "PT":
                    code = "pt-PT";
                    break;
                case "RO":
                    code = "ro-RO";
                    break;
                case "RU":
                    code = "RU";
                    break;
                case "SK":
                    code = "sk-SK";
                    break;
                case "SL":
                    code = "sl-SI";
                    break;
                case "SV":
                    code = "sv-SE";
                    break;
                case "TR":
                    code = "Tr-tr";
                    break;
                default:
                    code = "Unknown Language Code";
                    break;

            }
            return code;
        }
    }
}
