package net.atos.daf.ct2.processing;

import java.io.Serializable;
import java.time.Duration;

import org.apache.flink.api.common.eventtime.SerializableTimestampAssigner;
import org.apache.flink.api.common.eventtime.WatermarkStrategy;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.windowing.assigners.TumblingProcessingTimeWindows;
import org.apache.flink.streaming.api.windowing.time.Time;

import net.atos.daf.ct2.bo.TripMileage;
import net.atos.daf.ct2.bo.VehicleMileage;

/**
 * Class water marks every trip message and performs mileage calculation based on the VID
 *
 * @version 1.0
 * @author  A676107
 */
public class MileageProcessing implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	/**
	 * This method assigns water mark to every trip message and performs mileage calculation in the
	 * provided window duration. The mileage calculations are done per vehicleId.
	 *
	 * @param statusDataStream is the incoming trip completion stream from the source system
	 * @param watermarkTime is the time window for WatermarkStrategy
	 * @param mileageTime is the processing time window
	 * @return  mileage trip calculation as SingleOutputStreamOperator<TripMileage> 
	 * 
	 */
	public SingleOutputStreamOperator<TripMileage> mileageDataProcessing(
			SingleOutputStreamOperator<VehicleMileage> statusDataStream, long watermarkTime, long mileageTime) {
		return statusDataStream.assignTimestampsAndWatermarks(
				WatermarkStrategy.<VehicleMileage>forBoundedOutOfOrderness(Duration.ofSeconds(watermarkTime))
						.withTimestampAssigner(new SerializableTimestampAssigner<VehicleMileage>() {

							private static final long serialVersionUID = 1L;

							@Override
							public long extractTimestamp(VehicleMileage element, long recordTimestamp) {
								return element.getEvtDateTime();
							}
						}))
				.keyBy(value -> value.getVin()).window(TumblingProcessingTimeWindows.of(Time.seconds(mileageTime)))
				.process(new MileageDataCalculation());
	}

}
