package net.atos.daf.ct2.processing;

import java.math.BigDecimal;

import org.apache.flink.api.common.state.MapState;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.util.Collector;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.bo.FuelDeviation;
import net.atos.daf.ct2.bo.FuelMeasurement;
import net.atos.daf.ct2.bo.FuelDeviationData;
import net.atos.daf.ct2.util.FuelDeviationConstants;

public class FuelDeviationDuringTripCalculation
		extends ProcessWindowFunction<FuelDeviationData, FuelDeviation, String, TimeWindow> {
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(FuelDeviationDuringTripCalculation.class);
	ParameterTool envParam = null;
	private MapState<String, FuelMeasurement> fuelMeasurementState;

	@Override
	public void process(String key, Context ctx, Iterable<FuelDeviationData> values, Collector<FuelDeviation> out) {

		try {
			BigDecimal tripIncreaseThresholdVal = new BigDecimal(
					envParam.get(FuelDeviationConstants.FUEL_DEVIATION_DURING_TRIP_INCREASE_THRESHOLD_VAL));
			BigDecimal tripDecreaseThresholdVal = new BigDecimal(
					envParam.get(FuelDeviationConstants.FUEL_DEVIATION_DURING_TRIP_DECREASE_THRESHOLD_VAL));
			long timeMeasurementVal = Long
					.valueOf(envParam.get(FuelDeviationConstants.FUEL_DEVIATION_MEASUREMENT_SECONDS_VAL));

			for (FuelDeviationData vFuelTripObj : values) {

				FuelMeasurement vFuelPrevTripRecData = fuelMeasurementState.get(vFuelTripObj.getVin());

				if (FuelDeviationConstants.INDEX_TRIP_START == vFuelTripObj.getVEvtId()) {
					FuelMeasurement fuelMeasurementAtStart = createFuelMeasurementObj(vFuelTripObj);
					fuelMeasurementState.put(vFuelTripObj.getVin(), fuelMeasurementAtStart);
					
					logger.info("fuelMeasurementAtStart Obj :" + fuelMeasurementAtStart);
				} else {

					if (vFuelPrevTripRecData != null) {
						Long measurementTime = TimeFormatter.getInstance()
								.addSecondsToUTCTime(vFuelPrevTripRecData.getEvtDateTime(), timeMeasurementVal);

						if (vFuelTripObj.getEvtDateTime() >= measurementTime) {
							// 5 mins measurement
							fuelMeasurementState.put(vFuelTripObj.getVin(), createFuelMeasurementObj(vFuelTripObj));
							
							logger.info("fuelMeasurementAfterUpdate Obj :"+ fuelMeasurementState.get(vFuelTripObj.getVin()));
							
							if (vFuelTripObj.getVFuelLevel() != null && vFuelPrevTripRecData.getVFuelLevel() != null) {
								BigDecimal fuelIncreaseDiff = vFuelTripObj.getVFuelLevel()
										.subtract(vFuelPrevTripRecData.getVFuelLevel());
								
								logger.info("Fuel Increase during Trip, tripStartFuel : " + vFuelTripObj.getVFuelLevel()
										+ " vFuelPrevTripRecData : " + vFuelPrevTripRecData.getVFuelLevel()
										+ " fuelIncreaseDeviation : " + fuelIncreaseDiff);

								if (fuelIncreaseDiff.compareTo(BigDecimal.ZERO) > 0
										&& fuelIncreaseDiff.compareTo(tripIncreaseThresholdVal) > 0) {

									FuelDeviation fuelIncreaseEvt = createFuelDeviationEvtObj(vFuelTripObj,
											FuelDeviationConstants.FUEL_DEVIATION_INCREASE_EVENT,
											FuelDeviationConstants.FUEL_DEVIATION_RUNNING_ACTIVITY_TYPE);
									fuelIncreaseEvt.setFuelDiff(fuelIncreaseDiff.doubleValue());
									
									out.collect(fuelIncreaseEvt);
								} else {

									BigDecimal fuelDecreaseDiff = (vFuelPrevTripRecData.getVFuelLevel()
											.subtract(vFuelTripObj.getVFuelLevel()));
									logger.info("Fuel Loss during Trip, tripStartFuel : " + vFuelTripObj.getVFuelLevel()
											+ " vFuelPrevTripRecData : " + vFuelPrevTripRecData.getVFuelLevel()
											+ " fuelLossDeviation : " + fuelDecreaseDiff);

									BigDecimal fuelDecreaseDiffAbsVal = fuelDecreaseDiff.abs();
									if (fuelDecreaseDiffAbsVal.compareTo(tripDecreaseThresholdVal) > 0) {

										FuelDeviation fuelDecreaseEvt = createFuelDeviationEvtObj(vFuelTripObj,
												FuelDeviationConstants.FUEL_DEVIATION_DECREASE_EVENT,
												FuelDeviationConstants.FUEL_DEVIATION_RUNNING_ACTIVITY_TYPE);
										fuelDecreaseEvt.setFuelDiff(fuelDecreaseDiff.doubleValue());
										
										out.collect(fuelDecreaseEvt);
									}
								}
							}

						} else {
							// less than 5 mins ignore
							logger.info("Index message subscription less than 5 minutes ");
						}

					} else {
						logger.info("vFuelPrevTripRecData is not Present ");
					}

				}
			}

		} catch (Exception e) {
			logger.error("Issue while processing FuelDeviation Data for key : " + key + "  error :: " + e.getMessage());
			e.printStackTrace();
		}
	}

	@Override
	public void open(org.apache.flink.configuration.Configuration config) {
		envParam = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		MapStateDescriptor<String, FuelMeasurement> descriptor = new MapStateDescriptor<String, FuelMeasurement>(
				"modelState", TypeInformation.of(String.class), TypeInformation.of(FuelMeasurement.class));
		fuelMeasurementState = getRuntimeContext().getMapState(descriptor);
	}

	private FuelDeviation createFuelDeviationEvtObj(FuelDeviationData vFuelStopObj, String evtType,
			String activityType) {
		FuelDeviation fuelDeviationEvt = new FuelDeviation();
		fuelDeviationEvt.setTripId(vFuelStopObj.getTripId());
		fuelDeviationEvt.setVDist(vFuelStopObj.getVDist());
		fuelDeviationEvt.setGpsHeading(vFuelStopObj.getGpsHeading());
		fuelDeviationEvt.setGpsLatitude(vFuelStopObj.getGpsLatitude());
		fuelDeviationEvt.setGpsLongitude(vFuelStopObj.getGpsLongitude());
		fuelDeviationEvt.setVid(vFuelStopObj.getVid());
		fuelDeviationEvt.setVin(vFuelStopObj.getVin());
		fuelDeviationEvt.setEvtDateTime(vFuelStopObj.getEvtDateTime());
		fuelDeviationEvt.setFuelEvtType(evtType);
		fuelDeviationEvt.setVehActivityType(activityType);

		return fuelDeviationEvt;
	}

	private FuelMeasurement createFuelMeasurementObj(FuelDeviationData vFuelTripObj) {
		FuelMeasurement fuelMeasurement = new FuelMeasurement();
		fuelMeasurement.setEvtDateTime(vFuelTripObj.getEvtDateTime());
		fuelMeasurement.setVFuelLevel(vFuelTripObj.getVFuelLevel());
		return fuelMeasurement;
	}

}
