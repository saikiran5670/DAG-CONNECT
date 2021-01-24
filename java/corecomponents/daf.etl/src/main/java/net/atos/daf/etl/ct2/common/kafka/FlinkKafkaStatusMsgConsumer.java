package net.atos.daf.etl.ct2.common.kafka;

import java.util.Properties;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.kafka.clients.consumer.ConsumerConfig;

import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.serde.KafkaMessageDeSerializeSchema;
import net.atos.daf.etl.ct2.common.util.ETLConstants;

public class FlinkKafkaStatusMsgConsumer {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	
	public static DataStream<KafkaRecord<Status>> consumeStatusMsgs(ParameterTool envParams, StreamExecutionEnvironment env) {

		Properties properties = new Properties();

		properties.setProperty("client.id", "conti_ct2_ingress_client");
		// properties.setProperty(ConsumerConfig.REQUEST_TIMEOUT_MS_CONFIG,"6000");
		properties.setProperty(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, envParams.get(ETLConstants.AUTO_OFFSET_RESET_CONFIG));
		properties.setProperty(ETLConstants.GROUP_ID, envParams.get(ETLConstants.GROUP_ID));
		properties.setProperty(ETLConstants.BOOTSTRAP_SERVERS, envParams.get(ETLConstants.EVENT_HUB_BOOTSTRAP));
		properties.setProperty(ETLConstants.SECURITY_PROTOCOL, envParams.get(ETLConstants.SECURITY_PROTOCOL));
		properties.setProperty(ETLConstants.SASL_MECHANISM, envParams.get(ETLConstants.SASL_MECHANISM));
		properties.setProperty(ETLConstants.SASL_JAAS_CONFIG, envParams.get(ETLConstants.SASL_JAAS_CONFIG));

		return env.addSource(new FlinkKafkaConsumer<KafkaRecord<Status>>(envParams.get(ETLConstants.STATUS_TOPIC),
				new KafkaMessageDeSerializeSchema<Status>(), properties));

	}

}
