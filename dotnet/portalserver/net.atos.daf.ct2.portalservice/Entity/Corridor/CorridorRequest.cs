using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Corridor
{
    public class CorridorRequest
    {
        public int Id { get; set; }
        [Required]
        public int OrganizationId { get; set; }
        [Required]
        [StringLength(1)]
        public string CorridorType { get; set; }
        [Required]
        [StringLength(50)]
        public string CorridorLabel { get; set; }
        [Required]
        [StringLength(100)]
        public string StartAddress { get; set; }
        [Required]
        public double StartLatitude { get; set; }
        [Required]
        public double StartLongitude { get; set; }
        [Required]
        public string EndAddress { get; set; }
        [Required]
        public double EndLatitude { get; set; }
        [Required]
        public double EndLongitude { get; set; }
        public int Width { get; set; }
        public List<ViaDetails> ViaAddressDetails { get; set; }
        public bool TransportData { get; set; }
        public bool TrafficFlow { get; set; }
        [StringLength(1)]
        public string State { get; set; }
        public long Created_At { get; set; }
        public int Created_By { get; set; }
        public long Modified_At { get; set; }
        public int Modified_By { get; set; }
        public Attribute attribute { get; set; }
        public Exclusion exclusion { get; set; }
        public VehicleSize vehicleSize { get; set; }
    }

    public class ViaDetails
    {
        [StringLength(100)]
        public string ViaRoutName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class VehicleSize
    {
        public int VehicleSizeHeight { get; set; }
        public int VehicleSizeWidth { get; set; }
        public int VehicleSizeLength { get; set; }
        public int VehicleSizeLimitedWeight { get; set; }
        public int VehicleSizeWeightPerAxle { get; set; }
    }

    public class Exclusion
    {
        [StringLength(1)]
        public string TollRoad { get; set; }
        [StringLength(1)]
        public string Mortorway { get; set; }
        [StringLength(1)]
        public string BoatFerries { get; set; }
        [StringLength(1)]
        public string RailFerries { get; set; }
        [StringLength(1)]
        public string Tunnels { get; set; }
        [StringLength(1)]
        public string DirtRoad { get; set; }
    }

    public class Attribute
    {
        public int Trailer { get; set; }
        public bool Explosive { get; set; }
        public bool Gas { get; set; }
        public bool Flammable { get; set; }
        public bool Combustible { get; set; }
        public bool organic { get; set; }
        public bool poision { get; set; }
        public bool RadioActive { get; set; }
        public bool Corrosive { get; set; }
        public bool PoisonousInhalation { get; set; }
        public bool WaterHarm { get; set; }
        public bool Other { get; set; }


    }
}
