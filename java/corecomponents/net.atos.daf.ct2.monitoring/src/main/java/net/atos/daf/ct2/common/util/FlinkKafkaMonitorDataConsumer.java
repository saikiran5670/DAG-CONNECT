package net.atos.daf.ct2.common.util;

import java.util.Properties;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.common.realtime.dataprocess.MonitorDataProcess;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.serde.KafkaMessageDeSerializeSchema;

public class FlinkKafkaMonitorDataConsumer {

	public DataStream<KafkaRecord<Monitor>> connectToKafkaTopic(ParameterTool envParams,
			StreamExecutionEnvironment env) {
		
		Logger log = LoggerFactory.getLogger(MonitorDataProcess.class);
		log.info("========= in a connectToKafkaTopic ==========");

		Properties properties = new Properties();

		properties.setProperty("client.id", envParams.get(DafConstants.EVENT_HUB_CLIENTID));
		// properties.setProperty(ConsumerConfig.REQUEST_TIMEOUT_MS_CONFIG,"6000");
		properties.setProperty(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, envParams.get(DafConstants.AUTO_OFFSET_RESET));
		properties.setProperty("group.id", envParams.get(DafConstants.EVENT_HUB_GROUPID));
		properties.setProperty("bootstrap.servers", envParams.get(DafConstants.EVENT_HUB_BOOTSTRAP));
		properties.setProperty("security.protocol", "SASL_SSL");
		properties.setProperty("sasl.mechanism", "PLAIN");
		properties.setProperty("sasl.jaas.config", envParams.get(DafConstants.EVENT_HUB_CONFIG));

		DataStream<KafkaRecord<Monitor>> ds = env
				.addSource(new FlinkKafkaConsumer<KafkaRecord<Monitor>>(envParams.get(DafConstants.MONITORING_TOPIC),
						new KafkaMessageDeSerializeSchema<Monitor>(), properties));

		return ds;
	}

}
