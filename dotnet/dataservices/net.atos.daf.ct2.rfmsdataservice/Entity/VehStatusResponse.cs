using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.rfmsdataservice.Entity
{
    public class VehStatusResponse
    {
        [DataMember(Name = "vin", EmitDefaultValue = false)]
        public string Vin { get; set; }

        [DataMember(Name = "triggerType", EmitDefaultValue = false)]
        public TriggerObject TriggerType { get; set; }

        [DataMember(Name = "createdDateTime", EmitDefaultValue = false)]
        public string CreatedDateTime { get; set; }

        [DataMember(Name = "receivedDateTime", EmitDefaultValue = false)]
        public string ReceivedDateTime { get; set; }

        [DataMember(Name = "gnssPosition", EmitDefaultValue = false)]
        public GNSSPositionObject GnssPosition { get; set; }

        [DataMember(Name = "hrTotalVehicleDistance", EmitDefaultValue = false)]
        public int HrTotalVehicleDistance { get; set; }

        [DataMember(Name = "totalEngineHours", EmitDefaultValue = false)]
        public double TotalEngineHours { get; set; }


        [DataMember(Name = "totalFuelUsedGaseous", EmitDefaultValue = false)]
        public int TotalFuelUsedGaseous { get; set; }

        [DataMember(Name = "grossCombinationVehicleWeight", EmitDefaultValue = false)]
        public int GrossCombinationVehicleWeight { get; set; }

        [DataMember(Name = "status2OfDoors", EmitDefaultValue = false)]
        public string Status2OfDoors { get; set; }


        [DataMember(Name = "doorStatus", EmitDefaultValue = false)]
        public DoorStatusObject DoorStatus { get; set; }

        //doorStatus:list

    }
}