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

public abstract class KafkaCdcStream implements Serializable {

    protected StreamExecutionEnvironment env;
    private static final long serialVersionUID = 1L;
    protected ParameterTool parameterTool;

    public KafkaCdcStream(StreamExecutionEnvironment env,ParameterTool parameterTool){
        this.env=env;
        this.parameterTool=parameterTool;
    }

    public KafkaCdcStream(){

    }

    protected abstract KeyedStream<AlertCdc, String> init();
    protected abstract SingleOutputStreamOperator<List<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>>> processCdcPayload(KeyedStream<AlertCdc, String> alertCdcStream);
    protected abstract SingleOutputStreamOperator<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>> flatternCdcStream(SingleOutputStreamOperator<List<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>>> listPayloadStream);


    public SingleOutputStreamOperator<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>> cdcStream(){
        KeyedStream<AlertCdc, String> keyedStream = init();
        SingleOutputStreamOperator<List<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>>> payloadStream = processCdcPayload(keyedStream);
        return flatternCdcStream(payloadStream);
    }

}
