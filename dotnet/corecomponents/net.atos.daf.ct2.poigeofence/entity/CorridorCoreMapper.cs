using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence.entity
{
   public class CorridorCoreMapper
    {

        public RouteCorridor Map(dynamic record)
        {
            RouteCorridor routeCorridor = new RouteCorridor();
            routeCorridor.Id = record.id;
            routeCorridor.OrganizationId = record.organization_id > 0 ? record.organization_id : 0;
            routeCorridor.CorridorLabel = !string.IsNullOrEmpty(record.name) ? record.name : string.Empty;
            routeCorridor.StartAddress = !string.IsNullOrEmpty(record.address) ? record.address : string.Empty;
            routeCorridor.CorridorType = !string.IsNullOrEmpty(record.type) ? record.type : string.Empty;
            routeCorridor.Width = record.distance > 0 ? record.distance : 0;
            routeCorridor.State = !string.IsNullOrEmpty(record.state) ? Convert.ToString(MapCharToLandmarkState(record.state)) : string.Empty;
            routeCorridor.Created_At = record.created_at > 0 ? record.created_at : 0;
            routeCorridor.Created_By = record.created_by > 0 ? record.created_by : 0;
            routeCorridor.Modified_At = record.modified_at > 0 ? record.modified_at : 0;
            routeCorridor.Modified_By = record.modified_by > 0 ? record.modified_by : 0;
            return routeCorridor;

            //id, organization_id,  name, address, type, distance, state, created_at, created_by, modified_at, modified_by
        }

        public string MapCharToLandmarkState(string state)
        {

            var ptype = string.Empty;
            switch (state)
            {
                case "A":
                    ptype = "Active";
                    break;
                case "I":
                    ptype = "Inactive";
                    break;
                case "D":
                    ptype = "Delete";
                    break;
            }
            return ptype;
        }
        public string MapType(string Type)
        {
            var ctype = string.Empty;
            switch (Type)
            {
                case "Route Corridor":
                    ctype = "R";
                    break;
                //case "CATEGORY":
                //    ctype = "C";
                //    break;
            }
            return ctype;

        }
    }
}
