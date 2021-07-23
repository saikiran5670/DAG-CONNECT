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
@AllArgsConstructor
@NoArgsConstructor
public class Status implements Serializable {

  private static final long serialVersionUID = 1L;

  @JsonProperty(value = "receivedTimestamp")
  private Long receivedTimestamp;

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

  @JsonFormat(pattern = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")
  @JsonProperty(value = "EventDateTimeFirstIndex")
  private String eventDateTimeFirstIndex;

  @JsonProperty(value = "Jobname")
  private String jobName;
  @JsonProperty(value = "NumberOfIndexMessage")
  private Long numberOfIndexMessage;
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

  @JsonProperty(value = "DriverID")
  private String driverID;

  @JsonFormat(pattern = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")
  @JsonProperty(value = "GPSStartDateTime")
  private String gpsStartDateTime;

  @JsonFormat(pattern = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")
  @JsonProperty(value = "GPSEndDateTime")
  private String gpsEndDateTime;
  @JsonProperty(value = "GPSStartLatitude")
  private Double gpsStartLatitude;
  @JsonProperty(value = "GPSEndLatitude")
  private Double gpsEndLatitude;
  @JsonProperty(value = "GPSStartLongitude")
  private Double gpsStartLongitude;
  @JsonProperty(value = "GPSEndLongitude")
  private Double gpsEndLongitude;
  @JsonProperty(value = "GPSStartVehDist")
  private Long gpsStartVehDist;
  @JsonProperty(value = "GPSStopVehDist")
  private Long gpsStopVehDist;
  @JsonProperty(value = "VBrakeDuration")
  private Long vBrakeDuration;
  @JsonProperty(value = "VCruiseControlDist")
  private Long vCruiseControlDist;
  @JsonProperty(value = "VHarshBrakeDuration")
  private Long vHarshBrakeDuration;
  @JsonProperty(value = "VIdleDuration")
  private Long vIdleDuration;
  @JsonProperty(value = "VPosAltitudeVariation")
  private Long vPosAltitudeVariation;
  @JsonProperty(value = "VNegAltitudeVariation")
  private Long vNegAltitudeVariation;
  @JsonProperty(value = "VPTOCnt")
  private Long vptoCnt;
  @JsonProperty(value = "VPTODuration")
  private Long vptoDuration;
  @JsonProperty(value = "VPTODist")
  private Long vptoDist;
  @JsonProperty(value = "VStartFuel")
  private Long vStartFuel;
  @JsonProperty(value = "VStopFuel")
  private Long vStopFuel;
  @JsonProperty(value = "VStartTankLevel")
  private Integer vStartTankLevel;
  @JsonProperty(value = "VStopTankLevel")
  private Integer vStopTankLevel;
  @JsonProperty(value = "VUsedFuel")
  private Long vUsedFuel;

  @JsonProperty(value = "VIdleDurationDistr")
  private Distribution vIdleDurationDistr;

  @JsonProperty(value = "DocFormat")
  private String docFormat;
  @JsonProperty(value = "DocVersion")
  private String docVersion;
  @JsonProperty(value = "Document")
  private StatusDocument document;

  @Override
  public String toString() {

    String json = null;
    try {
      ObjectMapper mapper = new ObjectMapper();
      json = mapper.writeValueAsString(this);

    } catch (JsonProcessingException e) {
      e.printStackTrace();
    }
    return json;
  }
}
