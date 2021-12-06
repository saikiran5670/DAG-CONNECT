package net.atos.daf.ct2.common.processing;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class TripPreviosData {
	
	private Long endTimeStamp;
	private Integer tripDistance;
	private Long drivingTime;
	private Long odometerValue;
	private String tripId;

}
