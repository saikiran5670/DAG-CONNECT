package net.atos.ct2.kafka;

import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Status;
import org.apache.kafka.clients.producer.KafkaProducer;
import org.apache.kafka.clients.producer.ProducerRecord;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Objects;
import java.util.Properties;

public class KafkaProducerString {

    public static void main(String[] args) throws IOException {
        System.out.println("Command liner runner............!");
        String prop = args[0];
        String jsonFile = args[1];
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
        KafkaProducer<String, String> producer = new KafkaProducer<>(kafkaTopicProp);

        Files.readAllLines(Paths.get(jsonFile))
                .stream()
                .forEach(
                        line -> {
                            try {
                                producer.send(new ProducerRecord(sinkTopicName, "Input", line));
                                System.out.println("data pushed:: "+line);
                            } catch (Exception e) {
                                e.printStackTrace();
                            }
                        }
                );
    }
}
