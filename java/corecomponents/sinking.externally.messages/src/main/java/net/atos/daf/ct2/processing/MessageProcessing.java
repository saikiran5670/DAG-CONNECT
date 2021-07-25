package net.atos.daf.ct2.processing;

import java.util.Properties;
import java.util.UUID;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;

public class MessageProcessing<T> {

	private static final Logger log = LogManager.getLogger(MessageProcessing.class);
	  
  public void consumeMessages(
      DataStream<KafkaRecord<T>> messageDataStream, String key, Properties properties) {

	  messageDataStream
        .map(
            new MapFunction<KafkaRecord<T>, KafkaRecord<String>>() {
              /**
				 * 
				 */
				private static final long serialVersionUID = 1L;

			@Override
              public KafkaRecord<String> map(KafkaRecord<T> value) throws Exception {

                String jsonMessage = null;
                KafkaRecord<String> kafkaRecord = new KafkaRecord<String>();
                log.info("Inside Sink message :: " + value);
                
                if (value.getValue() instanceof Index) {
                  Index index = (Index) value.getValue();

                  jsonMessage = index.toString();
                  log.info("DAF standard Index Message: " + jsonMessage);
                } else if (value.getValue() instanceof Status) {
                  Status status = (Status) value.getValue();

                  jsonMessage = status.toString();
                  log.info("DAF standard Status Message: " + jsonMessage);
                 
                } else if (value.getValue() instanceof Monitor) {
                  Monitor monitor = (Monitor) value.getValue();

                  jsonMessage = monitor.toString();
                  log.info("DAF standardMonitor Message: " + jsonMessage);
                 
                }else {
                	jsonMessage =  (String) value.getValue();
                	log.info("Source system Message: " + jsonMessage);
                }

                kafkaRecord.setKey(UUID.randomUUID().toString());
                kafkaRecord.setValue(jsonMessage);
                log.info("External Sink Message: " + kafkaRecord);
                return kafkaRecord;
              }
            })
        .addSink(
            new FlinkKafkaProducer<KafkaRecord<String>>(
                properties.getProperty(DAFCT2Constant.SINK_JSON_STRING_TOPIC_NAME),
                new KafkaMessageSerializeSchema<String>(
                    properties.getProperty(DAFCT2Constant.SINK_JSON_STRING_TOPIC_NAME)),
                properties,
                FlinkKafkaProducer.Semantic.EXACTLY_ONCE));

    // singleOutputStreamOperator.print();

    
  
  }
  
  public void consumeStsMessages(
			DataStream<KafkaRecord<Status>> messageDataStream, String key, Properties properties) {

		messageDataStream.map(new MapFunction<KafkaRecord<Status>, KafkaRecord<String>>() {
			/**
			 * 
			 */
			private static final long serialVersionUID = 1L;

			@Override
			public KafkaRecord<String> map(KafkaRecord<Status> value) throws Exception {

				String jsonMessage = null;
				KafkaRecord<String> kafkaRecord = new KafkaRecord<String>();
				Status statusMsg = value.getValue();
				jsonMessage = statusMsg.toString();
				kafkaRecord.setKey(UUID.randomUUID().toString());
				kafkaRecord.setValue(jsonMessage);

				log.info("Message: " + kafkaRecord);
				return kafkaRecord;
			}
		}).addSink(new FlinkKafkaProducer<KafkaRecord<String>>(
				properties.getProperty(DAFCT2Constant.SINK_JSON_STRING_TOPIC_NAME),
				new KafkaMessageSerializeSchema<String>(
						properties.getProperty(DAFCT2Constant.SINK_JSON_STRING_TOPIC_NAME)),
				properties, FlinkKafkaProducer.Semantic.EXACTLY_ONCE));
	}
}
