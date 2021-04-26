﻿using System;
using System.Collections.Generic;
using System.Text;


namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleMileage
    {
        public long RequestTimestamp { get; set; }        
        public List<VehiclesCSV> VehiclesCSV { get; set; }
        public List<Vehicles> Vehicles { get; set; } 
    }
    public class Vehicles
    {
        public string EvtDateTime { get; set; }
        public string VIN { get; set; }
        public decimal TachoMileage { get; set; }
        public decimal GPSMileage { get; set; }
        public string RealMileageAlgorithmVersion { get; set; }
    }
    public class VehiclesCSV
    {
        public string EvtDateTime { get; set; }
        public string VIN { get; set; }
        public decimal TachoMileage { get; set; }
        public decimal RealMileage { get; set; }
        public string RealMileageAlgorithmVersion { get; set; }
    }
}
