package net.atos.daf.ct2.etl.common.processing;

import java.io.Serializable;

import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.functions.co.BroadcastProcessFunction;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.etl.common.bo.FuelCoEfficient;
import net.atos.daf.ct2.etl.common.bo.TripAggregatedData;
import net.atos.daf.ct2.etl.common.bo.TripStatusData;


public class TripCalculations implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(TripCalculations.class);
	
	public SingleOutputStreamOperator<TripAggregatedData> calculateTripStatistics(
			 DataStream<TripStatusData> statusDataStream,
			 BroadcastStream<FuelCoEfficient> fuelCoffBroadcast) {


		return  statusDataStream.connect(fuelCoffBroadcast)
					.process(new BroadcastProcessFunction<TripStatusData, FuelCoEfficient, TripAggregatedData>() {
						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;
						
						private final MapStateDescriptor<String, Double> fuelCoffMapStateDescriptor = new MapStateDescriptor<String, Double>("fuelCoffState",
								TypeInformation.of(String.class), TypeInformation.of(Double.class));  
					  	
						@Override
						public void processElement(TripStatusData value, ReadOnlyContext ctx,
								Collector<TripAggregatedData> out) {

							TripAggregatedData tripStsAggr = null;
							try {
															
								tripStsAggr = new TripAggregatedData();
								tripStsAggr.setVid(value.getVid());
								ReadOnlyBroadcastState<String, Double> mapBrodcast = ctx.getBroadcastState(fuelCoffMapStateDescriptor);
								
								if(mapBrodcast.contains(value.getFuelType())){
									logger.info("Inside FuelCoefficientCalculation fueltpe:{},  coff is: {} ",value.getFuelType(), mapBrodcast.get(value.getFuelType()));
									tripStsAggr.setTripCalC02Emission((mapBrodcast.get(value.getFuelType()) * (value.getVStopFuel() - value.getVStartFuel())) / 1000000);
								}else{
									tripStsAggr.setTripCalC02Emission(Double.valueOf(0));
								}
						
								
								tripStsAggr.setTripId(value.getTripId());
								tripStsAggr.setVid(value.getVid());
								tripStsAggr.setVin(value.getVin());
								tripStsAggr.setStartDateTime(value.getStartDateTime());
								tripStsAggr.setEndDateTime(value.getEndDateTime());
								tripStsAggr.setGpsTripDist(value.getGpsTripDist());
								tripStsAggr.setTripCalDist(value.getTripCalGpsVehDistDiff());
								tripStsAggr.setVIdleDuration(value.getVIdleDuration());
								tripStsAggr.setVTripIdlePTODuration(value.getVTripIdlePTODuration());
								tripStsAggr.setVTripIdleWithoutPTODuration(value.getVTripIdleWithoutPTODuration());
								tripStsAggr.setRoName(value.getRoName());
								
								if(0 != value.getTripCalGpsVehTimeDiff() ){
									tripStsAggr.setTripCalAvgSpeed(Double.valueOf(value.getTripCalGpsVehDistDiff()) / value.getTripCalGpsVehTimeDiff() );
									tripStsAggr.setTripCalPtoDuration((Double.valueOf(value.getVPTODuration()) / (value.getTripCalGpsVehTimeDiff() * 0.001)) * 100 );
								}else{
									tripStsAggr.setTripCalAvgSpeed(Double.valueOf(0));
									tripStsAggr.setTripCalPtoDuration(Double.valueOf(value.getVPTODuration()));
								}
														
								tripStsAggr.setGpsStartVehDist(value.getGpsStartVehDist());
								tripStsAggr.setGpsStopVehDist(value.getGpsStopVehDist());
								tripStsAggr.setGpsStartLatitude(value.getGpsStartLatitude());
								tripStsAggr.setGpsStartLongitude(value.getGpsStartLongitude());
								tripStsAggr.setGpsEndLatitude(value.getGpsEndLatitude());
								tripStsAggr.setGpsEndLongitude(value.getGpsEndLongitude());
								tripStsAggr.setVUsedFuel(value.getVUsedFuel());
								tripStsAggr.setTripCalUsedFuel(value.getVStopFuel() - value.getVStartFuel());
								tripStsAggr.setVTripMotionDuration(value.getVTripMotionDuration());
								tripStsAggr.setTripCalDrivingTm(value.getTripCalGpsVehTimeDiff()/1000 - value.getVIdleDuration());
								tripStsAggr.setReceivedTimestamp(value.getReceivedTimestamp());
								
								if(0 != value.getTripCalGpsVehDistDiff()){
									tripStsAggr.setTripCalFuelConsumption((Double.valueOf(value.getVUsedFuel()) / value.getTripCalGpsVehDistDiff() ) * 100);
									tripStsAggr.setTripCalAvgTrafficClsfn(Double.valueOf(value.getVTripDPABrakingCount() +value.getVTripDPAAnticipationCount())/value.getTripCalGpsVehDistDiff());
								}else{
									tripStsAggr.setTripCalFuelConsumption(Double.valueOf(value.getVUsedFuel()));
									tripStsAggr.setTripCalAvgTrafficClsfn(Double.valueOf(value.getVTripDPABrakingCount() +value.getVTripDPAAnticipationCount()));
								}
									
								if(0 != value.getVBrakeDuration()){
									tripStsAggr.setTripCalHarshBrakeDuration((Double.valueOf(value.getVHarshBrakeDuration()) / value.getVBrakeDuration() ) * 100);
								}else
									tripStsAggr.setTripCalHarshBrakeDuration(Double.valueOf(value.getVHarshBrakeDuration()));
								
								if(0 != value.getVTripAccelerationTime()){
									tripStsAggr.setTripCalHeavyThrottleDuration((Double.valueOf(value.getVMaxThrottlePaddleDuration()) / value.getVTripAccelerationTime() ) * 100);
								}else
									tripStsAggr.setTripCalHeavyThrottleDuration(Double.valueOf(value.getVMaxThrottlePaddleDuration()));
															
								tripStsAggr.setTripCalCrsCntrlDist25To50(value.getTripCalCrsCntrlDist25To50());
								tripStsAggr.setTripCalCrsCntrlDist50To75(value.getTripCalCrsCntrlDist50To75());
								tripStsAggr.setTripCalCrsCntrlDistAbv75(value.getTripCalCrsCntrlDistAbv75());
								
								if(0 != value.getVCruiseControlDist()){
									tripStsAggr.setTripCalCCFuelConsumption((Double.valueOf(value.getVCruiseControlFuelConsumed()) / value.getVCruiseControlDist() ) * 100);
								}else
									tripStsAggr.setTripCalCCFuelConsumption(Double.valueOf(value.getVCruiseControlFuelConsumed()));

								tripStsAggr.setVCruiseControlFuelConsumed(value.getVCruiseControlFuelConsumed());
								tripStsAggr.setVCruiseControlDist(value.getVCruiseControlDist());
								tripStsAggr.setVIdleFuelConsumed(value.getVIdleFuelConsumed());
								tripStsAggr.setKafkaProcessingTS(value.getKafkaProcessingTS());
																
								if(0 != (value.getTripCalGpsVehDistDiff() - value.getVCruiseControlDist())){
									double ccFuelConsumption = 0;
									if(0 != value.getVCruiseControlDist())
										ccFuelConsumption = Double.valueOf(value.getVCruiseControlFuelConsumed())/value.getVCruiseControlDist();
									else
										ccFuelConsumption = value.getVCruiseControlDist();
									
									tripStsAggr.setTripCalfuelNonActiveCnsmpt(((value.getVUsedFuel() - ccFuelConsumption) / (value.getTripCalGpsVehDistDiff() - value.getVCruiseControlDist())) * 100 );
								}else
									tripStsAggr.setTripCalfuelNonActiveCnsmpt(Double.valueOf(0));
								
								tripStsAggr.setDriverId(value.getDriverId());
								tripStsAggr.setTripCalGpsVehTime(value.getTripCalGpsVehTimeDiff());
								tripStsAggr.setTripProcessingTS(value.getTripProcessingTS());
								tripStsAggr.setEtlProcessingTS(value.getEtlProcessingTS());
								tripStsAggr.setNumberOfIndexMessage(value.getNumberOfIndexMessage());
								tripStsAggr.setVTripDPABrakingCount(value.getVTripDPABrakingCount());
								tripStsAggr.setVTripDPAAnticipationCount(value.getVTripDPAAnticipationCount());
								tripStsAggr.setVSumTripDPABrakingScore(value.getVSumTripDPABrakingScore());
								tripStsAggr.setVSumTripDPAAnticipationScore(value.getVSumTripDPAAnticipationScore());
								tripStsAggr.setVStopFuel(value.getVStopFuel());
								tripStsAggr.setVStartFuel(value.getVStartFuel());
								tripStsAggr.setVHarshBrakeDuration(value.getVHarshBrakeDuration());
								tripStsAggr.setVBrakeDuration(value.getVBrakeDuration());
								tripStsAggr.setVMaxThrottlePaddleDuration(value.getVMaxThrottlePaddleDuration());
								tripStsAggr.setVTripAccelerationTime(value.getVTripAccelerationTime());
								tripStsAggr.setVPTODuration(value.getVPTODuration());
								tripStsAggr.setRpmTorque(value.getRpmTorque());
								tripStsAggr.setAbsRpmTorque(value.getAbsRpmTorque());
								tripStsAggr.setOrdRpmTorque(value.getOrdRpmTorque());
								tripStsAggr.setNonZeroRpmTorqueMatrix(value.getNonZeroRpmTorqueMatrix());
								tripStsAggr.setNumValRpmTorque(value.getNumValRpmTorque());
								tripStsAggr.setClmnIdnxRpmTorque(value.getClmnIdnxRpmTorque());
								
								tripStsAggr.setRpmSpeed(value.getRpmSpeed());
								tripStsAggr.setAbsRpmSpeed(value.getAbsRpmSpeed());
								tripStsAggr.setOrdRpmSpeed(value.getOrdRpmSpeed());
								tripStsAggr.setNonZeroRpmSpeedMatrix(value.getNonZeroRpmSpeedMatrix());
								
								tripStsAggr.setNumValRpmSpeed(value.getNumValRpmSpeed());
								tripStsAggr.setClmnIdnxRpmSpeed(value.getClmnIdnxRpmSpeed());
								
								tripStsAggr.setAclnSpeed(value.getAclnSpeed());
								tripStsAggr.setAbsAclnSpeed(value.getAbsAclnSpeed());
								tripStsAggr.setOrdAclnSpeed(value.getOrdAclnSpeed());
								tripStsAggr.setNonZeroAclnSpeedMatrix(value.getNonZeroAclnSpeedMatrix());
								
								tripStsAggr.setNonZeroBrakePedalAclnSpeedMatrix(value.getNonZeroBrakePedalAclnSpeedMatrix());
								tripStsAggr.setNumValAclnSpeed(value.getNumValAclnSpeed());
								tripStsAggr.setClmnIdnxAclnSpeed(value.getClmnIdnxAclnSpeed());
								
								double brakingScore = 0;
								double anticipationScore = 0;
								if(0 != value.getVTripDPABrakingCount()){
									brakingScore = Double.valueOf(value.getVSumTripDPABrakingScore()) / value.getVTripDPABrakingCount();
								}
								
								if(0 != value.getVTripDPAAnticipationCount()){
									anticipationScore = Double.valueOf(value.getVSumTripDPAAnticipationScore()) / value.getVTripDPAAnticipationCount();
								}
							
								tripStsAggr.setTripCalDpaScore((brakingScore + anticipationScore)/2);
								
								tripStsAggr.setVTripIdlePTOFuelConsumed(value.getVTripIdlePTOFuelConsumed());
								tripStsAggr.setVPtoDist(value.getVPtoDist());
								
								if(0 != value.getVPtoDist())
									tripStsAggr.setIdlingConsumptionWithPTO((Double.valueOf(value.getVTripIdlePTOFuelConsumed())/value.getVPtoDist()) *100);
								else
									tripStsAggr.setIdlingConsumptionWithPTO(Double.valueOf(value.getVTripIdlePTOFuelConsumed()));
									
								tripStsAggr.setVTripCruiseControlDuration(value.getVTripCruiseControlDuration());
								tripStsAggr.setVTripIdleWithoutPTOFuelConsumed(value.getVTripIdleWithoutPTOFuelConsumed());
								tripStsAggr.setVTripMotionFuelConsumed(value.getVTripMotionFuelConsumed());
								tripStsAggr.setVTripMotionBrakeCount(value.getVTripMotionBrakeCount());
								tripStsAggr.setVTripMotionBrakeDist(value.getVTripMotionBrakeDist());
								tripStsAggr.setVTripMotionPTODuration(value.getVTripMotionPTODuration());
								tripStsAggr.setVTripMotionPTOFuelConsumed(value.getVTripMotionPTOFuelConsumed());
								tripStsAggr.setAclnPedalDistr(value.getAclnPedalDistr());
								tripStsAggr.setAclnMinRangeInt(value.getAclnMinRangeInt());
								tripStsAggr.setAclnMaxRangeInt(value.getAclnMaxRangeInt());
								tripStsAggr.setAclnDistrStep(value.getAclnDistrStep());
								tripStsAggr.setAclnDistrArrayTime(value.getAclnDistrArrayTime());
								tripStsAggr.setVRetarderTorqueActualDistr(value.getVRetarderTorqueActualDistr());
								tripStsAggr.setVRetarderTorqueMinRangeInt(value.getVRetarderTorqueMinRangeInt());
								tripStsAggr.setVRetarderTorqueMaxRangeInt(value.getVRetarderTorqueMaxRangeInt());
								tripStsAggr.setVRetarderTorqueDistrStep(value.getVRetarderTorqueDistrStep());
								tripStsAggr.setVRetarderTorqueDistrArrayTime(value.getVRetarderTorqueDistrArrayTime());
								tripStsAggr.setVEngineLoadAtEngineSpeedDistr(value.getVEngineLoadAtEngineSpeedDistr());
								tripStsAggr.setVEngineLoadMinRangeInt(value.getVEngineLoadMinRangeInt());
								tripStsAggr.setVEngineLoadMaxRangeInt(value.getVEngineLoadMaxRangeInt());
								tripStsAggr.setVEngineLoadDistrStep(value.getVEngineLoadDistrStep());
								tripStsAggr.setVEngineLoadDistrArrayTime(value.getVEngineLoadDistrArrayTime());

								

							} catch(Exception e) {
								logger.error(" Issue in TripCalculations while processing : {}",value);
								logger.error(" Issue in TripCalculations exception : {}",e);
							}
							logger.info("In Fuel Coeff calculation,  tripStsAggr :{}",tripStsAggr);
							out.collect(tripStsAggr);
						}
						@Override
						public void processBroadcastElement(FuelCoEfficient value, Context ctx,
								Collector<TripAggregatedData> out) throws Exception {
							logger.info("Broadcast FuelCoefficientCalculation updated from history :" + value);
							ctx.getBroadcastState(fuelCoffMapStateDescriptor).put( value.getFuelType(), value.getFuelCoefficient());
						}
					});
					
		}
		
	
}
