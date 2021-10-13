package net.atos.daf.ct2.common.util;

import net.atos.daf.ct2.common.realtime.dataprocess.MonitorDataProcess;
import org.apache.flink.streaming.api.datastream.DataStreamSource;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import static org.junit.Assert.*;

public class FlinkUtilTest {

    Logger logger = LoggerFactory.getLogger(FlinkUtilTest.class);
    private StreamExecutionEnvironment env;

    @Before
    public void init(){
         env = StreamExecutionEnvironment.getExecutionEnvironment();
    }

    @Test
    public void createStreamExecutionEnvironment() {
        System.out.println("Testing started.......");

    }

    @After
    public void execute() throws Exception {
        env.execute();
    }
}