package net.atos.daf.postgre.bo;



import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class DriverActivityPojo implements Serializable{
	
	private static final long serialVersionUID = 1L;  
	
	private String tripId;
	private String vid;
	private String vin;
	private  Long tripStartTimeStamp;
	private  Long tripEndTimeStamp;
	private  Long activityDate;
	private String driverID;
	private  String code;
	private  Long startTime;
	private  Long endTime;
	private  Long duration;
	private  Long createdAtM2m ;
	private  Long createdAtKafka ;
	private  Long createdAtDm  ;
	private  Long modifiedAt  ;
	private  Long lastProcessedMessageTimestamp;
	private Boolean isDriver1;
	
	private String formattedCode;
	
	
	

}
