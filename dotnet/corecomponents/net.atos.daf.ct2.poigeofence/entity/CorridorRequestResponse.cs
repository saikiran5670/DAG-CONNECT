using System.Collections.Generic;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class CorridorLookUp
    {
        public List<CorridorResponse> GridView { get; set; }
        public CorridorEditViewResponse EditView { get; set; }
    }
    public class CorridorResponse
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string CorridorType { get; set; }
        public string CorridoreName { get; set; }
        public string StartPoint { get; set; }
        public double StartLat { get; set; }
        public double StartLong { get; set; }
        public string EndPoint { get; set; }
        public double EndLat { get; set; }
        public double EndLong { get; set; }
        public double Distance { get; set; }
        public double Width { get; set; }
        public string State { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public string Description { get; set; }
        public List<ViaAddressDetail> ViaAddressDetails { get; set; }
        public List<ExistingTrip> CorridoreTrips { get; set; }

    }
    public class CorridorRequest
    {
        public int OrganizationId { get; set; }

        public int CorridorId { get; set; }
    }
    public class ViaAddressDetail
    {
        public int CorridorViaStopId { get; set; }
        public string CorridorViaStopName { get; set; }
        public int Latitude { get; set; }
        public int Longitude { get; set; }
    }

    public class CorridorEditViewResponse : CorridorResponse
    {
        public int CorridorPropertiesId { get; set; }
        public bool IsTransportData { get; set; }
        public bool IsTrafficFlow { get; set; }
        public long CreatedAtForCP { get; set; }
        public long ModifiedAtForCP { get; set; }
        //public CorridorAdvanceOptions corridorAdvanceOptions { get; set; }
        public int NoOfTrailers { get; set; }
        public bool IsExplosive { get; set; }
        public bool IsGas { get; set; }
        public bool IsFlammable { get; set; }
        public bool IsCombustible { get; set; }
        public bool IsOrganic { get; set; }
        public bool IsPoision { get; set; }
        public bool IsRadioActive { get; set; }
        public bool IsCorrosive { get; set; }
        public bool IsPoisonousInhalation { get; set; }
        public bool IsWaterHarm { get; set; }
        public bool IsOther { get; set; }

        public string TollRoadType { get; set; }
        public string Mortorway { get; set; }
        public string BoatFerriesType { get; set; }
        public string RailFerriesType { get; set; }
        public string TunnelsType { get; set; }
        public string DirtRoadType { get; set; }

        public double VehicleHeight { get; set; }
        public double VehicleWidth { get; set; }
        public double VehicleLength { get; set; }
        public double VehicleLimitedWeight { get; set; }
        public double VehicleWeightPerAxle { get; set; }
    }



    //public class Attribute
    //{
    //    public int NoOfTrailers { get; set; }
    //    public bool IsExplosive { get; set; }
    //    public bool IsGas { get; set; }
    //    public bool IsFlammable { get; set; }
    //    public bool IsCombustible { get; set; }
    //    public bool IsOrganic { get; set; }
    //    public bool IsPoision { get; set; }
    //    public bool IsRadioActive { get; set; }
    //    public bool IsCorrosive { get; set; }
    //    public bool IsPoisonousInhalation { get; set; }
    //    public bool IsWaterHarm { get; set; }
    //    public bool IsOther { get; set; }
    //}

    //public class Exclusion
    //{
    //    public string TollRoadType { get; set; }
    //    public string mortorway { get; set; }
    //    public string BoatFerriesType { get; set; }
    //    public string RailFerriesType { get; set; }
    //    public string TunnelsType { get; set; }
    //    public string DirtRoadType { get; set; }
    //}

    //public class VehicleSize
    //{
    //    public int VehicleHeight { get; set; }
    //    public int vehicleWidth { get; set; }
    //    public int vehicleLength { get; set; }
    //    public int vehicleLimitedWeight { get; set; }
    //    public int vehicleWeightPerAxle { get; set; }
    //}

    //public class CorridorEditViewResponse : CorridorResponse
    //{
    //    public CorridorAdvanceOptions corridorAdvanceOptions { get; set; }
    //}

    //public class CorridorAdvanceOptions
    //{
    //    public int CorridorpropertiesId { get; set; }
    //    public bool IsTransportData { get; set; }
    //    public bool IsTrafficFlow { get; set; }
    //    public Attribute Attribute { get; set; }
    //    public Exclusion Exclusion { get; set; }
    //    public VehicleSize VehicleSize { get; set; }
    //    public long CreatedAt { get; set; }
    //    public long ModifiedAt { get; set; }
    //}
}
