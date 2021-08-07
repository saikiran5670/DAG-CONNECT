package net.atos.daf.ct2.kafka;

import java.io.Serializable;
import java.util.Properties;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.kafka.clients.consumer.ConsumerConfig;

import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.serde.KafkaMessageDeSerializeSchema;
import net.atos.daf.ct2.util.FuelDeviationConstants;

public class FlinkKafkaFuelDeviationMsgConsumer implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	
	public static DataStream<KafkaRecord<Index>> consumeIndexMsgs(ParameterTool envParams, StreamExecutionEnvironment env) {

		Properties properties = new Properties();

		properties.setProperty(FuelDeviationConstants.CLIENT_ID, envParams.get(FuelDeviationConstants.CLIENT_ID));
		properties.setProperty(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, envParams.get(FuelDeviationConstants.AUTO_OFFSET_RESET_CONFIG));
		properties.setProperty(FuelDeviationConstants.GROUP_ID, envParams.get(FuelDeviationConstants.GROUP_ID));
		properties.setProperty(FuelDeviationConstants.BOOTSTRAP_SERVERS, envParams.get(FuelDeviationConstants.EVENT_HUB_BOOTSTRAP));
		properties.setProperty(FuelDeviationConstants.SECURITY_PROTOCOL, envParams.get(FuelDeviationConstants.SECURITY_PROTOCOL));
		properties.setProperty(FuelDeviationConstants.SASL_MECHANISM, envParams.get(FuelDeviationConstants.SASL_MECHANISM));
		properties.setProperty(FuelDeviationConstants.SASL_JAAS_CONFIG, envParams.get(FuelDeviationConstants.SASL_JAAS_CONFIG));

		return env
				.addSource(new FlinkKafkaConsumer<KafkaRecord<Index>>(envParams.get(FuelDeviationConstants.INDEX_TOPIC),
						new KafkaMessageDeSerializeSchema<Index>(), properties));

	}

}
