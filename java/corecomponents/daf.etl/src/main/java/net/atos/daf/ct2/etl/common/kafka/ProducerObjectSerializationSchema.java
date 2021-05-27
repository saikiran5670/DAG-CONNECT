package net.atos.daf.ct2.etl.common.kafka;

import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.core.JsonProcessingException;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.ObjectMapper;
import org.apache.flink.streaming.connectors.kafka.KafkaSerializationSchema;
import org.apache.kafka.clients.producer.ProducerRecord;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.ct2.pojo.KafkaRecord;

public class ProducerObjectSerializationSchema<U> implements KafkaSerializationSchema<KafkaRecord<U>>{

    /**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(ProducerObjectSerializationSchema.class);
	private String topic;   
    private ObjectMapper mapper;

    public ProducerObjectSerializationSchema(String topic) {
        super();
        this.topic = topic;
    }

    @Override
    public ProducerRecord<byte[], byte[]> serialize(KafkaRecord<U> rec, Long timestamp) {
        byte[] b = null;
        if (mapper == null) {
            mapper = new ObjectMapper();
        }
         try {
            b= mapper.writeValueAsBytes(rec);
        } catch (JsonProcessingException e) {
            logger.error("Issue while egress trip aggregated data in ProducerObjectSerializationSchema :: " + e); 
        }
        return new ProducerRecord<byte[], byte[]>(topic, b);
    }

}