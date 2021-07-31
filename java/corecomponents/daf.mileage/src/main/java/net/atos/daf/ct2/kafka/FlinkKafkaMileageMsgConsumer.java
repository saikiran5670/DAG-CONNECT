package net.atos.daf.ct2.kafka;

import java.io.Serializable;
import java.util.Properties;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.kafka.clients.consumer.ConsumerConfig;

import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.serde.KafkaMessageDeSerializeSchema;
import net.atos.daf.ct2.util.MileageConstants;

public class FlinkKafkaMileageMsgConsumer implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	
	public static DataStream<KafkaRecord<Status>> consumeStatusMsgs(ParameterTool envParams, StreamExecutionEnvironment env) {

		Properties properties = new Properties();

		properties.setProperty(MileageConstants.CLIENT_ID, envParams.get(MileageConstants.CLIENT_ID));
		// properties.setProperty(ConsumerConfig.REQUEST_TIMEOUT_MS_CONFIG,"6000");
		properties.setProperty(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, envParams.get(MileageConstants.AUTO_OFFSET_RESET_CONFIG));
		properties.setProperty(MileageConstants.GROUP_ID, envParams.get(MileageConstants.GROUP_ID));
		properties.setProperty(MileageConstants.BOOTSTRAP_SERVERS, envParams.get(MileageConstants.EVENT_HUB_BOOTSTRAP));
		properties.setProperty(MileageConstants.SECURITY_PROTOCOL, envParams.get(MileageConstants.SECURITY_PROTOCOL));
		properties.setProperty(MileageConstants.SASL_MECHANISM, envParams.get(MileageConstants.SASL_MECHANISM));
		properties.setProperty(MileageConstants.SASL_JAAS_CONFIG, envParams.get(MileageConstants.SASL_JAAS_CONFIG));

		return env.addSource(new FlinkKafkaConsumer<KafkaRecord<Status>>(envParams.get(MileageConstants.STATUS_TOPIC),
				new KafkaMessageDeSerializeSchema<Status>(), properties));

	}

}
