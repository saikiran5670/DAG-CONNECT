package net.atos.ct2.kafka;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.databind.ObjectMapper;
import net.atos.ct2.kafka.util.Utils;
import net.atos.daf.ct2.pojo.standard.Index;
import org.apache.kafka.clients.producer.KafkaProducer;
import org.apache.kafka.clients.producer.ProducerRecord;

import java.io.FileInputStream;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Objects;
import java.util.Properties;
import java.util.stream.Collectors;

public class IndexApplicationContinue {
    public static ObjectMapper mapper = new ObjectMapper();

    public static void main(String[] args) throws Exception {
        mapper.setSerializationInclusion(JsonInclude.Include.NON_NULL);
        if(args.length < 0 )
            throw new Exception("property or json file not provided");
        String prop = args[0];
        String jsonFile = args[1];
        run(prop,jsonFile);
    }

    public static void run(String prop,String jsonFile) throws Exception {
        System.out.println("Command liner runner............!");
        Properties kafkaTopicProp = new Properties();
        Properties env = new Properties();
        InputStream iStream = new FileInputStream(prop);
        env.load(iStream);


        kafkaTopicProp.put("request.timeout.ms", env.getProperty("request.timeout.ms","60000"));
        kafkaTopicProp.put("client.id", env.getProperty("client.id"));
        kafkaTopicProp.put("auto.offset.reset", env.getProperty("auto.offset.reset"));
        kafkaTopicProp.put("group.id", env.getProperty("group.id"));
        kafkaTopicProp.put("bootstrap.servers", env.getProperty("bootstrap.servers"));
        if(Objects.nonNull(env.getProperty("security.protocol")))
            kafkaTopicProp.put("security.protocol", env.getProperty("security.protocol"));
        if(Objects.nonNull(env.getProperty("sasl.jaas.config")))
            kafkaTopicProp.put("sasl.jaas.config", env.getProperty("sasl.jaas.config"));
        if(Objects.nonNull(env.getProperty("sasl.mechanism")))
            kafkaTopicProp.put("sasl.mechanism", env.getProperty("sasl.mechanism"));

        kafkaTopicProp.put("value.serializer", "net.atos.ct2.kafka.serde.Serializastion");
        kafkaTopicProp.put("key.serializer", "org.apache.kafka.common.serialization.StringSerializer");

        String sinkTopicName=env.getProperty("daf.produce.topic");
        KafkaProducer<String, Index> producer = new KafkaProducer<>(kafkaTopicProp);

        Index index1 = Files.readAllLines(Paths.get(jsonFile))
                .stream()
                .map(json -> {
                    Index index = new Index();
                    try {
                        index = mapper.readValue(json, Index.class);
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                    return index;
                })
                .filter(index -> index.getVin() != null)
                .collect(Collectors.toList()).get(0);
        System.out.println("Init data ::"+mapper.writeValueAsString(index1));

        Integer msgLimit = Integer.valueOf(env.getProperty("index.set.msg.limit", "30"));
        Long inival=3L;
        for(int i=0; i < msgLimit; i++){
            long timeMillis = System.currentTimeMillis();
            index1.setEvtDateTime(Utils.convertMillisecondToDateTime(timeMillis));
            index1.setVDist(inival);
            index1.getDocument().setVTachographSpeed(inival.intValue());
            producer.send(new ProducerRecord<String, Index>(sinkTopicName, "Index", index1));
            System.out.println("Data Send ::"+mapper.writeValueAsString(index1));
            long sleepTime = Long.valueOf(env.getProperty("index.set.system.event.time.sleep", "1000"));
            Thread.sleep(sleepTime);
            inival = inival+3;
        }

    }
}
