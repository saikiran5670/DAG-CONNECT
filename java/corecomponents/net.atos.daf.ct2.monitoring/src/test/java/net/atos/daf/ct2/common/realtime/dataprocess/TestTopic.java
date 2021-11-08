package net.atos.daf.ct2.common.realtime.dataprocess;

import static net.atos.daf.ct2.common.util.DafConstants.AUTO_OFFSET_RESET_CONFIG;

import java.io.FileReader;
import java.io.IOException;
import java.util.Map;
import java.util.Properties;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.common.util.FlinkKafkaMonitorDataConsumer;
import net.atos.daf.ct2.common.util.FlinkUtil;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

public class TestTopic {
	private static final Logger log = LogManager.getLogger(MonitorDataProcess.class);
    public static String FILE_PATH;
    private StreamExecutionEnvironment streamExecutionEnvironment;
    private static final long serialVersionUID = 1L;
	
    public static Properties configuration() throws DAFCT2Exception {

        Properties properties = new Properties();
        try {
            properties.load(new FileReader(FILE_PATH));
            properties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, properties.getProperty(AUTO_OFFSET_RESET_CONFIG));
            log.info("Configuration Loaded for Connecting Kafka inorder to Perform Mapping.");
        } catch (IOException e) {
            log.error("Unable to Find the File " + FILE_PATH, e);
            throw new DAFCT2Exception("Unable to Find the File " + FILE_PATH, e);
        }
        return properties;
    }
	
	public static void main(String[] args) throws Exception {

		Map<String, String> auditMap = null;
		AuditETLJobClient auditing = null;

		ParameterTool envParams = null;

		try {

			ParameterTool params = ParameterTool.fromArgs(args);

			if (params.get("input") != null)
				envParams = ParameterTool.fromPropertiesFile(params.get("input"));

			final StreamExecutionEnvironment env = FlinkUtil.createStreamExecutionEnvironment(envParams,
					envParams.get(DafConstants.MONITOR_JOB));

			log.info("env :: " + env);
			FlinkKafkaMonitorDataConsumer flinkKafkaConsumer = new FlinkKafkaMonitorDataConsumer();
			env.getConfig().setGlobalJobParameters(envParams);

			DataStream<KafkaRecord<Monitor>> consumerStream =flinkKafkaConsumer.connectToKafkaTopic(envParams, env);
			consumerStream.print();
			
			env.execute(" TestTopic");
	}catch(Exception e) {
		e.printStackTrace();
	}
	}
	
	
}
