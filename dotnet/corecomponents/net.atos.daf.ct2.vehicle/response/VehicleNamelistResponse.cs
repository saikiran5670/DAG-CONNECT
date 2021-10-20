using System.Collections.Generic;

namespace net.atos.daf.ct2.vehicle.response
{
    public class VehicleRelations
    {
        public string VIN { get; set; }
        public string Name { get; set; } = string.Empty;
        public string RegNo { get; set; } = string.Empty;
        public List<Relation> Relations { get; set; }
    }

    public class Relation
    {
        public string Type { get; set; }
        public string OrgId { get; set; }
    }

    public class VehicleRelationDto
    {
        public string VIN { get; set; }
        public string Type { get; set; }
        public string OrgId { get; set; }
    }

    public class VehicleNamelistResponse
    {
        public List<VehicleRelations> VehicleRelations { get; set; }
    }
}
