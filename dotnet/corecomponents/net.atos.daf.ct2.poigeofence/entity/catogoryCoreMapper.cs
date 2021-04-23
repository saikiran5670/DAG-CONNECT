using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class catogoryCoreMapper
    {

        public Category Map (dynamic record)
        {
            Category category = new Category();
            category.Id = record.id;
            category.Organization_Id = !string.IsNullOrEmpty(record.Organization_Id) ? record.Organization_Id : string.Empty;
            category.Name = !string.IsNullOrEmpty(record.Name) ? record.Name : string.Empty;
            category.Icon_Id = record.Icon_Id > 0 ? record.Icon_Id : 0;
            category.Type = !string.IsNullOrEmpty(record.Type) ? record.Type : string.Empty;
            category.Parent_Id = record.Parent_Id > 0 ? record.Parent_Id : 0;
            category.State = !string.IsNullOrEmpty(record.state) ? Convert.ToString( MapCharToCategoryState(record.state)) : string.Empty;
            category.Created_At = record.Created_At > 0 ? record.Created_At : 0;
            category.Created_By = record.Created_By > 0 ? record.Created_By : 0;
            category.Modified_At = record.Modified_At > 0 ? record.Modified_At : 0;
            category.Modified_By = record.Modified_By > 0 ? record.Modified_By : 0;
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
    }
}
