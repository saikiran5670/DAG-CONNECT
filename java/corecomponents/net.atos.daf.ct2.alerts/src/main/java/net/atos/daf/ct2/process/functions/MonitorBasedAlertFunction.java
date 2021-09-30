package net.atos.daf.ct2.process.functions;

import java.io.Serializable;
import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.Optional;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.mysql.cj.xdevapi.Schema;

import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.process.Message;
import net.atos.daf.ct2.models.process.Target;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.process.service.AlertLambdaExecutor;

import static net.atos.daf.ct2.props.AlertConfigProp.INCOMING_MESSAGE_UUID;
import static net.atos.daf.ct2.util.Utils.convertDateToMillis;

public class MonitorBasedAlertFunction implements Serializable {
	private static final long serialVersionUID = -2623908626314058510L;
	private static final Logger logger = LoggerFactory.getLogger(MonitorBasedAlertFunction.class);

	public static AlertLambdaExecutor<Message, Target> repairMaintenanceFun = (Message s) -> {
		Monitor monitor = (Monitor) s.getPayload().get();
		Map<String, Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
		List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold
				.get("repairMaintenanceFun");

		List<String> priorityList = Arrays.asList("C","W");

		// List<String> priorityList = Arrays.asList("E", "O");

		try {
			System.out.println("inside try");
			for (String priority : priorityList) {
				for (AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas) {
					if (schema.getAlertType().equalsIgnoreCase("O")
							&& schema.getUrgencyLevelType().equalsIgnoreCase("C")) {
						if (monitor.getDocument().getVWarningClass() >= 4
								&& monitor.getDocument().getVWarningClass() <= 7) {

							if (monitor.getVEvtID() == 46) {
								System.out.println("inside Monitor Function--alert raised");
								logger.info("Critical Alert generated for VEvtid=46  :{} msg UUD :: {}", monitor.getJobName());

								return getTarget(monitor, schema, 46);
							} else if (monitor.getVEvtID() == 63) {
								System.out.println("inside Monitor Function--alert raised");
								logger.info("Critical Alert generated for VEvtid=63  :{} msg UUD :: {}", monitor.getJobName());
								return getTarget(monitor, schema, 63);
							} else if (monitor.getVEvtID() == 44) {
								System.out.println("inside Monitor Function--alert raised");
								logger.info("Critical Alert generated for VEvtid=44  :{} msg UUD :: {}", monitor.getJobName());
								return getTarget(monitor, schema, 44);
							}

						}
					}
					

					if (schema.getAlertType().equalsIgnoreCase("E")
							&& schema.getUrgencyLevelType().equalsIgnoreCase("W")) {
						if (monitor.getDocument().getVWarningClass() >= 8
								&& monitor.getDocument().getVWarningClass() <=11) {

							if (monitor.getVEvtID() == 46) {
								System.out.println("inside Monitor Function--alert raised");
								logger.info("Warning Alert generated for VEvtid=46  :{} msg UUD :: {}", monitor.getJobName());
								return getTarget(monitor, schema, 46);
							} else if (monitor.getVEvtID() == 63) {
								System.out.println("inside Monitor Function--alert raised");
								logger.info("Warning Alert generated for VEvtid=63  :{} msg UUD :: {}", monitor.getJobName());
								return getTarget(monitor, schema, 63);
							} else if (monitor.getVEvtID() == 44) {
								System.out.println("inside Monitor Function--alert raised");
								logger.info("Warning Alert generated for VEvtid=44  :{} msg UUD :: {}", monitor.getJobName());
								return getTarget(monitor, schema, 44);
							}

						}
					}

				

				}
			}
		} catch (Exception ex) {
			logger.error("Error while calculating Repair and Maintenance:: {}", ex);
		}

		return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
	};


	private static Target getTarget(Monitor moniter, AlertUrgencyLevelRefSchema urgency, Object valueAtAlertTime) {
		String alertGeneratedTime = String.valueOf(System.currentTimeMillis());
		try{
			alertGeneratedTime = String.valueOf(convertDateToMillis(moniter.getEvtDateTime()));
		}catch (Exception ex){
			logger.error("Error while converting event time to milliseconds {} error {} ",String.format(INCOMING_MESSAGE_UUID,moniter.getJobName()));
		}
		return Target.builder().alert(Optional.of(Alert.builder()
						.tripid(" ")
						.vin(moniter.getVin())
						.categoryType(urgency.getAlertCategory())
						.type(urgency.getAlertType())
						.alertid("" + urgency.getAlertId())
						.alertGeneratedTime(alertGeneratedTime)
						.thresholdValue("" + urgency.getThresholdValue())
						.thresholdValueUnitType(urgency.getUnitType())
						.valueAtAlertTime("0.0")
						.urgencyLevelType(urgency.getUrgencyLevelType()).build()))
				.build();
	}
}