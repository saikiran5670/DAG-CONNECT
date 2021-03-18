package net.atos.daf.postgre.bo;

import java.io.Serializable;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Transient;

@Entity
//@Table(name = "tripdetail.trip_statistics")
@Table(name = "tripdetail.dummy_table")
public class TripMessage implements Serializable {

	
	@Id
	//@GeneratedValue
	@Column(name = "id")
	private int id;
	
	@Column(name = "trip_id")
	private String tripId;
	
	@Column(name = "vin")
	private String vin;
	
	@Column(name = "start_time_stamp")
	private Long startDateTime;

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getTripId() {
		return tripId;
	}

	public void setTripId(String tripId) {
		this.tripId = tripId;
	}

	public String getVin() {
		return vin;
	}

	public void setVin(String vin) {
		this.vin = vin;
	}

	public Long getStartDateTime() {
		return startDateTime;
	}

	public void setStartDateTime(Long startDateTime) {
		this.startDateTime = startDateTime;
	}
	
	
}
