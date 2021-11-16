package net.atos.daf.ct2.processing;

import java.util.Objects;
import java.util.Properties;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.java.tuple.Tuple3;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.models.scheamas.VehicleStatusSchema;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.Message;
import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;
import net.atos.daf.ct2.utils.JsonMapper;

public class MessageProcessing<U,R, T> {

    private static final Logger logger = LoggerFactory.getLogger(MessageProcessing.class);

  
  public void consumeKeyedContiMessage(
	      DataStream<KafkaRecord<Tuple3<String, String, Object>>> messageDataStream,
	      Properties properties,
	     BroadcastStream<KafkaRecord<VehicleStatusSchema>> broadcastStream) {
	   
	   SingleOutputStreamOperator<KafkaRecord<Tuple3<String, String, Object>>> contiBrdCastStream = messageDataStream
	        .connect(broadcastStream)
	        .process(new BroadcastMessageProcessor(properties))
				.name("Broadcast processing ");
	       // .keyBy(rec -> rec.getKey())
			   
	   egressContiMessage(
			   contiBrdCastStream,
			   properties.getProperty(DAFCT2Constant.MONITOR_TRANSID),
			   properties.getProperty(DAFCT2Constant.SINK_MONITOR_TOPIC_NAME),
			   properties);
	   
	   egressContiMessage(
			   contiBrdCastStream,
			     properties.getProperty(DAFCT2Constant.INDEX_TRANSID),
			     properties.getProperty(DAFCT2Constant.SINK_INDEX_TOPIC_NAME),
			     properties);
	   
	   egressContiMessage(
			   contiBrdCastStream,
			   properties.getProperty(DAFCT2Constant.STATUS_TRANSID),
			     properties.getProperty(DAFCT2Constant.SINK_STATUS_TOPIC_NAME),
			     properties);
	  
	   egressContiRawMessage(
			   contiBrdCastStream,
			   properties.getProperty(DAFCT2Constant.EGRESS_RAW_DATA_TOPIC_NAME),
			   properties);
	 
  }
	
  public void egressContiMessage(
		  SingleOutputStreamOperator<KafkaRecord<Tuple3<String, String, Object>>> contiStreamToEgress,
	      String messageType,
	      String sinkTopicName,
	      Properties properties) {
	  
	  contiStreamToEgress
	  .filter(rec ->  messageType.equals(rec.getValue().f1))
	  .map( new MapFunction<KafkaRecord<Tuple3<String, String, Object>>, KafkaRecord<T>>() {
	              /**
					 * 
					 */
					private static final long serialVersionUID = 1L;

				@Override
	              public KafkaRecord<T> map(KafkaRecord<Tuple3<String, String, Object>> value) throws Exception {
	               logger.debug("KafkaRecord before converting to object : {}", value);
	                try{
	                
	                     KafkaRecord<T> kafkaRecord = new KafkaRecord<T>();
	                     //kafkaRecord.setKey(key);
	                     kafkaRecord.setKey(value.getKey());
	                     kafkaRecord.setValue((T)value.getValue().f2);
	                     logger.info("Final KafkaRecord to kafka topic: {} record : {}",sinkTopicName , kafkaRecord);
	                     
	                     return kafkaRecord;
	                }catch(Exception e){
	                	logger.error("Issue while Json convertion to Object : {} record : {}",sinkTopicName , value);
	                	logger.error("Issue while Json convertion to Object : {} ",e.getMessage());
	                	return null;
	                }
	            
	              }
	            }).name("Map Kafka Record " + messageType)
	        .filter( rec -> Objects.nonNull(rec)).name("Filter Null records")
	        .keyBy(rec -> rec.getKey())
	        .addSink(
	            new FlinkKafkaProducer<KafkaRecord<T>>(
	                sinkTopicName,
	                new KafkaMessageSerializeSchema<T>(sinkTopicName),
	                properties,
	                FlinkKafkaProducer.Semantic.AT_LEAST_ONCE)).name(" Topic : "+sinkTopicName);
  }
  
  public void egressContiRawMessage(
		  SingleOutputStreamOperator<KafkaRecord<Tuple3<String, String, Object>>> contiRawStreamToEgress,
	      String sinkTopicName,
	      Properties properties) {
	  
	  if(DAFCT2Constant.ATOS_STANDARD.equals(properties.getProperty(DAFCT2Constant.EGRESS_RAW_DATA_FORMAT))){
			 logger.info( "Raw data to egress topic : {}",properties.getProperty(DAFCT2Constant.EGRESS_RAW_DATA_TOPIC_NAME));
			 
			 contiRawStreamToEgress
			 .map(new MapFunction<KafkaRecord<Tuple3<String, String, Object>>, KafkaRecord<String>>(){
				 /**
				 * 
				 */
				private static final long serialVersionUID = 1L;
				ObjectMapper mapper = new ObjectMapper();
				@Override
				public KafkaRecord<String> map(KafkaRecord<Tuple3<String, String, Object>> value) throws Exception {
					KafkaRecord<String> kafkaRec = new KafkaRecord<>();
					kafkaRec.setKey(value.getKey());
					
					try {
						kafkaRec.setValue(mapper.writeValueAsString(value.getValue().f2));
					} catch (JsonProcessingException e) {
						logger.error("Issue while converting Raw Object to String:{} " ,e.getMessage());
					}
					logger.debug("Raw Json data structure ::{} ",kafkaRec);

					return kafkaRec;
				}
				 
			 }).filter(rec -> Objects.nonNull(rec.getValue()))
			 .addSink(
			            new FlinkKafkaProducer<KafkaRecord<String>>(
			            	sinkTopicName,
			                new KafkaMessageSerializeSchema<String>(sinkTopicName),
			                properties,
			                FlinkKafkaProducer.Semantic.AT_LEAST_ONCE)).name(" Topic : "+sinkTopicName);
		  }

  }
  
 
  public void contiKeyedMessageForHistorical(
	      DataStream<KafkaRecord<Tuple3<String, String, Object>>> messageDataStream,
	      Properties properties,
		  BroadcastStream<KafkaRecord<R>> broadcastStream) {

		messageDataStream.connect(broadcastStream)
				.process(new KeyedBroadcastProcessFunction<String, KafkaRecord<Tuple3<String, String, Object>>, KafkaRecord<R>, KafkaRecord<String>>() {
					/**
					 * 
					 */
					private static final long serialVersionUID = 1L;
					private final MapStateDescriptor<Message<U>, KafkaRecord<R>> broadcastStateDescriptor = new BroadcastState<U,R>()
							.stateInitialization(properties.getProperty(DAFCT2Constant.BROADCAST_NAME));

					@Override
					public void processElement(KafkaRecord<Tuple3<String, String, Object>> inputRec,
							KeyedBroadcastProcessFunction<String, KafkaRecord<Tuple3<String, String, Object>>, KafkaRecord<R>, KafkaRecord<String>>.ReadOnlyContext ctx,
							Collector<KafkaRecord<String>> out) throws Exception {
			
						KafkaRecord<String> historyRec = new KafkaRecord<String>();
						try {
							//vid mapped to vin
							String vin = ctx.getCurrentKey();
							logger.debug("History Record for VID: {}" , vin);
							
							ReadOnlyBroadcastState<Message<U>, KafkaRecord<R>> mapBrodcast = ctx.getBroadcastState(broadcastStateDescriptor);
							Message<U> keyMessage = new Message<>((U) vin);
							if(mapBrodcast.contains(keyMessage)){
								KafkaRecord<R> rKafkaRecord = mapBrodcast.get(keyMessage);
								VehicleStatusSchema vinStatusRecord = (VehicleStatusSchema)rKafkaRecord.getValue();
								vin = vinStatusRecord.getVin();
							}
							
							historyRec.setKey(inputRec.getValue().f1 + "_" + vin + "_"
									+ TimeFormatter.getInstance().getCurrentUTCTime());
							
							historyRec.setValue(JsonMapper.configuring().writeValueAsString(inputRec.getValue().f2));
							logger.debug("Final History Record key :: {} ",historyRec.getKey());

						} catch (Exception e) {
							historyRec.setKey("UnknownMessage" + "_" + TimeFormatter.getInstance().getCurrentUTCTime());
							//historyRec.setValue(JsonMapper.configuring().writeValueAsString(inputRec.getValue().f2));
						}
						out.collect(historyRec);
					}
					
					@Override
					public void processBroadcastElement(KafkaRecord<R> value,
							KeyedBroadcastProcessFunction<String, KafkaRecord<Tuple3<String, String, Object>>, KafkaRecord<R>, KafkaRecord<String>>.Context ctx,
							Collector<KafkaRecord<String>> out) throws Exception {
						logger.debug("Broadcast updated from history :" + value);
						ctx.getBroadcastState(broadcastStateDescriptor).put(new Message<U>((U) value.getKey()), value);
					}
					
				}).filter(rec -> !rec.getKey().startsWith("UnknownMessage")).returns(new TypeHint<KafkaRecord<String>>() {
				}.getTypeInfo()).name("Filter UnknownMessage")
				.addSink(new StoreHistoricalData(properties.getProperty(DAFCT2Constant.HBASE_ZOOKEEPER_QUORUM),
						properties.getProperty(DAFCT2Constant.HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT),
						properties.getProperty(DAFCT2Constant.ZOOKEEPER_ZNODE_PARENT),
						properties.getProperty(DAFCT2Constant.HBASE_REGIONSERVER),
						properties.getProperty(DAFCT2Constant.HBASE_MASTER),
						properties.getProperty(DAFCT2Constant.HBASE_REGIONSERVER_PORT),
						properties.getProperty(DAFCT2Constant.HBASE_CONTI_HISTORICAL_TABLE_NAME),
						properties.getProperty(DAFCT2Constant.HBASE_CONTI_HISTORICAL_TABLE_CF)))
				.name("Historial Data load ");
	}
  
 
	  
}
