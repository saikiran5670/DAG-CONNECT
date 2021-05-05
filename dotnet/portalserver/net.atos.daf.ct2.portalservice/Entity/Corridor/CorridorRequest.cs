using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Corridor
{
    public class CorridorRequest
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public char CorridorType { get; set; }
        public string CorridorLabel { get; set; }
        public string StartAddress { get; set; }
        public string EndAddress { get; set; }
        public int Width { get; set; }
        public char Trailer { get; set; }
        public bool TransportData { get; set; }
        public bool TrafficFlow { get; set; }
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
        public char TollRoad { get; set; }
        public char Mortorway { get; set; }
        public char BoatFerries { get; set; }
        public char RailFerries { get; set; }
        public char Tunnels { get; set; }
        public char DirtRoad { get; set; }
        public int VehicleSizeHeight { get; set; }
        public int VehicleSizeWidth { get; set; }
        public int VehicleSizeLength { get; set; }
        public int VehicleSizeLimitedWeight { get; set; }
        public int VehicleSizeWeightPerAxle { get; set; }
        public string State { get; set; }
        public long Created_At { get; set; }
        public int Created_By { get; set; }
        public long Modified_At { get; set; }
        public int Modified_By { get; set; }
    }
}
