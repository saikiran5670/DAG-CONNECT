package net.atos.daf.ct2.processing;

import java.io.Serializable;

import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.windowing.assigners.TumblingProcessingTimeWindows;
import org.apache.flink.streaming.api.windowing.time.Time;

import net.atos.daf.ct2.bo.FuelDeviation;
import net.atos.daf.ct2.bo.FuelDeviationData;

public class FuelDeviationProcess implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	public SingleOutputStreamOperator<FuelDeviation> fuelDeviationProcessingDuringStop(
			SingleOutputStreamOperator<FuelDeviationData> fuelStopStream, long fuelDeviationTmWindow) {
		return fuelStopStream.keyBy(value -> value.getVin())
				.window(TumblingProcessingTimeWindows.of(Time.seconds(fuelDeviationTmWindow)))
				.process(new FuelDeviationDuringStopCalculation());
	}

	public SingleOutputStreamOperator<FuelDeviation> fuelDeviationProcessingDuringTrip(
			SingleOutputStreamOperator<FuelDeviationData> fuelTripStream, long fuelDeviationTmWindow) {
		return fuelTripStream.keyBy(value -> value.getTripId())
				.window(TumblingProcessingTimeWindows.of(Time.seconds(fuelDeviationTmWindow)))
				.process(new FuelDeviationDuringTripCalculation());
	}

}
