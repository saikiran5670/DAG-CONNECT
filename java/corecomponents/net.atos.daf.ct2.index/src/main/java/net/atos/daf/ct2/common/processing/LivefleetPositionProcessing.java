package net.atos.daf.ct2.common.processing;

import static net.atos.daf.ct2.common.util.DafConstants.INCOMING_MESSAGE_UUID;

import java.util.UUID;

import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.ct2.pojo.standard.Index;


public class LivefleetPositionProcessing {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(LivefleetPositionProcessing.class);

	public SingleOutputStreamOperator<Index> liveFleetPosition(SingleOutputStreamOperator<Index> indexStream,
			long livefleetPositionCountWindow) {
		return indexStream.map(index -> {
			index.setJobName(UUID.randomUUID().toString());
			logger.info("index message received for processing :: {}  {}", index,
					String.format(INCOMING_MESSAGE_UUID, index.getJobName()));
			return index;
		})
				.filter(value -> value.getDocument().getTripID()!=null)
				.keyBy(value -> value.getVin()!=null ? value.getVin() : value.getVid())
				.countWindow(livefleetPositionCountWindow)
				.process(new LivefleetPositionCountCalculation());
	}

}
