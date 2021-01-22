package net.atos.daf.ct2.pojo.standard;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.io.*;
import java.sql.Timestamp;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class Index implements Serializable {

  private static final long serialVersionUID = 1L;

  @JsonProperty(value = "receivedTimestamp")
  private Long receivedTimestamp;

  @JsonFormat(pattern = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")
  @JsonProperty(value = "EvtDateTime")
  private Timestamp evtDateTime;
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

  @JsonProperty(value = "Jobname")
  private String jobName;
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

  @JsonFormat(pattern = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")
  @JsonProperty(value = "GPSDateTime")
  private Timestamp gpsDateTime;
  @JsonProperty(value = "GPSLatitude")
  private Double gpsLatitude;
  @JsonProperty(value = "GPSLongitude")
  private Double gpsLongitude;
  @JsonProperty(value = "GPSAltitude")
  private Integer gpsAltitude;
  @JsonProperty(value = "GPSHeading")
  private Double gpsHeading;

  @JsonProperty(value = "DriverID")
  private String driverID;

  @JsonProperty(value = "VCumulatedFuel")
  private Integer vCumulatedFuel;
  @JsonProperty(value = "VDist")
  private Integer vDist;

  @JsonProperty(value = "VUsedFuel")
  private Integer vUsedFuel;
  @JsonProperty(value = "VIdleDuration")
  private Integer vIdleDuration;

  @JsonProperty(value = "DocFormat")
  private String docFormat;
  @JsonProperty(value = "DocVersion")
  private String docVersion;
  @JsonProperty(value = "Document")
  private IndexDocument document;

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
