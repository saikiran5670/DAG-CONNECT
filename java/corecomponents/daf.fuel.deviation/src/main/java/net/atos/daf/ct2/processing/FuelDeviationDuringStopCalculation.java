package net.atos.daf.ct2.processing;

import java.math.BigDecimal;
import java.util.Objects;

import org.apache.flink.api.common.state.MapState;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.util.Collector;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.ct2.bo.FuelDeviation;
import net.atos.daf.ct2.bo.FuelDeviationData;
import net.atos.daf.ct2.util.FuelDeviationConstants;

public class FuelDeviationDuringStopCalculation extends ProcessWindowFunction<FuelDeviationData, FuelDeviation, String, TimeWindow> {
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(FuelDeviationDuringStopCalculation.class);
	ParameterTool envParam = null;
	private MapState<String, BigDecimal> fuelStopState;
	
	
	@Override
	public void process(String key, Context ctx, Iterable<FuelDeviationData> values, Collector<FuelDeviation> out) {

		try {
			BigDecimal stopIncreaseThresholdVal = new BigDecimal(envParam.get(FuelDeviationConstants.FUEL_DEVIATION_DURING_STOP_INCREASE_THRESHOLD_VAL));
			BigDecimal stopDecreaseThresholdVal = new BigDecimal(envParam.get(FuelDeviationConstants.FUEL_DEVIATION_DURING_STOP_DECREASE_THRESHOLD_VAL));
			
			for (FuelDeviationData vFuelObj : values) {
				
				BigDecimal vFuelStopPrevVal = fuelStopState.get(key);
				
				if(vFuelStopPrevVal == null){
					fuelStopState.put(key, vFuelObj.getVFuelLevel());
				}else{
					//1st cond is not required cross check 
					if(Objects.nonNull(vFuelStopPrevVal) && Objects.nonNull(vFuelObj.getVFuelLevel())){
					
						if((FuelDeviationConstants.INDEX_TRIP_START).intValue() == vFuelObj.getVEvtId()){
							BigDecimal fuelIncreaseDiff = vFuelObj.getVFuelLevel().subtract(vFuelStopPrevVal);
							logger.info("Fuel Increase Stop Deviation, vin:{}, trip_id :{}, tripStartFuel: {} , vFuelStopPrevVal:{}, fuelIncreaseDiff: {}, stopIncreaseThresholdVal: {}  ", vFuelObj.getVin(), vFuelObj.getTripId(), vFuelObj.getVFuelLevel(), vFuelStopPrevVal, fuelIncreaseDiff, stopIncreaseThresholdVal);

							//1 when fuelIncreaseDiff > threshold
							if(fuelIncreaseDiff.compareTo(stopIncreaseThresholdVal) > 0){
								FuelDeviation fuelIncreaseEvt = createFuelDeviationEvtObj(vFuelObj, FuelDeviationConstants.FUEL_DEVIATION_INCREASE_EVENT, FuelDeviationConstants.FUEL_DEVIATION_STOP_ACTIVITY_TYPE);
								fuelIncreaseEvt.setFuelDiff(fuelIncreaseDiff.doubleValue());
								
								out.collect(fuelIncreaseEvt);
							}
						
							BigDecimal fuelDecreaseDiff = vFuelStopPrevVal.subtract(vFuelObj.getVFuelLevel());
							logger.info("Fuel decrease Stop Deviation, vin:{}, trip_id :{}, tripStartFuel: {} , vFuelStopPrevVal:{},fuelDecreaseDiff:{}, stopDecreaseThresholdVal: {}, vin:{}  ", vFuelObj.getVin(), vFuelObj.getTripId(), vFuelObj.getVFuelLevel(),  vFuelStopPrevVal, fuelDecreaseDiff, stopDecreaseThresholdVal);

							//1 when fuelIncreaseDiff > threshold
							if(fuelDecreaseDiff.compareTo(stopDecreaseThresholdVal) > 0){
								FuelDeviation fuelDecreaseEvt = createFuelDeviationEvtObj(vFuelObj, FuelDeviationConstants.FUEL_DEVIATION_DECREASE_EVENT, FuelDeviationConstants.FUEL_DEVIATION_STOP_ACTIVITY_TYPE);
								fuelDecreaseEvt.setFuelDiff(fuelDecreaseDiff.doubleValue());
								
								out.collect(fuelDecreaseEvt);
							}
						}
						
						fuelStopState.put(key, vFuelObj.getVFuelLevel());
					}
					
				}
			}

		} catch (Exception e) {
			logger.error("Issue while processing Fuel Deviation Data for key : " + key + "  error :: " + e.getMessage());
			e.printStackTrace();
		}
	}

	@Override
	public void open(org.apache.flink.configuration.Configuration config) {
		envParam = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		MapStateDescriptor<String, BigDecimal> descriptor = new MapStateDescriptor<String, BigDecimal>("modelState",
				TypeInformation.of(String.class), TypeInformation.of(BigDecimal.class));
		fuelStopState = getRuntimeContext().getMapState(descriptor);
	}

	private FuelDeviation createFuelDeviationEvtObj(FuelDeviationData vFuelObj, String evtType, String activityType) {
		FuelDeviation fuelDeviationEvt = new FuelDeviation();
		fuelDeviationEvt.setTripId(vFuelObj.getTripId());
		fuelDeviationEvt.setVDist(vFuelObj.getVDist());
		fuelDeviationEvt.setGpsHeading(vFuelObj.getGpsHeading());
		fuelDeviationEvt.setGpsLatitude(vFuelObj.getGpsLatitude());
		fuelDeviationEvt.setGpsLongitude(vFuelObj.getGpsLongitude());
		fuelDeviationEvt.setVid(vFuelObj.getVid());
		fuelDeviationEvt.setVin(vFuelObj.getVin());
		fuelDeviationEvt.setEvtDateTime(vFuelObj.getEvtDateTime());
		fuelDeviationEvt.setFuelEvtType(evtType);
		fuelDeviationEvt.setVehActivityType(activityType);

		return fuelDeviationEvt;
	}

}
