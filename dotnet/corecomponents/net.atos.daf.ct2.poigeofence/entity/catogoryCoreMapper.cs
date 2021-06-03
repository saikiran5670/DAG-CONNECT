using System;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class CatogoryCoreMapper
    {

        public Category Map(dynamic record)
        {
            Category category = new Category();
            category.Id = record.id;
            category.Organization_Id = record.organization_id > 0 ? record.organization_id : 0;
            category.Name = !string.IsNullOrEmpty(record.name) ? record.name : string.Empty;
            // category.Icon_Id = record.Icon_Id > 0 ? record.Icon_Id : 0;
            category.Type = !string.IsNullOrEmpty(record.type) ? record.Type : string.Empty;
            category.Parent_Id = record.parent_id > 0 ? record.parent_id : 0;
            category.State = !string.IsNullOrEmpty(record.state) ? Convert.ToString(MapCharToCategoryState(record.state)) : string.Empty;
            category.Created_At = record.created_at > 0 ? record.created_at : 0;
            category.Created_By = record.created_by > 0 ? record.created_by : 0;
            category.Modified_At = record.modified_at > 0 ? record.modified_at : 0;
            category.Modified_By = record.modified_by > 0 ? record.modified_by : 0;
            return category;
        }

        public string MapCharToCategoryState(string state)
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
                case "SUBCATEGORY":
                    ctype = "S";
                    break;
                case "CATEGORY":
                    ctype = "C";
                    break;
            }
            return ctype;

        }
    }
}
