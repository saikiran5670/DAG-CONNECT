package net.atos.daf.ct2.common.processing;

import static net.atos.daf.ct2.common.util.Utils.convertDateToMillis;

import java.io.Serializable;
import java.util.Objects;

import org.apache.flink.api.common.state.MapState;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.windows.GlobalWindow;
import org.apache.flink.util.Collector;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.ct2.pojo.standard.Index;



public class LivefleetPositionCountCalculation extends ProcessWindowFunction<Index, Index, String, GlobalWindow>
		implements Serializable {

	
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(LivefleetPositionCountCalculation.class);
	private MapState<String, DriverPreviousData> previousDriverRecord;
	ParameterTool envParam = null;
	DriverPreviousData pData;

	@Override
	public void process(String key, ProcessWindowFunction<Index, Index, String, GlobalWindow>.Context arg1,
			Iterable<Index> values, Collector<Index> out) throws Exception {
		// TODO Auto-generated method stub
		try {
			//System.out.println("Inside calculation method");
			for (Index indexData : values) {
				DriverPreviousData driverPreviousData = previousDriverRecord.get(key);
				if (driverPreviousData == null) {
					pData = new DriverPreviousData();
					pData.setStarDate(convertDateToMillis(indexData.getEvtDateTime()));
					pData.setDriveTime(0);
					pData.setTripId(indexData.getDocument().getTripID());
					previousDriverRecord.put(key, pData);
				} else {
					if (Objects.nonNull(previousDriverRecord)) {
						// add Logger

						if (!pData.getTripId().equals(indexData.getDocument().getTripID())) {
							pData.setStarDate(convertDateToMillis(indexData.getEvtDateTime()));
							pData.setDriveTime(0);
							pData.setTripId(indexData.getDocument().getTripID());
							previousDriverRecord.put(key, pData);
						} else {

						Long currentEvtDateTime = convertDateToMillis(indexData.getEvtDateTime());
						Long drivingTime = currentEvtDateTime - pData.getStarDate() + pData.getDriveTime()
								- indexData.getVIdleDuration();
						
						indexData.setNumSeq(drivingTime); // here we are using this field to store drive time

						
						// Index positionDetail= crateIndexObject(indexData, drivingTime);
						//System.out.println("data ready for insert");
						logger.info("data is processed to insert" + indexData.toString());
						}

					}

				}
				out.collect(indexData);
			}

		} catch (Exception e) {
			//System.out.println("inside catch");
			logger.error("error in processing of LivefleetPosition" +e.getMessage());
			e.printStackTrace();
		}

	}

	@Override
	public void open(org.apache.flink.configuration.Configuration config) {
		envParam = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		MapStateDescriptor<String, DriverPreviousData> descriptor = new MapStateDescriptor<String, DriverPreviousData>(
				"modelState", TypeInformation.of(String.class), TypeInformation.of(DriverPreviousData.class));
		previousDriverRecord = getRuntimeContext().getMapState(descriptor);
	}

	/*public Index crateIndexObject(Index indexData, Long drivingTime) {
		System.out.println("inside calculation population method");
		Index positionDetail = new Index();
		IndexDocument doc = new IndexDocument();
		doc.setDriver1RemainingDrivingTime(drivingTime); // Its driver driving time
		positionDetail.setDocument(doc);

		positionDetail.setEvtDateTime(null);
		// positionDetail.

		return positionDetail;

	} */

}
