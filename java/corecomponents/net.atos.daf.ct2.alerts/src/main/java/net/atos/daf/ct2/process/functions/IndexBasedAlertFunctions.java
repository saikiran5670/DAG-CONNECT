package net.atos.daf.ct2.process.functions;

import static net.atos.daf.ct2.props.AlertConfigProp.INCOMING_MESSAGE_UUID;
import static net.atos.daf.ct2.props.AlertConfigProp.exitCorridorRouteTree;
import static net.atos.daf.ct2.util.Utils.*;
import static net.atos.daf.ct2.util.Utils.convertDateToMillis;
import static net.atos.daf.ct2.util.Utils.getCurrentDayOfWeek;
import static net.atos.daf.ct2.util.Utils.getCurrentTimeInSecond;
import static net.atos.daf.ct2.util.Utils.getDayOfWeekFromDbArr;
import static net.atos.daf.ct2.util.Utils.millisecondsToSeconds;

import java.io.Serializable;
import java.math.BigDecimal;
import java.util.*;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import net.atos.daf.ct2.models.VehicleGeofenceState;
import net.atos.daf.ct2.props.AlertConfigProp;
import net.atos.daf.ct2.service.geofence.CircularGeofence;
import net.atos.daf.ct2.service.geofence.RayCasting;
import net.atos.daf.ct2.service.geofence.exit.corridor.GeoCorridor;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.api.common.state.MapState;
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
    	net.atos.daf.ct2.models.Index idx = (net.atos.daf.ct2.models.Index) s.getPayload().get();
    	Index index=idx.getIndexList().get(0);
        Map<String, Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("excessiveAverageSpeed");
        List<String> priorityList = Arrays.asList("C", "W", "A");
       
        try {
        	
            for (String priority : priorityList) {
                for (AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas) {
                    if (schema.getUrgencyLevelType().equalsIgnoreCase(priority)) {
                        if (idx.getAverageSpeed() > Double.valueOf(schema.getThresholdValue())) {
                            logger.info("alert found excessiveAverageSpeed ::type {} , threshold {} , index {}", schema.getAlertType(), schema.getThresholdValue(), idx);
                            return getTarget(index, schema, idx.getAverageSpeed());
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
    	net.atos.daf.ct2.models.Index idx = (net.atos.daf.ct2.models.Index) s.getPayload().get();
    	Index index=idx.getIndexList().get(0);
    	
        Map<String, Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("excessiveIdling");
        List<String> priorityList = Arrays.asList("C", "W", "A");
       
        try {
            for (String priority : priorityList) {
                for (AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas) {
                    if (schema.getUrgencyLevelType().equalsIgnoreCase(priority)) {
                    	
                        if (idx.getIdleDuration() > Double.valueOf(schema.getThresholdValue())) {
                            logger.info("alert found excessiveIdling ::type {} , threshold {} , index {}", schema.getAlertType(), schema.getThresholdValue(), idx);
                            return getTarget(index, schema, idx.getIdleDuration());
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
    							return getTarget(originalIdxMsg, schema, fuelIncreaseDiff);
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
        BigDecimal vFuelEndVal = index.getVFuelStopPrevVal();
        BigDecimal currentFuelVal = null;
        Integer tripstart = Integer.valueOf(4) ;
        try {
        	
        	if(Objects.nonNull(originalIdxMsg.getDocument()) && Objects.nonNull(originalIdxMsg.getDocument().getVFuelLevel1()))
        		currentFuelVal = BigDecimal.valueOf(originalIdxMsg.getDocument().getVFuelLevel1());
            
        	for (String priority : priorityList) {
                for (AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas) {
                    if (schema.getUrgencyLevelType().equalsIgnoreCase(priority)) {
                      
                    	if(Objects.nonNull(vFuelEndVal) && Objects.nonNull(currentFuelVal)){
    						BigDecimal fuelDecreaseDiff = BigDecimal.ZERO;
    						
    						if(tripstart == originalIdxMsg.getVEvtID())
    							fuelDecreaseDiff = vFuelEndVal.subtract(currentFuelVal);
    						logger.info("Fuel Decrease Stop Deviation, currentFuelVal: {} , vFuelEndVal:{}, stopIncreaseThresholdVal: {}, vEvtId: {}  ",currentFuelVal,  vFuelEndVal, schema.getThresholdValue(), originalIdxMsg.getVEvtID());

    						//1 when fuelIncreaseDiff > threshold
    						if(fuelDecreaseDiff.compareTo(BigDecimal.ZERO) > 0 && fuelDecreaseDiff.compareTo(BigDecimal.valueOf(schema.getThresholdValue())) > 0){
    							logger.info("Raising alert for fuelDecreaseDuringStop fuelIncreaseDiff: {} thereshold: {} ",fuelDecreaseDiff, schema.getThresholdValue());
    							return getTarget(originalIdxMsg, schema, fuelDecreaseDiff);
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
    

    public static AlertLambdaExecutor<Message, Target> fuelDuringTripFun = (Message s) -> {
        net.atos.daf.ct2.models.Index index = (net.atos.daf.ct2.models.Index) s.getPayload().get();
        Map<String, Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("fuelDuringTripFunAlertDef");
        List<String> priorityList = Arrays.asList("C", "W", "A");
        Index originalIdxMsg = index.getIndexList().get(0);
        BigDecimal vFuelPrevVal = index.getVFuelStopPrevVal();
        BigDecimal currentFuelVal = null;
        
        logger.info("Inside fuelDuringTripFun urgencyLevelRefSchemas:{} , index:{} ",urgencyLevelRefSchemas, index);

        try {
        	
        	if(Objects.nonNull(originalIdxMsg.getDocument()) && Objects.nonNull(originalIdxMsg.getDocument().getVFuelLevel1()))
        		currentFuelVal = BigDecimal.valueOf(originalIdxMsg.getDocument().getVFuelLevel1());
            
        	for (String priority : priorityList) {
                for (AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas) {
                    if (schema.getUrgencyLevelType().equalsIgnoreCase(priority)) {
                      
                    	if(Objects.nonNull(vFuelPrevVal) && Objects.nonNull(currentFuelVal)){
                    	//if (vFuelTripObj.getVFuelLevel() != null && vFuelPrevTripRecData.getVFuelLevel() != null) {
							BigDecimal fuelDecreaseDiff = vFuelPrevVal
									.subtract(currentFuelVal);
							
							logger.info("Fuel decrease during Trip, vFuelPrevVal: {}, currentFuelVal: {}, fuelDecreaseDiff:{} ", vFuelPrevVal, currentFuelVal, fuelDecreaseDiff);

							if (fuelDecreaseDiff.compareTo(BigDecimal.ZERO) > 0
									&& fuelDecreaseDiff.compareTo(BigDecimal.valueOf(schema.getThresholdValue())) > 0) {

								logger.info("Raising alert for fuelDecreaseDuringTrip fuelDecreadeDiff: {} thereshold: {} ",fuelDecreaseDiff,schema.getThresholdValue());
    							return getTarget(originalIdxMsg, schema, fuelDecreaseDiff);
							} 
						}
                    }
                }
            }
        } catch (Exception ex) {
            logger.error("Error while calculating fuelDuringTripFun:: {}", ex);
        }
        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };


    public static AlertLambdaExecutor<Message, Target> excessiveUnderUtilizationInHoursFun = (Message s) -> {
        net.atos.daf.ct2.models.Index index = (net.atos.daf.ct2.models.Index) s.getPayload().get();
        Map<String, Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("excessiveUnderUtilizationInHours");
        List<String> priorityList = Arrays.asList("A", "W", "C");
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
                                long fromTimeInSeconds =  millisecondsToSeconds(getCurrentTimeInUTC()) - schema.getThresholdValue().longValue();
                                long endTimeInSeconds =   millisecondsToSeconds(getCurrentTimeInUTC());
                                if(eventTimeInSeconds > fromTimeInSeconds && eventTimeInSeconds <= endTimeInSeconds){
                                    logger.info("Alert found excessiveUnderUtilizationInHours ::type {} , threshold {} , urgency {} index {}", schema.getAlertType(), schema.getThresholdValue(), schema.getUrgencyLevelType(), index);
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


    /**
     * Entering zone function
     */
    public static AlertLambdaExecutor<Message, Target> enteringZoneFun = (Message s) -> {
        Target build = Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
        Index index=(Index)s.getPayload().get();
        Map<String, Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("enteringAndExitingZoneFun");
        MapState<String, VehicleGeofenceState> vehicleGeofenceSateEnteringZone = (MapState<String, VehicleGeofenceState>) threshold.get("enteringAndExitingZoneVehicleState");
        try {
            Map<Long, List<AlertUrgencyLevelRefSchema>> alertMap = new HashMap<>();
            Map<Long, List<AlertUrgencyLevelRefSchema>> alertCircleMap = new HashMap<>();
            for(AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas){
                // Checking only for polygon geofence
                populateGeofenceAlertMap(alertMap, schema, "O");
                // Checking only for circular geofence
                populateGeofenceAlertMap(alertCircleMap, schema, "C");
                // Checking only for POI geofence
                populateGeofenceAlertMap(alertCircleMap, schema, "P");
            }
            Double [] point = new Double[]{ index.getGpsLatitude(), index.getGpsLongitude() };

                if(! alertMap.isEmpty()){
                    List<Target> targetList = alertMap.entrySet()
                            .stream()
                            .map(entries -> entries.getValue())
                            .map(schemaList -> checkGeofence(index, schemaList, point,vehicleGeofenceSateEnteringZone,"enterZone","O"))
                            .filter(target -> target.getAlert().isPresent())
                            .collect(Collectors.toList());
                    if(!targetList.isEmpty()){
                        logger.info("Entering zone alert generated for polygon geofence vin: {} , {}",index.getVin(),String.format(INCOMING_MESSAGE_UUID,index.getJobName()));
                       return  targetList.get(0);
                    }
                }
            /**
             * Checking for circular geofence of entering zone
             */
            if(! alertCircleMap.isEmpty()){
                List<Target> targetList =  alertCircleMap.entrySet()
                         .stream()
                         .map(entries -> entries.getValue())
                         .map(schemaList -> checkGeofence(index, schemaList, point,vehicleGeofenceSateEnteringZone,"enterZone","C"))
                         .filter(target -> target.getAlert().isPresent())
                         .collect(Collectors.toList());
                if(!targetList.isEmpty()){
                    logger.info("Entering zone alert generated for circular geofence vin: {} , {}",index.getVin(),String.format(INCOMING_MESSAGE_UUID,index.getJobName()));
                    return  targetList.get(0);
                }
            }
        } catch (Exception ex) {
            logger.error("Error while calculating enteringZoneFun:: {}", ex);
        }
        return build;
    };

    /**
     * Exit zone function
     */
    public static AlertLambdaExecutor<Message, Target> exitZoneFun = (Message s) -> {
        Target build = Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
        Index index=(Index)s.getPayload().get();
        Map<String, Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("exitingZoneFun");
        MapState<String, VehicleGeofenceState> vehicleGeofenceSateExitZone = (MapState<String, VehicleGeofenceState>) threshold.get("exitingZoneVehicleState");
        try {
            Map<Long, List<AlertUrgencyLevelRefSchema>> alertMap = new HashMap<>();
            Map<Long, List<AlertUrgencyLevelRefSchema>> alertCircleMap = new HashMap<>();
            for(AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas){
                // Checking only for polygon geofence
                populateGeofenceAlertMap(alertMap, schema, "O");
                // Checking only for circular geofence
                populateGeofenceAlertMap(alertCircleMap, schema, "C");
                // Checking only for POI geofence
                populateGeofenceAlertMap(alertCircleMap, schema, "P");
            }
            Double [] point = new Double[]{ index.getGpsLatitude(), index.getGpsLongitude() };
            if(! alertMap.isEmpty()){
                List<Target> targetList = alertMap.entrySet()
                        .stream()
                        .map(entries -> entries.getValue())
                        .map(schemaList -> checkGeofence(index, schemaList, point,vehicleGeofenceSateExitZone,"exitZone","O"))
                        .filter(target -> target.getAlert().isPresent())
                        .collect(Collectors.toList());
                if(!targetList.isEmpty()){
                    logger.info("Exit zone alert generated for polygon geofence vin: {} , {}",index.getVin(),String.format(INCOMING_MESSAGE_UUID,index.getJobName()));
                    return  targetList.get(0);
                }
            }
            /**
             * Checking for circular geofence of entering zone
             */
            if(! alertCircleMap.isEmpty()){
                List<Target> targetList =  alertCircleMap.entrySet()
                        .stream()
                        .map(entries -> entries.getValue())
                        .map(schemaList -> checkGeofence(index, schemaList, point,vehicleGeofenceSateExitZone,"exitZone","C"))
                        .filter(target -> target.getAlert().isPresent())
                        .collect(Collectors.toList());
                if(!targetList.isEmpty()){
                    logger.info("Exit zone alert generated for circular geofence vin: {} , {}",index.getVin(),String.format(INCOMING_MESSAGE_UUID,index.getJobName()));
                    return  targetList.get(0);
                }
            }

        } catch (Exception ex) {
            logger.error("Error while calculating enteringZoneFun:: {}", ex);
        }
        return build;
    };

    /**
     * Exit corridor function
     */
    public static AlertLambdaExecutor<Message, Target> exitCorridorFun = (Message s) -> {
        Index index = (Index) s.getPayload().get();
        Map<String,Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("exitCorridorFun");
        try{
            if(Objects.isNull(index.getDocument()))
                return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();

            Map<Long, List<AlertUrgencyLevelRefSchema>> alertMapExistingRoute = new HashMap<>();
            Map<Long, List<AlertUrgencyLevelRefSchema>> alertMapNewRoute = new HashMap<>();
            for(AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas){
                populateGeofenceAlertMap(alertMapExistingRoute, schema, "E");
                populateGeofenceAlertMap(alertMapNewRoute, schema, "R");
            }

            if(!alertMapExistingRoute.isEmpty()){
                List<Target> targetList = alertMapExistingRoute.entrySet()
                        .stream()
                        .map(entries -> entries.getValue())
                        .map(schemaList -> checkExitCorridor(index, schemaList,"E"))
                        .filter(target -> target.getAlert().isPresent())
                        .collect(Collectors.toList());
                if(!targetList.isEmpty()){
                    logger.info("Exit corridor trip based alert generated for  vin: {} , {}",index.getVin(),String.format(INCOMING_MESSAGE_UUID,index.getJobName()));
                    return  targetList.get(0);
                }
            }
            if(!alertMapNewRoute.isEmpty()){
                List<Target> targetList = alertMapNewRoute.entrySet()
                        .stream()
                        .map(entries -> entries.getValue())
                        .map(schemaList -> checkExitCorridor(index, schemaList,"R"))
                        .filter(target -> target.getAlert().isPresent())
                        .collect(Collectors.toList());
                if(!targetList.isEmpty()){
                    logger.info("Exit corridor route based alert generated for  vin: {} , {}",index.getVin(),String.format(INCOMING_MESSAGE_UUID,index.getJobName()));
                    return  targetList.get(0);
                }
            }

        }catch (Exception ex){
            logger.error("Error while calculating exitCorridorFun:: {}",ex);
        }
        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };

    public static Target checkGeofence(Index index, List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas,
                                       Double[] point,MapState<String, VehicleGeofenceState> vehicleGeofenceSateEnteringZone,
                                       String alertType,String areaType
                                       ) {

        List<String> priorityList = Arrays.asList("C", "W", "A");
        Boolean isPolygon = areaType.equalsIgnoreCase("O");
        String messageUUID = String.format(INCOMING_MESSAGE_UUID,index.getJobName());
        // Get vehicle sate for geofence
        VehicleGeofenceState vehicleState=VehicleGeofenceState.builder().isInside(Boolean.FALSE).landMarkId(-1).build();
        try {
            if(vehicleGeofenceSateEnteringZone.contains(index.getVin())){
                vehicleState= vehicleGeofenceSateEnteringZone.get(index.getVin());
            }else{
                vehicleGeofenceSateEnteringZone.put(index.getVin(),vehicleState);
            }
        } catch (Exception e) {
            logger.error("Error while retrieve previous state for vin {} "+alertType+"  {}",index.getVin(),messageUUID);
        }
        for (String priority : priorityList) {
            AlertUrgencyLevelRefSchema tempSchema = new AlertUrgencyLevelRefSchema();


            /**
             * sort lat lon based not node seq
             */
            //group the schema by landmark id
            Map<Integer, List<AlertUrgencyLevelRefSchema>> groupSchema= urgencyLevelRefSchemas.stream()
                    .filter(alertUrgencyLevelRefSchema -> alertUrgencyLevelRefSchema.getUrgencyLevelType().equalsIgnoreCase(priority))
                    .sorted(Comparator.comparing(AlertUrgencyLevelRefSchema::getLandmarkId).thenComparing(AlertUrgencyLevelRefSchema::getNodeSeq))
                    .collect(Collectors.groupingBy(AlertUrgencyLevelRefSchema::getLandmarkId));
            Set<Map.Entry<Integer, List<AlertUrgencyLevelRefSchema>>> entries = groupSchema.entrySet();

            Iterator<Map.Entry<Integer, List<AlertUrgencyLevelRefSchema>>> iterator = entries.iterator();
            while (iterator.hasNext()){
                List<Double> polygonPointList = new ArrayList<>();
                Map.Entry<Integer, List<AlertUrgencyLevelRefSchema>> next = iterator.next();
                List<AlertUrgencyLevelRefSchema> schemasOrdered = next.getValue();
                for (AlertUrgencyLevelRefSchema schema : schemasOrdered) {
                    polygonPointList.add(isPolygon ? schema.getLatitude() : schema.getCircleLatitude());
                    polygonPointList.add(isPolygon ?  schema.getLongitude() : schema.getCircleLongitude());
                    tempSchema = schema;
                }
                if(! polygonPointList.isEmpty()){
                    /**
                     * Convert arraylist of lat lon to polygon matrix
                     */
                    logger.trace("Geofence testing nodes {} for {}",polygonPointList,messageUUID);
                    logger.trace("Geofence testing point {} for {}",Arrays.asList(point),messageUUID);
                    Double[][] polygonPoints = new Double[polygonPointList.size() / 2][polygonPointList.size() / 2];
                    int indexCounter=0;
                    for (int i = 0; i < polygonPointList.size(); i=i+2) {
                        polygonPoints[indexCounter] = new Double[]{polygonPointList.get(i), polygonPointList.get((i + 1) % polygonPointList.size())};
                        indexCounter++;
                    }
                    // Check weather point inside or outside of polygon
                    Boolean inside = isPolygon ? RayCasting.isInside(polygonPoints, point)
                            : CircularGeofence.isInsideByHaversine(polygonPoints[0], point, tempSchema.getCircleRadius());
                    logger.info("Geofence testing result  {} points: {} test point {} for {}",inside,Arrays.asList(polygonPoints),Arrays.asList(point),messageUUID);
                    // If the state change raise an alert for entering zone
                    if (checkVehicleStateForZone(index, vehicleGeofenceSateEnteringZone, vehicleState, tempSchema, inside,alertType)){
                        Target target = getTarget(index, tempSchema, 0);
                        logger.info("Geofence alert generated for {} landmark type {} landmarkId {} alert message {} {}",
                                alertType,areaType,tempSchema.getLandmarkId(),target.getAlert().get(),messageUUID);
                        return target;
                    }

                }
            }
        }
        return Target.builder().alert(Optional.empty()).build();
    }

    public static boolean checkVehicleStateForZone(Index index, MapState<String, VehicleGeofenceState> vehicleGeofenceSate,
                                                   VehicleGeofenceState vehicleState, AlertUrgencyLevelRefSchema tempSchema,
                                                   Boolean inside, String alertType) {

        String messageUUId = String.format(INCOMING_MESSAGE_UUID, index.getJobName());
        //Enter zone check
        Boolean enterZoneTrue = ! vehicleState.getIsInside() && inside && alertType.equalsIgnoreCase("enterZone");
        Boolean enterZoneFalse = vehicleState.getIsInside()  && !inside && alertType.equalsIgnoreCase("enterZone");

        //Exit zone check
        Boolean exitZoneTrue = vehicleState.getIsInside() && !inside && alertType.equalsIgnoreCase("exitZone");
        Boolean exitZoneFalse = !vehicleState.getIsInside() && inside && alertType.equalsIgnoreCase("exitZone");

        if (enterZoneTrue || exitZoneTrue) {
            if(enterZoneTrue)
              logger.info("Vehicle enter into zone vin {} landmark type {} landmarkId {} {} ",index.getVin(),tempSchema.getLandMarkType(), tempSchema.getLandmarkId(), messageUUId);
            if(exitZoneTrue)
                logger.info("Vehicle exit from zone vin {} landmark type {} landmarkId {} {} ",index.getVin(),tempSchema.getLandMarkType(), tempSchema.getLandmarkId(), messageUUId);
            logger.info(alertType + " alert generated for vin {} for alertId {} {}", index.getVin(), tempSchema.getAlertId(), messageUUId );
            try {
                vehicleGeofenceSate.put(index.getVin(), VehicleGeofenceState.builder().isInside(inside).landMarkId(tempSchema.getLandmarkId()).build());
            } catch (Exception e) {
                logger.error("Error while retrieve previous state for vin {} " + alertType + " {}", index.getVin(), messageUUId);
            }
            return true;
        }
        // If the state change raise an alert for exiting zone
        if (enterZoneFalse || exitZoneFalse) {
            try {
                if(enterZoneFalse)
                    logger.info("Vehicle exit from zone vin {} landmark type {} landmarkId {} {} ",index.getVin(),tempSchema.getLandMarkType(), tempSchema.getLandmarkId(), messageUUId);
                if(exitZoneFalse)
                    logger.info("Vehicle enter into zone vin {} landmark type {} landmarkId {} {} ",index.getVin(),tempSchema.getLandMarkType(), tempSchema.getLandmarkId(), messageUUId);

                vehicleGeofenceSate.put(index.getVin(), VehicleGeofenceState.builder().isInside(inside).landMarkId(tempSchema.getLandmarkId()).build());
            } catch (Exception e) {
                logger.error("Error while retrieve previous state for vin {} " + alertType + " {} error {}", index.getVin(), messageUUId, e);
            }
        }
        return false;
    }

    private static void populateGeofenceAlertMap(Map<Long, List<AlertUrgencyLevelRefSchema>> alertMap, AlertUrgencyLevelRefSchema schema, String landmarkType) {
        if (schema.getLandMarkType().equalsIgnoreCase(landmarkType)) {
            if (alertMap.containsKey(schema.getAlertId())) {
                alertMap.get(schema.getAlertId()).add(schema);
            } else {
                List<AlertUrgencyLevelRefSchema> tmplist = new ArrayList<>();
                tmplist.add(schema);
                alertMap.put(schema.getAlertId(), tmplist);
            }
        }
    }

    public static Target checkExitCorridor(Index index, List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas,String landMarkType){
        List<String> priorityList = Arrays.asList("C", "W", "A");
        String messageUUID = String.format(INCOMING_MESSAGE_UUID,index.getJobName());
        Double [] point = new Double[]{ index.getGpsLatitude(), index.getGpsLongitude() };
        for (String priority : priorityList) {
            /**
             * sort lat lon based not node seq
             */
            //group the schema by landmark id
            Comparator<AlertUrgencyLevelRefSchema> seqComprator = Comparator.comparing(AlertUrgencyLevelRefSchema::getLandmarkId)
                    .thenComparing((o1, o2) -> landMarkType.equalsIgnoreCase("E") ?
                            o1.getNodeSeq().compareTo(o2.getNodeSeq()) :
                            o1.getCorriSeq().compareTo(o2.getCorriSeq()));

            Map<Integer, List<AlertUrgencyLevelRefSchema>> groupSchema= urgencyLevelRefSchemas.stream()
                    .filter(alertUrgencyLevelRefSchema -> alertUrgencyLevelRefSchema.getUrgencyLevelType().equalsIgnoreCase(priority))
                    .sorted(seqComprator)
                    .collect(Collectors.groupingBy(AlertUrgencyLevelRefSchema::getLandmarkId));

            AlertUrgencyLevelRefSchema tempSchema = new AlertUrgencyLevelRefSchema();
            Set<Map.Entry<Integer, List<AlertUrgencyLevelRefSchema>>> entries = groupSchema.entrySet();
            Iterator<Map.Entry<Integer, List<AlertUrgencyLevelRefSchema>>> iterator = entries.iterator();

            while (iterator.hasNext()) {
                List<Double> routePointList = new ArrayList<>();
                Map.Entry<Integer, List<AlertUrgencyLevelRefSchema>> next = iterator.next();
                List<AlertUrgencyLevelRefSchema> schemasOrdered = next.getValue();
                for (AlertUrgencyLevelRefSchema schema : schemasOrdered) {
                    routePointList.add(landMarkType.equalsIgnoreCase("E") ? schema.getLatitude() : schema.getCorriLatitude());
                    routePointList.add(landMarkType.equalsIgnoreCase("E") ? schema.getLongitude() : schema.getCorriLatitude());
                    tempSchema = schema;
                }
                if(! routePointList.isEmpty()){
                    logger.trace("Exit corridor  nodes {} for {}",routePointList,messageUUID);
                    logger.trace("Exit corridor test point {} for {}",Arrays.asList(point),messageUUID);
//                    double[][] route = routePointList.toArray(new double[0][0]);
                    double[][] route = new double[routePointList.size() / 2][routePointList.size() / 2];
                    int indexCounter=0;
                    for (int i = 0; i < routePointList.size(); i=i+2) {
                        route[indexCounter] = new double[]{routePointList.get(i), routePointList.get((i + 1) % routePointList.size())};
                        indexCounter++;
                    }
                    // TODO get the width from table
                    GeoCorridor instance = getRouteTree(tempSchema, route, messageUUID);
                    if(!instance.liesWithin(point[0], point[1])){
                        Target target = getTarget(index, tempSchema, 0);
                        logger.info("Exit corridor alert generated for alertId {} landmarkId {} alert message {} {}",
                                target.getAlert().get().getAlertid(),tempSchema.getLandmarkId(),target.getAlert().get(),messageUUID);
                        return target;

                    }
                }
            }
        }
        return Target.builder().alert(Optional.empty()).build();
    }

    private static GeoCorridor getRouteTree(AlertUrgencyLevelRefSchema schema,double[][] route,String messageUUID){
        GeoCorridor geoCorridor = exitCorridorRouteTree.get(schema.getLandmarkId());
        if(Objects.isNull(geoCorridor)){
            logger.info("Exit corridor route tree not found creating new one for landmark {} {}",schema.getLandmarkId(),messageUUID);
            geoCorridor = setGeoCorridorTree(schema, route);
        }
        // Update exiting if change occur in alert defination
        if(geoCorridor.getLastModifiedTimeStamp() != schema.getTimestamp()){
            logger.info("Exit corridor route tree alert definition updated for landmark {} {}",schema.getLandmarkId(),messageUUID);
            geoCorridor = setGeoCorridorTree(schema, route);
        }
        return geoCorridor;
    }

    private static GeoCorridor setGeoCorridorTree(AlertUrgencyLevelRefSchema schema, double[][] route) {
        GeoCorridor geoCorridor;
        geoCorridor= new GeoCorridor(route, schema.getWidth());
        geoCorridor.setLastModifiedTimeStamp(schema.getTimestamp());
        exitCorridorRouteTree.put(schema.getLandmarkId(),geoCorridor);
        return geoCorridor;
    }

    private static Target getTarget(Index index, AlertUrgencyLevelRefSchema urgency, Object valueAtAlertTime) {

        return Target.builder()
                .alert(Optional.of(Alert.builder()
                        .tripid(index.getDocument() !=null ? index.getDocument().getTripID() : "")
                        .vin(index.getVin())
                        .categoryType(urgency.getAlertCategory())
                        .type(urgency.getAlertType())
                        .alertid("" + urgency.getAlertId())
                        .alertGeneratedTime(""+ getCurrentTimeInUTC())
                        .thresholdValue("" + urgency.getThresholdValue())
                        .thresholdValueUnitType(urgency.getUnitType())
                        .valueAtAlertTime(""+valueAtAlertTime)
                        .urgencyLevelType(urgency.getUrgencyLevelType())
                        .latitude(""+index.getGpsLatitude())
                        .longitude(""+index.getGpsLongitude())
                        .build()))
                .build();
    }
}
