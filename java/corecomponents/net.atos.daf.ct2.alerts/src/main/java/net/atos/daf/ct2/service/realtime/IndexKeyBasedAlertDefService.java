package net.atos.daf.ct2.service.realtime;

import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.AlertFuelMeasurement;
import net.atos.daf.ct2.models.LandMarkDetails;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.process.config.AlertConfig;
import org.apache.flink.api.common.state.MapState;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

import static net.atos.daf.ct2.props.AlertConfigProp.*;

public class IndexKeyBasedAlertDefService extends KeyedBroadcastProcessFunction<Object, Tuple2<Index, Payload<Set<Long>>>, Payload<Object>, Index> implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(IndexKeyBasedAlertDefService.class);
    private static final long serialVersionUID = 1L;
    private Map<Object, Object> configMap;

    public IndexKeyBasedAlertDefService(Map<Object, Object> configMap){
        this.configMap=configMap;
    }
    private MapState<String, String> vehicleGeofenceSateEnteringZone;

    @Override
    public void processElement(Tuple2<Index, Payload<Set<Long>>> indexTup2, KeyedBroadcastProcessFunction<Object, Tuple2<Index, Payload<Set<Long>>>, Payload<Object>, Index>.ReadOnlyContext readOnlyContext, Collector<Index> collector) throws Exception {
        ReadOnlyBroadcastState<Long, Payload> broadcastState = readOnlyContext.getBroadcastState(THRESHOLD_CONFIG_DESCRIPTOR);
        logger.info("Fetch alert definition from cache for {}", indexTup2);
        Index f0 = indexTup2.f0;
        Payload<Set<Long>> f1 = indexTup2.f1;
        Set<Long> alertIds = f1.getData().get();
        Map<String, Object> functionThresh = new HashMap<>();
        List<AlertUrgencyLevelRefSchema> hoursOfServiceAlertDef = new ArrayList<>();
        List<AlertUrgencyLevelRefSchema> excessiveAverageSpeedAlertDef = new ArrayList<>();
        List<AlertUrgencyLevelRefSchema> excessiveUnderUtilizationInHoursAlertDef = new ArrayList<>();
        List<AlertUrgencyLevelRefSchema> fuelIncreaseDuringStopFunAlertDef = new ArrayList<>();
        List<AlertUrgencyLevelRefSchema> fuelDecreaseDuringStopFunAlertDef = new ArrayList<>();
        List<AlertUrgencyLevelRefSchema> fuelDuringTripFunAlertDef = new ArrayList<>();
        List<AlertUrgencyLevelRefSchema> excessiveIdlingAlertDef = new ArrayList<>();
        List<AlertUrgencyLevelRefSchema> enteringExitingZoneAlertDef = new ArrayList<>();

        for (Long alertId : alertIds) {
            if (broadcastState.contains(alertId)) {
                List<AlertUrgencyLevelRefSchema> thresholdSet = (List<AlertUrgencyLevelRefSchema>) broadcastState.get(alertId).getData().get();
                for (AlertUrgencyLevelRefSchema schema : thresholdSet) {
                    if (schema.getAlertCategory().equalsIgnoreCase("L") && schema.getAlertType().equalsIgnoreCase("S")) {
                        hoursOfServiceAlertDef.add(schema);
                    }
                    if (schema.getAlertCategory().equalsIgnoreCase("F") && schema.getAlertType().equalsIgnoreCase("A")) {
                    	excessiveAverageSpeedAlertDef.add(schema);
                    }
                    if (schema.getAlertCategory().equalsIgnoreCase("L") && schema.getAlertType().equalsIgnoreCase("H")) {
                        excessiveUnderUtilizationInHoursAlertDef.add(schema);
                    }

                    if (schema.getAlertCategory().equalsIgnoreCase("F") && schema.getAlertType().equalsIgnoreCase("I")) {
                		excessiveIdlingAlertDef.add(schema);
                	}
                    if (schema.getAlertCategory().equalsIgnoreCase("F") && schema.getAlertType().equalsIgnoreCase("P")) {
                    	fuelIncreaseDuringStopFunAlertDef.add(schema);
                    }
                    if (schema.getAlertCategory().equalsIgnoreCase("F") && schema.getAlertType().equalsIgnoreCase("L")) {
                    	fuelDecreaseDuringStopFunAlertDef.add(schema);
                    }
                    if (schema.getAlertCategory().equalsIgnoreCase("F") && schema.getAlertType().equalsIgnoreCase("T")) {
                    	fuelDuringTripFunAlertDef.add(schema);
                    }
                    if (schema.getAlertCategory().equalsIgnoreCase("L") && schema.getAlertType().equalsIgnoreCase("N")) {
                        enteringExitingZoneAlertDef.add(schema);
                    }
                }
            }
        }
        logger.info("Alert definition from cache for vin :{} alertDef {} {}", f0.getVin(), hoursOfServiceAlertDef,String.format(INCOMING_MESSAGE_UUID,f0.getJobName()));
        functionThresh.put("hoursOfService", hoursOfServiceAlertDef);
        functionThresh.put("excessiveAverageSpeed", excessiveAverageSpeedAlertDef);
        functionThresh.put("excessiveUnderUtilizationInHours", excessiveUnderUtilizationInHoursAlertDef);
        functionThresh.put("excessiveIdling", excessiveIdlingAlertDef);
        functionThresh.put("fuelIncreaseDuringStopFunAlertDef", fuelIncreaseDuringStopFunAlertDef);
        functionThresh.put("fuelDecreaseDuringStopFunAlertDef", fuelDecreaseDuringStopFunAlertDef);
        functionThresh.put("fuelDuringTripFunAlertDef", fuelDuringTripFunAlertDef);
        functionThresh.put("enteringAndExitingZoneFun", enteringExitingZoneAlertDef);
        functionThresh.put("enteringAndExitingZoneVehicleState", vehicleGeofenceSateEnteringZone);

        // Get vehicle sate for geofence
//        if(vehicleGeofenceSate.contains(f0.getVin())){
//            f0.setJobName(vehicleGeofenceSate.get(f0.getVin()));
//        }else{
//           vehicleGeofenceSate.put(f0.getVin(),"false");
//        }
        AlertConfig
                .buildMessage(f0, configMap, functionThresh)
                .process()
                .getAlert()
                .ifPresent(
                        alerts -> {
                            alerts.stream()
                                    .forEach(alert -> {
                                        /*if(alert.getType().equalsIgnoreCase("N") && alert.getCategoryType().equalsIgnoreCase("L")){
                                            // change the state of vehicle for entering zone
                                            try {
                                                if(vehicleGeofenceSate.contains(f0.getVin())){
                                                    vehicleGeofenceSate.put(f0.getVin(),alert.getValueAtAlertTime());
                                                    if(alert.getValueAtAlertTime().equalsIgnoreCase("true")){
                                                        logger.info("Alert has benn raised for entering zone :{} {}",alert, String.format(INCOMING_MESSAGE_UUID,f0.getJobName()));
                                                        alert.setValueAtAlertTime(""+0.0);
                                                        readOnlyContext.output(OUTPUT_TAG, alert);
                                                    }
                                                }
                                            } catch (Exception e) {
                                                logger.error("Error while checking for geofencing vehicle:: {}  {}",e, String.format(INCOMING_MESSAGE_UUID,f0.getJobName()));
                                            }
                                        }
                                        //Collect alert as it is
                                        else{
                                            readOnlyContext.output(OUTPUT_TAG, alert);
                                        }*/

                                        readOnlyContext.output(OUTPUT_TAG, alert);

                                    });
                        }
                );
        logger.info("Alert process for  {} check those alerts {} {}", f0, functionThresh, String.format(INCOMING_MESSAGE_UUID,f0.getJobName()));
        collector.collect(f0);
    }

    @Override
    public void processBroadcastElement(Payload<Object> payload, KeyedBroadcastProcessFunction<Object, Tuple2<Index, Payload<Set<Long>>>, Payload<Object>, Index>.Context context, Collector<Index> collector) throws Exception {
        /**
         * Update threshold alert definition
         */
        CacheService.updateAlertDefinationCache(payload, context);
    }
    @Override
    public void open(org.apache.flink.configuration.Configuration config) {
        MapStateDescriptor<String, String> descriptor = new MapStateDescriptor("vehicleGeofenceSateEnteringZone",
                TypeInformation.of(String.class),TypeInformation.of(String.class));
        vehicleGeofenceSateEnteringZone = getRuntimeContext().getMapState(descriptor);
    }
}
