using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.kafkacdc.entity
{
    //{"schema":[],"payload":{"data":"{ \"vin\":\"M4A1117\",\"alertid\":\"yes\",\"state\":\"A\" }","op":"I","namespace":"master.vehiclealertref","ts_ms":1625681684823}}
    //{
    //"schema": "master.vehiclealertref",
    //"payload": "{ \"alertId\": \"112\", \"vinOps\": [{ \"vins\": \"MHFI-544\", \"op\": \"I\" }, { \"vins\": \"MHFI-546\", \"op\": \"D\" } ] }",
    //"operation": "I",
    //"namespace": "alerts",
    //"timeStamp": 1627539915
    //}
    // VehicleAlertRefKafkaMessage myDeserializedClass = JsonConvert.DeserializeObject<VehicleAlertRefKafkaMessage>(myJsonResponse); 
    public class AlertCdcKafkaJsonMessage
    {
        public string Schema { get; set; }
        public Payload Payload { get; set; }
    }
    public class Payload
    {
        public string Data { get; set; }
        public string Operation { get; set; }
        public string Namespace { get; set; }
        public long Ts_ms { get; set; }
    }
    public class VehicleAlertRefMsgFormat
    {
        public int AlertId { get; set; }
        public List<VehicleStateMsgFormat> VinOps { get; set; }
    }
    public class VehicleStateMsgFormat
    {
        public string VIN { get; set; }
        public string Op { get; set; }
    }

}
