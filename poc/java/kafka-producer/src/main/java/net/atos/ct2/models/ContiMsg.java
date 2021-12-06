
package net.atos.ct2.models;

import java.util.HashMap;
import java.util.Map;
import javax.annotation.Generated;
import com.fasterxml.jackson.annotation.JsonAnyGetter;
import com.fasterxml.jackson.annotation.JsonAnySetter;
import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonPropertyOrder;

@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonPropertyOrder({
    "GPSStartLatitude",
    "VStartFuel",
    "Increment",
    "GPSStartLongitude",
    "DocFormat",
    "EventDateTimeFirstIndex",
    "TransID",
    "GPSEndLongitude",
    "TenantID",
    "VStartTankLevel",
    "DocVersion",
    "Reserve0",
    "GPSStopVehDist",
    "ROProfil",
    "ROrelease",
    "VStopTankLevel",
    "VNegAltitudeVariation",
    "Jobname",
    "GPSStartVehDist",
    "VPosAltitudeVariation",
    "GPSEndDateTime",
    "VIdleDurationDistr",
    "NumSeq",
    "VUsedFuel",
    "VCruiseControlDist",
    "ROmodel",
    "VHarshBrakeDuration",
    "VPTOCnt",
    "TargetM2M",
    "VStopFuel",
    "Reserve1",
    "PartitionID",
    "DriverID",
    "VBrakeDuration",
    "ROName",
    "VEvtID",
    "GPSEndLatitude",
    "GPSStartDateTime",
    "NumberOfIndexMessage",
    "VID",
    "VIdleDuration",
    "VPTODuration",
    "VPTODist",
    "Timestamps",
    "EvtDateTime",
    "Document",
    "messageKey",
    "receivedTimestamp",
    "hostname",
    "partitionId",
    "queue"
})
@Generated("jsonschema2pojo")
public class ContiMsg {

    @JsonProperty("GPSStartLatitude")
    private String gPSStartLatitude;
    @JsonProperty("VStartFuel")
    private String vStartFuel;
    @JsonProperty("Increment")
    private String increment;
    @JsonProperty("GPSStartLongitude")
    private String gPSStartLongitude;
    @JsonProperty("DocFormat")
    private String docFormat;
    @JsonProperty("EventDateTimeFirstIndex")
    private String eventDateTimeFirstIndex;
    @JsonProperty("TransID")
    private String transID;
    @JsonProperty("GPSEndLongitude")
    private String gPSEndLongitude;
    @JsonProperty("TenantID")
    private String tenantID;
    @JsonProperty("VStartTankLevel")
    private String vStartTankLevel;
    @JsonProperty("DocVersion")
    private String docVersion;
    @JsonProperty("Reserve0")
    private String reserve0;
    @JsonProperty("GPSStopVehDist")
    private String gPSStopVehDist;
    @JsonProperty("ROProfil")
    private String rOProfil;
    @JsonProperty("ROrelease")
    private String rOrelease;
    @JsonProperty("VStopTankLevel")
    private String vStopTankLevel;
    @JsonProperty("VNegAltitudeVariation")
    private String vNegAltitudeVariation;
    @JsonProperty("Jobname")
    private String jobname;
    @JsonProperty("GPSStartVehDist")
    private String gPSStartVehDist;
    @JsonProperty("VPosAltitudeVariation")
    private String vPosAltitudeVariation;
    @JsonProperty("GPSEndDateTime")
    private String gPSEndDateTime;
    @JsonProperty("VIdleDurationDistr")
    private VIdleDurationDistr vIdleDurationDistr;
    @JsonProperty("NumSeq")
    private String numSeq;
    @JsonProperty("VUsedFuel")
    private String vUsedFuel;
    @JsonProperty("VCruiseControlDist")
    private String vCruiseControlDist;
    @JsonProperty("ROmodel")
    private String rOmodel;
    @JsonProperty("VHarshBrakeDuration")
    private String vHarshBrakeDuration;
    @JsonProperty("VPTOCnt")
    private String vPTOCnt;
    @JsonProperty("TargetM2M")
    private String targetM2M;
    @JsonProperty("VStopFuel")
    private String vStopFuel;
    @JsonProperty("Reserve1")
    private String reserve1;
    @JsonProperty("PartitionID")
    private String partitionID;
    @JsonProperty("DriverID")
    private String driverID;
    @JsonProperty("VBrakeDuration")
    private String vBrakeDuration;
    @JsonProperty("ROName")
    private String rOName;
    @JsonProperty("VEvtID")
    private String vEvtID;
    @JsonProperty("GPSEndLatitude")
    private String gPSEndLatitude;
    @JsonProperty("GPSStartDateTime")
    private String gPSStartDateTime;
    @JsonProperty("NumberOfIndexMessage")
    private String numberOfIndexMessage;
    @JsonProperty("VID")
    private String vid;
    @JsonProperty("VIdleDuration")
    private String vIdleDuration;
    @JsonProperty("VPTODuration")
    private String vPTODuration;
    @JsonProperty("VPTODist")
    private String vPTODist;
    @JsonProperty("Timestamps")
    private Timestamps timestamps;
    @JsonProperty("EvtDateTime")
    private String evtDateTime;
    @JsonProperty("Document")
    private Document document;
    @JsonProperty("messageKey")
    private String messageKey;
    @JsonProperty("receivedTimestamp")
    private Long receivedTimestamp;
    @JsonProperty("hostname")
    private String hostname;
    @JsonProperty("partitionId")
    private String partitionId;
    @JsonProperty("queue")
    private String queue;
    @JsonIgnore
    private Map<String, Object> additionalProperties = new HashMap<String, Object>();

    @JsonProperty("GPSStartLatitude")
    public String getGPSStartLatitude() {
        return gPSStartLatitude;
    }

    @JsonProperty("GPSStartLatitude")
    public void setGPSStartLatitude(String gPSStartLatitude) {
        this.gPSStartLatitude = gPSStartLatitude;
    }

    @JsonProperty("VStartFuel")
    public String getVStartFuel() {
        return vStartFuel;
    }

    @JsonProperty("VStartFuel")
    public void setVStartFuel(String vStartFuel) {
        this.vStartFuel = vStartFuel;
    }

    @JsonProperty("Increment")
    public String getIncrement() {
        return increment;
    }

    @JsonProperty("Increment")
    public void setIncrement(String increment) {
        this.increment = increment;
    }

    @JsonProperty("GPSStartLongitude")
    public String getGPSStartLongitude() {
        return gPSStartLongitude;
    }

    @JsonProperty("GPSStartLongitude")
    public void setGPSStartLongitude(String gPSStartLongitude) {
        this.gPSStartLongitude = gPSStartLongitude;
    }

    @JsonProperty("DocFormat")
    public String getDocFormat() {
        return docFormat;
    }

    @JsonProperty("DocFormat")
    public void setDocFormat(String docFormat) {
        this.docFormat = docFormat;
    }

    @JsonProperty("EventDateTimeFirstIndex")
    public String getEventDateTimeFirstIndex() {
        return eventDateTimeFirstIndex;
    }

    @JsonProperty("EventDateTimeFirstIndex")
    public void setEventDateTimeFirstIndex(String eventDateTimeFirstIndex) {
        this.eventDateTimeFirstIndex = eventDateTimeFirstIndex;
    }

    @JsonProperty("TransID")
    public String getTransID() {
        return transID;
    }

    @JsonProperty("TransID")
    public void setTransID(String transID) {
        this.transID = transID;
    }

    @JsonProperty("GPSEndLongitude")
    public String getGPSEndLongitude() {
        return gPSEndLongitude;
    }

    @JsonProperty("GPSEndLongitude")
    public void setGPSEndLongitude(String gPSEndLongitude) {
        this.gPSEndLongitude = gPSEndLongitude;
    }

    @JsonProperty("TenantID")
    public String getTenantID() {
        return tenantID;
    }

    @JsonProperty("TenantID")
    public void setTenantID(String tenantID) {
        this.tenantID = tenantID;
    }

    @JsonProperty("VStartTankLevel")
    public String getVStartTankLevel() {
        return vStartTankLevel;
    }

    @JsonProperty("VStartTankLevel")
    public void setVStartTankLevel(String vStartTankLevel) {
        this.vStartTankLevel = vStartTankLevel;
    }

    @JsonProperty("DocVersion")
    public String getDocVersion() {
        return docVersion;
    }

    @JsonProperty("DocVersion")
    public void setDocVersion(String docVersion) {
        this.docVersion = docVersion;
    }

    @JsonProperty("Reserve0")
    public String getReserve0() {
        return reserve0;
    }

    @JsonProperty("Reserve0")
    public void setReserve0(String reserve0) {
        this.reserve0 = reserve0;
    }

    @JsonProperty("GPSStopVehDist")
    public String getGPSStopVehDist() {
        return gPSStopVehDist;
    }

    @JsonProperty("GPSStopVehDist")
    public void setGPSStopVehDist(String gPSStopVehDist) {
        this.gPSStopVehDist = gPSStopVehDist;
    }

    @JsonProperty("ROProfil")
    public String getROProfil() {
        return rOProfil;
    }

    @JsonProperty("ROProfil")
    public void setROProfil(String rOProfil) {
        this.rOProfil = rOProfil;
    }

    @JsonProperty("ROrelease")
    public String getROrelease() {
        return rOrelease;
    }

    @JsonProperty("ROrelease")
    public void setROrelease(String rOrelease) {
        this.rOrelease = rOrelease;
    }

    @JsonProperty("VStopTankLevel")
    public String getVStopTankLevel() {
        return vStopTankLevel;
    }

    @JsonProperty("VStopTankLevel")
    public void setVStopTankLevel(String vStopTankLevel) {
        this.vStopTankLevel = vStopTankLevel;
    }

    @JsonProperty("VNegAltitudeVariation")
    public String getVNegAltitudeVariation() {
        return vNegAltitudeVariation;
    }

    @JsonProperty("VNegAltitudeVariation")
    public void setVNegAltitudeVariation(String vNegAltitudeVariation) {
        this.vNegAltitudeVariation = vNegAltitudeVariation;
    }

    @JsonProperty("Jobname")
    public String getJobname() {
        return jobname;
    }

    @JsonProperty("Jobname")
    public void setJobname(String jobname) {
        this.jobname = jobname;
    }

    @JsonProperty("GPSStartVehDist")
    public String getGPSStartVehDist() {
        return gPSStartVehDist;
    }

    @JsonProperty("GPSStartVehDist")
    public void setGPSStartVehDist(String gPSStartVehDist) {
        this.gPSStartVehDist = gPSStartVehDist;
    }

    @JsonProperty("VPosAltitudeVariation")
    public String getVPosAltitudeVariation() {
        return vPosAltitudeVariation;
    }

    @JsonProperty("VPosAltitudeVariation")
    public void setVPosAltitudeVariation(String vPosAltitudeVariation) {
        this.vPosAltitudeVariation = vPosAltitudeVariation;
    }

    @JsonProperty("GPSEndDateTime")
    public String getGPSEndDateTime() {
        return gPSEndDateTime;
    }

    @JsonProperty("GPSEndDateTime")
    public void setGPSEndDateTime(String gPSEndDateTime) {
        this.gPSEndDateTime = gPSEndDateTime;
    }

    @JsonProperty("VIdleDurationDistr")
    public VIdleDurationDistr getVIdleDurationDistr() {
        return vIdleDurationDistr;
    }

    @JsonProperty("VIdleDurationDistr")
    public void setVIdleDurationDistr(VIdleDurationDistr vIdleDurationDistr) {
        this.vIdleDurationDistr = vIdleDurationDistr;
    }

    @JsonProperty("NumSeq")
    public String getNumSeq() {
        return numSeq;
    }

    @JsonProperty("NumSeq")
    public void setNumSeq(String numSeq) {
        this.numSeq = numSeq;
    }

    @JsonProperty("VUsedFuel")
    public String getVUsedFuel() {
        return vUsedFuel;
    }

    @JsonProperty("VUsedFuel")
    public void setVUsedFuel(String vUsedFuel) {
        this.vUsedFuel = vUsedFuel;
    }

    @JsonProperty("VCruiseControlDist")
    public String getVCruiseControlDist() {
        return vCruiseControlDist;
    }

    @JsonProperty("VCruiseControlDist")
    public void setVCruiseControlDist(String vCruiseControlDist) {
        this.vCruiseControlDist = vCruiseControlDist;
    }

    @JsonProperty("ROmodel")
    public String getROmodel() {
        return rOmodel;
    }

    @JsonProperty("ROmodel")
    public void setROmodel(String rOmodel) {
        this.rOmodel = rOmodel;
    }

    @JsonProperty("VHarshBrakeDuration")
    public String getVHarshBrakeDuration() {
        return vHarshBrakeDuration;
    }

    @JsonProperty("VHarshBrakeDuration")
    public void setVHarshBrakeDuration(String vHarshBrakeDuration) {
        this.vHarshBrakeDuration = vHarshBrakeDuration;
    }

    @JsonProperty("VPTOCnt")
    public String getVPTOCnt() {
        return vPTOCnt;
    }

    @JsonProperty("VPTOCnt")
    public void setVPTOCnt(String vPTOCnt) {
        this.vPTOCnt = vPTOCnt;
    }

    @JsonProperty("TargetM2M")
    public String getTargetM2M() {
        return targetM2M;
    }

    @JsonProperty("TargetM2M")
    public void setTargetM2M(String targetM2M) {
        this.targetM2M = targetM2M;
    }

    @JsonProperty("VStopFuel")
    public String getVStopFuel() {
        return vStopFuel;
    }

    @JsonProperty("VStopFuel")
    public void setVStopFuel(String vStopFuel) {
        this.vStopFuel = vStopFuel;
    }

    @JsonProperty("Reserve1")
    public String getReserve1() {
        return reserve1;
    }

    @JsonProperty("Reserve1")
    public void setReserve1(String reserve1) {
        this.reserve1 = reserve1;
    }

    @JsonProperty("PartitionID")
    public String getPartitionID() {
        return partitionID;
    }

    @JsonProperty("PartitionID")
    public void setPartitionID(String partitionID) {
        this.partitionID = partitionID;
    }

    @JsonProperty("DriverID")
    public String getDriverID() {
        return driverID;
    }

    @JsonProperty("DriverID")
    public void setDriverID(String driverID) {
        this.driverID = driverID;
    }

    @JsonProperty("VBrakeDuration")
    public String getVBrakeDuration() {
        return vBrakeDuration;
    }

    @JsonProperty("VBrakeDuration")
    public void setVBrakeDuration(String vBrakeDuration) {
        this.vBrakeDuration = vBrakeDuration;
    }

    @JsonProperty("ROName")
    public String getROName() {
        return rOName;
    }

    @JsonProperty("ROName")
    public void setROName(String rOName) {
        this.rOName = rOName;
    }

    @JsonProperty("VEvtID")
    public String getVEvtID() {
        return vEvtID;
    }

    @JsonProperty("VEvtID")
    public void setVEvtID(String vEvtID) {
        this.vEvtID = vEvtID;
    }

    @JsonProperty("GPSEndLatitude")
    public String getGPSEndLatitude() {
        return gPSEndLatitude;
    }

    @JsonProperty("GPSEndLatitude")
    public void setGPSEndLatitude(String gPSEndLatitude) {
        this.gPSEndLatitude = gPSEndLatitude;
    }

    @JsonProperty("GPSStartDateTime")
    public String getGPSStartDateTime() {
        return gPSStartDateTime;
    }

    @JsonProperty("GPSStartDateTime")
    public void setGPSStartDateTime(String gPSStartDateTime) {
        this.gPSStartDateTime = gPSStartDateTime;
    }

    @JsonProperty("NumberOfIndexMessage")
    public String getNumberOfIndexMessage() {
        return numberOfIndexMessage;
    }

    @JsonProperty("NumberOfIndexMessage")
    public void setNumberOfIndexMessage(String numberOfIndexMessage) {
        this.numberOfIndexMessage = numberOfIndexMessage;
    }

    @JsonProperty("VID")
    public String getVid() {
        return vid;
    }

    @JsonProperty("VID")
    public void setVid(String vid) {
        this.vid = vid;
    }

    @JsonProperty("VIdleDuration")
    public String getVIdleDuration() {
        return vIdleDuration;
    }

    @JsonProperty("VIdleDuration")
    public void setVIdleDuration(String vIdleDuration) {
        this.vIdleDuration = vIdleDuration;
    }

    @JsonProperty("VPTODuration")
    public String getVPTODuration() {
        return vPTODuration;
    }

    @JsonProperty("VPTODuration")
    public void setVPTODuration(String vPTODuration) {
        this.vPTODuration = vPTODuration;
    }

    @JsonProperty("VPTODist")
    public String getVPTODist() {
        return vPTODist;
    }

    @JsonProperty("VPTODist")
    public void setVPTODist(String vPTODist) {
        this.vPTODist = vPTODist;
    }

    @JsonProperty("Timestamps")
    public Timestamps getTimestamps() {
        return timestamps;
    }

    @JsonProperty("Timestamps")
    public void setTimestamps(Timestamps timestamps) {
        this.timestamps = timestamps;
    }

    @JsonProperty("EvtDateTime")
    public String getEvtDateTime() {
        return evtDateTime;
    }

    @JsonProperty("EvtDateTime")
    public void setEvtDateTime(String evtDateTime) {
        this.evtDateTime = evtDateTime;
    }

    @JsonProperty("Document")
    public Document getDocument() {
        return document;
    }

    @JsonProperty("Document")
    public void setDocument(Document document) {
        this.document = document;
    }

    @JsonProperty("messageKey")
    public String getMessageKey() {
        return messageKey;
    }

    @JsonProperty("messageKey")
    public void setMessageKey(String messageKey) {
        this.messageKey = messageKey;
    }

    @JsonProperty("receivedTimestamp")
    public Long getReceivedTimestamp() {
        return receivedTimestamp;
    }

    @JsonProperty("receivedTimestamp")
    public void setReceivedTimestamp(Long receivedTimestamp) {
        this.receivedTimestamp = receivedTimestamp;
    }

    @JsonProperty("hostname")
    public String getHostname() {
        return hostname;
    }

    @JsonProperty("hostname")
    public void setHostname(String hostname) {
        this.hostname = hostname;
    }

    @JsonProperty("partitionId")
    public String getPartitionId() {
        return partitionId;
    }

    @JsonProperty("partitionId")
    public void setPartitionId(String partitionId) {
        this.partitionId = partitionId;
    }

    @JsonProperty("queue")
    public String getQueue() {
        return queue;
    }

    @JsonProperty("queue")
    public void setQueue(String queue) {
        this.queue = queue;
    }

    @JsonAnyGetter
    public Map<String, Object> getAdditionalProperties() {
        return this.additionalProperties;
    }

    @JsonAnySetter
    public void setAdditionalProperty(String name, Object value) {
        this.additionalProperties.put(name, value);
    }

}
