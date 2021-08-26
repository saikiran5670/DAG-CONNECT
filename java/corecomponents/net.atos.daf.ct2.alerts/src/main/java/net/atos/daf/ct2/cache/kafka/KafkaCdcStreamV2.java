package net.atos.daf.ct2.cache.kafka;

import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.kafka.AlertCdc;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;

import java.io.Serializable;

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
    protected abstract Tuple2<BroadcastStream<VehicleAlertRefSchema>, BroadcastStream<Payload<Object>>> processCdcPayload(KeyedStream<AlertCdc, String> alertCdcStream);


    public Tuple2<BroadcastStream<VehicleAlertRefSchema>, BroadcastStream<Payload<Object>>> bootCache(){
        KeyedStream<AlertCdc, String> init = init();
        return processCdcPayload(init);
    }
}
