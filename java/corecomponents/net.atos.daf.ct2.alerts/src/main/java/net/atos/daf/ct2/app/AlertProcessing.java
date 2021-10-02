package net.atos.daf.ct2.app;

import net.atos.daf.ct2.cache.kafka.KafkaCdcStreamV2;
import net.atos.daf.ct2.cache.kafka.impl.KafkaCdcImplV2;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.props.AlertConfigProp;
import net.atos.daf.ct2.service.kafka.KafkaConnectionService;
import net.atos.daf.ct2.starter.AlertProcessStarter;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.UUID;

import static net.atos.daf.ct2.props.AlertConfigProp.INCOMING_MESSAGE_UUID;
import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_DAF_STATUS_MSG_TOPIC;
import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_EGRESS_INDEX_MSG_TOPIC;
import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_EGRESS_MONITERING_MSG_TOPIC;

public class AlertProcessing implements Serializable {

    private static final Logger logger = LoggerFactory.getLogger(AlertProcessing.class);
    private static final long serialVersionUID = 1L;



    public static void main(String[] args) throws Exception{
        final StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
        ParameterTool parameterTool = ParameterTool.fromArgs(args);
        logger.info("AlertProcessing started with properties :: {}", parameterTool.getProperties());
        /**
         * Creating param tool from given property
         */
        ParameterTool propertiesParamTool = ParameterTool.fromPropertiesFile(parameterTool.get("prop"));
        env.getConfig().setGlobalJobParameters(propertiesParamTool);
        logger.info("PropertiesParamTool :: {}", parameterTool.getProperties());

        /**
         * Initiating starter class
         */
        AlertProcessStarter alertProcessStarter = new AlertProcessStarter(propertiesParamTool,env);

        /**
         *  Booting cache
         */
        KafkaCdcStreamV2 kafkaCdcStreamV2 = new KafkaCdcImplV2(env,propertiesParamTool);
        Tuple2<BroadcastStream<VehicleAlertRefSchema>, BroadcastStream<Payload<Object>>> bootCache = kafkaCdcStreamV2.bootCache();

        AlertConfigProp.vehicleAlertRefSchemaBroadcastStream = bootCache.f0;
        AlertConfigProp.alertUrgencyLevelRefSchemaBroadcastStream = bootCache.f1;

        /**
         * Index stream reader
         */

        SingleOutputStreamOperator<Index> indexStringStream = KafkaConnectionService.connectIndexObjectTopic(
                        propertiesParamTool.get(KAFKA_EGRESS_INDEX_MSG_TOPIC),
                        propertiesParamTool, env)
                .map(indexKafkaRecord -> indexKafkaRecord.getValue())
                .returns(Index.class)
                .filter(index -> index.getVid() != null)
                .returns(Index.class)
                .map(idx -> {
                    idx.setJobName(UUID.randomUUID().toString());
                    logger.info("Index message received for alert processing :: {}  {}",idx, String.format(INCOMING_MESSAGE_UUID, idx.getJobName()));
                    return idx;})
                .returns(Index.class);

        /**
         * Monitor stream reader
         */
        SingleOutputStreamOperator<Monitor> monitorStringStream = KafkaConnectionService
                .connectMoniteringObjectTopic(propertiesParamTool.get(KAFKA_EGRESS_MONITERING_MSG_TOPIC),
                        propertiesParamTool, env)
                .map(moniterKafkaRecord -> moniterKafkaRecord.getValue()).returns(
                        Monitor.class)
                .filter(moniter -> moniter.getVid() != null && moniter.getMessageType() == 10
                        && (46 == moniter.getVEvtID() || 44 == moniter.getVEvtID() || 45 == moniter.getVEvtID()
                        || 63 == moniter.getVEvtID()))
                .returns(Monitor.class)
                .map(m -> {
                    m.setJobName(UUID.randomUUID().toString());
                    logger.info("Monitor msg received :: {}, msg UUD ::{}", m, m.getJobName());
                    return m;
                }).returns(Monitor.class);


        /**
         * Status stream reader
         */
        SingleOutputStreamOperator<Status> statusStream = KafkaConnectionService.connectStatusObjectTopic(
                        propertiesParamTool.get(KAFKA_DAF_STATUS_MSG_TOPIC),
                        propertiesParamTool,
                        env)
                .map(statusKafkaRecord -> statusKafkaRecord.getValue())
                .returns(Status.class)
                .map(status -> {
                    status.setJobName(UUID.randomUUID().toString());
                    logger.info("Status msg received for alert processing :: {}, msg UUD ::{}", status, status.getJobName());
                    return status;
                })
                .filter(status -> status.getVid() != null && status.getVin() != null)
                .returns(Status.class);

        // Booting  alert process for all three
        alertProcessStarter
                .start(indexStringStream, Index.class)
                .start(monitorStringStream, Monitor.class)
                .start(statusStream,Status.class);


        env.execute(AlertProcessing.class.getSimpleName());

    }
}
