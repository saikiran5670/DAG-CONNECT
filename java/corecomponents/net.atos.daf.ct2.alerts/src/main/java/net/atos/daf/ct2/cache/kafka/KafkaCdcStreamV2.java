package net.atos.daf.ct2.cache.kafka;

import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.kafka.AlertCdc;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;

import java.io.Serializable;
import java.util.List;

public abstract class KafkaCdcStreamV2 implements Serializable {

    protected StreamExecutionEnvironment env;
    private static final long serialVersionUID = 1L;
    protected ParameterTool parameterTool;

    public KafkaCdcStreamV2(StreamExecutionEnvironment env,ParameterTool parameterTool){
        this.env=env;
        this.parameterTool=parameterTool;
    }

    public KafkaCdcStreamV2(){
    }

    protected abstract KeyedStream<AlertCdc, String> init();
    protected abstract void processCdcPayload(KeyedStream<AlertCdc, String> alertCdcStream);
}
