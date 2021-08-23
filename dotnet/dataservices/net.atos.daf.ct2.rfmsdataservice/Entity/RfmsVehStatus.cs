using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.rfmsdataservice.Entity
{
    public class RfmsVehStatusResponse
    {


        [DataMember(Name = "vehiclePositionResponse")]
        public RfmsVehStatus RfmsVehStatus { get; set; }

        [DataMember(Name = "moreDataAvailable")]
        public bool? MoreDataAvailable { get; set; }

        [DataMember(Name = "moreDataAvailableLink")]
        public string MoreDataAvailableLink { get; set; }

        [DataMember(Name = "requestServerDateTime", EmitDefaultValue = false)]
        public string RequestServerDateTime { get; set; }



    }

    public class RfmsVehStatus
    {
        [DataMember(Name = "vehiclePositions")]
        public List<VehStatusResponse> VehicleStatuses { get; set; }
    }
}
