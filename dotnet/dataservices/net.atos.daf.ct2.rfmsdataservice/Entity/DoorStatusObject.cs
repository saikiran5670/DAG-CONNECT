using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.rfmsdataservice.Entity
{
    public class DoorStatusObject
    {
        [DataMember(Name = "doorEnabledStatus", EmitDefaultValue = false)]
        public string DoorEnabledStatus { get; set; }


        [DataMember(Name = "DoorOpenStatus", EmitDefaultValue = false)]
        public string DoorOpenStatus { get; set; }


        [DataMember(Name = "doorLockStatus", EmitDefaultValue = false)]
        public string DoorLockStatus { get; set; }

        [DataMember(Name = "doorNumber", EmitDefaultValue = false)]
        public string DoorNumber { get; set; }
    }
}

