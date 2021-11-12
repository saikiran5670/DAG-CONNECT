package net.atos.daf.ct2.processing;

import java.io.Serializable;
import java.util.Properties;

import org.apache.commons.lang.exception.ExceptionUtils;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.common.ct2.util.DAFConstants;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;
import net.atos.daf.ct2.util.MessageParseUtil;
import net.atos.daf.ct2.utils.JsonMapper;

public class MessageProcessing<U, T> implements Serializable {

	
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(MessageProcessing.class);
/* kafka topic data consuming as stream */
	public void consumeBoschMessage(DataStream<KafkaRecord<U>> messageDataStream, String messageType, String key,
			String sinkTopicName, Properties properties, Class<T> tClass) {
		System.out.println("message parsing start here");
		logger.info("consume Bosch Message message parsing start here");

		DataStream<KafkaRecord<T>> finalKafkaPublishMessageSteream = messageDataStream.map(new MapFunction<KafkaRecord<U>, KafkaRecord<T>>() {
			private static final long serialVersionUID = 1L;

			public KafkaRecord<T> map(KafkaRecord<U> value) {

				logger.error("INFO: Raw Message processing start ====>" + value.toString());
				System.out.println("INFO: Raw Message processing start at executor end ====>" + value.toString());
				try {
					if ("Index".equalsIgnoreCase(key)) {

						Index indexObj = MessageParseUtil.processIndexBoschMessage(value.getValue().toString(),
								properties, value.getTimeStamp().toString());
						if (indexObj.getTransID() != null && indexObj.getDocument().getTripID() != null
								&& indexObj.getVEvtID() != null && indexObj.getVid() != null
								&& indexObj.getVin() != null) {

							T record = JsonMapper.configuring().readValue(indexObj.toString(), tClass);

							KafkaRecord<T> kafkaRecord = new KafkaRecord<T>();
							kafkaRecord.setKey(indexObj.getVin().toString());
							kafkaRecord.setValue(record);
							logger.info("Before Bosch Index message publishing  kafka record :: Message TYpe: " + key +"  Kafka Key =>  "+ indexObj.getVin().toString() + " , Extracted Message is::" + record);
							System.out.println("before Bosch Index message publishing indexObj.toString() record :: "
									+ key + "  Kafka Key =>  "+ indexObj.getVin().toString() + " Extracted Message is::" + indexObj.toString());
							logger.info("Bosch Index message Data extraction is  successfully done.");
							return kafkaRecord;
						} else {
							System.err.println(key + ":"
									+ " Bosch Index Invalid message . mandatory fields are empty. Please check parse  Message"
									+ indexObj.toString());
							logger.info(key + ":"
									+ " Bosch Index Invalid message . mandatory fields are empty. Please check parse  Message"
									+ indexObj.toString());
						}

					} else if ("Monitor".equalsIgnoreCase(key)) {
						Monitor monitorObj = MessageParseUtil.processMonitorBoschMessage(value.getValue().toString(),
								properties, value.getTimeStamp().toString());

						if (monitorObj.getTransID() != null && monitorObj.getDocument().getTripID() != null
								&& monitorObj.getMessageType() != null && monitorObj.getVid() != null
								&& monitorObj.getVin() != null) {

							T record = JsonMapper.configuring().readValue(monitorObj.toString(), tClass);

							KafkaRecord<T> kafkaRecord = new KafkaRecord<T>();
							kafkaRecord.setKey(monitorObj.getVin().toString());
							kafkaRecord.setValue(record);
							logger.info("Before Bosch Monitor message publishing  kafka record :: Message TYpe:"
									+ messageType + " Kafka Key =>" + monitorObj.getVin().toString() + ", Extracted Message is::" + record);
							System.out.println("before Bosch Monitor message publishing indexObj.toString() record :: "
									+ messageType  + " Kafka Key =>" + monitorObj.getVin().toString() + " Extracted Message is::" + monitorObj.toString());
							logger.info("Bosch Monitor message Data extraction is  successfully done.");
							return kafkaRecord;
						} else {
							System.err.println(key + ":" 
									+ " Bosch Monitor message Invalid message . mandatory fields are empty. Please check parse  Message"
									+ monitorObj.toString());
							logger.info(key + ":"
									+ " Bosch Invalid Monitor  message . mandatory fields are empty. Please check parse  Message"
									+ monitorObj.toString());
						}

					} else if ("Status".equalsIgnoreCase(key)) {
						Status statusObj = MessageParseUtil.processStatusBoschMessage(value.getValue().toString(),
								properties, value.getTimeStamp().toString());
						if (statusObj.getTransID() != null && statusObj.getDocument().getTripID() != null
								&& statusObj.getVEvtID() != null && statusObj.getVid() != null
								&& statusObj.getVin() != null) {

							T record = JsonMapper.configuring().readValue(statusObj.toString(), tClass);

							KafkaRecord<T> kafkaRecord = new KafkaRecord<T>();
							kafkaRecord.setKey(statusObj.getVin().toString());
							kafkaRecord.setValue(record);
							logger.info("Before Bosch Status message publishing  kafka record :: Message TYpe:"
									+ messageType  + " Kafka Key =>" + statusObj.getVin().toString() + " Extracted Message is::" + record);
							System.out.println("Before Bosch Status publishing indexObj.toString() record :: "
									+ messageType + "  Kafka Key =>  "+ statusObj.getVin().toString() +" Extracted Message is::" + statusObj.toString());
							logger.info("Bosch Status Data extraction is  successfully done.");
							return kafkaRecord;
						} else {
							System.err.println(key + ":"
									+ " Bosch Status Invalid message . mandatory fields are empty. Please check parse  Message"
									+ statusObj.toString());
							logger.info(key + ":"
									+ " Bosch Invalid Status message . mandatory fields are empty. Please check parse  Message"
									+ statusObj.toString());
						}

					}

				} catch (Exception ex) {
					ex.printStackTrace();
					logger.error("Error while Bosch data processing for publish on kafka topic . Raw message is :"
							+ value.toString());
					System.out.println("Error while Bosch data processing for publish on kafka topic . Raw message is :"
							+ value.toString());
					logger.error("Bosch Raw Message processing  ending failed" + ex.getMessage());
					logger.error(" Bosch Data  parsing and  data setting in index object failed "
							+ ExceptionUtils.getFullStackTrace(ex));

				}
				return null;

			}
		}).filter(record -> record != null);
		
		finalKafkaPublishMessageSteream.addSink(new FlinkKafkaProducer<KafkaRecord<T>>(sinkTopicName,
						new KafkaMessageSerializeSchema<T>(sinkTopicName), properties,
						FlinkKafkaProducer.Semantic.EXACTLY_ONCE));

		System.out.println("Message publish on kafka topic " + sinkTopicName + "successfully.");
		logger.info("Message publish on kafka topic " + sinkTopicName + "successfully.");
		
		//TODO  for publish Raw Message.
		/*String rawSinkTopicName=properties.getProperty(DAFCT2Constant.RAW_MSG_SINK_TOPIC_NAME);
		logger.info("Raw Message publish on kafka topic:===>" + rawSinkTopicName);
		System.out.println("Raw Message publish on kafka topic:===>" + rawSinkTopicName);
		
		finalKafkaPublishMessageSteream.addSink(new FlinkKafkaProducer<KafkaRecord<T>>(rawSinkTopicName,
				new KafkaMessageSerializeSchema<T>(rawSinkTopicName), properties,
				FlinkKafkaProducer.Semantic.EXACTLY_ONCE));
		System.out.println("Message publish on kafka topic " + rawSinkTopicName + "successfully.");
		logger.info("Message publish on kafka topic " + rawSinkTopicName + "successfully.");*/

	}

}
