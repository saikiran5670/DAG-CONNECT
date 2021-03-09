package net.atos.daf.ct2.processing;

import static io.debezium.data.Envelope.FieldName.AFTER;
import static io.debezium.data.Envelope.FieldName.OPERATION;

import java.util.Properties;
import java.util.UUID;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

import org.apache.kafka.connect.data.Struct;
import org.apache.kafka.connect.source.SourceRecord;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import io.debezium.config.Configuration;
import io.debezium.data.VariableScaleDecimal;
import io.debezium.embedded.EmbeddedEngine;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.bo.TripStatistic;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.utils.Operation;
public class EgressCDCLayer implements Runnable {

	  private static final Logger log =
	      LogManager.getLogger(net.atos.daf.ct2.processing.MessageProcessing.class);
	  private final Configuration configuration;
	  private final Producer producer;
	  private final Properties properties;
	  private EmbeddedEngine embeddedEngine;
	  private ObjectMapper mapper;

	  public EgressCDCLayer(Configuration configuration, Properties properties) throws DAFCT2Exception {

	    this.configuration = configuration;
	    this.properties = properties;

	    this.producer = new Producer();
	    this.producer.createConnection(properties);
	    
	    // Creating the ObjectMapper object
	 	this.mapper = new ObjectMapper();
	 		
	  }

	  @Override
	  public void run() {

	    try {
	    	
	    	if ("tripEgress".equals(properties.getProperty(DAFCT2Constant.POSTGRE_CDC_NAME))) {
	    		this.embeddedEngine =
	  		          EmbeddedEngine.create().using(this.configuration).notifying(this::handleTripEvent).build();
	    	}else{
	    		this.embeddedEngine =
	  		          EmbeddedEngine.create().using(this.configuration).notifying(this::handleEvent).build();
	    	}
	    		

	      ExecutorService executorService = Executors.newSingleThreadExecutor();
	      executorService.execute(this.embeddedEngine);

	      Runtime.getRuntime()
	          .addShutdownHook(
	              new Thread(
	                  () -> {
	                    log.info("Requesting embedded engine to shut down");
	                    this.embeddedEngine.stop();
	                    // this.producer.closePublisher();
	                  }));

	      // the submitted task keeps running, only no more new ones can be added
	      executorService.shutdown();

	      awaitTermination(executorService);

	      log.info("Engine terminated");

	    } catch (Exception e) {
	      log.error("Unable to process Postgre CDC ", e.getMessage());
	    }
	  }

	  private void awaitTermination(ExecutorService executorService) {

	    try {
	      while (!executorService.awaitTermination(10, TimeUnit.SECONDS)) {
	        log.info("Waiting another 10 seconds for the embedded engine to complete");
	      }

	    } catch (InterruptedException e) {
	      Thread.currentThread().interrupt();
	    }
	  }

	  private void handleEvent(SourceRecord sourceRecord) {

	    Struct sourceRecordValue = (Struct) sourceRecord.value();

	    if (sourceRecordValue != null) {
	      try {
	        Operation operation = Operation.forCode((String) sourceRecordValue.get(OPERATION));

	        // Only if this is a transactional operation.
	        // if (operation != Operation.READ) {
	        Struct struct = (Struct) sourceRecordValue.get(AFTER);
	       
	        if (struct != null) {
	          this.producer.publishMessage(
	              UUID.randomUUID().toString(), struct.toString(), this.properties);
	          System.out.println("Message:"+struct.toString());
	        }

	      } catch (Exception e) {
	        // log.warn(e.getMessage());
	    	  log.error(e);
	      }
	    }
	  }
	  
	  private void handleTripEvent(SourceRecord sourceRecord) {

		    Struct sourceRecordValue = (Struct) sourceRecord.value();

		    if (sourceRecordValue != null) {
		      try {
		        Operation operation = Operation.forCode((String) sourceRecordValue.get(OPERATION));

		        // Only if this is a transactional operation.
		        // if (operation != Operation.READ) {
		        Struct struct = (Struct) sourceRecordValue.get(AFTER);
		       
		        if (struct != null) {
		        	String tripJsonStr = populateTripStats(struct);
		        	if(tripJsonStr != null){
		        		this.producer.publishMessage(
					              UUID.randomUUID().toString(), tripJsonStr, this.properties);
		        	}
		          
		          System.out.println("tripJsonStr :: "+tripJsonStr);
		       }

		      } catch (Exception e) {
		        // log.warn(e.getMessage());
		    	  log.error("Issue while egress of vehicle trip data :: "+e);
		      }
		    }
		  }
	  
   private String populateTripStats(Struct value) throws JsonProcessingException
	{
		TripStatistic trip = new TripStatistic();
		
		if (value.get("trip_id") != null)
			trip.setTripId(value.get("trip_id").toString());

		if (value.get("vin") != null)
			trip.setVin(value.get("vin").toString());
		
		if (value.get("start_time_stamp") != null)
			trip.setStartDateTime(value.getInt64("start_time_stamp"));
		
		if (value.get("end_time_stamp") != null)
			trip.setEndDateTime(value.getInt64("end_time_stamp"));
		
		if (value.get("veh_message_distance") != null)
			trip.setGpsTripDist(value.getInt32("veh_message_distance"));
		
		if (value.get("etl_gps_distance") != null)
			trip.setTripCalDist(value.getInt64("etl_gps_distance"));
		
		if (value.get("idle_duration") != null)
			trip.setVIdleDuration(value.getInt32("idle_duration"));
		
		if (value.get("average_speed") != null)
			trip.setTripCalAvgSpeed(VariableScaleDecimal.toLogical((Struct) value.get("average_speed")).getDecimalValue().get());
		
		if (value.get("average_weight") != null)
			trip.setVGrossWeightCombination(VariableScaleDecimal.toLogical((Struct) value.get("average_weight")).getDecimalValue().get());
		
		if (value.get("start_odometer") != null)
			trip.setGpsStartVehDist(value.getInt64("start_odometer"));
		
		if (value.get("last_odometer") != null)
			trip.setGpsStopVehDist(value.getInt64("last_odometer"));
		
		if (value.get("start_position_lattitude") != null)
			trip.setGpsStartLatitude(value.getFloat64("start_position_lattitude"));
		
		if (value.get("start_position_longitude") != null)
			trip.setGpsStartLongitude(value.getFloat64("start_position_longitude"));
		
		if (value.get("end_position_lattitude") != null)
			trip.setGpsEndLatitude(value.getFloat64("end_position_lattitude"));
		
		if (value.get("end_position_longitude") != null)
			trip.setGpsEndLongitude(value.getFloat64("end_position_longitude"));
		
		if (value.get("veh_message_fuel_consumed") != null)
			trip.setVUsedFuel(value.getInt32("veh_message_fuel_consumed"));
		
		if (value.get("etl_gps_fuel_consumed") != null)
			trip.setTripCalUsedFuel(value.getInt64("etl_gps_fuel_consumed"));
		
		if (value.get("veh_message_driving_time") != null)
			trip.setVTripMotionDuration(value.getInt32("veh_message_driving_time"));
		
		if (value.get("etl_gps_driving_time") != null)
			trip.setTripCalDrivingTm(value.getInt32("etl_gps_driving_time"));
		
		if (value.get("message_received_timestamp") != null)
			trip.setReceivedTimestamp(value.getInt64("message_received_timestamp"));
		
	/*	if (value.get("message_inserted_into_kafka_timestamp") != null)
			trip.set(value.getInt64("message_inserted_into_kafka_timestamp"));*/
		
		/*if (value.get("message_inserted_into_hbase_timestamp") != null)
			trip.setDriver2Id(value.get("message_inserted_into_hbase_timestamp").toString());*/
		
		if (value.get("message_processed_by_etl_process_timestamp") != null)
			trip.setEtlProcessingTS(value.getInt64("message_processed_by_etl_process_timestamp"));
		
		if (value.get("co2_emission") != null)
			trip.setTripCalC02Emission(VariableScaleDecimal.toLogical((Struct) value.get("co2_emission")).getDecimalValue().get());
			//trip.setTripCalC02Emission(value.getFloat64("co2_emission"));
		
		if (value.get("fuel_consumption") != null)
			trip.setTripCalFuelConsumption(VariableScaleDecimal.toLogical((Struct) value.get("fuel_consumption")).getDecimalValue().get());
			//trip.setTripCalFuelConsumption(value.getFloat64("fuel_consumption"));
		
		if (value.get("max_speed") != null)
			trip.setVTachographSpeed(VariableScaleDecimal.toLogical((Struct) value.get("max_speed")).getDecimalValue().get());
			//trip.setVTachographSpeed(value.getFloat64("max_speed"));
		
		if (value.get("average_gross_weight_comb") != null)
			trip.setTripCalAvgGrossWtComb(VariableScaleDecimal.toLogical((Struct) value.get("average_gross_weight_comb")).getDecimalValue().get());
		
		if (value.get("pto_duration") != null)
			trip.setTripCalPtoDuration(VariableScaleDecimal.toLogical((Struct) value.get("pto_duration")).getDecimalValue().get());
			//trip.setTripCalPtoDuration(value.getFloat64("pto_duration"));
		
		if (value.get("harsh_brake_duration") != null)
			trip.setTriCalHarshBrakeDuration(VariableScaleDecimal.toLogical((Struct) value.get("harsh_brake_duration")).getDecimalValue().get());
			//trip.setTriCalHarshBrakeDuration(value.getFloat64("harsh_brake_duration"));
		
		if (value.get("heavy_throttle_duration") != null)
			trip.setTripCalHeavyThrottleDuration(VariableScaleDecimal.toLogical((Struct) value.get("heavy_throttle_duration")).getDecimalValue().get());
			//trip.setTripCalHeavyThrottleDuration(value.getFloat64("heavy_throttle_duration"));
		
		if (value.get("cruise_control_distance_30_50") != null)
			trip.setTripCalCrsCntrlDistBelow50(VariableScaleDecimal.toLogical((Struct) value.get("cruise_control_distance_30_50")).getDecimalValue().get());
			//trip.setTripCalCrsCntrlDistBelow50(value.getFloat64("cruise_control_distance_30_50"));
		
		if (value.get("cruise_control_distance_50_75") != null)
			trip.setTripCalCrsCntrlDistAbv50(VariableScaleDecimal.toLogical((Struct) value.get("cruise_control_distance_50_75")).getDecimalValue().get());
			//trip.setTripCalCrsCntrlDistAbv50(value.getFloat64("cruise_control_distance_50_75"));
		
		if (value.get("cruise_control_distance_more_than_75") != null)
			trip.setTripCalCrsCntrlDistAbv75(VariableScaleDecimal.toLogical((Struct) value.get("cruise_control_distance_more_than_75")).getDecimalValue().get());
			//trip.setTripCalCrsCntrlDistAbv75(value.getFloat64("cruise_control_distance_more_than_75"));
		
		if (value.get("average_traffic_classification") != null)
			trip.setTripCalAvgTrafficClsfn(value.getString("average_traffic_classification"));
			//trip.setTripCalAvgTrafficClsfn(VariableScaleDecimal.toLogical((Struct) value.get("average_traffic_classification")).getDecimalValue().get());
			
		
		if (value.get("cc_fuel_consumption") != null)
			trip.setTripCalCCFuelConsumption(VariableScaleDecimal.toLogical((Struct) value.get("cc_fuel_consumption")).getDecimalValue().get());
			//trip.setTripCalCCFuelConsumption(value.getFloat64("cc_fuel_consumption"));
		
		if (value.get("v_cruise_control_fuel_consumed_for_cc_fuel_consumption") != null)
			trip.setVCruiseControlFuelConsumed(value.getInt32("v_cruise_control_fuel_consumed_for_cc_fuel_consumption"));
		
		if (value.get("v_cruise_control_dist_for_cc_fuel_consumption") != null)
			trip.setVCruiseControlDist(value.getInt32("v_cruise_control_dist_for_cc_fuel_consumption"));
		
		if (value.get("fuel_consumption_cc_non_active") != null)
			trip.setTripCalfuelNonActiveCnsmpt(VariableScaleDecimal.toLogical((Struct) value.get("fuel_consumption_cc_non_active")).getDecimalValue().get());
			//trip.setTripCalfuelNonActiveCnsmpt(value.getFloat64("fuel_consumption_cc_non_active"));
		
		if (value.get("idling_consumption") != null)
			trip.setVIdleFuelConsumed(value.getInt32("idling_consumption"));
		
		/*if (value.get("dpa_score") != null)
			trip.setDriver2Id(value.get("dpa_score").toString());
		if (value.get("endurance_brake") != null)
			trip.setDriver2Id(value.get("endurance_brake").toString());
		if (value.get("coasting") != null)
			trip.setDriver2Id(value.get("coasting").toString());
		if (value.get("eco_rolling") != null)
			trip.setDriver2Id(value.get("eco_rolling").toString());*/
		
		if (value.get("driver1_id") != null)
			trip.setDriverId(value.get("driver1_id").toString());
		
		if (value.get("driver2_id") != null)
			trip.setDriver2Id(value.get("driver2_id").toString());
		
		if (value.get("etl_gps_trip_time") != null)
			trip.setTripCalGpsVehTime(value.getInt32("etl_gps_trip_time"));

		// Creating the ObjectMapper object
		//ObjectMapper mapper = new ObjectMapper();
		// Converting the Object to JSONString
		return mapper.writeValueAsString(trip);
		
	}
}
