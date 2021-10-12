using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.fmsdataservice.entity
{
    public class VehicleStatusRequest
    {
        private string _vin;
        public string VIN
        {
            get { return this._vin; }
            set { _vin = value?.Trim(); }
        }
        public string Since
        {
            get { return this._since; }
            set { _since = value?.Trim(); }
        }
        private string _since;
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
