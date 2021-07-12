package net.atos.daf.postgre.bo;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor


public class TwoMinuteRulePojo {
	private  Long start_time;
	private  String code;
	private Long duration;
}
