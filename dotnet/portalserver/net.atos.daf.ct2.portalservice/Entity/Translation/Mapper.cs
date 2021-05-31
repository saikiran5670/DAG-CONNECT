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
            if (request != null && request.file != null)
            {
                foreach (var item in request.file)
                {
                    response.File.Add(new TranslationData() { Code = item.code, Type = item.type, Name = item.name, Value = item.value });
                }
            }
            response.FileName = request.file_name;
            response.Description = request.description;
            response.FileSize = request.file_size;
            response.FailureCount = request.failure_count;
            response.AddedCount = request.added_count;
            response.UpdatedCount = request.updated_count;

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
            foreach (var x in request.dtcWarningToImport)
            {
                var dtcRequest = new dtcwarning()
                {
                    Code = x.code,
                    Type = x.type,
                    VehType = x.veh_type != null ? x.veh_type : string.Empty,
                    WarningClass = x.warning_class,
                    Number = x.number,
                    Description = x.description,
                    Advice = x.advice,
                    ExpiresAt = x.expires_at,
                    IconId = x.icon_id,
                    CreatedBy = x.created_by
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
            acceptedTermConditionRequest.VersionNo = request.version_no;
            acceptedTermConditionRequest.TermsAndConditionId = request.Terms_And_Condition_Id;
            return acceptedTermConditionRequest;
        }

        public IconUpdateRequest ToImportDTCWarningIcon(DTCWarningIconUpdateRequest request)
        {

            var dtcRequests = new IconUpdateRequest();

            foreach (var x in request.dtcWarningUpdateIcon)
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
            string Code = string.Empty;

            switch (LanguageCode)
            {
                case "BG":
                    Code = "bg-BG";
                    break;
                case "CS":
                    Code = "cs-CZ";
                    break;
                case "DA":
                    Code = "da-DK";
                    break;
                case "DE":
                    Code = "de-DE";
                    break;
                case "EL":
                    Code = "EL";
                    break;
                case "EN":
                    Code = "EN-GB";
                    break;
                case "ES":
                    Code = "es-ES";
                    break;
                case "ET":
                    Code = "et-EE";
                    break;
                case "FI":
                    Code = "fi-FI";
                    break;
                case "FR":
                    Code = "fr-FR";
                    break;
                case "HR":
                    Code = "hr-HR";
                    break;
                case "HU":
                    Code = "hu-HU";
                    break;
                case "IT":
                    Code = "it-IT";
                    break;
                case "LT":
                    Code = "lt-LT";
                    break;
                case "LV":
                    Code = "lv-LV";
                    break;
                case "NL":
                    Code = "nl-NL";
                    break;
                case "NO":
                    Code = "nb-NO";
                    break;
                case "PL":
                    Code = "pl-PL";
                    break;
                case "PT":
                    Code = "pt-PT";
                    break;
                case "RO":
                    Code = "ro-RO";
                    break;
                case "RU":
                    Code = "RU";
                    break;
                case "SK":
                    Code = "sk-SK";
                    break;
                case "SL":
                    Code = "sl-SI";
                    break;
                case "SV":
                    Code = "sv-SE";
                    break;
                case "TR":
                    Code = "Tr-tr";
                    break;
                default:
                    Code = "Unknown Language Code";
                    break;

            }
            return Code;

        }


    }
}
