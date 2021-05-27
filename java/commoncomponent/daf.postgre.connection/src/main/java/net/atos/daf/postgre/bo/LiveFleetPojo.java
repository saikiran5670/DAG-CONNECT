package net.atos.daf.postgre.bo;



import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor

public class LiveFleetPojo implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L; 
	
	private String tripId;
	private String vid;
	private String vin;
	private  Double messageTimestamp ;
	private  Double gpsAltitude            ;
	private  Double gpsHeading            ;
	private  Double gpsLatitude            ;
	private  Double gpsLongitude          ;
	private  Double co2Emission           ;
	private  Double fuelConsumption     ;
	private  Integer lastOdometerValue         ;
	private  Integer distUntilNextService ;
	private  Long created_at_m2m          ;
	private  Long created_at_kafka         ;
	private  Long created_at_dm             ;
	
	/*public String getTripId() {
		return tripId;
	}
	public void setTripId(String tripId) {
		this.tripId = tripId;
	}
	public String getVid() {
		return vid;
	}
	public void setVid(String vid) {
		this.vid = vid;
	}
	public String getVin() {
		return vin;
	}
	public void setVin(String vin) {
		this.vin = vin;
	}
	public Double getMessageTimestamp() {
		return messageTimestamp;
	}
	public void setMessageTimestamp(Double messageTimestamp) {
		this.messageTimestamp = messageTimestamp;
	}
	public Double getGpsAltitude() {
		return gpsAltitude;
	}
	public void setGpsAltitude(Double gpsAltitude) {
		this.gpsAltitude = gpsAltitude;
	}
	public Double getGpsHeading() {
		return gpsHeading;
	}
	public void setGpsHeading(Double gpsHeading) {
		this.gpsHeading = gpsHeading;
	}
	public Double getGpsLatitude() {
		return gpsLatitude;
	}
	public void setGpsLatitude(Double gpsLatitude) {
		this.gpsLatitude = gpsLatitude;
	}
	public Double getGpsLongitude() {
		return gpsLongitude;
	}
	public void setGpsLongitude(Double gpsLongitude) {
		this.gpsLongitude = gpsLongitude;
	}
	public Double getCo2Emission() {
		return co2Emission;
	}
	public void setCo2Emission(Double co2Emission) {
		this.co2Emission = co2Emission;
	}
	public Double getFuelConsumption() {
		return fuelConsumption;
	}
	public void setFuelConsumption(Double fuelConsumption) {
		this.fuelConsumption = fuelConsumption;
	}
	public int getLastOdometerValue() {
		return lastOdometerValue;
	}
	public void setLastOdometerValue(int lastOdometerValue) {
		this.lastOdometerValue = lastOdometerValue;
	}
	public int getDistUntilNextService() {
		return distUntilNextService;
	}
	public void setDistUntilNextService(int distUntilNextService) {
		this.distUntilNextService = distUntilNextService;
	}
	public long getCreated_at_m2m() {
		return created_at_m2m;
	}
	public void setCreated_at_m2m(long created_at_m2m) {
		this.created_at_m2m = created_at_m2m;
	}
	public long getCreated_at_kafka() {
		return created_at_kafka;
	}
	public void setCreated_at_kafka(long created_at_kafka) {
		this.created_at_kafka = created_at_kafka;
	}
	public long getCreated_at_dm() {
		return created_at_dm;
	}
	public void setCreated_at_dm(long created_at_dm) {
		this.created_at_dm = created_at_dm;
	}*/
	
	
	
	

}
