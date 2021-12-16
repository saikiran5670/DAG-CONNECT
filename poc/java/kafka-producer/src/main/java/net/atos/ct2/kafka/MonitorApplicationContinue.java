package net.atos.ct2.kafka;

import java.io.FileInputStream;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import java.util.Properties;
import java.util.stream.Collectors;

import org.apache.kafka.clients.producer.KafkaProducer;
import org.apache.kafka.clients.producer.ProducerRecord;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.databind.ObjectMapper;

import net.atos.ct2.kafka.util.Utils;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.MonitorDocument;
import net.atos.daf.ct2.pojo.standard.Warning;
import net.atos.daf.ct2.pojo.standard.WarningObject;

public class MonitorApplicationContinue {
    public static ObjectMapper mapper = new ObjectMapper();
    private static final Logger logger = LoggerFactory.getLogger(MonitorApplicationContinue.class);

    public static void main(String[] args) throws Exception {
        mapper.setSerializationInclusion(JsonInclude.Include.NON_NULL);
        if (args.length < 0)
            throw new Exception("property or json file not provided");
        String prop = args[0];
        String jsonFile = args[1];
        run(prop, jsonFile);
    }

    public static void run(String prop, String jsonFile) throws Exception {
        System.out.println("Command liner runner............!");
        Properties kafkaTopicProp = new Properties();
        Properties env = new Properties();
        InputStream iStream = new FileInputStream(prop);
        env.load(iStream);


        kafkaTopicProp.put("request.timeout.ms", env.getProperty("request.timeout.ms", "60000"));
        kafkaTopicProp.put("client.id", env.getProperty("client.id"));
        kafkaTopicProp.put("auto.offset.reset", env.getProperty("auto.offset.reset"));
        kafkaTopicProp.put("group.id", env.getProperty("group.id"));
        kafkaTopicProp.put("bootstrap.servers", env.getProperty("bootstrap.servers"));
        if (Objects.nonNull(env.getProperty("security.protocol")))
            kafkaTopicProp.put("security.protocol", env.getProperty("security.protocol"));
        if (Objects.nonNull(env.getProperty("sasl.jaas.config")))
            kafkaTopicProp.put("sasl.jaas.config", env.getProperty("sasl.jaas.config"));
        if (Objects.nonNull(env.getProperty("sasl.mechanism")))
            kafkaTopicProp.put("sasl.mechanism", env.getProperty("sasl.mechanism"));

        kafkaTopicProp.put("value.serializer", "net.atos.ct2.kafka.serde.Serializastion");
        kafkaTopicProp.put("key.serializer", "org.apache.kafka.common.serialization.StringSerializer");
        String sinkTopicName = env.getProperty("daf.produce.topic");

        long sleepTime = Long.valueOf(env.getProperty("monitor.set.system.event.time.sleep"));
        Integer msgLimit = Integer.valueOf(env.getProperty("monitor.set.msg.limit"));

        KafkaProducer<String, Monitor> producer = new KafkaProducer<>(kafkaTopicProp);



        Monitor monitor = mapper.readValue(Paths.get(jsonFile).toFile(), Monitor.class);
        System.out.println("Start time-->" + System.currentTimeMillis());
//        producer.send(new ProducerRecord<String, Monitor>(sinkTopicName, "Monitor2", monitor2));

        int message_counter = 0;
        int current_counter = 3;

        for (int i = 0; i < msgLimit; i++) {
            String dateTime = Utils.convertMillisecondToDateTime(System.currentTimeMillis());
            Integer driverWorkingState=3;
           if( message_counter < 40 && current_counter == 3 ){
               driverWorkingState=3;
           }else{
               if(current_counter==3){
                   message_counter=0;
                   current_counter=1;
               }
               if( message_counter < 5 && current_counter == 1 ){
                   driverWorkingState=1;
               }else{
                   if(current_counter==1){
                       message_counter=0;
                       current_counter=2;
                   }
                   if( message_counter < 5 && current_counter == 2 ){
                       driverWorkingState=2;
                   }else{
                       message_counter=0;
                       current_counter=3;
                   }
               }
           }
            message_counter++;
            monitor.getDocument().setDriver1WorkingState(driverWorkingState);
            monitor.setEvtDateTime(dateTime);
            monitor.setKafkaProcessingTS(String.valueOf(System.currentTimeMillis()));
            producer.send(new ProducerRecord<String, Monitor>(sinkTopicName, monitor.getVin(), monitor));
            System.out.println("Message send counter --> "+i);
//            System.out.println("data --> "+mapper.writeValueAsString(monitor));
            Thread.sleep(sleepTime);
        }
        System.out.println("End time-->" + System.currentTimeMillis());
    }
}
