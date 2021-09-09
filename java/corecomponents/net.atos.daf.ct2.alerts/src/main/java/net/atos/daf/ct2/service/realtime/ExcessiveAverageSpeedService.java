package net.atos.daf.ct2.service.realtime;

import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.StreamSupport;

public class ExcessiveAverageSpeedService extends ProcessWindowFunction<Index, Index, String, TimeWindow>
		implements Serializable {
	private static final Logger logger = LoggerFactory.getLogger(ExcessiveAverageSpeedService.class);
	private static final long serialVersionUID = 1L;

	@Override
	public void process(String arg0, ProcessWindowFunction<Index, Index, String, TimeWindow>.Context arg1,
			Iterable<Index> indexMsg, Collector<Index> arg3) throws Exception {
		try {

			List<Index> indexList = StreamSupport.stream(indexMsg.spliterator(), false).collect(Collectors.toList());
			if (!indexList.isEmpty()) {
				logger.info("list size :{}", indexList.size());
				net.atos.daf.ct2.models.Index idx = new net.atos.daf.ct2.models.Index();
				Index startIndex = indexList.get(0);
				idx.setVin(startIndex.getVin());
				idx.setVid(startIndex.getVid());
				idx.setDocument(startIndex.getDocument());
				idx.getIndexList().add(startIndex);

				if (indexList.size() == 1) {

					if (null != indexList.get(0).getDocument().getVTachographSpeed()) {

						Double techoSpeed = Double.valueOf(indexList.get(0).getDocument().getVTachographSpeed());
						Double averageSpeed = (techoSpeed * 1000 / 3600);

						// rounded value till 2 digits
						double roundOffAverageSpeed = (double) Math.round(averageSpeed * 100) / 100;

						idx.setAverageSpeed(roundOffAverageSpeed);
						logger.info("Final averageSpeed--" + idx.getAverageSpeed());
					} else {
						idx.setAverageSpeed(0.0);
					}
					idx.setIdleDuration(indexList.get(0).getVIdleDuration());
				} else {

					Index endIndex = indexList.get(indexList.size() - 1);
					logger.info("startIndex: {}", startIndex);
					logger.info("endIndex: {}", endIndex);
					Double average = Utils.calculateAverage(startIndex, endIndex);
					idx.setAverageSpeed(average);
					Long idleDuration = Utils.calculateIdleDuration(indexMsg);
					idx.setIdleDuration(idleDuration);
				}

				arg3.collect(idx);
			}
		} catch (Exception e) {
			logger.error("Issue while preparing data for ExcessiveAvgSpeed :{} , error {}", indexMsg, e);
		}
	}
}
