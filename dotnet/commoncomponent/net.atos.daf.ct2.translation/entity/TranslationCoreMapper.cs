using System;
using net.atos.daf.ct2.translation.Enum;

namespace net.atos.daf.ct2.translation.entity
{
    public class TranslationCoreMapper
    {
        public DTCwarning MapWarningDetails(dynamic record)
        {
            string AdviceText = !string.IsNullOrEmpty(record.advice.Replace("\\n", Environment.NewLine)) ? record.advice : string.Empty;


            DTCwarning Entity = new DTCwarning();
            Entity.Id = record.id;
            Entity.Code = record.code;
            Entity.WarningType = !string.IsNullOrEmpty(record.type) ? Convert.ToString(MapCharToDTCType(record.type)) : string.Empty;
            Entity.VehType = !string.IsNullOrEmpty(record.veh_type) ? record.veh_type : string.Empty;
            Entity.WarningClass = record.warningclass;
            Entity.Number = record.number;
            Entity.Description = !string.IsNullOrEmpty(record.description) ? record.description : string.Empty;
            Entity.Advice = AdviceText;
            Entity.ExpiresAt = record.expires_at;
            Entity.IconId = record.icon_id;
            Entity.CreatedAt = record.created_at;
            Entity.CreatedBy = record.created_by;
            return Entity;
        }

        public WarningType MapCharToDTCType(string type)
        {
            var statetype = WarningType.DTC;
            switch (type)
            {
                case "D":
                    statetype = WarningType.DTC;
                    break;
                case "M":
                    statetype = WarningType.DM;
                    break;

            }
            return statetype;

        }

        public string MapDTCTLanguageCode(string languageCode)
        {
            string code;

            switch (languageCode.ToUpper())
            {
                case "BG":
                case "BG-BG":
                    code = "bg-BG";
                    break;
                case "CS":
                case "CS-CZ":
                    code = "cs-CZ";
                    break;
                case "DA":
                case "DA-DK":
                    code = "da-DK";
                    break;
                case "DE":
                case "DE-DE":
                    code = "de-DE";
                    break;
                case "EL":
                case "EL-EL":
                    code = "el-EL";
                    break;
                case "EN":
                case "EN-GB":
                    code = "EN-GB";
                    break;
                case "ES":
                case "ES-ES":
                    code = "es-ES";
                    break;
                case "ET":
                case "ET-EE":
                    code = "et-EE";
                    break;
                case "FI":
                case "FI-FI":
                    code = "fi-FI";
                    break;
                case "FR":
                case "FR-FR":
                    code = "fr-FR";
                    break;
                case "HR":
                case "HR-HR":
                    code = "hr-HR";
                    break;
                case "HU":
                case "HU-HU":
                    code = "hu-HU";
                    break;
                case "IT":
                case "IT-IT":
                    code = "it-IT";
                    break;
                case "LT":
                case "LT-LT":
                    code = "lt-LT";
                    break;
                case "LV":
                case "LV-LV":
                    code = "lv-LV";
                    break;
                case "NL":
                case "NL-NL":
                    code = "nl-NL";
                    break;
                case "NO":
                case "NB-NO":
                    code = "nb-NO";
                    break;
                case "PL":
                case "PL-PL":
                    code = "pl-PL";
                    break;
                case "PT":
                case "PT-PT":
                    code = "pt-PT";
                    break;
                case "RO":
                case "RO-RO":
                    code = "ro-RO";
                    break;
                case "RU":
                case "RU-RU":
                    code = "ru-RU";
                    break;
                case "SK":
                case "SK-SK":
                    code = "sk-SK";
                    break;
                case "SL":
                case "SL-SI":
                    code = "sl-SI";
                    break;
                case "SV":
                case "SV-SE":
                    code = "sv-SE";
                    break;
                case "TR":
                case "Tr-TR":
                    code = "Tr-tr";
                    break;
                default:
                    code = "Unknown";
                    break;

            }
            return code;

        }
    }
}





















