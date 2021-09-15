package net.atos.daf.ct2.service.realtime;


import java.math.BigDecimal;
import java.text.ParseException;
import java.util.Objects;

import org.apache.flink.api.common.state.MapState;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.models.AlertFuelMeasurement;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.props.AlertConfigProp;

public class FuelDuringTripProcessor extends ProcessWindowFunction<Index, Index, String, TimeWindow> {
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(FuelDuringTripProcessor.class);
	ParameterTool envParam = null;

	private MapState<String, AlertFuelMeasurement> fuelMeasurementState;

	@Override
	public void process(String key, Context ctx, Iterable<Index> values, Collector<Index> out) {

		try {
			long timeMeasurementVal = Long.valueOf(envParam.get(AlertConfigProp.ALERT_MEASUREMENT_MILLISECONDS_VAL));

			for (Index msgIndex : values) {

				String vin = null;
				
				if(Objects.nonNull(msgIndex.getVin() ))
					vin = msgIndex.getVin();
				else
					vin = msgIndex.getVid();
				
				AlertFuelMeasurement vFuelPrevTripRecData = fuelMeasurementState.get(vin);
				logger.info("msgIndex:{}, evtId:{}, vFuelPrevTripRecData :{}",msgIndex ,msgIndex.getVEvtID() , vFuelPrevTripRecData );

				if (AlertConfigProp.INDEX_TRIP_START == msgIndex.getVEvtID() || Objects.isNull(vFuelPrevTripRecData)) {
					if (Objects.nonNull(msgIndex.getDocument())
							&& Objects.nonNull(msgIndex.getDocument().getVFuelLevel1())) {
						AlertFuelMeasurement fuelMeasurementAtStart = createFuelMeasurementObj(
								msgIndex.getEvtDateTime(), msgIndex.getDocument().getVFuelLevel1());
						fuelMeasurementState.put(vin, fuelMeasurementAtStart);
						logger.info("1st time state inserted vin: {} fuelMeasurementAtStart: {} ", vin, fuelMeasurementAtStart);
					}
				} else {
					// this condition will never happen
					if (vFuelPrevTripRecData != null) {
						Long measurementTime = TimeFormatter.getInstance()
								.addMilliSecToUTCTime(vFuelPrevTripRecData.getEvtDateTime(), timeMeasurementVal);
						
						logger.info("measurementTime {} , current time:: {}, previousTime:{},  timeMeasurementVal :{}", measurementTime, TimeFormatter.getInstance().convertUTCToEpochMilli(msgIndex.getEvtDateTime(),
								AlertConfigProp.DATE_FORMAT) , vFuelPrevTripRecData.getEvtDateTime(), timeMeasurementVal);
						if (TimeFormatter.getInstance().convertUTCToEpochMilli(msgIndex.getEvtDateTime(),
								AlertConfigProp.DATE_FORMAT) >= measurementTime) {
							// 5 mins measurement
							if (Objects.nonNull(msgIndex.getDocument())
									&& Objects.nonNull(msgIndex.getDocument().getVFuelLevel1())) {
								fuelMeasurementState.put(vin, createFuelMeasurementObj(
										msgIndex.getEvtDateTime(), msgIndex.getDocument().getVFuelLevel1()));

								net.atos.daf.ct2.models.Index index1 = new net.atos.daf.ct2.models.Index();
								index1.setVid(msgIndex.getVid());
								index1.setVin(vin);
								index1.setVEvtID(msgIndex.getVEvtID());
								index1.setVFuelStopPrevVal(vFuelPrevTripRecData.getVFuelLevel());
								index1.getIndexList().add(msgIndex);
								logger.info("State updated Fuel during trip data: {}", index1);
								out.collect(index1);
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
			logger.error("Issue while processing Alert FuelDuringTripProcessor Data for key : " + key + "  error :: " + e.getMessage());
			e.printStackTrace();
		}
	}

	@Override
	public void open(org.apache.flink.configuration.Configuration config) {
		envParam = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		MapStateDescriptor<String, AlertFuelMeasurement> descriptor = new MapStateDescriptor<String, AlertFuelMeasurement>(
				"modelState", TypeInformation.of(String.class), TypeInformation.of(AlertFuelMeasurement.class));
		fuelMeasurementState = getRuntimeContext().getMapState(descriptor);
	}

	private AlertFuelMeasurement createFuelMeasurementObj(String evtDateTime, Double vFuelLevel) throws ParseException {
		AlertFuelMeasurement fuelMeasurement = new AlertFuelMeasurement();
		fuelMeasurement.setEvtDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(evtDateTime, AlertConfigProp.DATE_FORMAT));
		fuelMeasurement.setVFuelLevel(BigDecimal.valueOf(vFuelLevel));
		return fuelMeasurement;
	}

}