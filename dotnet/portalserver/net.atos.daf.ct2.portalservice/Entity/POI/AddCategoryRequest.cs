using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.POI
{
    public class AddCategoryRequest
    {
        public int Id { get; set; }
        public int Organization_Id { get; set; }
        public string Name { get; set; }
        public string IconName { get; set; }
        public string Type { get; set; }
        public int Parent_Id { get; set; }
        public string State { get; set; }
        public string Description { get; set; }
        public long Created_At { get; set; }
        public int Created_By { get; set; }
        public long Modified_At { get; set; }
        public int Modified_By { get; set; }
        public byte[] icon { get; set; }
    }

    public class EditCategoryRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconName { get; set; }
        public string Description { get; set; }
        public int Modified_By { get; set; }
        public byte[] icon { get; set; }
        public int Organization_Id { get; set; }
    }
    public class DeleteCategoryRequest
    {
        public int Id { get; set; }
    }

    public class GetCategoryTypes
    {
        public string Type { get; set; }
    }
}
