package net.atos.daf.ct2.service.realtime;

import net.atos.daf.ct2.pojo.standard.Index;
import org.apache.flink.api.common.state.ValueState;
import org.apache.flink.api.common.state.ValueStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.configuration.Configuration;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.StreamSupport;

public class ExcessiveUnderUtilizationProcessor extends ProcessWindowFunction<Index, Index, String, TimeWindow> implements Serializable {

    private static final Logger logger = LoggerFactory.getLogger(ExcessiveUnderUtilizationProcessor.class);
    private static final long serialVersionUID = 1L;

    private transient ValueState<Index> previousTripLastIndex;
    @Override
    public void process(String s, ProcessWindowFunction<Index, Index, String, TimeWindow>.Context context, Iterable<Index> indexMsg, Collector<Index> collector) throws Exception {
        try {
			List<Index> indexList = StreamSupport.stream(indexMsg.spliterator(), false)
			        .collect(Collectors.toList());
			logger.info("ExcessiveUnderUtilization window message size :: {}",indexList.size());
			net.atos.daf.ct2.models.Index idx = new net.atos.daf.ct2.models.Index();
			if(indexList.size() > 1){
			    for(Index index : indexList){
			        idx.setVid(index.getVid());
			        idx.setVin(index.getVin());
			        idx.getIndexList().add(index);
			    }
			    collector.collect(idx);
			}else{
			    /**
			     * If only one index message is found , then retrieve the previous index message.
			     */
			    Index lastTripIndexMsg = previousTripLastIndex.value();
			    Index currentTripIndexMsg = indexList.get(0);
			    idx.setVid(currentTripIndexMsg.getVid());
			    idx.setVin(currentTripIndexMsg.getVin());
			    idx.getIndexList().add(lastTripIndexMsg);
			    idx.getIndexList().add(currentTripIndexMsg);
			    collector.collect(idx);
			    logger.info("Extracted index msg from previous trip {}",lastTripIndexMsg);
			}
			previousTripLastIndex.update(indexList.get(indexList.size()-1));
		} catch (Exception e) {
			// TODO Auto-generated catch block
			logger.info("Issue while preparing data for ExcessiveUnderUtilizationProcessor :{}",indexMsg);
			e.printStackTrace();
		}
    }
    @Override
    public void open(Configuration parameters) throws Exception {
        ValueStateDescriptor<Index> descriptor =  new ValueStateDescriptor<Index>("lastTripIndexMsg", TypeInformation.of(Index.class));
        previousTripLastIndex = getRuntimeContext().getState(descriptor);
    }
}
