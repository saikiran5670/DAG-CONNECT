using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace net.atos.daf.ct2.rfmsdataservice.Entity
{
    public class VehiclePositionResponseObject
    {
        [DataMember(Name = "vehiclePositionResponse")]
        public VehiclePositionResponse VehiclePositionResponse { get; set; }

        [DataMember(Name = "moreDataAvailable")]
        public bool? MoreDataAvailable { get; set; }

        [DataMember(Name = "moreDataAvailableLink")]
        public string MoreDataAvailableLink { get; set; }

        [DataMember(Name = "requestServerDateTime", EmitDefaultValue = false)]
        public string RequestServerDateTime { get; set; }
    }

    public class VehiclePositionResponse
    {
        [DataMember(Name = "vehiclePositions")]
        public List<VehiclePositions> VehiclePositions { get; set; }
    }

}
