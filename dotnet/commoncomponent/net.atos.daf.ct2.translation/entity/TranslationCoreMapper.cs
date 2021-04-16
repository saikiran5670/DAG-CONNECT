using net.atos.daf.ct2.translation.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.translation.entity
{
    public class TranslationCoreMapper
    {
        public DTCwarning MapWarningDetails(dynamic record)
        {
            DTCwarning Entity = new DTCwarning();
            Entity.id = record.id;
            Entity.code = record.code;
            Entity.Warning_type = !string.IsNullOrEmpty(record.type) ? Convert.ToString(MapCharToDTCType(record.type)) : string.Empty;
            Entity.veh_type = !string.IsNullOrEmpty(record.veh_type) ? record.veh_type : string.Empty;
            Entity.warning_class = record.warningclass;
            Entity.number = record.number;
            Entity.description =  !string.IsNullOrEmpty(record.description) ? record.description : string.Empty;
            Entity.advice = !string.IsNullOrEmpty(record.advice) ? record.advice : string.Empty; 
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
    }
}
