package net.atos.daf.ct2;


import net.atos.daf.ct2.kafka.KafkaConnectionService;
import net.atos.daf.ct2.kafka.KafkaProducer;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;

import static net.atos.daf.ct2.props.DataTransferProp.KAFKA_MONITOR_TOPIC_SOURCE;

public class Application implements Serializable {
	private static final Logger logger = LoggerFactory.getLogger(Application.class);
	private static final long serialVersionUID = 1L;

	public static void main(String[] args) throws Exception {

		logger.info("Data transfer application started...");
		final StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
		ParameterTool parameterTool = ParameterTool.fromArgs(args);
		ParameterTool propertiesParamTool = ParameterTool.fromPropertiesFile(parameterTool.get("prop"));
		logger.debug("Data transfer propertiesParamTool :: {}", propertiesParamTool.getProperties());

		DataStream<KafkaRecord<Monitor>> kafkaRecordDataStream = KafkaConnectionService.connectMonitoringObjectTopic(propertiesParamTool.get(KAFKA_MONITOR_TOPIC_SOURCE),
				propertiesParamTool, env);
		KafkaProducer.transferMonitorMsg(kafkaRecordDataStream,propertiesParamTool,env);
		env.execute(propertiesParamTool.get("application.job.name"));

	}

}
