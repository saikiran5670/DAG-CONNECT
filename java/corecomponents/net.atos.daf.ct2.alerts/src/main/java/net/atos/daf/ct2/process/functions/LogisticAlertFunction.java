package net.atos.daf.ct2.process.functions;


import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.process.Message;
import net.atos.daf.ct2.models.process.Target;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.process.service.AlertLambdaExecutor;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.Map;
import java.util.Optional;
import java.util.Set;

public class LogisticAlertFunction implements Serializable {

    private static final long serialVersionUID = -2623908626314058510L;
    private static final Logger logger = LoggerFactory.getLogger(LogisticAlertFunction.class);

    public static AlertLambdaExecutor<Message, Target> excessiveDistanceDone  = (Message s) -> {
        Status status = (Status) s.getPayload().get();
        Map<String,Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        Set<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (Set<AlertUrgencyLevelRefSchema>) threshold.get("excessiveDistanceDone");
        Double value = Double.valueOf(status.getGpsStopVehDist()) - Double.valueOf(status.getGpsStartVehDist());

        for(AlertUrgencyLevelRefSchema urgency : urgencyLevelRefSchemas){

            if(urgency.getUrgencyLevelType().equalsIgnoreCase("C")){
                if(thresholdBreach(value, Double.valueOf(urgency.getThresholdValue()))){
                    logger.info("alert found excessiveDistanceDone ::type {} , threshold {} , status {}",urgency.getAlertType(),urgency.getThresholdValue(),status);
                    return getTarget(status, urgency);
                }
            }
            if(urgency.getUrgencyLevelType().equalsIgnoreCase("W")){
                if(thresholdBreach(value, Double.valueOf(urgency.getThresholdValue()))){
                    logger.info("alert found excessiveDistanceDone ::type {} , threshold {} , status {}",urgency.getAlertType(),urgency.getThresholdValue(),status);
                    return getTarget(status, urgency);
                }
            }
            if(urgency.getUrgencyLevelType().equalsIgnoreCase("A")){
                if(thresholdBreach(value, Double.valueOf(urgency.getThresholdValue()))){
                    logger.info("alert found excessiveDistanceDone ::type {} , threshold {} , status {}",urgency.getAlertType(),urgency.getThresholdValue(),status);
                    return getTarget(status, urgency);
                }
            }
        }

        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };


    public static AlertLambdaExecutor<Message,Target> excessiveGlobalMileage = (Message s) -> {
        Status status = (Status) s.getPayload().get();
        Map<String,Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        Set<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (Set<AlertUrgencyLevelRefSchema>) threshold.get("excessiveGlobalMileage");

        for(AlertUrgencyLevelRefSchema urgency : urgencyLevelRefSchemas){

            if(urgency.getUrgencyLevelType().equalsIgnoreCase("C")){
                if(thresholdBreach(Double.valueOf(status.getGpsStopVehDist()), Double.valueOf(urgency.getThresholdValue()))){
                    logger.info("alert found excessiveGlobalMileage ::type {} , threshold {} , status {}",urgency.getAlertType(),urgency.getThresholdValue(),status);
                    return getTarget(status, urgency);
                }
            }
            if(urgency.getUrgencyLevelType().equalsIgnoreCase("W")){
                if(thresholdBreach(Double.valueOf(status.getGpsStopVehDist()), Double.valueOf(urgency.getThresholdValue()))){
                    logger.info("alert found excessiveGlobalMileage ::type {} , threshold {} , status {}",urgency.getAlertType(),urgency.getThresholdValue(),status);
                    return getTarget(status, urgency);
                }
            }
            if(urgency.getUrgencyLevelType().equalsIgnoreCase("A")){
                if(thresholdBreach(Double.valueOf(status.getGpsStopVehDist()), Double.valueOf(urgency.getThresholdValue()))){
                    logger.info("alert found excessiveGlobalMileage ::type {} , threshold {} , status {}",urgency.getAlertType(),urgency.getThresholdValue(),status);
                    return getTarget(status, urgency);
                }
            }
        }

        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };

    private static boolean thresholdBreach(Double aDouble, Double thresholdValue) {
        return aDouble > thresholdValue;
    }

    private static Target getTarget(Status status, AlertUrgencyLevelRefSchema urgency) {
        return Target.builder()
                .alert(Optional.of(Alert.builder()
                        .tripid(status.getTransID())
                        .vin(status.getVin())
                        .categoryType(urgency.getAlertCategory())
                        .type(urgency.getAlertType())
                        .alertid("" + urgency.getAlertId())
                        .alertGeneratedTime(String.valueOf(System.currentTimeMillis()))
                        .thresholdValue("" + urgency.getThresholdValue())
                        .thresholdValueUnitType(urgency.getUnitType())
                        .valueAtAlertTime(""+status.getGpsStopVehDist())
                        .urgencyLevelType(urgency.getUrgencyLevelType())
                        .build()))
                .build();
    }

}
