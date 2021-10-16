package net.atos.daf.ct2.processing;

import java.util.Properties;

import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.java.tuple.Tuple3;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.models.scheamas.VehicleStatusSchema;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.Message;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;

public class BroadcastMessageProcessor<U,R> extends KeyedBroadcastProcessFunction<String, KafkaRecord<U>, KafkaRecord<R>, KafkaRecord<U>> {

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
	public void processBroadcastElement(KafkaRecord<R> value,
			KeyedBroadcastProcessFunction<String, KafkaRecord<U>, KafkaRecord<R>, KafkaRecord<U>>.Context ctx,
			Collector<KafkaRecord<U>> arg2) throws Exception {
		    logger.info("Broadcast state updated from BroadcastMessageProcessor:: {}" , value);
	        ctx.getBroadcastState(broadcastStateDescriptor).put(new Message<U>((U) value.getKey()), value);
	}
	
	@Override
	public void processElement(KafkaRecord<U> inputRec,
			KeyedBroadcastProcessFunction<String, KafkaRecord<U>, KafkaRecord<R>, KafkaRecord<U>>.ReadOnlyContext ctx,
			Collector<KafkaRecord<U>> out) throws Exception {
		logger.info("Single record from processBroadcastElement :: {}",inputRec);

        Message<U> msgVid = new Message<>((U) ctx.getCurrentKey());
        ReadOnlyBroadcastState<Message<U>, KafkaRecord<R>> broadcastStateMap = ctx.getBroadcastState(broadcastStateDescriptor);
       
        if(broadcastStateMap.contains(msgVid)){
            KafkaRecord<R> kafkaRecord = broadcastStateMap.get(msgVid);
            VehicleStatusSchema vinStatusRecord = (VehicleStatusSchema)kafkaRecord.getValue();
            if(vinStatusRecord.getStatus().equals(DAFCT2Constant.CONNECTED_OTA_OFF) || vinStatusRecord.getStatus().equals(DAFCT2Constant.CONNECTED_OTA_ON)){
            	 //JsonNode jsonNode = JsonMapper.configuring().readTree(value.getValue().toString());
            	Tuple3<String, String, Object> value = (Tuple3<String, String, Object>)inputRec.getValue();
                /* KafkaRecord<U> updatedkafkaRecord = new KafkaRecord<U>();
                 
            	((ObjectNode) jsonNode).put("VIN", vinStatusRecord.getVin());
                ((ObjectNode) jsonNode).put("STATUS", vinStatusRecord.getStatus());
                ((ObjectNode) jsonNode).put("FuelType", vinStatusRecord.getFuelType());
                
                
                
                updatedkafkaRecord.setKey(vinStatusRecord.getVin());
                updatedkafkaRecord.setValue((U) JsonMapper.configuring().writeValueAsString(jsonNode));*/
            	if(value.f2 instanceof Index){
            		Index indx = ((Index)value.f2);
            		indx.setVin(vinStatusRecord.getVin());
            		indx.setFuelType(vinStatusRecord.getFuelType());
            		inputRec.setValue((U) Tuple3.of(value.f0, value.f1, indx));
            	}
            	if(value.f2 instanceof Monitor){
            		Monitor monitor = ((Monitor)value.f2);
            		monitor.setVin(vinStatusRecord.getVin());
            		monitor.setFuelType(vinStatusRecord.getFuelType());
            		inputRec.setValue((U) Tuple3.of(value.f0, value.f1, monitor));
            	}
            	if(value.f2 instanceof Status){
            		Status status = ((Status)value.f2);
            		status.setVin(vinStatusRecord.getVin());
            		status.setFuelType(vinStatusRecord.getFuelType());
            		inputRec.setValue((U) Tuple3.of(value.f0, value.f1, status));
            	}
            		
                logger.info("VID and VIN mapping found for message. VID:{} value: {}",ctx.getCurrentKey(), inputRec);
                out.collect(inputRec);
               
            }else{
            	 logger.info("Vehicle is not connected, ignoring vehicle data :: {}",inputRec);
            }
            
        }else {
        	 Tuple3<String, String, Object> value = (Tuple3<String, String, Object>)inputRec.getValue();
             
             if(value.f2 instanceof Index){
         		Index indx = ((Index)value.f2);
         		 if("UNKNOWN".equals(ctx.getCurrentKey()))
                 	indx.setVid(ctx.getCurrentKey());
         		
         		 indx.setVin(ctx.getCurrentKey());
         		 
         		inputRec.setValue((U) Tuple3.of(value.f0, value.f1, indx));
         	}
         	if(value.f2 instanceof Monitor){
         		Monitor monitor = ((Monitor)value.f2);
         		 if("UNKNOWN".equals(ctx.getCurrentKey()))
         			monitor.setVid(ctx.getCurrentKey());
          		
         		monitor.setVin(ctx.getCurrentKey());
         		inputRec.setValue((U) Tuple3.of(value.f0, value.f1, monitor));
         	}
         	if(value.f2 instanceof Status){
         		Status status = ((Status)value.f2);
         		
         		if("UNKNOWN".equals(ctx.getCurrentKey()))
         			status.setVid(ctx.getCurrentKey());
          		
         		status.setVin(ctx.getCurrentKey());
         		inputRec.setValue((U) Tuple3.of(value.f0, value.f1, status));
         	}
                     
            logger.info("VID and VIN mapping not found for message. VID and VIN:{} value: {}",ctx.getCurrentKey(), inputRec);
            out.collect(inputRec);
        }
		
	}

}
