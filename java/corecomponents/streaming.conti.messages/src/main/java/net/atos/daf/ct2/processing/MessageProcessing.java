package net.atos.daf.ct2.processing;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.Message;
import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;
import net.atos.daf.ct2.utils.JsonMapper;
import org.apache.flink.api.common.functions.FilterFunction;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.functions.co.BroadcastProcessFunction;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.flink.util.Collector;

import java.util.Map;
import java.util.Properties;

public class MessageProcessing<U, T> {

  private static ObjectMapper objectMapper;

  public void consumeContiMessage(
      DataStream<KafkaRecord<U>> messageDataStream,
      String messageType,
      String key,
      String sinkTopicName,
      Properties properties,
      Class<T> tClass,
      BroadcastStream<KafkaRecord<U>> broadcastStream) {

    objectMapper = JsonMapper.configuring();
    // SingleOutputStreamOperator<KafkaRecord<T>> singleOutputStreamOperator =
    messageDataStream
        .filter(
            new FilterFunction<KafkaRecord<U>>() {
              @Override
              public boolean filter(KafkaRecord<U> value) throws Exception {
                String transId =
                    objectMapper.readTree((String) value.getValue()).get("TransID").asText();
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
                System.out.println("Single Record: " + value);

                String valueRecord =
                    objectMapper.readTree((String) value.getValue()).get("VID").asText();
                System.out.println("Record VID: " + valueRecord);

                for (Map.Entry<Message<U>, KafkaRecord<U>> map :
                    ctx.getBroadcastState(broadcastStateDescriptor).immutableEntries()) {
                  String key = map.getKey().get().toString();
                  System.out.println("Map: " + map);
                  System.out.println("Key: " + key);

                  if (key.equalsIgnoreCase(valueRecord)) {

                    String values = map.getValue().getValue().toString();
                    System.out.println("Broadcats Values: " + values);
                    JsonNode jsonNode = objectMapper.readTree(value.getValue().toString());
                    ((ObjectNode) jsonNode).put("VIN", values);

                    KafkaRecord<U> kafkaRecord = new KafkaRecord<U>();
                    kafkaRecord.setKey(key);
                    kafkaRecord.setValue((U) objectMapper.writeValueAsString(jsonNode));
                    System.out.println("New Values: " + kafkaRecord);
                    out.collect(kafkaRecord);
                    break;
                  }
                }
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
                T record = objectMapper.readValue((String) value.getValue(), tClass);

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
}
