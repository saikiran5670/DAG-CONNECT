using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.fmsdataservice.Entity
{
    public class VehicleStatusRequest
    {
        public string VIN { get; set; }
        public string Since { get; set; }
    }
    public class VehicleStatus
    {
        public string VIN { get; set; }
        public VehiclePosition VehiclePosition { get; set; }
        public int CatalystFuelLevel { get; set; }
        public string Driver1Id { get; set; }
        public string Driver1WorkingState { get; set; }
        public int EngineTotalFuelUsed { get; set; }
        public int EventTimestamp { get; set; }
        public int FuelLevel1 { get; set; }
        public int GrossCombinationVehicleWeight { get; set; }
        public int HRTotalVehicleDistance { get; set; }
        public double TachographSpeed { get; set; }
        public int TotalEngineHours { get; set; }
        public double WheelBasedSpeed { get; set; }
    }

    public class VehicleStatusResponse
    {
        public int RequestTimestamp { get; set; }
        public List<VehicleStatus> VehicleStatus { get; set; }
    }
}
