package net.atos.daf.ct2.common.processing;




import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class DriverPreviousData {
	
	private Long starDate;
	private Integer driveTime; 
	private String tripId;

}
