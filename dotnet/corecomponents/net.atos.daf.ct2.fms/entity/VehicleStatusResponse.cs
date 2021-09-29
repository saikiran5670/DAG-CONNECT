﻿using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.fms.entity
{
    public class VehicleStatusResponse
    {
        public long RequestTimestamp { get; set; }
        public List<VehicleStatus> VehicleStatus { get; set; }
    }
    public class VehicleStatus
    {
        public string VIN { get; set; }
        public List<VehiclePositionForStatus> VehiclePosition { get; set; }
        public int CatalystFuelLevel { get; set; }
        public string Driver1Id { get; set; }
        public string Driver1WorkingState { get; set; }
        public long EngineTotalFuelUsed { get; set; }
        public long EventTimestamp { get; set; }
        public int FuelLevel1 { get; set; }
        public long GrossCombinationVehicleWeight { get; set; }
        public long HRTotalVehicleDistance { get; set; }
        public double TachographSpeed { get; set; }
        public long TotalEngineHours { get; set; }
        public double WheelBasedSpeed { get; set; }
    }
    public class VehiclePositionForStatus
    {
        public int Altitude { get; set; }
        public double Heading { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long GPSTimestamp { get; set; }
        public decimal Speed { get; set; }
    }
}


