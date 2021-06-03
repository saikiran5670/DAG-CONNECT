using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.POI
{
    public class LandmarkGroup
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int OrganizationId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int IconId { get; set; }
        public string State { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        [Required]
        public List<GroupPois> Poilist { get; set; }
    }

    public class GroupPois
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public string Type { get; set; }
    }

    public enum LandmarkType
    {
        None = 'N',
        POI = 'P',
        CircularGeofence = 'C',
        PolygonGeofence = 'O',
        ExistingTripCorridor = 'E',
        RouteCorridor = 'R'


    }
}
