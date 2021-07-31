package net.atos.daf.ct2.processing;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.flink.api.common.state.MapState;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.util.Collector;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.bo.TripMileage;
import net.atos.daf.ct2.bo.VehicleMileage;
import net.atos.daf.ct2.util.MileageConstants;

/**
 * Class performs mileage calculation based on the VID
 *
 * @version 1.0
 * @author  A676107
 */
public class MileageDataCalculation extends ProcessWindowFunction<VehicleMileage, TripMileage, String, TimeWindow> {
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(MileageDataCalculation.class);
	ParameterTool envParam = null;
	private MapState<String, Map<Long, VehicleMileage>> modelState;
	private MapState<String, List<Long>> vehEvtTimeListState;

	/**
	 * The actual mileage calculations are performed in the process method.
	 *
	 * @param key mileage calculations are perform per vehicleId 
	 * @param ctx provides access to runtime context
	 * @param values is the list of VehicleMileage objects    
	 * @return out collected list of TripMileage data. 
	 * 
	 */
	@Override
	public void process(String key, Context ctx, Iterable<VehicleMileage> values, Collector<TripMileage> out) {

		TripMileage tripMileage = new TripMileage();
		double odoDistance = 0;
		double gpsDistance = 0;
		long lastBusinessTS = 0;
		List<Long> vehDeleteTripTs = new ArrayList<>();

		try {
			Map<Long, VehicleMileage> vMileageMap = modelState.get(key);
			List<Long> vTimestampList = vehEvtTimeListState.get(key);
			double errorMargin = Double.parseDouble(envParam.get(MileageConstants.MILEAGE_ERROR_MARGIN));

			if (vTimestampList == null)
				vTimestampList = new ArrayList<>();

			for (VehicleMileage vMileageObj : values) {

				if (vMileageObj.getEvtDateTime() != null)
					vTimestampList.add(vMileageObj.getEvtDateTime());

				if (vMileageMap == null) {
					vMileageMap = new HashMap<>();
					if (vMileageObj.getEvtDateTime() != null)
						vMileageMap.put(vMileageObj.getEvtDateTime(), vMileageObj);
				} else {
					if (vMileageObj.getEvtDateTime() != null)
						vMileageMap.put(vMileageObj.getEvtDateTime(), vMileageObj);
				}

				modelState.put(key, vMileageMap);

			}

			Collections.sort(vTimestampList, new Comparator<Long>() {
				@Override
				public int compare(Long timeStmp1, Long timeStmp2) {
					return Long.compare(timeStmp2, timeStmp1);
				}
			});

			lastBusinessTS = TimeFormatter.getInstance().subSecondsFromUTCTime(vTimestampList.get(0),
					Long.parseLong(envParam.get(MileageConstants.MILEAGE_BUSINESS_TIME_WINDOW_SECONDS)));
			VehicleMileage vMileage = vMileageMap.get(vTimestampList.get(0));

			if (vMileage.getVin() != null)
				tripMileage.setVin(vMileage.getVin());
			else
				tripMileage.setVin(vMileage.getVid());

			tripMileage.setOdoMileage(vMileage.getOdoMileage());
			tripMileage.setEvtDateTime(vMileage.getEvtDateTime());

			for (Map.Entry<Long, VehicleMileage> entry : vMileageMap.entrySet()) {
				if (entry.getKey().longValue() > lastBusinessTS) {
					if (entry.getValue().getOdoDistance() != null) {
						odoDistance = odoDistance + entry.getValue().getOdoDistance();
					}

					if (entry.getValue().getGpsDistance() != null) {
						gpsDistance = gpsDistance + entry.getValue().getGpsDistance();
					}
				} else {
					vehDeleteTripTs.add(entry.getKey());
				}
			}
			odoDistance = odoDistance / 1000;
			gpsDistance = gpsDistance / 1000;
			tripMileage.setGpsDistance(gpsDistance);
			tripMileage.setOdoDistance(odoDistance);
			tripMileage.setModifiedAt(TimeFormatter.getInstance().getCurrentUTCTime());

			double realMileage = (odoDistance * errorMargin > gpsDistance) ? odoDistance : gpsDistance;
			tripMileage.setRealDistance(realMileage);

			for (Long vTimestamp : vehDeleteTripTs) {
				logger.info(
						"deleting trip that does not fall under business critera :" + vMileageMap.get(vTimestamp));
				vMileageMap.remove(vTimestamp);
			}
			modelState.put(key, vMileageMap);

			out.collect(tripMileage);
		} catch (Exception e) {
			logger.error("Issue while processing Mileage Data for key : " + key + "  error :: " + e.getMessage());
			e.printStackTrace();
		}
	}

	@Override
	public void open(org.apache.flink.configuration.Configuration config) {
		envParam = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
		TypeInformation<Map<Long, VehicleMileage>> typeInformation = TypeInformation
				.of(new TypeHint<Map<Long, VehicleMileage>>() {
				});

		TypeInformation<List<Long>> evtTimeTypeInfo = TypeInformation.of(new TypeHint<List<Long>>() {
		});

		MapStateDescriptor<String, Map<Long, VehicleMileage>> descriptor = new MapStateDescriptor<String, Map<Long, VehicleMileage>>(
				"modelState", TypeInformation.of(String.class), typeInformation);
		modelState = getRuntimeContext().getMapState(descriptor);

		MapStateDescriptor<String, List<Long>> vehTimeDescriptor = new MapStateDescriptor<String, List<Long>>(
				"vehEvtTimeListState", TypeInformation.of(String.class), evtTimeTypeInfo);
		vehEvtTimeListState = getRuntimeContext().getMapState(vehTimeDescriptor);
		logger.info("Created the Map state for Mileage Job ");
	}

}
