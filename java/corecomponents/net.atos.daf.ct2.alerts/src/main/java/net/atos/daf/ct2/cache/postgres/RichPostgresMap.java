package net.atos.daf.ct2.cache.postgres;

import org.apache.flink.api.common.functions.RichMapFunction;
import org.apache.flink.api.java.utils.ParameterTool;

import java.io.Serializable;

public abstract class RichPostgresMap<T,R> extends RichMapFunction<T, R> implements Serializable {

    private static final long serialVersionUID = 1L;

    protected ParameterTool parameterTool;

    public RichPostgresMap(){

    }

    public RichPostgresMap(ParameterTool parameterTool){
      this.parameterTool=parameterTool;
    }

}
