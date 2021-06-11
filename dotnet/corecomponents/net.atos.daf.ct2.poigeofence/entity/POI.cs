using System.Collections.Generic;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class POI
    {
        public int Id { get; set; }

        public int? OrganizationId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; }
        public string State { get; set; }
        //  public int TripId { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public byte[] Icon { get; set; }
    }

    public class TripEntityRequest
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public string VIN { get; set; }
    }

    public class TripEntityResponce
    {
        public int Id { get; set; }
        public string TripId { get; set; }
        public string VIN { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverLastName { get; set; }
        public string DriverId1 { get; set; }
        public string DriverId2 { get; set; }
        public int Distance { get; set; }
        public string StartAddress { get; set; }
        public string EndAddress { get; set; }
        public double StartPositionlattitude { get; set; }
        public double StartPositionLongitude { get; set; }
        public double EndPositionLattitude { get; set; }
        public double EndPositionLongitude { get; set; }
        public long StartTimeStamp { get; set; }
        public long EndTimeStamp { get; set; }
        public List<LiveFleetPosition> LiveFleetPosition { get; set; }
    }
    public class LiveFleetPosition
    {
        public double GpsAltitude { get; set; }
        public double GpsHeading { get; set; }
        public double GpsLatitude { get; set; }
        public double GpsLongitude { get; set; }
        public int Id { get; set; }
    }
}
