package net.atos.daf.postgre.bo;

import java.io.Serializable;

import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Data
@AllArgsConstructor
@NoArgsConstructor
@ToString
public class WarningStatisticsPojo implements Serializable{
	
	private static final long serialVersionUID = 1L;  
	
	private String tripId;
	private String vid;
	private String vin;
	private  Long warningTimeStamp;
	private Integer warningClass;
	private Integer warningNumber;	
	private Double latitude;
	private  Double longitude;
	private  Double heading;
	private String vehicleHealthStatusType;
	private String vehicleDrivingStatusType;
	private String driverID;
	private String warningType;
	private Long distanceUntilNextService;
	private Long odometerVal;
	private Long lastestProcessedMessageTimeStamp;
	private Long createdAt;
	private  Long modifiedAt  ;
	private Integer messageType;
	
	private Integer id;

}
