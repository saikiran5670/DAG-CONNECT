package net.atos.daf.ct2.main;

import static net.atos.daf.ct2.constant.DAFCT2Constant.AUTO_OFFSET_RESET_CONFIG;
import static net.atos.daf.ct2.constant.DAFCT2Constant.BROADCAST_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.GRPC_PORT;
import static net.atos.daf.ct2.constant.DAFCT2Constant.GRPC_SERVER;
import static net.atos.daf.ct2.constant.DAFCT2Constant.JOB_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.MASTER_DATA_TOPIC_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_CDC_FETCH_DATA_QUERY;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_DATABASE_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_DRIVER;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_HOSTNAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_PASSWORD;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_PORT;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_USER;
import static net.atos.daf.ct2.constant.DAFCT2Constant.SOURCE_TOPIC_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.VEHICLE_STATUS_SCHEMA_DEF;

import java.io.FileReader;
import java.io.IOException;
import java.io.Serializable;
import java.util.Objects;
import java.util.Properties;
import java.util.concurrent.TimeUnit;

import net.atos.daf.ct2.postgre.VehicleStatusSource;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.restartstrategy.RestartStrategies;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.time.Time;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.functions.KeySelector;
import org.apache.flink.api.java.io.jdbc.JDBCInputFormat;
import org.apache.flink.api.java.tuple.Tuple3;
import org.apache.flink.api.java.typeutils.RowTypeInfo;
import org.apache.flink.runtime.state.StateBackend;
import org.apache.flink.runtime.state.filesystem.FsStateBackend;
import org.apache.flink.streaming.api.CheckpointingMode;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.CheckpointConfig.ExternalizedCheckpointCleanup;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.fasterxml.jackson.databind.JsonNode;

import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.models.scheamas.CdcPayloadWrapper;
import net.atos.daf.ct2.models.scheamas.VehicleStatusSchema;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.Message;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.processing.BroadcastState;
import net.atos.daf.ct2.processing.ConsumeSourceStream;
import net.atos.daf.ct2.processing.KafkaAuditService;
import net.atos.daf.ct2.processing.MessageProcessing;
import net.atos.daf.ct2.util.Utils;
import net.atos.daf.ct2.utils.JsonMapper;

public class ContiHBaseProcessing implements Serializable {

    private static final Logger logger = LoggerFactory.getLogger(ContiHBaseProcessing.class);
    public static String FILE_PATH;
    private StreamExecutionEnvironment streamExecutionEnvironment;
    private static final long serialVersionUID = 1L;

    public static Properties configuration() throws DAFCT2Exception {

        Properties properties = new Properties();
        try {
            properties.load(new FileReader(FILE_PATH));
            properties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, properties.getProperty(AUTO_OFFSET_RESET_CONFIG));
            logger.info("Configuration Loaded for Connecting Kafka inorder to Perform Mapping.");
        } catch (IOException e) {
            logger.error("Unable to Find the File " + FILE_PATH, e);
            throw new DAFCT2Exception("Unable to Find the File " + FILE_PATH, e);
        }
        return properties;
    }

    public static void main(String[] args) {
        ContiHBaseProcessing contiHBaseProcessing = new ContiHBaseProcessing();
        Properties properties = null;
        try {
            FILE_PATH = args[0];

            properties = configuration();
            contiHBaseProcessing.auditContiJobDetails(properties, "Conti HBase streaming job started");

			if(properties.getProperty("flink.streaming.evn").equalsIgnoreCase("default"))
				contiHBaseProcessing.flinkConnection();
			else contiHBaseProcessing.flinkConnection(properties);
            contiHBaseProcessing.processing(properties);
            contiHBaseProcessing.startExecution();

        } catch (DAFCT2Exception e) {
            logger.error("Exception: {}", e);
            contiHBaseProcessing.auditContiJobDetails(properties, "Conti HBase streaming job failed :: " + e.getMessage());
        } finally {
            //       auditETLJobClient.closeChannel();
        }
    }

    public void flinkConnection() {

        this.streamExecutionEnvironment = StreamExecutionEnvironment.getExecutionEnvironment();
        logger.info("Flink Processing Started.");
    }
    
    public void flinkConnection(Properties properties) {

		// this.streamExecutionEnvironment =
		// StreamExecutionEnvironment.getExecutionEnvironment();
		StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
		env.setParallelism(Integer.parseInt(properties.getProperty(DAFCT2Constant.PARALLELISM)));

		// start a checkpoint every 1000 ms and mode set to EXACTLY_ONCE
		env.enableCheckpointing(Long.parseLong(properties.getProperty(DAFCT2Constant.CHECKPOINT_INTERVAL)),
				CheckpointingMode.EXACTLY_ONCE);

		// make sure 500 ms of progress happen between checkpoints
		env.getCheckpointConfig().setMinPauseBetweenCheckpoints(
				Long.parseLong(properties.getProperty(DAFCT2Constant.MINIMUM_PAUSE_BETWEEN_CHECKPOINTS)));

		// checkpoints have to complete within one minute, or are discarded
		env.getCheckpointConfig()
				.setCheckpointTimeout(Long.parseLong(properties.getProperty(DAFCT2Constant.CHECKPOINT_TIMEOUT)));

		// allow only one checkpoint to be in progress at the same time
		env.getCheckpointConfig().setMaxConcurrentCheckpoints(
				Integer.parseInt(properties.getProperty(DAFCT2Constant.MAX_CONCURRENT_CHECKPOINTS)));

		env.setStateBackend(
				(StateBackend) new FsStateBackend(properties.getProperty(DAFCT2Constant.CHECKPOINT_DIRECTORY), true));

		// enable externalized checkpoints which are retained after job
		// cancellation
		env.getCheckpointConfig().enableExternalizedCheckpoints(ExternalizedCheckpointCleanup.RETAIN_ON_CANCELLATION);

		logger.info("RESTART_FLAG :: " + properties.getProperty(DAFCT2Constant.RESTART_FLAG));
		if ("true".equals(properties.getProperty(DAFCT2Constant.RESTART_FLAG))) {
			if ("true".equals(properties.getProperty(DAFCT2Constant.FIXED_RESTART_FLAG))) {
				env.setRestartStrategy(RestartStrategies.fixedDelayRestart(
						Integer.parseInt(properties.getProperty(DAFCT2Constant.RESTART_ATTEMPS)), // no of restart attempts
						Long.parseLong(properties.getProperty(DAFCT2Constant.RESTART_INTERVAL))) // time in milliseconds between restarts
				);
			} else {
				env.setRestartStrategy(RestartStrategies.failureRateRestart(
						Integer.parseInt(properties.getProperty(DAFCT2Constant.RESTART_FAILURE_RATE)), // max failures per interval
						Time.of(Long.parseLong(properties.getProperty(DAFCT2Constant.RESTART_FAILURE_INTERVAL)),
								TimeUnit.MILLISECONDS), // time interval for measuring failure rate
						Time.of(Long.parseLong(properties.getProperty(DAFCT2Constant.RESTART_FAILURE_DELAY)),
								TimeUnit.MILLISECONDS) // delay
				));
			}
		} else {
			env.setRestartStrategy(RestartStrategies.noRestart());
		}

		logger.info("RestartStrategy ::{}",env.getRestartStrategy());
		this.streamExecutionEnvironment = env;
		logger.info("Flink Processing Started.");
	}

    public void processing(Properties properties) {

        ConsumeSourceStream consumeSrcStream = new ConsumeSourceStream();
       
        MapStateDescriptor<Message<String>, KafkaRecord<VehicleStatusSchema>> mapStateDescriptor =
                new BroadcastState<String, VehicleStatusSchema>()
                        .stateInitialization(properties.getProperty(BROADCAST_NAME));

        SingleOutputStreamOperator<VehicleStatusSchema> kafkaCDCMessage = consumeSrcStream.consumeSourceInputStream(
                streamExecutionEnvironment, MASTER_DATA_TOPIC_NAME, properties)
                .map(json -> {
                    try{
                        CdcPayloadWrapper wrapper  = (CdcPayloadWrapper)Utils.readValueAsObject(json.getValue(), CdcPayloadWrapper.class);
                        if(Objects.nonNull(wrapper.getNamespace()) && wrapper.getNamespace().equalsIgnoreCase("vehicleManagement")){
                            VehicleStatusSchema schema =  (VehicleStatusSchema) Utils.readValueAsObject(wrapper.getPayload(), VehicleStatusSchema.class);
                            schema.setOperationType(wrapper.getOperation());
                            schema.setNamespace(wrapper.getNamespace());
                            logger.info("Message from kafka cdc topic:: {}",json);
                            return schema;
                        }
                    }catch (Exception e){
                        logger.error("Un-parsable cdc event :: {} exception :: {}",json,e);
                    }
                    return VehicleStatusSchema.builder().build();

                })
                .returns(VehicleStatusSchema.class)
                .filter(schema -> Objects.nonNull(schema.getVid()))
                .returns(VehicleStatusSchema.class);


        /**
         * New code added for fetching status data from databases
         * Using jdbcInput format
         */
		//TODO After flink update to 1.14.0 use below code
        /*RowTypeInfo rowTypeInfo = new RowTypeInfo(VEHICLE_STATUS_SCHEMA_DEF);

        String jdbcUrl = new StringBuilder("jdbc:postgresql://")
                .append(properties.getProperty(POSTGRE_HOSTNAME))
                .append(":" + properties.getProperty(POSTGRE_PORT) + "/")
                .append(properties.getProperty(POSTGRE_DATABASE_NAME))
                .append("?user=" + properties.getProperty(POSTGRE_USER))
                .append("&password=" + properties.getProperty(POSTGRE_PASSWORD))
                .append("&sslmode=require")
                .toString();

        JDBCInputFormat jdbcInputFormat = JDBCInputFormat
                .buildJDBCInputFormat()
                .setDrivername(properties.getProperty(POSTGRE_DRIVER))
                .setDBUrl(jdbcUrl)
                .setQuery(properties.getProperty(POSTGRE_CDC_FETCH_DATA_QUERY))
                .setRowTypeInfo(rowTypeInfo)
                .finish();

        logger.info("Connection done and data fetched using query:: {}",properties.getProperty(POSTGRE_CDC_FETCH_DATA_QUERY));

        SingleOutputStreamOperator<VehicleStatusSchema> dbVehicleStatusStream = streamExecutionEnvironment.createInput(jdbcInputFormat)
                .map(row -> VehicleStatusSchema.builder()
                        .vin(String.valueOf(row.getField(0)))
                        .vid(String.valueOf(row.getField(1)))
                        .status(String.valueOf(row.getField(2)))
                        .fuelType(String.valueOf(row.getField(3)))
                        .build())
                .returns(VehicleStatusSchema.class);*/

		SingleOutputStreamOperator<VehicleStatusSchema> dbVehicleStatusStream = streamExecutionEnvironment.addSource(new VehicleStatusSource(properties));


		DataStream<VehicleStatusSchema> statusSchemaDataStream = kafkaCDCMessage.union(dbVehicleStatusStream);

        BroadcastStream<KafkaRecord<VehicleStatusSchema>> broadcastStream = statusSchemaDataStream
                .<KafkaRecord<VehicleStatusSchema>>map(vehicleStatusSchema -> {
                    KafkaRecord<VehicleStatusSchema> kafkaRecord = new KafkaRecord<>();
                    kafkaRecord.setKey(vehicleStatusSchema.getVid());
                    kafkaRecord.setValue(vehicleStatusSchema);
                    return kafkaRecord;
                })
                .returns(TypeInformation.of(new TypeHint<KafkaRecord<VehicleStatusSchema>>() {
                    @Override
                    public TypeInformation<KafkaRecord<VehicleStatusSchema>> getTypeInfo() {
                        return super.getTypeInfo();
                    }
                }))
                .broadcast(mapStateDescriptor);

                
        SingleOutputStreamOperator<KafkaRecord<Tuple3<String, String, Object>>> contiInputStream = consumeSrcStream.consumeSourceInputStream(
                streamExecutionEnvironment, SOURCE_TOPIC_NAME, properties)
        		//.rebalance()
        		//.keyBy(inputRec -> Objects.nonNull(inputRec.getKey()) ? inputRec.getKey() : "UNKNOWN")
        		.keyBy(new KeySelector<KafkaRecord<String>, String>(){

					/**
					 * 
					 */
					private static final long serialVersionUID = 1L;

					@Override
					public String getKey(KafkaRecord<String> value) throws Exception {
						String vid = DAFCT2Constant.UNKNOWN;
						try {
							JsonNode jsonNodeRec = JsonMapper.configuring().readTree((String) value.getValue());
							JsonNode jsonVid = jsonNodeRec.get("VID");
							if (Objects.nonNull(jsonVid))
								vid = jsonVid.asText();
						} catch (Exception e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
						return vid;
					}
        			
        		})
        .map(new MapFunction<KafkaRecord<String>, KafkaRecord<Tuple3<String, String, Object>>>(){

			/**
			 * 
			 */
			private static final long serialVersionUID = 1L;

			@Override
			public KafkaRecord<Tuple3<String, String, Object>> map(KafkaRecord<String> value) throws Exception {
				String transId = DAFCT2Constant.UNKNOWN;
				String vid = DAFCT2Constant.UNKNOWN;
				
				KafkaRecord<Tuple3<String, String, Object>> kafkaRec = new KafkaRecord<Tuple3<String, String, Object>>();
				
				try{
					JsonNode jsonNodeRec = JsonMapper.configuring().readTree((String) value.getValue());
					//((ObjectNode) jsonNodeRec).put("kafkaProcessingTS", value.getTimeStamp());
					
					JsonNode jsonTransId = jsonNodeRec.get("TransID");
					if (Objects.nonNull(jsonTransId))
						transId = jsonTransId.asText();

					JsonNode jsonVid = jsonNodeRec.get("VID");
					if (Objects.nonNull(jsonVid))
						vid = jsonVid.asText();

					kafkaRec.setKey(vid);
					
					if (DAFCT2Constant.TRANSID_INDEX.equals(transId)){
						
						Index indxObj = JsonMapper.configuring().readValue((String) value.getValue(), Index.class);
						indxObj.setKafkaProcessingTS(String.valueOf(value.getTimeStamp()));
						
						kafkaRec.setValue(Tuple3.of(vid, transId, indxObj));
					}else if (DAFCT2Constant.TRANSID_STATUS.equals(transId)){
						
						Status stsObj = JsonMapper.configuring().readValue((String) value.getValue(), Status.class);
						stsObj.setKafkaProcessingTS(String.valueOf(value.getTimeStamp()));
						
						kafkaRec.setValue(Tuple3.of(vid, transId, stsObj));
					}else if (DAFCT2Constant.TRANSID_MONITOR.equals(transId)){
						
						Monitor monitorObj = JsonMapper.configuring().readValue((String) value.getValue(), Monitor.class);
						monitorObj.setKafkaProcessingTS(String.valueOf(value.getTimeStamp()));
						
						kafkaRec.setValue(Tuple3.of(vid,transId, monitorObj));
					}else{
						logger.debug("Issue received different transId type for HBase processing:{}", value);
						kafkaRec.setValue(Tuple3.of(vid, transId, value.getValue()));
					}
					logger.info("KafkaRecord entry :{}",kafkaRec.getValue());
					
			}catch(Exception e){
				kafkaRec.setKey(DAFCT2Constant.CORRUPT);
				kafkaRec.setValue(Tuple3.of(vid, transId, value.getValue()));
			}
			return kafkaRec;
        }
        }).name("Conti Src Stream ");
       
        KeyedStream<KafkaRecord<Tuple3<String, String, Object>>, String> contiKeyedHBaseStream = contiInputStream
        		.filter(rec ->  !DAFCT2Constant.CORRUPT.equals(rec.getKey()) )
        		.name("Filter Valid Records")
        		.keyBy(rec -> rec.getKey());
        
       	new MessageProcessing<Tuple3<String, String, Object>, VehicleStatusSchema, String>()
        	.contiKeyedMessageForHistorical(
        			contiKeyedHBaseStream,
        			properties,
        			broadcastStream
        			);
              
    }

    public StreamExecutionEnvironment getstreamExecutionEnvironment() {
        return this.streamExecutionEnvironment;
    }

    public void startExecution() throws DAFCT2Exception {

        try {
            this.streamExecutionEnvironment.execute("Conti HBase Records");

        } catch (Exception e) {
            logger.error("Unable to process Message using Flink {}", e);
            throw new DAFCT2Exception("Unable to process Message using Flink ", e);
        }
    }

    public void auditContiJobDetails(Properties properties, String message) {
        try {
            new KafkaAuditService().auditTrail(
                    properties.getProperty(GRPC_SERVER),
                    properties.getProperty(GRPC_PORT),
                    JOB_NAME,
                    message);
        } catch (Exception e) {
            logger.error("Issue while auditing streaming conti job ");
        }
    }
}
