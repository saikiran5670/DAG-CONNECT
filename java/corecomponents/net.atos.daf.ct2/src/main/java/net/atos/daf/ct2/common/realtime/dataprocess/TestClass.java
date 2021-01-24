package net.atos.daf.ct2.common.realtime.dataprocess;

import java.io.FileReader;
import java.io.IOException;
import java.time.Duration;
import java.util.Collections;
import java.util.Properties;

import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.apache.kafka.clients.consumer.ConsumerRecord;
import org.apache.kafka.clients.consumer.ConsumerRecords;
import org.apache.kafka.clients.consumer.KafkaConsumer;
import org.apache.kafka.common.serialization.Deserializer;
import org.apache.kafka.common.serialization.StringDeserializer;

import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.serde.CustomDeserializer;

public class TestClass {
	
	public static void main(String[]  args) throws IOException {

		Properties properties = new Properties();
		properties.put(ConsumerConfig.CLIENT_ID_CONFIG,"Kafka-EventHub-1");
		properties.put(ConsumerConfig.GROUP_ID_CONFIG,"Testing_EventHub_12118");
		properties.put(ConsumerConfig.KEY_DESERIALIZER_CLASS_CONFIG,StringDeserializer.class);
		//properties.put(ConsumerConfig.VALUE_DESERIALIZER_CLASS_CONFIG,StringDeserializer.class);
		properties.put(ConsumerConfig.REQUEST_TIMEOUT_MS_CONFIG,"6000");
		properties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG,"earliest");
		
		properties.put("bootstrap.servers","daf-lan2-d-euwe-cdp-evh.servicebus.windows.net:9093");
		properties.put("security.protocol","SASL_SSL");
		properties.put("sasl.mechanism","PLAIN");
		properties.put("sasl.jaas.config","org.apache.kafka.common.security.plain.PlainLoginModule required username='$ConnectionString' password='Endpoint=sb://daf-lan2-d-euwe-cdp-evh.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=d3jbLGmr9wqli4V56FaGMZTylbs21b2drB8T6FRfo1Q=';"); 
		

		//properties.load(new FileReader("src/main/resources/configuration.properties"));
		
		Deserializer<Monitor> statusDeserializer = new CustomDeserializer<Monitor>();
		properties.put(ConsumerConfig.VALUE_DESERIALIZER_CLASS_CONFIG, statusDeserializer.getClass());

		KafkaConsumer<String,Monitor> indexConsumer=new KafkaConsumer<>(properties);
		indexConsumer.subscribe(Collections.singletonList("egress.conti.monitordata.object"));

		while (true) {
			ConsumerRecords<String,Monitor> consumerRecords = indexConsumer.poll(Duration.ofMillis(1000));
			for(ConsumerRecord <String, Monitor> record : consumerRecords) {
				System.out.println("Topic Name: "+record.topic()+" Partition ID: "+ record.partition()+ " Key: "+ record.key() +" Value: " + record.value().toString()+ " Offset:" + record.offset());
			}
			indexConsumer.commitAsync();
			System.out.println("Records");
			}
		
	}
	

}
