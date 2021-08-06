package net.atos.daf.ct2.cache.postgres;


import net.atos.daf.ct2.models.Payload;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.DataStreamSource;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;

import java.io.Serializable;

public abstract class TableStream<T> implements Serializable {


    protected StreamExecutionEnvironment env;
    protected ParameterTool parameters;

    public DataStreamSource<T> scanTable(String fetchQuery, TypeInformation<?>[] typeInfo){ return  null;}

    public abstract  DataStreamSource<T> scanTable(String fetchQuery, TypeInformation<?>[] typeInfo,String jdbcUrl);

    public abstract DataStream<Payload> joinTable(DataStreamSource<T> first, DataStreamSource<T> second);

    public TableStream(){

    }

    public TableStream(StreamExecutionEnvironment env){
        this.env=env;
    }

    public TableStream(ParameterTool parameters){
        this.parameters=parameters;
    }

    public TableStream(StreamExecutionEnvironment env, ParameterTool parameters) {
        this.env = env;
        this.parameters = parameters;
    }
}
