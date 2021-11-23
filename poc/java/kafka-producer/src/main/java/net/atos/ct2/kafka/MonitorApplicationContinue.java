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
        if(args.length < 0 )
            throw new Exception("property or json file not provided");
        String prop = args[0];
        String jsonFile = args[1];
        String jsonFile2 = args[2];
        run(prop,jsonFile,jsonFile2);
    }

    public static void run(String prop,String jsonFile,String jsonFile2) throws Exception {
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
        KafkaProducer<String, Monitor> producer = new KafkaProducer<>(kafkaTopicProp);

        Monitor monitor1 = Files.readAllLines(Paths.get(jsonFile))
                .stream()
                .map(json -> {
                	Monitor monitor = new Monitor();
                    try {
                        monitor = mapper.readValue(json, Monitor.class);
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                    return monitor;
                })
                .filter(monitor -> monitor.getVin() != null)
                .collect(Collectors.toList()).get(0);
        
        Monitor monitor2 = Files.readAllLines(Paths.get(jsonFile2))
                .stream()
                .map(json -> {
                	Monitor monitor = new Monitor();
                    try {
                        monitor = mapper.readValue(json, Monitor.class);
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                    return monitor;
                })
                .filter(monitor -> monitor.getVin() != null)
                .collect(Collectors.toList()).get(0);
        System.out.println("Init data ::"+mapper.writeValueAsString(monitor1));
        System.out.println("Init data ::"+mapper.writeValueAsString(monitor2));

        Integer msgLimit = Integer.valueOf(env.getProperty("monitor.set.msg.limit"));
   MonitorDocument doc= new MonitorDocument();
   WarningObject warObj= new WarningObject();
   List<Warning> warningList = new ArrayList<Warning>();
  Warning war = new Warning();
  Warning war1 = new Warning();
  war.setWarningClass(4);
  war.setWarningNumber(4);
  
  war1.setWarningClass(5);
  war1.setWarningNumber(5);
  
  warningList.add(war);
  warningList.add(war1);
  warObj.setWarningList(warningList);

        int warningclass=1;
        int warningNumber=2;
        System.out.println("start time-->"+ System.currentTimeMillis());
        for(int i=0; i < msgLimit; i++){
        	String vinNo= "abcd" + (int)Math.floor(Math.random() * 100);
        	monitor2.setVin(vinNo);
        	monitor1.setVin(vinNo);
        	System.out.println("current mesage count-->"+ i);
            long timeMillis = System.currentTimeMillis();
            monitor1.setEvtDateTime(Utils.convertMillisecondToDateTime(timeMillis));
            if(i % 5==0) {
            	monitor1.setVEvtID(63);
            	//doc.setVWarningClass((int)Math.floor(Math.random() * 11));
            	//doc.setVWarningNumber((int)Math.floor(Math.random() * 11));
            	doc.setWarningObject(warObj);
            	monitor2.setDocument(doc);
            	 producer.send(new ProducerRecord<String, Monitor>(sinkTopicName, "Monitor2", monitor2));
            }
            
            else {
            	monitor1.setVEvtID(44);
            	 MonitorDocument doc1= new MonitorDocument();
            	doc1.setVWarningClass(warningclass);
            	doc1.setVWarningNumber(warningNumber);
            	monitor1.setDocument(doc1);
            	producer.send(new ProducerRecord<String, Monitor>(sinkTopicName, "Monitor", monitor1));
            }
            
            //monitor1.setVDist(inival);
           // monitor1.getDocument().setVTachographSpeed(inival.intValue());
           // producer.send(new ProducerRecord<String, Monitor>(sinkTopicName, "Monitor", monitor1));
            //producer.send(new ProducerRecord<String, Monitor>(sinkTopicName, "Monitor2", monitor2));
			
			/*
			 * if(i % 5==0) {
			 * System.out.println("Data Send ::"+mapper.writeValueAsString(monitor2));
			 * 
			 * } else {
			 * System.out.println("Data Send ::"+mapper.writeValueAsString(monitor1)); }
			 */
            
            long sleepTime = Long.valueOf(env.getProperty("monitor.set.system.event.time.sleep"));
            Thread.sleep(sleepTime);
            warningclass = warningclass+1;
            warningNumber = warningNumber+1;
            if(warningclass==10) {
            	warningclass=1;
            	warningNumber=1;
            }
        }
        System.out.println("end time-->"+ System.currentTimeMillis());
    }
}
