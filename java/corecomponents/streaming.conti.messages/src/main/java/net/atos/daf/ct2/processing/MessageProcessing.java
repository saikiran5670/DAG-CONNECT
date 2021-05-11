package net.atos.daf.ct2.processing;

import java.util.Map;
import java.util.Properties;

import org.apache.flink.api.common.functions.FilterFunction;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.state.MapStateDescriptor;
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

public class MessageProcessing<U, T> {

  public void consumeContiMessage(
      DataStream<KafkaRecord<U>> messageDataStream,
      String messageType,
      String key,
      String sinkTopicName,
      Properties properties,
      Class<T> tClass,
      BroadcastStream<KafkaRecord<U>> broadcastStream) {

    // SingleOutputStreamOperator<KafkaRecord<T>> singleOutputStreamOperator =
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
                System.out.println("Trans ID " + transId);
                return transId.equalsIgnoreCase(messageType);
              }
            })
        .connect(broadcastStream)
        .process(
            new BroadcastProcessFunction<KafkaRecord<U>, KafkaRecord<U>, KafkaRecord<U>>() {

              private final MapStateDescriptor<Message<U>, KafkaRecord<U>>
                  broadcastStateDescriptor =
                      new BroadcastState<U>()
                          .stateInitialization(
                              properties.getProperty(DAFCT2Constant.BROADCAST_NAME));

              @Override
              public void processElement(
                  KafkaRecord<U> value, ReadOnlyContext ctx, Collector<KafkaRecord<U>> out)
                  throws Exception {
                boolean flag = false;
                System.out.println("Single Record: " + value);
                //System.out.println("ctx--:"+ ctx.getBroadcastState(broadcastStateDescriptor).immutableEntries());
                		
                String valueRecord =
                    JsonMapper.configuring()
                        .readTree((String) value.getValue())
                        .get("VID")
                        .asText();
                System.out.println("Record VID: " + valueRecord);
				 
                for (Map.Entry<Message<U>, KafkaRecord<U>> map :
                    ctx.getBroadcastState(broadcastStateDescriptor).immutableEntries()) {
                  String key = map.getKey().get().toString();
                  System.out.println("Map: " + map);
                  System.out.println("Key: " + key);

                  if (key.equalsIgnoreCase(valueRecord)) {
                    String values = map.getValue().getValue().toString();
                    String[] arrOfStr =values.split("-");
               
                    System.out.println("Broadcats Values: " + values);
                    JsonNode jsonNode = 
					    JsonMapper.configuring().readTree(value.getValue().toString());
                   
                    ((ObjectNode) jsonNode).put("VIN", arrOfStr[0]);
                    ((ObjectNode) jsonNode).put("STATUS", arrOfStr[1]);
                   
                    
                    System.out.println("VIN---=" + arrOfStr[0]);
                    System.out.println("STATUS---=" + arrOfStr[1]);
                    
                    KafkaRecord<U> kafkaRecord = new KafkaRecord<U>();
                    kafkaRecord.setKey(key);
                    kafkaRecord.setValue((U) JsonMapper.configuring().writeValueAsString(jsonNode));
                    System.out.println("New Values: " + kafkaRecord);

                    if(arrOfStr[1].equals(DAFCT2Constant.CONNECTED_OTA_OFF) || arrOfStr[1].equals(DAFCT2Constant.CONNECTED_OTA_ON)){
                    out.collect(kafkaRecord);
                    }
                    flag = true;
                    break;
                  }
                }
                if (!flag) out.collect(value);
             }

              @Override
              public void processBroadcastElement(
                  KafkaRecord<U> value, Context ctx, Collector<KafkaRecord<U>> out)
                  throws Exception {
                System.out.println("Broadcast:" + value);
                ctx.getBroadcastState(broadcastStateDescriptor)
                    .put(new Message<U>((U) value.getKey()), value);
              }
            })
        .map(
            new MapFunction<KafkaRecord<U>, KafkaRecord<T>>() {
              @Override
              public KafkaRecord<T> map(KafkaRecord<U> value) throws Exception {

                System.out.println("Class:" + tClass);
                System.out.println("value.getValue() :: "+value.getValue());
                T record = JsonMapper.configuring().readValue((String) value.getValue(), tClass);

                KafkaRecord<T> kafkaRecord = new KafkaRecord<T>();
                kafkaRecord.setKey(key);
                kafkaRecord.setValue(record);
                System.out.println("Final: " + kafkaRecord);
                return kafkaRecord;
              }
            })
        .addSink(
            new FlinkKafkaProducer<KafkaRecord<T>>(
                sinkTopicName,
                new KafkaMessageSerializeSchema<T>(sinkTopicName),
                properties,
                FlinkKafkaProducer.Semantic.EXACTLY_ONCE));

    // singleOutputStreamOperator.print();

  }
  
  public void contiMessageForHistorical(
	      DataStream<KafkaRecord<String>> messageDataStream,
	      Properties properties,
		  BroadcastStream<KafkaRecord<U>> broadcastStream) {

		messageDataStream.connect(broadcastStream)
				.process(new BroadcastProcessFunction<KafkaRecord<String>, KafkaRecord<U>, KafkaRecord<String>>() {

					/**
					 * 
					 */
					private static final long serialVersionUID = 1L;
					private final MapStateDescriptor<Message<U>, KafkaRecord<U>> broadcastStateDescriptor = new BroadcastState<U>()
							.stateInitialization(properties.getProperty(DAFCT2Constant.BROADCAST_NAME));

					@Override
					public void processElement(KafkaRecord<String> value, ReadOnlyContext ctx,
							Collector<KafkaRecord<String>> out) {

						try {
							JsonNode jsonNodeRec = JsonMapper.configuring().readTree((String) value.getValue());
							String vid = jsonNodeRec.get("VID").asText();
							System.out.println("History Record VID: " + vid);
							String vin = DAFCT2Constant.UNKNOWN;

							for (Map.Entry<Message<U>, KafkaRecord<U>> map : ctx
									.getBroadcastState(broadcastStateDescriptor).immutableEntries()) {
								String key = map.getKey().get().toString();
								System.out.println("Map: " + map);
								System.out.println("History Key: " + key);

								if (key.equalsIgnoreCase(vid)) {
									vin = map.getValue().getValue().toString();
									System.out.println("Broadcats Values: " + vid);
									break;
								}
							}
							value.setKey(jsonNodeRec.get("TransID").asText() + "_" + vin + "_"
									+ TimeFormatter.getInstance().getCurrentUTCTime());

						} catch (Exception e) {
							value.setKey("UnknownMessage" + "_" + TimeFormatter.getInstance().getCurrentUTCTime());
						}

						out.collect(value);
					}

					@Override
					public void processBroadcastElement(KafkaRecord<U> value, Context ctx,
							Collector<KafkaRecord<String>> out) throws Exception {
						System.out.println("Broadcast:" + value);
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
