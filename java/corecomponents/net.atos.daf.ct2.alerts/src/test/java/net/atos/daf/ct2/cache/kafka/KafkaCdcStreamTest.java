package net.atos.daf.ct2.cache.kafka;

import net.atos.daf.ct2.cache.kafka.impl.KafkaCdcImpl;
import net.atos.daf.ct2.models.kafka.AlertCdc;
import org.apache.flink.api.common.functions.FlatMapFunction;
import org.apache.flink.api.common.typeinfo.Types;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.DataStreamSource;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.source.SourceFunction;
import org.apache.flink.streaming.api.operators.StreamFlatMap;
import org.apache.flink.streaming.api.operators.StreamSource;
import org.apache.flink.streaming.util.KeyedOneInputStreamOperatorTestHarness;
import org.apache.flink.streaming.util.OneInputStreamOperatorTestHarness;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.MockitoAnnotations;
import org.mockito.runners.MockitoJUnitRunner;

import java.io.File;

import static org.junit.Assert.*;

@RunWith(MockitoJUnitRunner.class)
public class KafkaCdcStreamTest {

    @InjectMocks
    private KafkaCdcImpl kafkaCdcStream;
    private ParameterTool parameterTool;
    @Mock
    private StreamExecutionEnvironment env;

    @Mock
    private DataStreamSource<Object> dataStream;


    @Before
    public void setUp() throws Exception {
        File resourcesFile = new File("src/test/resources/application-local-test.properties");
        parameterTool = ParameterTool.fromPropertiesFile(resourcesFile);
        env = StreamExecutionEnvironment.getExecutionEnvironment();
        kafkaCdcStream = new KafkaCdcImpl(env, parameterTool);

        MockitoAnnotations.initMocks(this);
    }

    @Test
    public void init() throws Exception {
        Mockito.when(env.addSource(Mockito.any()))
                .thenReturn((DataStreamSource<Object>) dataStream);
    }

    @Test
    public void processCdcPayload() {
    }

    @Test
    public void flatternCdcStream() {
    }

    @Test
    public void cdcStream() {
    }
}