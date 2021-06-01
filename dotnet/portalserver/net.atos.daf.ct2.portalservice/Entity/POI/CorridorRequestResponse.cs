
namespace net.atos.daf.ct2.portalservice.Entity.POI
{
    public class CorridorResponse
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string CorridoreName { get; set; }
        public string StartPoint { get; set; }
        public double StartLat { get; set; }
        public double StartLong { get; set; }
        public string EndPoint { get; set; }
        public double EndLat { get; set; }
        public double EndLong { get; set; }
        public double Distance { get; set; }
        public double Width { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public string Description { get; set; }
    }

    public class CorridorRequest
    {
        public int OrganizationId { get; set; }

        public int CorridorId { get; set; }
    }
}
