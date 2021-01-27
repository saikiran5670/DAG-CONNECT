package net.atos.daf.ct2.common.util;

import java.util.Properties;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.kafka.clients.consumer.ConsumerConfig;

import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.serde.KafkaMessageDeSerializeSchema;

public class FlinkKafkaStatusDataConsumer {
	
	public DataStream<KafkaRecord<Status>> connectToKafkaTopic(ParameterTool envParams, StreamExecutionEnvironment env) {
		
		Properties properties = new Properties();
		
		/*
		 * properties.setProperty(DAFConstants.BOOTSTRAP_SERVERS,
		 * DAFConstants.WN0_KAFKA_INTERNAL_9092);
		 * properties.setProperty(DAFConstants.ZOOKEEPER_CONNECT,
		 * DAFConstants.WN0_KAFKA_INTERNAL_2181);
		 * properties.setProperty(DAFConstants.GROUP_ID, DAFConstants.MONITORING_TOPIC);
		 * return env.addSource(new FlinkKafkaConsumer<>(DAFConstants.MONITORING_TOPIC,
		 * new MonitorMsgDeserializer(), properties));
		 */
		
		
		/*properties.setProperty(DafConstants.BOOTSTRAP_SERVERS,envParams.get(DafConstants.BOOTSTRAP_SERVERS));
		properties.setProperty(DafConstants.ZOOKEEPER_CONNECT,envParams.get(DafConstants.ZOOKEEPER_CONNECT));
		properties.setProperty(DafConstants.GROUP_ID,envParams.get(DafConstants.MONITORING_TOPIC));*/
		
		
		properties.setProperty("client.id",envParams.get(DafConstants.EVENT_HUB_CLIENTID));
		  //properties.setProperty(ConsumerConfig.REQUEST_TIMEOUT_MS_CONFIG,"6000");
		  properties.setProperty(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG,"earliest");
		  properties.setProperty("group.id",envParams.get(DafConstants.EVENT_HUB_GROUPID));
		  properties.setProperty("bootstrap.servers",envParams.get(DafConstants.EVENT_HUB_BOOTSTRAP));
		  properties.setProperty("security.protocol","SASL_SSL");
		  properties.setProperty("sasl.mechanism","PLAIN");
		  properties.setProperty("sasl.jaas.config",envParams.get(DafConstants.EVENT_HUB_CONFIG));
		 
		
			/*
			 * properties.setProperty("client.id","conti_ct2_ingress_client");
			 * //properties.setProperty(ConsumerConfig.REQUEST_TIMEOUT_MS_CONFIG,"6000");
			 * properties.setProperty(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG,"earliest");
			 * properties.setProperty("group.id","ct2contiprocessing_group9996");
			 * properties.setProperty("bootstrap.servers",
			 * "daf-lan2-d-euwe-cdp-evh.servicebus.windows.net:9093");
			 * properties.setProperty("security.protocol","SASL_SSL");
			 * properties.setProperty("sasl.mechanism","PLAIN"); properties.setProperty(
			 * "sasl.jaas.config","org.apache.kafka.common.security.plain.PlainLoginModule required username='$ConnectionString' password='Endpoint=sb://daf-lan2-d-euwe-cdp-evh.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=d3jbLGmr9wqli4V56FaGMZTylbs21b2drB8T6FRfo1Q=';"
			 * );
			 * 
			 */
		  DataStream<KafkaRecord<Status>> ds=env.addSource(new FlinkKafkaConsumer<KafkaRecord<Status>>(envParams.get(DafConstants.STATUS_TOPIC), new KafkaMessageDeSerializeSchema<Status>(), properties));
		  //DataStream<KafkaRecord<Status>> ds=env.addSource(new FlinkKafkaConsumer<KafkaRecord<Status>>("egress.conti.statusdata.object", new KafkaMessageDeSerializeSchema<Status>(), properties));
				
		return ds;
	}

}
