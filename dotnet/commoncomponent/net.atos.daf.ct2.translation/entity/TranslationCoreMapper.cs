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
            Entity.id = record.id;
            Entity.code = record.code;
            Entity.Warning_type = !string.IsNullOrEmpty(record.type) ? Convert.ToString(MapCharToDTCType(record.type)) : string.Empty;
            Entity.veh_type = !string.IsNullOrEmpty(record.veh_type) ? record.veh_type : string.Empty;
            Entity.warning_class = record.warningclass;
            Entity.number = record.number;
            Entity.description = !string.IsNullOrEmpty(record.description) ? record.description : string.Empty;
            Entity.advice = AdviceText;
            Entity.expires_at = record.expires_at;
            Entity.icon_id = record.icon_id;
            Entity.created_at = record.created_at;
            Entity.created_by = record.created_by;
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
                    //default:
                    //    Code = "";
                    //    break;

            }
            return Code;

        }
    }
}





















