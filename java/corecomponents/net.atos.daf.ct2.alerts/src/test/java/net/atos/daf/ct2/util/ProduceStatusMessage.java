package net.atos.daf.ct2.util;

import com.esotericsoftware.kryo.util.ObjectMap;
import com.fasterxml.jackson.databind.ObjectMapper;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.pojo.standard.StatusDocument;
import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.junit.Ignore;
import org.junit.Test;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import java.util.Properties;

public class ProduceStatusMessage {

    private static final Logger logger = LoggerFactory.getLogger(ProduceStatusMessage.class);

    @Ignore
    @Test
    public void produceStatusMessge() throws Exception {
//        String propertyPath = "src/test/resources/application-local-test.properties";
        String propertyPath = "C:\\codebase\\personal\\net.atos.daf.ct2.alerts\\net.atos.daf.ct2.alerts\\src\\main\\resources\\application-test2.properties";
        final StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
        ParameterTool propertiesParamTool = ParameterTool.fromPropertiesFile(propertyPath);
        env.getConfig().setGlobalJobParameters(propertiesParamTool);
        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        logger.info("kafkaTopicProp :: {}" , kafkaTopicProp.entrySet());
        kafkaTopicProp.put("sasl.jaas.config",propertiesParamTool.get("" +
                "status.object.sasl.jaas.config"));
        kafkaTopicProp.put("bootstrap.servers",propertiesParamTool.get("status.object.bootstrap.servers"));

        String initData = "{ \"receivedTimestamp\": null, \"EvtDateTime\": \"1628163993562\", \"Increment\": null, \"ROProfil\": null, \"TenantID\": null, \"TransID\": \"s03bf625a-cce7-42b8-a712-7a5a161b1d003\", \"VID\": \"abc\", \"VIN\": \"abc\", \"kafkaProcessingTS\": null, \"EventDateTimeFirstIndex\": null, \"Jobname\": null, \"NumberOfIndexMessage\": null, \"NumSeq\": null, \"VEvtID\": null, \"ROmodel\": null, \"ROName\": null, \"ROrelease\": null, \"DriverID\": \"driver1\", \"GPSStartDateTime\": \"2021-03-28T02:38:15.000Z\", \"GPSEndDateTime\": \"2021-04-28T02:49:47.000Z\", \"GPSStartLatitude\": null, \"GPSEndLatitude\": null, \"GPSStartLongitude\": null, \"GPSEndLongitude\": null, \"GPSStartVehDist\": 23456, \"GPSStopVehDist\": 33456, \"VBrakeDuration\": null, \"VCruiseControlDist\": null, \"VHarshBrakeDuration\": null, \"VIdleDuration\": \"459\", \"VPosAltitudeVariation\": null, \"VNegAltitudeVariation\": null, \"VPTOCnt\": null, \"VPTODuration\": null, \"VPTODist\": null, \"VStartFuel\": null, \"VStopFuel\": null, \"VStartTankLevel\": null, \"VStopTankLevel\": null, \"VUsedFuel\": null, \"VIdleDurationDistr\": null, \"DocFormat\": \"json\", \"DocVersion\": \"0.0.1\", \"Document\": null }";

        ObjectMapper mapper = new ObjectMapper();

        Status status = mapper.readValue(initData, Status.class);

        String sinkTopicName="egress.conti.statusdata.object";

        status.setVin("XLR0998HGFFT74597");
        StatusDocument document = new StatusDocument();
        document.setTripID("s03bf625a-cce7-42b8-a712-7a5a161b1d009");
        status.setDocument(document);

        env.fromElements(status)
                .returns(Status.class)
                .map(status1 -> new KafkaRecord<Status>("Status",status1,System.currentTimeMillis()))
                .returns(new TypeHint<KafkaRecord<Status>>() {
                    @Override
                    public TypeInformation<KafkaRecord<Status>> getTypeInfo() {
                        return super.getTypeInfo();
                    }
                })
        .addSink(
                new FlinkKafkaProducer<KafkaRecord<Status>>(
                        sinkTopicName,
                        new KafkaMessageSerializeSchema<Status>(sinkTopicName),
                        kafkaTopicProp,
                        FlinkKafkaProducer.Semantic.EXACTLY_ONCE));


        env.execute("produceStatusMessge");


    }
}
