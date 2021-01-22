package net.atos.daf.ct2.pojo.standard;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.io.Serializable;
import java.util.Date;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class Status implements Serializable {

  @JsonProperty(value = "receivedTimestamp")
  private Long receivedTimestamp;

  @JsonFormat(pattern = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")
  @JsonProperty(value = "EvtDateTime")
  private Date evtDateTime;
  @JsonProperty(value = "Increment")
  private Integer increment;
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

  @JsonFormat(pattern = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")
  @JsonProperty(value = "EventDateTimeFirstIndex")
  private Date eventDateTimeFirstIndex;

  @JsonProperty(value = "Jobname")
  private String jobName;
  @JsonProperty(value = "NumberOfIndexMessage")
  private Integer numberOfIndexMessage;
  @JsonProperty(value = "NumSeq")
  private Integer numSeq;
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
  private Date gpsStartDateTime;

  @JsonFormat(pattern = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")
  @JsonProperty(value = "GPSEndDateTime")
  private Date gpsEndDateTime;
  @JsonProperty(value = "GPSStartLatitude")
  private Double gpsStartLatitude;
  @JsonProperty(value = "GPSEndLatitude")
  private Double gpsEndLatitude;
  @JsonProperty(value = "GPSStartLongitude")
  private Double gpsStartLongitude;
  @JsonProperty(value = "GPSEndLongitude")
  private Double gpsEndLongitude;
  @JsonProperty(value = "GPSStartVehDist")
  private Integer gpsStartVehDist;
  @JsonProperty(value = "GPSStopVehDist")
  private Integer gpsStopVehDist;
  @JsonProperty(value = "VBrakeDuration")
  private Integer vBrakeDuration;
  @JsonProperty(value = "VCruiseControlDist")
  private Integer vCruiseControlDist;
  @JsonProperty(value = "VHarshBrakeDuration")
  private Integer vHarshBrakeDuration;
  @JsonProperty(value = "VIdleDuration")
  private Integer vIdleDuration;
  @JsonProperty(value = "VPosAltitudeVariation")
  private Integer vPosAltitudeVariation;
  @JsonProperty(value = "VNegAltitudeVariation")
  private Integer vNegAltitudeVariation;
  @JsonProperty(value = "VPTOCnt")
  private Integer vptoCnt;
  @JsonProperty(value = "VPTODuration")
  private Integer vptoDuration;
  @JsonProperty(value = "VPTODist")
  private Integer vptoDist;
  @JsonProperty(value = "VStartFuel")
  private Integer vStartFuel;
  @JsonProperty(value = "VStopFuel")
  private Integer vStopFuel;
  @JsonProperty(value = "VStartTankLevel")
  private Integer vStartTankLevel;
  @JsonProperty(value = "VStopTankLevel")
  private Integer vStopTankLevel;
  @JsonProperty(value = "VUsedFuel")
  private Integer vUsedFuel;

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
