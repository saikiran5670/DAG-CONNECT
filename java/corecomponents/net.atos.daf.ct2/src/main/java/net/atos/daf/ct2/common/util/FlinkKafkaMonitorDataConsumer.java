package net.atos.daf.ct2.common.util;

import java.util.Properties;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.kafka.clients.consumer.ConsumerConfig;

import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.serde.KafkaMessageDeSerializeSchema;

public class FlinkKafkaMonitorDataConsumer {
	
	public DataStream<KafkaRecord<Monitor>> connectToKafkaTopic(ParameterTool envParams, StreamExecutionEnvironment env) {
		
		Properties properties = new Properties();
		
		
		/*
		 * properties.setProperty("client.id","conti_ct2_ingress_client");
		 * //properties.setProperty(ConsumerConfig.REQUEST_TIMEOUT_MS_CONFIG,"6000");
		 * properties.setProperty(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG,"earliest");
		 * properties.setProperty("group.id","ct2contiprocessing_group457");
		 * properties.setProperty("bootstrap.servers",
		 * "daf-lan2-d-euwe-cdp-evh.servicebus.windows.net:9093");
		 * properties.setProperty("security.protocol","SASL_SSL");
		 * properties.setProperty("sasl.mechanism","PLAIN"); properties.setProperty(
		 * "sasl.jaas.config","org.apache.kafka.common.security.plain.PlainLoginModule required username='$ConnectionString' password='Endpoint=sb://daf-lan2-d-euwe-cdp-evh.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=d3jbLGmr9wqli4V56FaGMZTylbs21b2drB8T6FRfo1Q=';"
		 * );
		 * 
		 */
		
		
		  properties.setProperty("client.id","conti_ct2_ingress_client");
		  //properties.setProperty(ConsumerConfig.REQUEST_TIMEOUT_MS_CONFIG,"6000");
		  properties.setProperty(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG,"earliest");
		  properties.setProperty("group.id","ct2contiprocessing_group999");
		  properties.setProperty("bootstrap.servers",envParams.get(DafConstants.EVENT_HUB_BOOTSTRAP));
		  properties.setProperty("security.protocol","SASL_SSL");
		  properties.setProperty("sasl.mechanism","PLAIN");
		  properties.setProperty("sasl.jaas.config",envParams.get(DafConstants.EVENT_HUB_CONFIG));
		 
		
		
		  DataStream<KafkaRecord<Monitor>> ds=env.addSource(new FlinkKafkaConsumer<KafkaRecord<Monitor>>(envParams.get(DafConstants.MONITORING_TOPIC), new KafkaMessageDeSerializeSchema<Monitor>(), properties));
		  
		  //DataStream<KafkaRecord<Monitor>> ds=env.addSource(new FlinkKafkaConsumer<KafkaRecord<Monitor>>("egress.conti.monitordata.object", new KafkaMessageDeSerializeSchema<Monitor>(), properties));
				
		return ds;
	}

}
