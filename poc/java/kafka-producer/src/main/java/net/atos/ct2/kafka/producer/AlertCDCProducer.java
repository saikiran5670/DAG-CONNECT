package net.atos.ct2.kafka.producer;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.databind.ObjectMapper;
import net.atos.ct2.kafka.kafka.CdcPayloadWrapper;
import net.atos.daf.ct2.pojo.standard.Status;
import org.apache.kafka.clients.producer.KafkaProducer;
import org.apache.kafka.clients.producer.ProducerRecord;

import java.io.FileInputStream;
import java.io.InputStream;
import java.util.Objects;
import java.util.Properties;

public class AlertCDCProducer {
    public static ObjectMapper mapper = new ObjectMapper();

    public static void main(String[] args) throws Exception {
        mapper.setSerializationInclusion(JsonInclude.Include.NON_NULL);
        if(args.length < 0 )
            throw new Exception("property or json file not provided");

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

        kafkaTopicProp.put("value.serializer", "org.apache.kafka.common.serialization.StringSerializer");
        kafkaTopicProp.put("key.serializer", "org.apache.kafka.common.serialization.StringSerializer");

        String sinkTopicName=env.getProperty("daf.cdc.topic");
        KafkaProducer<String, String> producer = new KafkaProducer<>(kafkaTopicProp);

        InputStream jsonInputStream = new FileInputStream(jsonFile);

        CdcPayloadWrapper value = mapper.readValue(jsonInputStream, CdcPayloadWrapper.class);
        String asJsonString = mapper.writeValueAsString(value);
        producer.send(new ProducerRecord<String, String>(sinkTopicName, "AlertCDC", asJsonString));
        System.out.println("CDC send ::"+asJsonString);

    }
}
