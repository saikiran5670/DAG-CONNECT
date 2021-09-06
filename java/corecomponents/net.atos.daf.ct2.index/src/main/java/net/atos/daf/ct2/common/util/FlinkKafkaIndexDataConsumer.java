package net.atos.daf.ct2.common.util;

import java.util.Properties;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.common.realtime.dataprocess.IndexDataProcess;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.serde.KafkaMessageDeSerializeSchema;

public class FlinkKafkaIndexDataConsumer {
	//This class has all setup parameters of Kafka consumer
	
	public DataStream<KafkaRecord<Index>> connectToKafkaTopic(ParameterTool envParams, StreamExecutionEnvironment env) {
		
		Logger log = LoggerFactory.getLogger(IndexDataProcess.class);
		log.info("========= in a connectToKafkaTopic ==========");

		Properties properties = new Properties();

		properties.setProperty("client.id", envParams.get(DafConstants.EVENT_HUB_CLIENTID));
		// properties.setProperty(ConsumerConfig.REQUEST_TIMEOUT_MS_CONFIG,"6000");
		//properties.setProperty(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, "earliest");
		properties.setProperty(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, envParams.get(DafConstants.AUTO_OFFSET_RESET));
		properties.setProperty("group.id", envParams.get(DafConstants.EVENT_HUB_GROUPID));
		properties.setProperty("bootstrap.servers", envParams.get(DafConstants.EVENT_HUB_BOOTSTRAP));
		properties.setProperty("security.protocol", "SASL_SSL");
		properties.setProperty("sasl.mechanism", "PLAIN");
		properties.setProperty("sasl.jaas.config", envParams.get(DafConstants.EVENT_HUB_CONFIG));

		DataStream<KafkaRecord<Index>> ds = env.addSource(new FlinkKafkaConsumer<KafkaRecord<Index>>(
				envParams.get(DafConstants.INDEX_TOPIC), new KafkaMessageDeSerializeSchema<Index>(), properties));
		
		return ds;
	}
}