package net.atos.ct2.kafka;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.json.JsonMapper;
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
import java.util.UUID;

public class KafkaProducerString {

    public static ObjectMapper mapper = new ObjectMapper();

    public static void main(String[] args) throws IOException {
        System.out.println("Command liner runner............!");
        String prop = args[0];
        String jsonFile = args[1];
        Properties env = new Properties();
        InputStream iStream = new FileInputStream(prop);
        env.load(iStream);

        String sinkTopicName=env.getProperty("daf.produce.topic");
        System.out.println("Topic name:: "+sinkTopicName);
        env.entrySet().forEach(System.out::println);
        KafkaProducer<String, String> producer = new KafkaProducer<>(env);

        Files.readAllLines(Paths.get(jsonFile))
                .stream()
                .forEach(
                        line -> {
                            try {
                                for(int i=0;i<10;i++){
                                    String key = UUID.randomUUID().toString();
                                    producer.send(new ProducerRecord(sinkTopicName,key , line));
                                    System.out.println("Data pushed:: key "+key+" data: "+line);
                                }

                            } catch (Exception e) {
                                e.printStackTrace();
                            }
                        }
                );
    }
}
