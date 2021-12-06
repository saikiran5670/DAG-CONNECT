package net.atos.daf.ct2.processing;

import java.nio.charset.StandardCharsets;

import org.apache.flink.streaming.connectors.kafka.KafkaSerializationSchema;
import org.apache.kafka.clients.producer.ProducerRecord;

import net.atos.daf.ct2.pojo.KafkaRecord;

public class ProducerStringSerializationSchema implements KafkaSerializationSchema<KafkaRecord<String>>{

    /**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private String topic;   

    public ProducerStringSerializationSchema(String topic) {
        super();
        this.topic = topic;
    }

    @Override
    public ProducerRecord<byte[], byte[]> serialize(KafkaRecord<String> element, Long timestamp) {
    	return new ProducerRecord<byte[], byte[]>(topic, element.getValue().getBytes(StandardCharsets.UTF_8));
    }

}
