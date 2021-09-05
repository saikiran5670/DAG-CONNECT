package net.atos.daf.ct2.service.realtime;

import net.atos.daf.ct2.pojo.standard.Index;
import org.apache.flink.api.common.state.MapState;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.configuration.Configuration;
import org.apache.flink.streaming.api.functions.ProcessFunction;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.math.BigDecimal;
import java.util.Objects;

public class FuelDuringStopProcessor extends ProcessFunction<Index, Index> implements Serializable {

    private static final Logger logger = LoggerFactory.getLogger(FuelDuringStopProcessor.class);
    private static final long serialVersionUID = 1L;

    private MapState<String, BigDecimal> fuelStopState;


    @Override
    public void processElement(Index index, ProcessFunction<Index, Index>.Context context, Collector<Index> collector) throws Exception {

        BigDecimal fuelPrevState = fuelStopState.get(index.getVin());
        if (fuelPrevState == null) {
        	if(Objects.nonNull(index.getDocument()) && Objects.nonNull(index.getDocument().getVFuelLevel1())){
        		fuelStopState.put(index.getVin(), BigDecimal.valueOf(index.getDocument().getVFuelLevel1()));
                logger.info("1st time state inserted {} vin : {}",BigDecimal.valueOf(index.getDocument().getVFuelLevel1()),index.getVin());
        	}
            
        } else {
            net.atos.daf.ct2.models.Index index1 = new net.atos.daf.ct2.models.Index();
            index1.setVid(index.getVid());
            index1.setVin(index.getVin());
            index1.setVEvtID(index.getVEvtID());
            index1.setVFuelStopPrevVal(fuelPrevState);
            index1.getIndexList().add(index);
            if(Objects.nonNull(index.getDocument()) && Objects.nonNull(index.getDocument().getVFuelLevel1())){
        		fuelStopState.put(index.getVin(), BigDecimal.valueOf(index.getDocument().getVFuelLevel1()));
        		logger.info("state upadted {} vin : {}",BigDecimal.valueOf(index.getDocument().getVFuelLevel1()),index.getVin());
        	}
                
            collector.collect(index1);
        }

    }

    @Override
    public void open(Configuration config) {
        MapStateDescriptor<String, BigDecimal> descriptor = new MapStateDescriptor<String, BigDecimal>("modelState",
                TypeInformation.of(String.class), TypeInformation.of(BigDecimal.class));
        fuelStopState = getRuntimeContext().getMapState(descriptor);
    }
}
