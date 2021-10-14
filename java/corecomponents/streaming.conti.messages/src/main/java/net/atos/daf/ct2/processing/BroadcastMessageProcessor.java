package net.atos.daf.ct2.processing;

import java.util.Properties;

import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.streaming.api.functions.co.BroadcastProcessFunction;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.node.ObjectNode;

import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.models.scheamas.VehicleStatusSchema;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.Message;
import net.atos.daf.ct2.utils.JsonMapper;

public class BroadcastMessageProcessor<U,R> extends BroadcastProcessFunction<KafkaRecord<U>, KafkaRecord<R>, KafkaRecord<U>> {
//extends KeyedBroadcastProcessFunction<String, KafkaRecord<U>, KafkaRecord<R>, KafkaRecord<U>> {

    /**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private static final Logger logger = LoggerFactory.getLogger(BroadcastMessageProcessor.class);

    private Properties properties;
    private final MapStateDescriptor<Message<U>, KafkaRecord<R>> broadcastStateDescriptor;
    
    public BroadcastMessageProcessor(Properties properties){
        this.properties = properties;
        broadcastStateDescriptor = new BroadcastState<U,R>()
                .stateInitialization(this.properties.getProperty(DAFCT2Constant.BROADCAST_NAME));
    }
    
    @Override
    public void processElement(KafkaRecord<U> value, ReadOnlyContext ctx, Collector<KafkaRecord<U>> out) throws Exception {
        logger.info("Single record from topic :: {}",value);

        String valueRecord = "UNKNOWN";
        try {
			valueRecord = JsonMapper.configuring().readTree((String) value.getValue()).get("VID").asText();
		} catch (Exception e) {
			logger.error("Issue VID is null, record {}",value);
		}
		logger.info("Record VID:  :: {}",valueRecord);

        Message<U> keyMessage = new Message<>((U) valueRecord);
        ReadOnlyBroadcastState<Message<U>, KafkaRecord<R>> broadcastStateMap = ctx.getBroadcastState(broadcastStateDescriptor);

        //logger.info("BroadcastMessageProcessor Record VID:  :: {} keyMessage :{} broadcastStateMap: {}",valueRecord, keyMessage, broadcastStateMap);
        
       
        if(broadcastStateMap.contains(keyMessage)){
            KafkaRecord<R> kafkaRecord = broadcastStateMap.get(keyMessage);
            VehicleStatusSchema vinStatusRecord = (VehicleStatusSchema)kafkaRecord.getValue();
            //logger.info("Broadcast info found for vin: {}, message: {}",keyMessage.get(), value);
            if(vinStatusRecord.getStatus().equals(DAFCT2Constant.CONNECTED_OTA_OFF) || vinStatusRecord.getStatus().equals(DAFCT2Constant.CONNECTED_OTA_ON)){
            	 JsonNode jsonNode = JsonMapper.configuring().readTree(value.getValue().toString());
                 KafkaRecord<U> updatedkafkaRecord = new KafkaRecord<U>();
                 
            	((ObjectNode) jsonNode).put("VIN", vinStatusRecord.getVin());
                ((ObjectNode) jsonNode).put("STATUS", vinStatusRecord.getStatus());
                ((ObjectNode) jsonNode).put("FuelType", vinStatusRecord.getFuelType());
                
                updatedkafkaRecord.setKey(vinStatusRecord.getVin());
                updatedkafkaRecord.setValue((U) JsonMapper.configuring().writeValueAsString(jsonNode));
                out.collect(updatedkafkaRecord);
               
            }else{
            	 logger.info(" Vehicle is not connected, ignoring vehicle data :: {}",value);
            }
            
        }else {
        	 JsonNode noMappingJsonNode = JsonMapper.configuring().readTree(value.getValue().toString());
             KafkaRecord<U> ignoredKafkaRecord = new KafkaRecord<U>();
             
            if("UNKNOWN".equals(valueRecord))
            	((ObjectNode) noMappingJsonNode).put("VID", valueRecord);
            
            ((ObjectNode) noMappingJsonNode).put("VIN", valueRecord);   
            ignoredKafkaRecord.setKey(valueRecord);
            ignoredKafkaRecord.setValue((U) JsonMapper.configuring().writeValueAsString(noMappingJsonNode));
            logger.info("VID and VIN mapping not found for message. VID and VIN:{} value: {}",valueRecord, ignoredKafkaRecord);
            out.collect(ignoredKafkaRecord);
        }
    }

    @Override
    public void processBroadcastElement(KafkaRecord<R> value, Context ctx, Collector<KafkaRecord<U>> out) throws Exception {
        logger.info("Broadcast state updated from BroadcastMessageProcessor:: {}" , value);
        ctx.getBroadcastState(broadcastStateDescriptor).put(new Message<U>((U) value.getKey()), value);
    }
    
	/*@Override
	public void processBroadcastElement(KafkaRecord<R> value,
			KeyedBroadcastProcessFunction<String, KafkaRecord<U>, KafkaRecord<R>, KafkaRecord<U>>.Context ctx,
			Collector<KafkaRecord<U>> arg2) throws Exception {
		    logger.info("Broadcast state updated from BroadcastMessageProcessor:: {}" , value);
	        ctx.getBroadcastState(broadcastStateDescriptor).put(new Message<U>((U) value.getKey()), value);
	}
	
	@Override
	public void processElement(KafkaRecord<U> value,
			KeyedBroadcastProcessFunction<String, KafkaRecord<U>, KafkaRecord<R>, KafkaRecord<U>>.ReadOnlyContext ctx,
			Collector<KafkaRecord<U>> out) throws Exception {
		logger.info("Single record from topic :: {}",value);

        String valueRecord = "UNKNOWN";
        try {
			valueRecord = JsonMapper.configuring().readTree((String) value.getValue()).get("VID").asText();
		} catch (Exception e) {
			logger.error("Issue VID is null, record {}",value);
		}
		logger.info("Record VID:  :: {}",valueRecord);

        Message<U> keyMessage = new Message<>((U) valueRecord);
        ReadOnlyBroadcastState<Message<U>, KafkaRecord<R>> broadcastStateMap = ctx.getBroadcastState(broadcastStateDescriptor);

        //logger.info("BroadcastMessageProcessor Record VID:  :: {} keyMessage :{} broadcastStateMap: {}",valueRecord, keyMessage, broadcastStateMap);
        
       
        if(broadcastStateMap.contains(keyMessage)){
            KafkaRecord<R> kafkaRecord = broadcastStateMap.get(keyMessage);
            VehicleStatusSchema vinStatusRecord = (VehicleStatusSchema)kafkaRecord.getValue();
            //logger.info("Broadcast info found for vin: {}, message: {}",keyMessage.get(), value);
            if(vinStatusRecord.getStatus().equals(DAFCT2Constant.CONNECTED_OTA_OFF) || vinStatusRecord.getStatus().equals(DAFCT2Constant.CONNECTED_OTA_ON)){
            	 JsonNode jsonNode = JsonMapper.configuring().readTree(value.getValue().toString());
                 KafkaRecord<U> updatedkafkaRecord = new KafkaRecord<U>();
                 
            	((ObjectNode) jsonNode).put("VIN", vinStatusRecord.getVin());
                ((ObjectNode) jsonNode).put("STATUS", vinStatusRecord.getStatus());
                ((ObjectNode) jsonNode).put("FuelType", vinStatusRecord.getFuelType());
                
                updatedkafkaRecord.setKey(vinStatusRecord.getVin());
                updatedkafkaRecord.setValue((U) JsonMapper.configuring().writeValueAsString(jsonNode));
                out.collect(updatedkafkaRecord);
               
            }else{
            	 logger.info(" Vehicle is not connected, ignoring vehicle data :: {}",value);
            }
            
        }else {
        	 JsonNode noMappingJsonNode = JsonMapper.configuring().readTree(value.getValue().toString());
             KafkaRecord<U> ignoredKafkaRecord = new KafkaRecord<U>();
             
            if("UNKNOWN".equals(valueRecord))
            	((ObjectNode) noMappingJsonNode).put("VID", valueRecord);
            
            ((ObjectNode) noMappingJsonNode).put("VIN", valueRecord);   
            ignoredKafkaRecord.setKey(valueRecord);
            ignoredKafkaRecord.setValue((U) JsonMapper.configuring().writeValueAsString(noMappingJsonNode));
            logger.info("VID and VIN mapping not found for message. VID and VIN:{} value: {}",valueRecord, ignoredKafkaRecord);
            out.collect(ignoredKafkaRecord);
        }
		
	}*/

}
