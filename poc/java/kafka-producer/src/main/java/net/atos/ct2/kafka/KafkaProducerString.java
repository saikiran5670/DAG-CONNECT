package net.atos.ct2.kafka;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.json.JsonMapper;
import net.atos.ct2.kafka.util.Utils;
import net.atos.ct2.models.ContiMsg;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Status;
import org.apache.kafka.clients.producer.KafkaProducer;
import org.apache.kafka.clients.producer.ProducerRecord;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

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
    private static final Logger logger = LoggerFactory.getLogger(KafkaProducerString.class);

    public static void main(String[] args) throws IOException {
        mapper.setSerializationInclusion(JsonInclude.Include.NON_NULL);
        logger.info("Command liner runner............!");
        String prop = args[0];
        String jsonFile = args[1];
        Properties env = new Properties();
        InputStream iStream = new FileInputStream(prop);
        env.load(iStream);

        String sinkTopicName = env.getProperty("daf.produce.topic");
        logger.info("Topic name:: " + sinkTopicName);
        KafkaProducer<String, String> producer = new KafkaProducer<>(env);

        long messgePushCount = Files.readAllLines(Paths.get(jsonFile))
                .stream()
                .map(json -> {
                    ContiMsg conti = new ContiMsg();
                    String outJson = null;
                    try {
                        for (int i = 0; i < Integer.valueOf(env.getProperty("conti.set.msg.limit")); i++) {
                            conti = new ContiMsg();
                            String tripId = UUID.randomUUID().toString()+"_lost";
                            long timeMillis = System.currentTimeMillis();
                            conti = mapper.readValue(json, ContiMsg.class);
                            conti.getDocument().setTripID(tripId);
                            conti.setEvtDateTime(Utils.convertMillisecondToDateTime(timeMillis));
                            outJson = mapper.writeValueAsString(conti);
                            Thread.sleep(Long.valueOf(env.getProperty("conti.set.system.event.time.sleep")));
                            producer.send(new ProducerRecord<String, String>(sinkTopicName, tripId, outJson));
                            logger.info("Data pushed:: key {} Topic name: {} data: {}", tripId, sinkTopicName, outJson);
                        }
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                    return outJson;
                }).count();

        logger.info("Total message pushed: {}"+messgePushCount);
    }
}
