package net.atos.daf.ct2.poc;

import com.fasterxml.jackson.databind.ObjectMapper;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;
import net.atos.daf.ct2.utils.JsonMapper;
import org.apache.flink.api.common.functions.FilterFunction;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.streaming.api.TimeCharacteristic;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;

import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Properties;
import java.util.stream.Collectors;

public class Testing<T> {

  public static void main(String[] arfs) throws Exception {
    Testing<Monitor> indexTesting = new Testing<Monitor>();
    indexTesting.main1(Monitor.class);
  }

  public void main1(Class<T> tClass) throws Exception {

    StreamExecutionEnvironment streamExecutionEnvironment =
        StreamExecutionEnvironment.getExecutionEnvironment();
    streamExecutionEnvironment.setStreamTimeCharacteristic(TimeCharacteristic.EventTime);

    KafkaRecord<String> kafkaRecord = new KafkaRecord<String>();
    kafkaRecord.setKey("data");
    kafkaRecord.setValue(
        Files.lines(Paths.get("src/main/resources/Monitor.json"))
            .collect(Collectors.joining(System.lineSeparator())));

    DataStream<KafkaRecord<String>> sourceInputStream =
        streamExecutionEnvironment.fromElements(kafkaRecord);

    ObjectMapper objectMapper = JsonMapper.configuring();
    DataStream<KafkaRecord<T>> sourceInputStream1 =
        sourceInputStream
            .filter(
                new FilterFunction<KafkaRecord<String>>() {
                  @Override
                  public boolean filter(KafkaRecord<String> value) throws Exception {
                    String transId =
                        objectMapper.readTree(value.getValue()).get("TransID").asText();
                    return transId.equalsIgnoreCase("03030");
                  }
                })
            .map(
                new MapFunction<KafkaRecord<String>, KafkaRecord<T>>() {
                  @Override
                  public KafkaRecord<T> map(KafkaRecord<String> value) throws Exception {

                    T record = objectMapper.readValue(value.getValue(), tClass);
                    KafkaRecord<T> kafkaRecord = new KafkaRecord<T>();
                    kafkaRecord.setKey("key");
                    kafkaRecord.setValue(record);
                    return kafkaRecord;
                  }
                });
    sourceInputStream1.print();


    Properties properties = new Properties();
    properties.put("client.id", "Kafka-EventHub-2");
    properties.put("group.id", "Testing_EventHub_2");
    properties.put("bootstrap.servers","localhost:9092");


    sourceInputStream1.addSink(
            new FlinkKafkaProducer<KafkaRecord<T>>(
                    "sinkTopicName",
                    new KafkaMessageSerializeSchema<T>("sinkTopicName"),
                    properties,
                    FlinkKafkaProducer.Semantic.EXACTLY_ONCE));

    streamExecutionEnvironment.execute();
  }
}
