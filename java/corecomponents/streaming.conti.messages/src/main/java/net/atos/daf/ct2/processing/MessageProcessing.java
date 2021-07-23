package net.atos.daf.ct2.processing;

import java.util.Map;
import java.util.Properties;

import net.atos.daf.ct2.models.scheamas.VehicleStatusSchema;
import org.apache.flink.api.common.functions.FilterFunction;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.functions.co.BroadcastProcessFunction;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.flink.util.Collector;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.node.ObjectNode;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.Message;
import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;
import net.atos.daf.ct2.utils.JsonMapper;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class MessageProcessing<U,R, T> {

    private static final Logger logger = LoggerFactory.getLogger(MessageProcessing.class);

  public void consumeContiMessage(
      DataStream<KafkaRecord<U>> messageDataStream,
      String messageType,
      String key,
      String sinkTopicName,
      Properties properties,
      Class<T> tClass,
      BroadcastStream<KafkaRecord<R>> broadcastStream) {
    messageDataStream
        .filter(
            new FilterFunction<KafkaRecord<U>>() {
              @Override
              public boolean filter(KafkaRecord<U> value) throws Exception {
                String transId =
                    JsonMapper.configuring()
                        .readTree((String) value.getValue())
                        .get("TransID")
                        .asText();
                logger.info("Filtered Trans ID " + transId);
                return transId.equalsIgnoreCase(messageType);
              }
            })
        .connect(broadcastStream)
        .process(new BroadcastMessageProcessor<>(properties))
        .map(
            new MapFunction<KafkaRecord<U>, KafkaRecord<T>>() {
              @Override
              public KafkaRecord<T> map(KafkaRecord<U> value) throws Exception {
                logger.info("map after process record value.getValue() :: {}",value.getValue());
                T record = JsonMapper.configuring().readValue((String) value.getValue(), tClass);

                KafkaRecord<T> kafkaRecord = new KafkaRecord<T>();
                kafkaRecord.setKey(key);
                kafkaRecord.setValue(record);
               logger.info("Final KafkaRecord to kafka topic: {} record : {}",sinkTopicName , kafkaRecord);
                return kafkaRecord;
              }
            })
        .addSink(
            new FlinkKafkaProducer<KafkaRecord<T>>(
                sinkTopicName,
                new KafkaMessageSerializeSchema<T>(sinkTopicName),
                properties,
                FlinkKafkaProducer.Semantic.EXACTLY_ONCE));

  }
  
  public void contiMessageForHistorical(
	      DataStream<KafkaRecord<String>> messageDataStream,
	      Properties properties,
		  BroadcastStream<KafkaRecord<R>> broadcastStream) {

		messageDataStream.connect(broadcastStream)
				.process(new BroadcastProcessFunction<KafkaRecord<String>, KafkaRecord<R>, KafkaRecord<String>>() {
					/**
					 * 
					 */
					private static final long serialVersionUID = 1L;
					private final MapStateDescriptor<Message<U>, KafkaRecord<R>> broadcastStateDescriptor = new BroadcastState<U,R>()
							.stateInitialization(properties.getProperty(DAFCT2Constant.BROADCAST_NAME));

					@Override
					public void processElement(KafkaRecord<String> value, ReadOnlyContext ctx,
							Collector<KafkaRecord<String>> out) {

						try {
							JsonNode jsonNodeRec = JsonMapper.configuring().readTree((String) value.getValue());
							String vid = jsonNodeRec.get("VID").asText();
							logger.info("History Record for VID: {}" , vid);
							String vin = DAFCT2Constant.UNKNOWN;

							ReadOnlyBroadcastState<Message<U>, KafkaRecord<R>> mapBrodcast = ctx.getBroadcastState(broadcastStateDescriptor);
							Message<U> keyMessage = new Message<>((U) vid);
							if(mapBrodcast.contains(keyMessage)){
								KafkaRecord<R> rKafkaRecord = mapBrodcast.get(keyMessage);
								VehicleStatusSchema vinStatusRecord = (VehicleStatusSchema)rKafkaRecord.getValue();
								vin = vinStatusRecord.getVin();
							}
							value.setKey(jsonNodeRec.get("TransID").asText() + "_" + vin + "_"
									+ TimeFormatter.getInstance().getCurrentUTCTime());

						} catch (Exception e) {
							value.setKey("UnknownMessage" + "_" + TimeFormatter.getInstance().getCurrentUTCTime());
						}
						out.collect(value);
					}
					@Override
					public void processBroadcastElement(KafkaRecord<R> value, Context ctx,
							Collector<KafkaRecord<String>> out) throws Exception {
						logger.info("Broadcast updated from history :" + value);
						ctx.getBroadcastState(broadcastStateDescriptor).put(new Message<U>((U) value.getKey()), value);
					}
				})
				.addSink(new StoreHistoricalData(properties.getProperty(DAFCT2Constant.HBASE_ZOOKEEPER_QUORUM),
						properties.getProperty(DAFCT2Constant.HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT),
						properties.getProperty(DAFCT2Constant.ZOOKEEPER_ZNODE_PARENT),
						properties.getProperty(DAFCT2Constant.HBASE_REGIONSERVER),
						properties.getProperty(DAFCT2Constant.HBASE_MASTER),
						properties.getProperty(DAFCT2Constant.HBASE_REGIONSERVER_PORT),
						properties.getProperty(DAFCT2Constant.HBASE_CONTI_HISTORICAL_TABLE_NAME),
						properties.getProperty(DAFCT2Constant.HBASE_CONTI_HISTORICAL_TABLE_CF)));
	}
	  
}
