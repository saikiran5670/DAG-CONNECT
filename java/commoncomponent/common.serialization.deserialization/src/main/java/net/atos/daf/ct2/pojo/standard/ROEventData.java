package net.atos.daf.ct2.pojo.standard;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.io.Serializable;
import java.util.Date;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class ROEventData  implements Serializable {

    private static final long serialVersionUID = 1L;

    @JsonProperty(value = "Network_Cell")
    private String networkCell;
    @JsonProperty(value = "Network_RSSI")
    private String networkRSSI;
    @JsonProperty(value = "Network_Type")
    private String networkType;
    @JsonProperty(value = "Order_ID")
    private String orderID;
    @JsonProperty(value = "Pairing_Result")
    private String pairingResult;
    @JsonProperty(value = "Time_JumpStr")
    private String timeJumpStr;
    @JsonProperty(value = "VIN")
    private String vin;
    @JsonProperty(value = "Message_Type")
    private Integer messageType;
    @JsonProperty(value = "Message_VEvtID")
    private Integer messageVEvtID;
    @JsonProperty(value = "Message_Timestamp")
    private Date messageTimestamp;
    @JsonProperty(value = "Message_NumSeq")
    private Long messageNumSeq;
    @JsonProperty(value = "Message_Name")
    private String messageName;

}
