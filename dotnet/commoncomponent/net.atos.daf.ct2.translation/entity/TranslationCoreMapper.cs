using net.atos.daf.ct2.translation.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.translation.entity
{
    public class TranslationCoreMapper
    {
        private DTCwarning MapWarningDetails(dynamic record)
        {
            DTCwarning Entity = new DTCwarning();
            Entity.id = record.id;
            Entity.code = record.code;
            Entity.type = !string.IsNullOrEmpty(record.type) ? MapCharToDTCType(record.type) : string.Empty;
            Entity.veh_type = record.veh_type;
            Entity.warning_class = record.warning_class;
            Entity.number = record.number;
            Entity.description = record.description;
            Entity.advice = record.advice;
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
