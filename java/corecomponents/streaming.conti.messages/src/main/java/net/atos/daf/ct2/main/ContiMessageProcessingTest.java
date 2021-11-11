package net.atos.daf.ct2.main;

import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.postgre.VehicleStatusSource;
import net.atos.daf.ct2.processing.ConsumeSourceStream;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.FileReader;
import java.io.IOException;
import java.util.Properties;

import static net.atos.daf.ct2.constant.DAFCT2Constant.*;
import static net.atos.daf.ct2.constant.DAFCT2Constant.AUTO_OFFSET_RESET_CONFIG;
import static net.atos.daf.ct2.constant.DAFCT2Constant.SOURCE_TOPIC_NAME;

public class ContiMessageProcessingTest {

    public static String FILE_PATH;
    private static StreamExecutionEnvironment streamExecutionEnvironment;
    private static final long serialVersionUID = 1L;
    private static final Logger logger = LoggerFactory.getLogger(ContiMessageProcessingTest.class);

    public static Properties configuration() throws DAFCT2Exception {

        Properties properties = new Properties();
        streamExecutionEnvironment = StreamExecutionEnvironment.getExecutionEnvironment();
        try {
            properties.load(new FileReader(FILE_PATH));
            properties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, properties.getProperty(AUTO_OFFSET_RESET_CONFIG));
            System.out.println("Configuration Loaded for Connecting Kafka inorder to Perform Mapping.");
        } catch (IOException e) {
            System.err.println("Unable to Find the File " + FILE_PATH);
            throw new DAFCT2Exception("Unable to Find the File " + FILE_PATH, e);
        }
        return properties;
    }

    public static void main(String[] args) {
        ContiMessageProcessing contiMessageProcessing = new ContiMessageProcessing();
        Properties properties = null;
        try{
            FILE_PATH = "src/main/resources/applications-dev1.properties";
            properties = configuration();
            contiMessageProcessing.flinkConnection(properties);

           /* ConsumeSourceStream consumeSrcStream = new ConsumeSourceStream();
            DataStream<KafkaRecord<String>> kafkaRecordDataStream = consumeSrcStream.consumeSourceInputStream(
                    streamExecutionEnvironment, SINK_INDEX_TOPIC_NAME, properties);

            kafkaRecordDataStream.map(
                    data -> {logger.info("Recived msg : {}",data.toString());return data;}
            ).returns(TypeInformation.of(new TypeHint<KafkaRecord<String>>() {
                @Override
                public TypeInformation<KafkaRecord<String>> getTypeInfo() {
                    return super.getTypeInfo();
                }
            }));
            kafkaRecordDataStream.print();*/

            streamExecutionEnvironment.addSource(new VehicleStatusSource(properties))
                            .print();

            streamExecutionEnvironment.execute("ContiMessageProcessing");

        }catch (Exception ex){
            ex.printStackTrace();
        }
    }
}
