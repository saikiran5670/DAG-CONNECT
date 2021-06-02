using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class RouteCorridor
    {

        public int Id { get; set; }

        [Column("organization_id")]
        public int? OrganizationId { get; set; }
        [Column("type")]
        public char CorridorType { get; set; }
        [Column("name")]
        public string CorridorLabel { get; set; }
        [Column("address")]
        public string StartAddress { get; set; }
        public double StartLatitude { get; set; }
        public double StartLongitude { get; set; }
        public string EndAddress { get; set; }
        public double EndLatitude { get; set; }
        public double EndLongitude { get; set; }

        [Column("width")]
        public int Width { get; set; }
        public int Trailer { get; set; }
        public bool TransportData { get; set; }
        public bool TrafficFlow { get; set; }
        public bool Explosive { get; set; }
        public bool Gas { get; set; }
        public bool Flammable { get; set; }
        public bool Combustible { get; set; }
        public bool Organic { get; set; }
        public bool Poision { get; set; }
        public bool RadioActive { get; set; }
        public bool Corrosive { get; set; }
        public bool PoisonousInhalation { get; set; }
        public bool WaterHarm { get; set; }
        public bool Other { get; set; }
        public char TollRoad { get; set; }
        public char Mortorway { get; set; }
        public char BoatFerries { get; set; }
        public char RailFerries { get; set; }
        public char Tunnels { get; set; }
        public char DirtRoad { get; set; }
        public double VehicleSizeHeight { get; set; }
        public double VehicleSizeWidth { get; set; }
        public double VehicleSizeLength { get; set; }
        public double VehicleSizeLimitedWeight { get; set; }
        public double VehicleSizeWeightPerAxle { get; set; }
        [Column("state")]
        public string State { get; set; }
        public long Created_At { get; set; }
        public int Created_By { get; set; }
        public long Modified_At { get; set; }
        public int Modified_By { get; set; }
        public List<ViaRoute> ViaRoutDetails { get; set; }
        public int Distance { get; set; }
    }

    public class ViaRoute
    {
        public string ViaStopName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CorridorID
    {
        public int Id { get; set; }
    }
}
