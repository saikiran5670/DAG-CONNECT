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

public class CurrentTripStatisticsCountCalculation extends ProcessWindowFunction<Index, Index, String, GlobalWindow>
implements Serializable {
	
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(CurrentTripStatisticsCountCalculation.class);
	private MapState<String, TripPreviosData> tripPreviousRecord1;
	ParameterTool envParam = null;
	TripPreviosData pData;

	@Override
	public void process(String key, ProcessWindowFunction<Index, Index, String, GlobalWindow>.Context arg1,
			Iterable<Index> values, Collector<Index> out) throws Exception {
		// TODO Auto-generated method stub
		try {
			//System.out.println("Inside calculation method");
			for (Index indexData : values) {
				TripPreviosData tripPreviousData = tripPreviousRecord1.get(key);
				if (tripPreviousData == null) {
					pData = new TripPreviosData();
					pData.setEndTimeStamp(convertDateToMillis(indexData.getEvtDateTime()));
					pData.setOdometerValue(indexData.getVDist());
					pData.setDrivingTime(0L);
					pData.setTripDistance(0);
					
					tripPreviousRecord1.put(key, pData);
				} else {
					if (Objects.nonNull(tripPreviousRecord1)) {
						// add Logger

						/*if (!pData.getTripId().equals(indexData.getDocument().getTripID())) {
							pData.setStarDate(convertDateToMillis(indexData.getEvtDateTime()));
							pData.setDriveTime(0);
							pData.setTripId(indexData.getDocument().getTripID());
							previousDriverRecord.put(key, pData);
						} else {*/

						//Driving Time Calculation
						Long currentEvtDateTime = convertDateToMillis(indexData.getEvtDateTime());
						Long drivingTime = currentEvtDateTime - pData.getEndTimeStamp() - indexData.getVIdleDuration() + pData.getDrivingTime();
						pData.setEndTimeStamp(currentEvtDateTime);
						pData.setDrivingTime(drivingTime);
						
						indexData.setNumSeq(drivingTime); // here we are using this field to store drive time
						
						//Distance Calculation
						if(indexData.getVDist()!=null) {
						Long distanceCovered= indexData.getVDist()- pData.getOdometerValue() + pData.getTripDistance();
						indexData.setVCumulatedFuel(distanceCovered);  // here we are using this field to store distance
						pData.setTripDistance(distanceCovered.intValue());
						pData.setOdometerValue(indexData.getVDist());
						}
						
						// Index positionDetail= crateIndexObject(indexData, drivingTime);
						System.out.println("data ready for insert");
						logger.info("data is processed to insert" + indexData.toString());
						}

					//}

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

	MapStateDescriptor<String, TripPreviosData> descriptor = new MapStateDescriptor<String, TripPreviosData>(
			"modelState", TypeInformation.of(String.class), TypeInformation.of(TripPreviosData.class));
	tripPreviousRecord1 = getRuntimeContext().getMapState(descriptor);
}

}
