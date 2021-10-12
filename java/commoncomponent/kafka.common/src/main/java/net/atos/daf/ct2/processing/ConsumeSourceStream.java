package net.atos.daf.ct2.processing;

import java.io.Serializable;
import java.util.Objects;
import java.util.Properties;

import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;

import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.serde.KafkaMessageDeSerializeSchema;

public class ConsumeSourceStream implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	public DataStream<KafkaRecord<String>> consumeSourceInputStream(StreamExecutionEnvironment env, String topicNm,
			Properties properties) {
		return env.addSource(
				new FlinkKafkaConsumer<KafkaRecord<String>>(
						properties.getProperty(topicNm),
						new KafkaMessageDeSerializeSchema<String>(), 
						properties))
				.filter(rec -> Objects.nonNull(rec));
	}

}
