using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.kafkacdc.entity
{
    public class VehicleCdc
    {
        public string Vin { get; set; }
        public string Vid { get; set; }
        public string Status { get; set; }
        public string FuelType { get; set; }
        public double FuelTypeCoefficient { get; set; }
    }

    class VehicleMgmtKafkaMessage
    {
        public string Schema { get; set; }
        public VehicleMgmtPayload Payload { get; set; }
    }
    public class VehicleMgmtPayload
    {
        public VehicleCdc Data { get; set; }
        public string Operation { get; set; }
        public string Namespace { get; set; }
        public long Timestamp { get; set; }
    }

}
