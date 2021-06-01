using System;

namespace net.atos.daf.ct2.features.entity
{
    public class FeatureCoreMapper
    {
        public FeatureSet Map(dynamic record)
        {
            FeatureSet feature = new FeatureSet();
            feature.FeatureSetID = record.id ?? 0;
            feature.Name = !string.IsNullOrEmpty(record.Name) ? record.Name : string.Empty;
            feature.Description = !string.IsNullOrEmpty(record.Description) ? record.Description : string.Empty;
            feature.State = !string.IsNullOrEmpty(record.State) ? MapCharToFeatureState(record.State) : string.Empty;
            return feature;
        }

        public StatusType MapCharToFeatureState(string status)
        {
            var statetype = StatusType.ACTIVE;
            switch (status)
            {
                case "A":
                    statetype = StatusType.ACTIVE;
                    break;
                case "I":
                    statetype = StatusType.INACTIVE;
                    break;
                case "D":
                    statetype = StatusType.DELETE;
                    break;
            }
            return statetype;

        }

        public DataAttributeSet MapDataAttributeSet(dynamic record)
        {
            //SELECT id, name, description, is_exlusive, created_at, created_by, modified_at, modified_by, state
            DataAttributeSet da = new DataAttributeSet();
            da.ID = record.ID ?? 0;
            da.Name = !string.IsNullOrEmpty(record.Name) ? record.Name : string.Empty;
            da.Description = !string.IsNullOrEmpty(record.Description) ? record.Description : string.Empty;
            da.Is_exlusive = record.Is_exlusive;
            da.Created_at = record.Created_at ?? 0;
            da.Created_by = record.Created_by ?? 0;
            da.Modified_at = record.Modified_at ?? 0;
            da.Modified_by = record.Modified_by ?? 0;
            da.State = record.State;
            return da;
        }

        public Feature MapFeature(dynamic record)
        {
            //SELECT f.id, f.name,  f.type, f.state,f.data_attribute_set_id,f.key,r.id as roleid  , r.organization_id
            //SELECT f.id, f.name,t.value, f.type, f.state, f.data_attribute_set_id, f.key, f.level, f.state

            Feature feature = new Feature();
            feature.Id = record.Id ?? 0;
            feature.Name = !string.IsNullOrEmpty(record.Name) ? record.Name : string.Empty;
            feature.Type = Convert.ToChar(record.Type) != null ? record.Type : string.Empty;
            feature.State = !string.IsNullOrEmpty(record.State) ? Convert.ToString(MapCharToFeatureState(record.State)) : string.Empty;
            feature.Data_attribute_Set_id = record.Data_attribute_Set_id ?? 0;
            feature.Key = !string.IsNullOrEmpty(record.Key) ? record.Key : string.Empty;
            feature.Level = record.Level ?? 0;            
            feature.RoleId = record.RoleId ?? 0;
            feature.Organization_Id = record.Organization_Id ?? 0;
            // feature.modified_by = record.modified_by;

            return feature;
        }


        public Feature MapFeatureSetDetails(dynamic record)
        {
            //Select f.id,f.name,t.value,f.type,f.state,f.data_attribute_set_id,f.key,f.level,fs.feature_set_id 

            Feature feature = new Feature();
            feature.Id = record.Id ?? 0;
            feature.Name = !string.IsNullOrEmpty(record.Name) ? record.Name : string.Empty;
            feature.Value = !string.IsNullOrEmpty(record.Value) ? record.Value : string.Empty;
            feature.Type = record.Type ?? string.Empty;
            feature.State = !string.IsNullOrEmpty(record.State) ? Convert.ToString(MapCharToFeatureState(record.State)) : string.Empty;
            feature.Data_attribute_Set_id = record.Data_attribute_Set_id ?? 0;
            feature.Key = !string.IsNullOrEmpty(record.Key) ? record.Key : string.Empty;
            feature.Level = record.Level ?? 0;
            //featureset.FeatureSetID = record.FeatureSetID != null ? record.FeatureSetID : 0;
            // feature.modified_by = record.modified_by;

            return feature;
        }
    }
}
