using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [StringLength(100)]
        [RegularExpression(@"^[^!@#$%&*]+$")]
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
        public int Distance { get; set; }
        [Required]
        public List<ViaStopDetails> ViaAddressDetails { get; set; }
        public bool TransportData { get; set; }
        public bool TrafficFlow { get; set; }
        [StringLength(1)]
        public string State { get; set; }
        public long Created_At { get; set; }
        public int Created_By { get; set; }
        public long Modified_At { get; set; }
        public int Modified_By { get; set; }
        public Attribute Attribute { get; set; }
        public Exclusion Exclusion { get; set; }
        public VehicleSize VehicleSize { get; set; }

    }

    public class ViaStopDetails
    {
        [Required]
        [StringLength(100)]
        public string ViaRoutName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class VehicleSize
    {
        // [RegularExpression(@"\d{1,3}cm")]
        public double VehicleSizeHeight { get; set; }
        public double VehicleSizeWidth { get; set; }
        public double VehicleSizeLength { get; set; }
        public double VehicleSizeLimitedWeight { get; set; }
        public double VehicleSizeWeightPerAxle { get; set; }
    }

    public class Exclusion
    {
        // [StringLength(1, MinimumLength = 1)]
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
        [Column("NoTrailer")]
        public int IsTrailer { get; set; }
        public bool IsExplosive { get; set; }
        public bool IsGas { get; set; }
        public bool IsFlammable { get; set; }
        public bool IsCombustible { get; set; }
        public bool Isorganic { get; set; }
        public bool Ispoision { get; set; }
        public bool IsRadioActive { get; set; }
        public bool IsCorrosive { get; set; }
        public bool IsPoisonousInhalation { get; set; }
        public bool IsWaterHarm { get; set; }
        public bool IsOther { get; set; }


    }
    public class DeleteCorridorIdRequest
    {
        public int Id { get; set; }
    }

}
