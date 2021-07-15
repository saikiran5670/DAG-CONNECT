using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.vehiclealertaggregator.entity
{
    //{"schema":[],"payload":{"data":"{ \"vid\":\"M4A1117\",\"alertid\":\"yes\",\"state\":\"A\" }","op":"I","namespace":"master.vehiclealertref","ts_ms":1625681684823}}
    // VehicleAlertRefKafkaMessage myDeserializedClass = JsonConvert.DeserializeObject<VehicleAlertRefKafkaMessage>(myJsonResponse); 
    class VehicleAlertRefKafkaMessage
    {
        public List<object> Schema { get; set; }
        public Payload Payload { get; set; }
    }
    public class Payload
    {
        public string Data { get; set; }
        public string Op { get; set; }
        public string Namespace { get; set; }
        public long Ts_ms { get; set; }
    }
}
