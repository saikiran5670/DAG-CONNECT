package net.atos.daf.ct2.common.realtime.dataprocess;

import static net.atos.daf.ct2.common.util.DafConstants.AUTO_OFFSET_RESET_CONFIG;
import static net.atos.daf.ct2.common.util.DafConstants.BROADCAST_NAME;
import static net.atos.daf.ct2.common.util.DafConstants.INCOMING_MESSAGE_UUID;

import java.io.FileReader;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import java.util.Properties;

import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.processing.DriverProcessing;
import net.atos.daf.ct2.common.realtime.hbase.MonitorDataHbaseSink;
import net.atos.daf.ct2.common.realtime.postgresql.DriverTimeManagementSink;
import net.atos.daf.ct2.common.realtime.postgresql.WarningStatisticsSink;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.common.util.FlinkKafkaMonitorDataConsumer;
import net.atos.daf.ct2.common.util.FlinkUtil;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.Message;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.processing.BroadcastState;
import net.atos.daf.ct2.processing.ConsumeSourceStream;
import net.atos.daf.ct2.processing.ValidateSourceStream;

public class MonitorDataProcess {
	
//    private static final Logger log = LoggerFactory.getLogger(MonitorDataProcess.class);
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
		Integer valueSeven=7;

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
			//consumerStream.print();
			
			consumerStream.addSink(new MonitorDataHbaseSink()); // Writing into HBase Table

			KeyedStream<KafkaRecord<Monitor>, String> consumerKeyedStream = consumerStream.keyBy(kafkaRecord -> kafkaRecord.getValue().getVin()!=null ? kafkaRecord.getValue().getVin() : kafkaRecord.getValue().getVid());
			
			DriverProcessing driverProcess= new DriverProcessing();
			
			
			SingleOutputStreamOperator<Monitor> monitorStream=consumerKeyedStream.map(record -> record.getValue()).returns(Monitor.class).filter(monitor -> monitor.getMessageType().equals(valueSeven)).returns(Monitor.class);
			
			SingleOutputStreamOperator<Monitor> driverManagementProcessing = driverProcess.driverManagementProcessing(monitorStream, Long.parseLong(envParams.get(DafConstants.DRIVER_MANAGEMENT_TIME_WINDOW_SECONDS)));
			
			driverManagementProcessing.map(monitor -> {
                log.info("monitor message received after driver calculation processing :: {}  {}", monitor, String.format(INCOMING_MESSAGE_UUID, monitor.getJobName()));
                return monitor;
            });
			driverManagementProcessing.addSink(new DriverTimeManagementSink());  // Drive Time Management
			
			consumerKeyedStream.addSink(new WarningStatisticsSink()); 
			

			log.info("after addsink");
			try {

				
				  auditing = new AuditETLJobClient(envParams.get(DafConstants.GRPC_SERVER),
				  Integer.valueOf(envParams.get(DafConstants.GRPC_PORT)));
				  
				  auditMap = createAuditMap(DafConstants.AUDIT_EVENT_STATUS_START,
				  "Realtime Data Monitoring processing Job Started");
				  
				  auditing.auditTrialGrpcCall(auditMap); auditing.closeChannel();
				 
			} catch (Exception e) {
				log.error("Issue while auditing :: " + e.getMessage());
			}

			env.execute(" Realtime_MonitorDataProcess");

		} catch (Exception e) {
			e.printStackTrace();

			log.error("Error in Message Data Processing - " + e.getMessage());

			try {
				
				  auditMap = createAuditMap(DafConstants.AUDIT_EVENT_STATUS_FAIL,
				  "Realtime Data Monitoring processing Job Failed, reason :: " +
				  e.getMessage());
				  
				  auditing = new AuditETLJobClient(envParams.get(DafConstants.GRPC_SERVER),
				  Integer.valueOf(envParams.get(DafConstants.GRPC_PORT)));
				  auditing.auditTrialGrpcCall(auditMap); auditing.closeChannel();
				 
			} catch (Exception ex) {
				log.error("Issue while auditing :: " + ex.getMessage());
			}

		}
	}

	private static Map<String, String> createAuditMap(String jobStatus, String message) {
		Map<String, String> auditMap = new HashMap<>();

		auditMap.put(DafConstants.JOB_EXEC_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
		auditMap.put(DafConstants.AUDIT_PERFORMED_BY, DafConstants.TRIP_JOB_NAME);
		auditMap.put(DafConstants.AUDIT_COMPONENT_NAME, DafConstants.TRIP_JOB_NAME);
		auditMap.put(DafConstants.AUDIT_SERVICE_NAME, DafConstants.AUDIT_SERVICE);
		auditMap.put(DafConstants.AUDIT_EVENT_TYPE, DafConstants.AUDIT_CREATE_EVENT_TYPE);// check
		auditMap.put(DafConstants.AUDIT_EVENT_TIME,
				String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
		auditMap.put(DafConstants.AUDIT_EVENT_STATUS, jobStatus);
		auditMap.put(DafConstants.AUDIT_MESSAGE, message);
		auditMap.put(DafConstants.AUDIT_SOURCE_OBJECT_ID, DafConstants.DEFAULT_OBJECT_ID);
		auditMap.put(DafConstants.AUDIT_TARGET_OBJECT_ID, DafConstants.DEFAULT_OBJECT_ID);
		auditMap.put(DafConstants.AUDIT_UPDATED_DATA, null);

		return auditMap;
	}
	
    public void flinkConnection() {

        this.streamExecutionEnvironment = StreamExecutionEnvironment.getExecutionEnvironment();
        log.info("Flink Processing Started.");
    }
    
    public void processing(Properties properties) {

        ConsumeSourceStream consumeSrcStream = new ConsumeSourceStream();
        ValidateSourceStream validateSourceStream = new ValidateSourceStream();
        
        MapStateDescriptor<Message<String>, KafkaRecord<Monitor>> mapStateDescriptor =
                new BroadcastState<String, Monitor>()
                        .stateInitialization(properties.getProperty(BROADCAST_NAME));

        DataStream<KafkaRecord<String>> monitorInputStream = consumeSrcStream.consumeSourceInputStream(
                streamExecutionEnvironment, DafConstants.MONITOR_TOPIC_NAME, properties);

        DataStream<Tuple2<Integer, KafkaRecord<String>>> monitorStreamValiditySts = validateSourceStream
                .isValidJSON(monitorInputStream);

    }
	
    public StreamExecutionEnvironment getstreamExecutionEnvironment() {
        return this.streamExecutionEnvironment;
    }
	

}
