package net.atos.ct2.kafka;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.pojo.standard.StatusDocument;
import org.apache.kafka.clients.producer.KafkaProducer;
import org.apache.kafka.clients.producer.ProducerRecord;

import java.io.FileInputStream;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Objects;
import java.util.Properties;


public class KafkaProducerApplication {

    public static  ObjectMapper mapper = new ObjectMapper();


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

        String sinkTopicName=env.getProperty("daf.produce.topic","egress.conti.statusdata.object");
        KafkaProducer<String, Status> producer = new KafkaProducer<>(kafkaTopicProp);

        Files.readAllLines(Paths.get(jsonFile))
                .stream()
                .map(json -> {
                    Status status2 = new Status();
                    try {
                        status2  = mapper.readValue(json, Status.class);
                    } catch (JsonProcessingException e) {
                        e.printStackTrace();
                    }
                    return status2;
                })
                .filter(status1 -> status1.getVin()!=null)
                .map(status -> {
                    StatusDocument document = new StatusDocument();
                    document.setTripID("s03bf625a-cce7-42b8-a712-7a5a161b1d"+(int) (Math.random()*100));
                    status.setDocument(document);
                    return status;
                })
                .forEach(status -> {
                    try {
                         producer.send(new ProducerRecord<String, Status>(sinkTopicName, "Status", status));
                         System.out.println("data pushed:: "+mapper.writeValueAsString(status));
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                });


    }
}
