package net.atos.daf.ct2.process.functions;


import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.process.Message;
import net.atos.daf.ct2.models.process.Target;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.process.service.AlertLambdaExecutor;
import net.atos.daf.ct2.util.Utils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.text.SimpleDateFormat;
import java.util.*;

import static net.atos.daf.ct2.props.AlertConfigProp.INCOMING_MESSAGE_UUID;
import static net.atos.daf.ct2.util.Utils.*;

public class LogisticAlertFunction implements Serializable {

    private static final long serialVersionUID = -2623908626314058510L;
    private static final Logger logger = LoggerFactory.getLogger(LogisticAlertFunction.class);


    public static AlertLambdaExecutor<Message, Target> excessiveDistanceDone  = (Message s) -> {
        Status status = (Status) s.getPayload().get();
        Map<String,Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("excessiveDistanceDone");
        Double value = Double.valueOf(status.getGpsStopVehDist()) - Double.valueOf(status.getGpsStartVehDist());

        List<String> priorityList = Arrays.asList("C", "W", "A");
        for(String priority : priorityList){
            for (AlertUrgencyLevelRefSchema urgency : urgencyLevelRefSchemas) {
                if (urgency.getUrgencyLevelType().equalsIgnoreCase(priority)) {
                    if (thresholdBreach(value, Double.valueOf(urgency.getThresholdValue()))) {
                        logger.info("alert found excessiveDistanceDone ::type {} , threshold {} , status {}", urgency.getAlertType(), urgency.getThresholdValue(), status);
                        return getTarget(status, urgency,value);
                    }
                }
            }
        }

        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };

    public static AlertLambdaExecutor<Message,Target> excessiveGlobalMileage = (Message s) -> {
        Status status = (Status) s.getPayload().get();
        Map<String,Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("excessiveGlobalMileage");

        List<String> priorityList = Arrays.asList("C", "W", "A");
        for(String priority : priorityList){
            for (AlertUrgencyLevelRefSchema urgency : urgencyLevelRefSchemas) {
                if (urgency.getUrgencyLevelType().equalsIgnoreCase(priority)) {
                    if (thresholdBreach(Double.valueOf(status.getGpsStopVehDist()), Double.valueOf(urgency.getThresholdValue()))) {
                        logger.info("alert found excessiveGlobalMileage ::type {} , threshold {} , status {}", urgency.getAlertType(), urgency.getThresholdValue(), status);
                        return getTarget(status, urgency,status.getGpsStopVehDist());
                    }
                }
            }
        }

        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };

    public static AlertLambdaExecutor<Message,Target> excessiveDriveTime = (Message s) -> {
        Status status = (Status) s.getPayload().get();
        Map<String,Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("excessiveDriveTime");
        /**
         * Drive time calculations
         */
        try{
            long diffBetweenDatesInSeconds = timeDiffBetweenDates(status.getGpsStartDateTime(), status.getGpsEndDateTime());
            long diffSeconds = diffBetweenDatesInSeconds - status.getVIdleDuration();

            List<String> priorityList = Arrays.asList("C", "W", "A");
            for(String priority : priorityList){
                for (AlertUrgencyLevelRefSchema urgency : urgencyLevelRefSchemas) {
                    if (urgency.getUrgencyLevelType().equalsIgnoreCase(priority)) {
                        if (thresholdBreach(diffSeconds, Double.valueOf(urgency.getThresholdValue()))) {
                            logger.info("alert found excessiveDriveTime ::type {} , threshold {} , status {}", urgency.getAlertType(), urgency.getThresholdValue(), status);
                            return getTarget(status, urgency,diffSeconds);
                        }
                    }
                }
            }
        }catch (Exception ex){
            logger.error("Error while checking excessiveDriveTime threshold are breach:: {}",ex);
        }
        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };

    private static boolean thresholdBreach(Double aDouble, Double thresholdValue) {
        return aDouble > thresholdValue;
    }

    private static boolean thresholdBreach(Long aLong, Long thresholdValue) {
        return aLong > thresholdValue;
    }
    private static boolean thresholdBreach(Long aLong, Double thresholdValue) {
        return aLong > thresholdValue;
    }

    private static Target getTarget(Status status, AlertUrgencyLevelRefSchema urgency, Object actualValue) {

        return Target.builder()
                .alert(Optional.of(Alert.builder()
                        .tripid(status.getDocument() !=null ? status.getDocument().getTripID() : "")
                        .vin(status.getVin())
                        .categoryType(urgency.getAlertCategory())
                        .type(urgency.getAlertType())
                        .alertid("" + urgency.getAlertId())
                        .alertGeneratedTime(""+ getCurrentTimeInUTC())
                        .thresholdValue("" + urgency.getThresholdValue())
                        .thresholdValueUnitType(urgency.getUnitType())
                        .valueAtAlertTime(""+actualValue)
                        .urgencyLevelType(urgency.getUrgencyLevelType())
                        .build()))
                .build();
    }


}
