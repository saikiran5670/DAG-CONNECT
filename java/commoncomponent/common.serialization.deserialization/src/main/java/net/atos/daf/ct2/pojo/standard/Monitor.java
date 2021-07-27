package net.atos.daf.ct2.pojo.standard;

import java.io.Serializable;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class Monitor implements Serializable {

    private static final long serialVersionUID = 1L;

    @JsonProperty(value = "receivedTimestamp")
    private Long receivedTimestamp;
    @JsonProperty(value = "storedTimestamp")
    private Long storedTimestamp;

    @JsonFormat(pattern = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")
    @JsonProperty(value = "EvtDateTime")
    private String evtDateTime;
    @JsonProperty(value = "Increment")
    private Long increment;
    @JsonProperty(value = "ROProfil")
    private String roProfil;
    @JsonProperty(value = "TenantID")
    private String tenantID;
    @JsonProperty(value = "TransID")
    private String transID;
    @JsonProperty(value = "VID")
    private String vid;
    @JsonProperty(value = "VIN")
    private String vin;
    @JsonProperty(value = "kafkaProcessingTS")
    private String kafkaProcessingTS;

    @JsonProperty(value = "Jobname")
    private String jobName;
    @JsonProperty(value = "MessageType")
    private Integer messageType;
    @JsonProperty(value = "NumSeq")
    private Long numSeq;
    @JsonProperty(value = "VEvtID")
    private Integer vEvtID;

    @JsonProperty(value = "ROmodel")
    private String roModel;
    @JsonProperty(value = "ROName")
    private String roName;
    @JsonProperty(value = "ROrelease")
    private String roRelease;

    @JsonFormat(pattern = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")
    @JsonProperty(value = "GPSDateTime")
    private String gpsDateTime;
    @JsonProperty(value = "GPSLatitude")
    private Double gpsLatitude;
    @JsonProperty(value = "GPSLongitude")
    private Double gpsLongitude;
    @JsonProperty(value = "GPSAltitude")
    private Long gpsAltitude;
    @JsonProperty(value = "GPSHeading")
    private Double gpsHeading;

    @JsonProperty(value = "DocFormat")
    private String docFormat;
    @JsonProperty(value = "DocVersion")
    private String docVersion;
    @JsonProperty(value = "Document")
    private MonitorDocument document;

    @Override
    public String toString() {

        String json = null;
        try {
            ObjectMapper mapper = new ObjectMapper();
            json = mapper.writeValueAsString(this);

        } catch (JsonProcessingException e) {
            System.out.println("Unable to Parse into JSON: "+e.getMessage());
        }
        return json;
    }
}
