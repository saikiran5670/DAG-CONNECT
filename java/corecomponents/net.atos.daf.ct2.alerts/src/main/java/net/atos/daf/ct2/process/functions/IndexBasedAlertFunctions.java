package net.atos.daf.ct2.process.functions;

import static net.atos.daf.ct2.util.Utils.convertDateToMillis;
import static net.atos.daf.ct2.util.Utils.getCurrentDayOfWeek;
import static net.atos.daf.ct2.util.Utils.getCurrentTimeInSecond;
import static net.atos.daf.ct2.util.Utils.getDayOfWeekFromDbArr;
import static net.atos.daf.ct2.util.Utils.millisecondsToSeconds;

import java.io.Serializable;
import java.math.BigDecimal;
import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.Optional;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.process.Message;
import net.atos.daf.ct2.models.process.Target;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.process.service.AlertLambdaExecutor;

public class IndexBasedAlertFunctions implements Serializable {
	private static final long serialVersionUID = -2623908626314058510L;
	private static final Logger logger = LoggerFactory.getLogger(IndexBasedAlertFunctions.class);

    public static AlertLambdaExecutor<Message, Target> hoursOfServiceFun = (Message s) -> {
        Index index = (Index) s.getPayload().get();
        Map<String,Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("hoursOfService");
        List<String> priorityList = Arrays.asList("C", "W", "A");
        try{
            if(Objects.nonNull(index.getDocument()) && index.getDocument().getVWheelBasedSpeed() <= 0L && index.getDocument().getVEngineSpeed() <= 0L)
                return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
            for(String priority : priorityList){
                for(AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas){
                    if(schema.getUrgencyLevelType().equalsIgnoreCase(priority)){
                        String currentDayOfWeek = getCurrentDayOfWeek();
                        String dayOfWeekFromDbArr = getDayOfWeekFromDbArr(schema.getDayTypeArray());
                        if(currentDayOfWeek.equalsIgnoreCase(dayOfWeekFromDbArr)){
                            if(schema.getPeriodType().equalsIgnoreCase("A")){
                                return getTarget(index, schema, millisecondsToSeconds(convertDateToMillis(index.getEvtDateTime())));
                            }
                            if(schema.getPeriodType().equalsIgnoreCase("C")){
                                int currentTimeInSecond = getCurrentTimeInSecond();
                                if(schema.getStartTime() <= currentTimeInSecond && schema.getEndTime() > currentTimeInSecond){
                                    return getTarget(index, schema,millisecondsToSeconds(convertDateToMillis(index.getEvtDateTime())));
                                }
                            }
                        }
                    }
                }
            }
        }catch (Exception ex){
            logger.error("Error while calculating hoursOfService:: {}",ex);
        }
        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };

    public static AlertLambdaExecutor<Message, Target> excessiveAverageSpeedFun = (Message s) -> {
        Index index = (Index) s.getPayload().get();
        Map<String, Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("excessiveAverageSpeed");
        List<String> priorityList = Arrays.asList("C", "W", "A");
        try {
            for (String priority : priorityList) {
                for (AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas) {
                    if (schema.getUrgencyLevelType().equalsIgnoreCase(priority)) {
                        if (index.getVDist() > Double.valueOf(schema.getThresholdValue())) {
                            logger.info("alert found excessiveAverageSpeed ::type {} , threshold {} , index {}", schema.getAlertType(), schema.getThresholdValue(), index);
                            return getTarget(index, schema, index.getVDist());
                        }
                    }
                }
            }
        } catch (Exception ex) {
            logger.error("Error while calculating excessiveAverageSpeed:: {}", ex);
        }
        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };
    
    public static AlertLambdaExecutor<Message, Target> excessiveIdlingFun = (Message s) -> {
        Index index = (Index) s.getPayload().get();
        Map<String, Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("excessiveIdling");
        List<String> priorityList = Arrays.asList("C", "W", "A");
       
        try {
            for (String priority : priorityList) {
                for (AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas) {
                    if (schema.getUrgencyLevelType().equalsIgnoreCase(priority)) {
                    	
                        if (index.getVIdleDuration() > Double.valueOf(schema.getThresholdValue())) {
                            logger.info("alert found excessiveIdling ::type {} , threshold {} , index {}", schema.getAlertType(), schema.getThresholdValue(), index);
                            return getTarget(index, schema, convertDateToMillis(index.getEvtDateTime()));
                        }
                    }
                }
            }
        } catch (Exception ex) {
            logger.error("Error while calculating excessiveIdlingFun:: {}", ex);
        }
        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };

    public static AlertLambdaExecutor<Message, Target> fuelIncreaseDuringStopFun = (Message s) -> {
        net.atos.daf.ct2.models.Index index = (net.atos.daf.ct2.models.Index) s.getPayload().get();
        Map<String, Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("fuelIncreaseDuringStopFunAlertDef");
        List<String> priorityList = Arrays.asList("C", "W", "A");
        Index originalIdxMsg = index.getIndexList().get(0);
        BigDecimal vFuelStopPrevVal = index.getVFuelStopPrevVal();
        BigDecimal currentFuelVal = null;
        Integer tripStart = Integer.valueOf(4) ;
        try {
        	if(Objects.nonNull(originalIdxMsg.getDocument()) && Objects.nonNull(originalIdxMsg.getDocument().getVFuelLevel1()))
        		currentFuelVal = BigDecimal.valueOf(originalIdxMsg.getDocument().getVFuelLevel1());
            
        	for (String priority : priorityList) {
                for (AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas) {
                    if (schema.getUrgencyLevelType().equalsIgnoreCase(priority)) {
                      
                    	if(Objects.nonNull(vFuelStopPrevVal) && Objects.nonNull(currentFuelVal)){
    						BigDecimal fuelIncreaseDiff = BigDecimal.ZERO;
    						
    						if(tripStart == originalIdxMsg.getVEvtID())
    							fuelIncreaseDiff = currentFuelVal.subtract(vFuelStopPrevVal);
    						logger.info("Fuel Stop Deviation, tripStartFuel: {} , vFuelStopPrevVal:{}, stopIncreaseThresholdVal: {}, vEvtId: {}  ",currentFuelVal,  vFuelStopPrevVal, schema.getThresholdValue(), originalIdxMsg.getVEvtID());

    						//1 when fuelIncreaseDiff > threshold
    						if(fuelIncreaseDiff.compareTo(BigDecimal.ZERO) > 0 && fuelIncreaseDiff.compareTo(BigDecimal.valueOf(schema.getThresholdValue())) > 0){
    							logger.info("Raising alert for fuelIncreaseDuringStop fuelIncreaseDiff: {} thereshold: {} ",fuelIncreaseDiff,schema.getThresholdValue());
    							return getTarget(index, schema, fuelIncreaseDiff);
    						}
    					}
                    }
                }
            }
        } catch (Exception ex) {
            logger.error("Error while calculating fuelIncreaseDuringStopFun:: {}", ex);
        }
        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };

    public static AlertLambdaExecutor<Message, Target> fuelDecreaseDuringStopFun = (Message s) -> {
        net.atos.daf.ct2.models.Index index = (net.atos.daf.ct2.models.Index) s.getPayload().get();
        Map<String, Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("fuelDecreaseDuringStopFunAlertDef");
        List<String> priorityList = Arrays.asList("C", "W", "A");
        Index originalIdxMsg = index.getIndexList().get(0);
        BigDecimal vFuelStartVal = index.getVFuelStopPrevVal();
        BigDecimal currentFuelVal = null;
        Integer tripEnd = Integer.valueOf(5) ;
        try {
        	
        	if(Objects.nonNull(originalIdxMsg.getDocument()) && Objects.nonNull(originalIdxMsg.getDocument().getVFuelLevel1()))
        		currentFuelVal = BigDecimal.valueOf(originalIdxMsg.getDocument().getVFuelLevel1());
            
        	for (String priority : priorityList) {
                for (AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas) {
                    if (schema.getUrgencyLevelType().equalsIgnoreCase(priority)) {
                      
                    	if(Objects.nonNull(vFuelStartVal) && Objects.nonNull(currentFuelVal)){
    						BigDecimal fuelDecreaseDiff = BigDecimal.ZERO;
    						
    						if(tripEnd == originalIdxMsg.getVEvtID())
    							fuelDecreaseDiff = vFuelStartVal.subtract(currentFuelVal);
    						logger.info("Fuel Decrease Stop Deviation, currentFuelVal: {} , vFuelStartVal:{}, stopIncreaseThresholdVal: {}, vEvtId: {}  ",currentFuelVal,  vFuelStartVal, schema.getThresholdValue(), originalIdxMsg.getVEvtID());

    						//1 when fuelIncreaseDiff > threshold
    						if(fuelDecreaseDiff.compareTo(BigDecimal.ZERO) > 0 && fuelDecreaseDiff.compareTo(BigDecimal.valueOf(schema.getThresholdValue())) > 0){
    							logger.info("Raising alert for fuelDecreaseDuringStop fuelIncreaseDiff: {} thereshold: {} ",fuelDecreaseDiff,schema.getThresholdValue());
    							return getTarget(index, schema, fuelDecreaseDiff);
    						}
    					}
                    }
                }
            }
        } catch (Exception ex) {
            logger.error("Error while calculating fuelDecreaseDuringStopFun:: {}", ex);
        }
        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };

    public static AlertLambdaExecutor<Message, Target> excessiveUnderUtilizationInHoursFun = (Message s) -> {
        net.atos.daf.ct2.models.Index index = (net.atos.daf.ct2.models.Index) s.getPayload().get();
        Map<String, Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("excessiveUnderUtilizationInHours");
        List<String> priorityList = Arrays.asList("C", "W", "A");
        logger.info("Checking excessiveUnderUtilizationInHours for vin:: {}, threshold::{}",index.getVin(),urgencyLevelRefSchemas);
        try {
            for (String priority : priorityList) {
                for (AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas) {
                    if (schema.getUrgencyLevelType().equalsIgnoreCase(priority)) {
                        Boolean isVehicleMoved=Boolean.FALSE;
                        List<Index> indexList = index.getIndexList();
                        for(int i=1; i < indexList.size(); i++){
                            Index previous = indexList.get(i-1);
                            Index next = indexList.get(i);
                            if( (previous.getGpsLatitude().doubleValue() != next.getGpsLatitude().doubleValue())
                                    && (previous.getGpsLongitude().doubleValue() != next.getGpsLongitude().doubleValue())
                                    && (previous.getVDist().longValue() != next.getVDist().longValue())){
                                isVehicleMoved=Boolean.TRUE;
                                break;
                            }
                        }
                        if (! isVehicleMoved ) {
                            for(Index idx : indexList){
                                long eventTimeInMillis = convertDateToMillis(idx.getEvtDateTime());
                                long eventTimeInSeconds = millisecondsToSeconds(eventTimeInMillis);
                                long fromTimeInSeconds =  millisecondsToSeconds(System.currentTimeMillis()) - schema.getThresholdValue().longValue();
                                long endTimeInSeconds =   millisecondsToSeconds(System.currentTimeMillis());
                                if(eventTimeInSeconds > fromTimeInSeconds && eventTimeInSeconds <= endTimeInSeconds){
                                    logger.info("alert found excessiveUnderUtilizationInHours ::type {} , threshold {} , index {}", schema.getAlertType(), schema.getThresholdValue(), index);
                                    return getTarget(index, schema, eventTimeInSeconds);
                                }
                            }
                        }
                    }
                }
            }
        } catch (Exception ex) {
            logger.error("Error while calculating excessiveUnderUtilizationInHours:: {}", ex);
        }
        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };

    private static Target getTarget(Index index, AlertUrgencyLevelRefSchema urgency, Object valueAtAlertTime) {
        return Target.builder()
                .alert(Optional.of(Alert.builder()
                        .tripid(index.getDocument() !=null ? index.getDocument().getTripID() : "")
                        .vin(index.getVin())
                        .categoryType(urgency.getAlertCategory())
                        .type(urgency.getAlertType())
                        .alertid("" + urgency.getAlertId())
                        .alertGeneratedTime(String.valueOf(System.currentTimeMillis()))
                        .thresholdValue("" + urgency.getThresholdValue())
                        .thresholdValueUnitType(urgency.getUnitType())
                        .valueAtAlertTime(""+valueAtAlertTime)
                        .urgencyLevelType(urgency.getUrgencyLevelType())
                        .build()))
                .build();
    }
}
