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

public class BroadcastMessageProcessor extends KeyedBroadcastProcessFunction<String, KafkaRecord<Tuple3<String, String, Object>>, KafkaRecord<VehicleStatusSchema>, KafkaRecord<Tuple3<String, String, Object>>> {

    /**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private static final Logger logger = LoggerFactory.getLogger(BroadcastMessageProcessor.class);

    private Properties properties;
    private final MapStateDescriptor<Message<String>, KafkaRecord<VehicleStatusSchema>> broadcastStateDescriptor;
    
    public BroadcastMessageProcessor(Properties properties){
        this.properties = properties;
        broadcastStateDescriptor = new BroadcastState<String,VehicleStatusSchema>()
                .stateInitialization(this.properties.getProperty(DAFCT2Constant.BROADCAST_NAME));
    }

	@Override
	public void processElement(KafkaRecord<Tuple3<String, String, Object>> inputRec,
							   KeyedBroadcastProcessFunction<String, KafkaRecord<Tuple3<String, String, Object>>,
									   KafkaRecord<VehicleStatusSchema>,
									   KafkaRecord<Tuple3<String, String, Object>>>.ReadOnlyContext ctx,
							   Collector<KafkaRecord<Tuple3<String, String, Object>>> out) throws Exception {
		//logger.info("Single record from processBroadcastElement :: {}",inputRec);

		Message<String> msgVid = new Message<>((String) ctx.getCurrentKey());
		ReadOnlyBroadcastState<Message<String>, KafkaRecord<VehicleStatusSchema>> broadcastStateMap = ctx.getBroadcastState(broadcastStateDescriptor);

		if(broadcastStateMap.contains(msgVid)){
			KafkaRecord<VehicleStatusSchema> kafkaRecord = broadcastStateMap.get(msgVid);
			VehicleStatusSchema vinStatusRecord = (VehicleStatusSchema)kafkaRecord.getValue();
			if(vinStatusRecord.getStatus().equals(DAFCT2Constant.CONNECTED_OTA_OFF) || vinStatusRecord.getStatus().equals(DAFCT2Constant.CONNECTED_OTA_ON)){
				//JsonNode jsonNode = JsonMapper.configuring().readTree(value.getValue().toString());
				Tuple3 value = (Tuple3)inputRec.getValue();
				inputRec.setKey(vinStatusRecord.getVin());
				
				if(value.f2 instanceof Index){
					Index indx = ((Index)value.f2);
					indx.setVin(vinStatusRecord.getVin());
					indx.setFuelType(vinStatusRecord.getFuelType());
					Tuple3<String, String, Object> of = Tuple3.of(String.valueOf(value.f0), String.valueOf(value.f1), indx);
					inputRec.setValue(of);
				}
				if(value.f2 instanceof Monitor){
					Monitor monitor = ((Monitor)value.f2);
					monitor.setVin(vinStatusRecord.getVin());
					monitor.setFuelType(vinStatusRecord.getFuelType());
					Tuple3<String, String, Object> of = Tuple3.of(String.valueOf(value.f0), String.valueOf(value.f1), monitor);
					inputRec.setValue(of);
				}
				if(value.f2 instanceof Status){
					Status status = ((Status)value.f2);
					status.setVin(vinStatusRecord.getVin());
					status.setFuelType(vinStatusRecord.getFuelType());
					Tuple3<String, String, Object> of = Tuple3.of(String.valueOf(value.f0), String.valueOf(value.f1), status);
					inputRec.setValue(of);
				}

				logger.info("VID and VIN mapping found for message. VID:{} value: {}",ctx.getCurrentKey(), inputRec);
				out.collect(inputRec);

			}else{
				logger.info("Vehicle is not connected, ignoring vehicle data :: {}",inputRec);
			}

		}else {
			Tuple3<String, String, Object> value = (Tuple3<String, String, Object>)inputRec.getValue();
			inputRec.setKey(ctx.getCurrentKey());
			 
			if(value.f2 instanceof Index){
				Index indx = ((Index)value.f2);
				if("UNKNOWN".equals(ctx.getCurrentKey()))
					indx.setVid(ctx.getCurrentKey());

				indx.setVin(ctx.getCurrentKey());
				Tuple3<String, String, Object> of = Tuple3.of(String.valueOf(value.f0), String.valueOf(value.f1), indx);
				inputRec.setValue(of);
			}
			if(value.f2 instanceof Monitor){
				Monitor monitor = ((Monitor)value.f2);
				if("UNKNOWN".equals(ctx.getCurrentKey()))
					monitor.setVid(ctx.getCurrentKey());

				monitor.setVin(ctx.getCurrentKey());
				Tuple3<String, String, Object> of = Tuple3.of(String.valueOf(value.f0), String.valueOf(value.f1), monitor);
				inputRec.setValue(of);
			}
			if(value.f2 instanceof Status){
				Status status = ((Status)value.f2);

				if("UNKNOWN".equals(ctx.getCurrentKey()))
					status.setVid(ctx.getCurrentKey());

				status.setVin(ctx.getCurrentKey());
				Tuple3<String, String, Object> of = Tuple3.of(String.valueOf(value.f0), String.valueOf(value.f1), status);
				inputRec.setValue(of);
			}

			logger.info("VID and VIN mapping not found for message. VID and VIN:{} value: {}",ctx.getCurrentKey(), inputRec);
			out.collect(inputRec);
		}
	}

	@Override
	public void processBroadcastElement(KafkaRecord<VehicleStatusSchema> vehicleStatusSchemaKafkaRecord,
										KeyedBroadcastProcessFunction<String, KafkaRecord<Tuple3<String, String, Object>>,
			KafkaRecord<VehicleStatusSchema>, KafkaRecord<Tuple3<String, String, Object>>>.Context context,
										Collector<KafkaRecord<Tuple3<String, String, Object>>> collector) throws Exception {
		logger.info("Broadcast state updated from BroadcastMessageProcessor:: {}" , vehicleStatusSchemaKafkaRecord);
		context.getBroadcastState(broadcastStateDescriptor).put(new Message<String>( vehicleStatusSchemaKafkaRecord.getKey()), vehicleStatusSchemaKafkaRecord);
	}
}
