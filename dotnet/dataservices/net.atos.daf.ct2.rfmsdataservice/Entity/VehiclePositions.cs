using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.rfmsdataservice.Entity
{
    public class VehiclePositions
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

        [DataMember(Name = "wheelBasedSpeed", EmitDefaultValue = false)]
        public double? WheelBasedSpeed { get; set; }

        [DataMember(Name = "tachographSpeed", EmitDefaultValue = false)]
        public double? TachographSpeed { get; set; }
    }
}
