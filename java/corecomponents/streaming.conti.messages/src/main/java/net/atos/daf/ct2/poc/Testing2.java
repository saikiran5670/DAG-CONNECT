package net.atos.daf.ct2.poc;

import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.MapperFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import net.atos.daf.ct2.pojo.KafkaRecord;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.BasicTypeInfo;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.functions.KeySelector;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.typeutils.TupleTypeInfo;
import org.apache.flink.streaming.api.TimeCharacteristic;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.util.Collector;

import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Map;
import java.util.stream.Collectors;

public class Testing2 {

  public static void main(String[] args) throws Exception {

    ObjectMapper objectMapper = new ObjectMapper();
    objectMapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
    objectMapper.configure(DeserializationFeature.FAIL_ON_NULL_FOR_PRIMITIVES, false);
    objectMapper.configure(DeserializationFeature.FAIL_ON_NUMBERS_FOR_ENUMS, false);
    objectMapper.configure(MapperFeature.ACCEPT_CASE_INSENSITIVE_PROPERTIES, true);

    StreamExecutionEnvironment streamExecutionEnvironment =
        StreamExecutionEnvironment.getExecutionEnvironment();
    streamExecutionEnvironment.setStreamTimeCharacteristic(TimeCharacteristic.EventTime);

    DataStream<String> inputStream =
        streamExecutionEnvironment.fromElements(
            Files.lines(Paths.get("src/main/resources/Index.json"))
                .collect(Collectors.joining(System.lineSeparator())));

    DataStream<KafkaRecord<String>> indexInputStream =
        inputStream.map(
            new MapFunction<String, KafkaRecord<String>>() {
              @Override
              public KafkaRecord<String> map(String value) throws Exception {
                KafkaRecord<String> kafkaRecord = new KafkaRecord<>();
                kafkaRecord.setKey("Records");
                kafkaRecord.setValue(value);
                return kafkaRecord;
              }
            });

    KeyedStream<KafkaRecord<String>, String> keyedStream =
        indexInputStream.keyBy(
            new KeySelector<KafkaRecord<String>, String>() {
              @Override
              public String getKey(KafkaRecord<String> value) throws Exception {
                return objectMapper.readTree(value.getValue()).get("VID").asText();
              }
            });

    DataStream<String> inputStream1 =
        streamExecutionEnvironment.fromElements(
            Files.lines(Paths.get("src/main/resources/Records.json"))
                .collect(Collectors.joining(System.lineSeparator())));

    DataStream<KafkaRecord<String>> indexInputStream1 =
        inputStream1.map(
            new MapFunction<String, KafkaRecord<String>>() {
              @Override
              public KafkaRecord<String> map(String value) throws Exception {
                KafkaRecord<String> kafkaRecord = new KafkaRecord<>();
                kafkaRecord.setKey(objectMapper.readTree(value).get("VID").asText());
                kafkaRecord.setValue(value);
                return kafkaRecord;
              }
            });

    indexInputStream1.print();
    DataStream<Tuple2<String, String>> masterDataInputStream1 =
        indexInputStream1.map(
            new MapFunction<KafkaRecord<String>, Tuple2<String, String>>() {
              @Override
              public Tuple2<String, String> map(KafkaRecord<String> value) throws Exception {
                String vid = objectMapper.readTree(value.getValue()).get("VID").asText();
                String vin = objectMapper.readTree(value.getValue()).get("VIN").asText();
                return new Tuple2<String, String>(vid, vin);
              }
            });

    // masterDataInputStream1.print();

    Class<Tuple2<String, String>> typedTuple =
        (Class<Tuple2<String, String>>) (Class<?>) Tuple2.class;

    TupleTypeInfo<Tuple2<String, String>> tupleTypeInfo =
        new TupleTypeInfo<Tuple2<String, String>>(
            typedTuple, BasicTypeInfo.STRING_TYPE_INFO, BasicTypeInfo.STRING_TYPE_INFO);

    /* MapStateDescriptor<String, Tuple2<String, String>> mapStateDescriptor =
        new MapStateDescriptor<String, Tuple2<String, String>>(
            "BroadcastState", BasicTypeInfo.STRING_TYPE_INFO, tupleTypeInfo);

    BroadcastStream<Tuple2<String, String>> broadcastStream =
        masterDataInputStream1.broadcast(mapStateDescriptor);*/

    MapStateDescriptor<String, KafkaRecord<String>> mapStateDescriptor =
        new MapStateDescriptor<String, KafkaRecord<String>>(
            "BroadcastState",
            BasicTypeInfo.STRING_TYPE_INFO,
            TypeInformation.of(
                new TypeHint<KafkaRecord<String>>() {
                  @Override
                  public TypeInformation<KafkaRecord<String>> getTypeInfo() {
                    return super.getTypeInfo();
                  }
                }));

    BroadcastStream<KafkaRecord<String>> broadcastStream =
        indexInputStream1.broadcast(mapStateDescriptor);

    /*DataStream<KafkaRecord<String>> output =
        keyedStream
            .connect(broadcastStream)
            .process(
                new KeyedBroadcastProcessFunction<
                    String, KafkaRecord<String>, Tuple2<String, String>, KafkaRecord<String>>() {

                  private final MapStateDescriptor<String, Tuple2<String, String>>
                      broadcastStateDescriptor =
                          new MapStateDescriptor<String, Tuple2<String, String>>(
                              "BroadcastState", BasicTypeInfo.STRING_TYPE_INFO, tupleTypeInfo);

                  @Override
                  public void processElement(
                      KafkaRecord<String> value,
                      ReadOnlyContext ctx,
                      Collector<KafkaRecord<String>> out)
                      throws Exception {
                    String keyRecord = value.getKey();
                    String valueRecord = objectMapper.readTree(value.getValue()).get("VID").asText();

                    for (Map.Entry<String, Tuple2<String, String>> map :
                        ctx.getBroadcastState(broadcastStateDescriptor).immutableEntries()) {
                      String key = map.getKey();
                      Tuple2<String, String> values = map.getValue();

                      if (values.f0.equalsIgnoreCase(valueRecord)) {
                        JsonNode rootNode = objectMapper.readTree(value.getValue());
                        ((ObjectNode) rootNode).put("VID", values.f1);

                        KafkaRecord kafkaRecord = new KafkaRecord();
                        kafkaRecord.setKey(key);
                        kafkaRecord.setValue(rootNode.toString());
                        out.collect(kafkaRecord);
                      }
                    }
                  }

                  @Override
                  public void processBroadcastElement(
                      Tuple2<String, String> value, Context ctx, Collector<KafkaRecord<String>> out)
                      throws Exception {
                    ctx.getBroadcastState(broadcastStateDescriptor).put(value.f0, value);
                  }
                });
    output.print();*/

    DataStream<KafkaRecord<String>> output =
        keyedStream
            .connect(broadcastStream)
            .process(
                new KeyedBroadcastProcessFunction<
                    String, KafkaRecord<String>, KafkaRecord<String>, KafkaRecord<String>>() {

                  private final MapStateDescriptor<String, KafkaRecord<String>>
                      broadcastStateDescriptor =
                          new MapStateDescriptor<String, KafkaRecord<String>>(
                              "BroadcastState",
                              BasicTypeInfo.STRING_TYPE_INFO,
                              TypeInformation.of(
                                  new TypeHint<KafkaRecord<String>>() {
                                    @Override
                                    public TypeInformation<KafkaRecord<String>> getTypeInfo() {
                                      return super.getTypeInfo();
                                    }
                                  }));

                  @Override
                  public void processElement(
                      KafkaRecord<String> value,
                      ReadOnlyContext ctx,
                      Collector<KafkaRecord<String>> out)
                      throws Exception {

                    String valueRecord =
                        objectMapper.readTree(value.getValue()).get("VID").asText();

                    for (Map.Entry<String, KafkaRecord<String>> map :
                        ctx.getBroadcastState(broadcastStateDescriptor).immutableEntries()) {
                      String key = map.getKey();
                      String values =
                          objectMapper
                              .readTree(map.getValue().getValue())
                              .get("VID")
                              .asText();

                      if (key.equalsIgnoreCase(valueRecord)) {
                        JsonNode rootNode = objectMapper.readTree(value.getValue());
                        ((ObjectNode) rootNode).put("VID", values);

                        KafkaRecord kafkaRecord = new KafkaRecord();
                        kafkaRecord.setKey(key);
                        kafkaRecord.setValue(rootNode.toString());
                        out.collect(kafkaRecord);
                      }
                    }
                  }

                  @Override
                  public void processBroadcastElement(
                      KafkaRecord<String> value, Context ctx, Collector<KafkaRecord<String>> out)
                      throws Exception {
                    ctx.getBroadcastState(broadcastStateDescriptor).put(value.getKey(), value);
                  }
                });
    output.print();
    streamExecutionEnvironment.execute();
  }
}
