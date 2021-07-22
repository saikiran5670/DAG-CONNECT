package net.atos.daf.ct2.processing;

import java.io.Serializable;
import java.util.Arrays;
import java.util.HashSet;
import java.util.Iterator;
import java.util.Properties;
import java.util.Set;
import java.util.stream.DoubleStream;
import java.util.stream.IntStream;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.node.ObjectNode;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.IndexDocument;
import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;
import net.atos.daf.ct2.utils.JsonMapper;

public class MessageProcessing<U, T> implements Serializable {

	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(MessageProcessing.class);

	/* kafka topic data consuming as stream */
	public void consumeBoschMessage(DataStream<KafkaRecord<U>> messageDataStream, String messageType, String key,
			String sinkTopicName, Properties properties, Class<T> tClass) {

		messageDataStream.map(new MapFunction<KafkaRecord<U>, KafkaRecord<T>>() {
			private static final long serialVersionUID = 1L;

			public KafkaRecord<T> map(KafkaRecord<U> value) throws Exception {

				Index indexobj = new Index();
				IndexDocument indexDocument = new IndexDocument();
				/* json message parsing and store in jsonObject */
				JSONObject jsonObject = transformMessages(value.toString());

				String vin = (String) getValueByAttributeKey("vin", jsonObject);

				String start = (String) getValueByAttributeKey("start", jsonObject);
				Object documentobj = getValueByAttributeKey("Document", jsonObject);
				JSONObject document = (JSONObject) documentobj;
				if (document != null) {
					Object gpsobj = getValueByAttributeKey("GPS", document);
					JSONObject gps = (JSONObject) gpsobj;

					Double latitude = (Double) getValueByAttributeKey("latitude", gps);
					Double longitude = (Double) getValueByAttributeKey("longitude", gps);
					Double direction = (Double) getValueByAttributeKey("direction", gps);
					Long altitude = (Long) getValueByAttributeKey("altitude", gps);
					Double gpsSpeed = (Double) getValueByAttributeKey("gpsSpeed", gps);
					String tripid = (String) getValueByAttributeKey("tripId", gps);

					indexDocument.setTripID(tripid);

					if (vin != null) {
						indexobj.setVin(vin);
					} else {
						indexobj.setVin(DAFCT2Constant.UNKNOWN);
					}

					indexDocument.setTripID("BOSCH_" + TimeFormatter.getInstance().getCurrentUTCTime());

					indexobj.setEvtDateTime(start);
					indexobj.setGpsDateTime(start);
					indexobj.setGpsLatitude(latitude);
					indexobj.setGpsAltitude(altitude);
					indexobj.setGpsLongitude(longitude);

					indexobj.setGpsHeading(direction);
					indexobj.setDocument(indexDocument);
					Double fuelused = (Double) getValueByAttributeKey("EngineTotalFuelUsed", document);
					if (fuelused != null) {
						indexobj.setVUsedFuel(getValueAsLong(fuelused));
					}

					String driver1Identification = getStringAsValue(
							(Double) getValueByAttributeKey("Driver1Identification", document));
					Double driver1WorkingState = (Double) getValueByAttributeKey("Driver1WorkingState", document);
					Integer driver2WorkingState = getValueAsInteger(
							(Double) getValueByAttributeKey("Driver2WorkingState", document));
					Double driverCardDriver2insert = (Double) getValueByAttributeKey("DriverCardDriver2", document);
					Double driver1CardInserted = (Double) getValueByAttributeKey("DriverCardDriver1", document);

					Integer[] engineOilPressure = getIntResult(
							(Double) getValueByAttributeKey("EngineOilPressure1", document));

					String driver2Identification = getStringAsValue(
							(Double) getValueByAttributeKey("Driver2Identification", document));
					Double[] engineCoolantLevel = getDoubleResult(
							(Double) getValueByAttributeKey("EngineCoolantLevel1", document));
					Integer[] engineCoolantTemperature = getIntResult(
							(Double) getValueByAttributeKey("EngineCoolantTemperature", document));

					Double[] engineOilLevel = getDoubleResult(
							(Double) getValueByAttributeKey("EngineOilLevel", document));
					Integer[] engineOilTemperature = getIntResult(
							(Double) getValueByAttributeKey("EngineOilTemperature1", document));

					Double slIBatteryPackStateOfCharge = (Double) getValueByAttributeKey("SLIBatteryPackStateOfCharge",
							document);
					Double batteryPotentialPowerInput = (Double) getValueByAttributeKey("BatteryPotentialPowerInput",
							document);
					Integer[] engineLoad = getIntResult(
							(Double) getValueByAttributeKey("ActualEnginePercentTorque", document));

					Long ambientAirTemperature = getValueAsLong(
							(Double) getValueByAttributeKey("AmbientAirTemperature", document));
					Long[] airPressure = getLongResult(
							(Double) getValueByAttributeKey("ServiceBrakeCircuit2AirPressure", document));
					// TODO
					// Integer[] airPressure = null;

					Long[] tachoVehicleSpeed = getLongResult(
							(Double) getValueByAttributeKey("TachographVehicleSpeed", document));

					Integer vDEFTankLevel = getValueAsInteger(
							(Double) getValueByAttributeKey("Aftertreatment1DieselExhaustFluidTankVolume", document));
					Long vEngineTotalHours = getValueAsLong(
							(Double) getValueByAttributeKey("EngineTotalHoursOfOperation", document));

					Long[] engineSpeed = getLongResult((Double) getValueByAttributeKey("EngineSpeed", document));
					Double vFuelLevel1 = (Double) getValueByAttributeKey("FuelLevel1", document);

					indexDocument.setDriver2ID(driver2Identification);

					indexobj.setDriverID(driver1Identification);
					if (driver2WorkingState != null) {
						indexDocument.setDriver2WorkingState((int) Math.round(driver2WorkingState));
					}

					if (driver1WorkingState != null) {
						indexDocument.setDriver1WorkingState((int) Math.round(driver1WorkingState));
					}

					indexDocument.setEngineOilPressure(engineOilPressure);

					indexDocument.setEngineCoolantLevel(engineCoolantLevel);

					indexDocument.setEngineCoolantTemperature(engineCoolantTemperature);

					indexDocument.setEngineOilLevel(engineOilLevel);

					indexDocument.setEngineOilTemperature(engineOilTemperature);

					indexDocument.setEngineSpeed(engineSpeed);
					if (gpsSpeed != null) {
						indexDocument.setGpsSpeed(getValueAsLong(gpsSpeed));
					}

					indexDocument.setVPowerBatteryChargeLevel(slIBatteryPackStateOfCharge);
					indexDocument.setVPowerBatteryVoltage(batteryPotentialPowerInput);
					// TODO
					Double vRetarderTorqueActual = (Double) getValueByAttributeKey("ActualRetarderPercentTorque",
							document);
					indexDocument.setVRetarderTorqueActual(vRetarderTorqueActual);
					Integer vRetarderTorqueMode = (Integer) getValueByAttributeKey("RetarderTorqueMode", document);
					;
					indexDocument.setVRetarderTorqueMode(vRetarderTorqueMode);
					Double gpsHdop = null;
					Long gpsSegmentDist = null;// getValueAsInteger((Double)
													// getValueByAttributeKey("RemDurationOfCurrentBreakRest",
													// document));;
					Double[] adBlueLevel = null;
					Long driver1RemainingDrivingTime = getValueAsLong(
							(Double) getValueByAttributeKey("RemDurationOfCurrentBreakRest", document));
					// TODO
					Long period = null;
					Long[] inletAirPressureInInletManifold = getLongResult(
							(Double) getValueByAttributeKey("EngineIntakeAirPressure", document));
					Long segmentHaversineDistance = null;
					Long startEltsTime = null;
					// TODO
					String driver2ID = (String) getValueByAttributeKey("Driver2identification", document);
					;
					Long vEngineTotalHoursIdle = (Long) getValueByAttributeKey("EngineTotalIdleHours", document);
					Long vFuelCumulatedIdle = (Long) getValueByAttributeKey("EngineTotalIdleFuelUsed", document);
					Long vFuelCumulated = getValueAsLong(
							(Double) getValueByAttributeKey("EngineTotalFuelUsedHighResolution", document));
					Long vFuelCumulatedLR = getValueAsLong(
							(Double) getValueByAttributeKey("EngineTotalFuelUsed", document));
					indexDocument.setDriver2ID(driver2ID);
					indexDocument.setVEngineTotalHoursIdle(vEngineTotalHoursIdle);
					indexDocument.setDriver2WorkingState(driver2WorkingState);
					// TODO
					indexDocument.setVFuelCumulated(vFuelCumulated);
					indexDocument.setVFuelCumulatedIdle(vFuelCumulatedIdle);
					indexDocument.setVFuelCumulatedLR(vFuelCumulatedLR);
					if (driverCardDriver2insert != null) {
						indexDocument.setDriver2CardInserted(true);
					} else {
						indexDocument.setDriver2CardInserted(false);
					} // TODO
					Long driver1RemainingRestTime = getValueAsLong((Double) getValueByAttributeKey("RemDurationOfCurrentBreakRest",
							document));
					Double[] ambientPressure = getDoubleResult(
							(Double) getValueByAttributeKey("BarometricPressure", document));
					indexDocument.setGpsHdop(gpsHdop);
					indexDocument.setGpsSegmentDist(gpsSegmentDist);
					indexDocument.setAdBlueLevel(adBlueLevel);
					indexDocument.setAirPressure(airPressure);
					// TODO
					indexDocument.setAmbientPressure(ambientPressure);

					if (driver1CardInserted != null) {
						indexDocument.setDriver1CardInserted(true);
					} else {
						indexDocument.setDriver1CardInserted(false);
					}
					indexDocument.setDriver1RemainingRestTime(driver1RemainingRestTime);
					indexDocument.setDriver1RemainingDrivingTime(driver1RemainingDrivingTime);
					indexDocument.setInletAirPressureInInletManifold(inletAirPressureInInletManifold);
					// start time - end time
					indexDocument.setPeriod(period);
					indexDocument.setSegmentHaversineDistance(segmentHaversineDistance);
					indexDocument.setStartEltsTime(startEltsTime);

					Long[] totalTachoMileage = null;
					Double vAcceleration = null;
					indexDocument.setTachoVehicleSpeed(tachoVehicleSpeed);
					indexDocument.setTripID(tripid);
					indexDocument.setTotalTachoMileage(totalTachoMileage);
					indexDocument.setVAcceleration(vAcceleration);

					indexDocument.setVAmbiantAirTemperature(ambientAirTemperature);
					// TODO

					indexDocument.setVDEFTankLevel(vDEFTankLevel);
					Integer vengineCoolantTemperature = null;
					Integer vGearCurrent = null;
					indexDocument.setVEngineCoolantTemperature(vengineCoolantTemperature);
					indexDocument.setEngineLoad(engineLoad);

					indexDocument.setVEngineTotalHours(vEngineTotalHours);
					// TODO
					indexDocument.setVFuelLevel1(vFuelLevel1);
					indexDocument.setVGearCurrent(vGearCurrent);
					Long vGrossWeightCombination = getValueAsLong(
							(Double) getValueByAttributeKey("GrossCombinationVehicleWeight", document));
					Double vPedalAcceleratorPosition1 = (Double) getValueByAttributeKey("AcceleratorPedalPosition1",
							document);
					// TODO
					indexDocument.setVGrossWeightCombination(vGrossWeightCombination);
					indexDocument.setVPedalAcceleratorPosition1(vPedalAcceleratorPosition1);
					Long vServiceBrakeAirPressure1 = getValueAsLong(
							(Double) getValueByAttributeKey("ServiceBrakeCircuit1AirPressure", document));
					indexDocument.setVServiceBrakeAirPressure1(vServiceBrakeAirPressure1);
					Long vserviceBrakeAirPressure2 = getValueAsLong(
							(Double) getValueByAttributeKey("ServiceBrakeCircuit2AirPressure", document));
					Integer transmissionCurrentGear = getValueAsInteger(
							(Double) getValueByAttributeKey("TransmissionCurrentGear", document));

					indexDocument.setVGearCurrent(transmissionCurrentGear);

					indexDocument.setVServiceBrakeAirPressure1(vserviceBrakeAirPressure2);
					Long vWheelBasedSpeed = getValueAsLong(
							(Double) getValueByAttributeKey("WheelBasedVehicleSpeed", document));

					indexDocument.setVWheelBasedSpeed(vWheelBasedSpeed);
					Double[] fuelTemperature = getDoubleResult(
							(Double) getValueByAttributeKey("EngineFuelTemperature1", document));
					indexDocument.setFuelTemperature(fuelTemperature);
					Double vTankDiff = null;
					indexDocument.setVTankDiff(vTankDiff);
					Double vSegmentFuelLevel1 = null;
					indexDocument.setVSegmentFuelLevel1(vSegmentFuelLevel1);
					Integer[] tt_ListValue = null;
					indexDocument.setTt_ListValue(tt_ListValue);
					String tt_Norm = null;
					indexDocument.setTt_Norm(tt_Norm);
				}
				indexobj.setDocument(indexDocument);

				T record = JsonMapper.configuring().readValue(indexobj.toString(), tClass);

				KafkaRecord<T> kafkaRecord = new KafkaRecord<T>();
				kafkaRecord.setKey(key);
				kafkaRecord.setValue(record);

				System.out.println("before publishing monitoringObj.toString() record :: " + indexDocument.toString());
				System.out.println("before publishing kafka record :: " + record);
				return kafkaRecord;
			}
		}).addSink(
				new FlinkKafkaProducer<KafkaRecord<T>>(sinkTopicName, new KafkaMessageSerializeSchema<T>(sinkTopicName),
						properties, FlinkKafkaProducer.Semantic.EXACTLY_ONCE));
	}

	
	public DataStream<Tuple2<Integer, KafkaRecord<String>>> boschSourceStreamStatus(
			DataStream<KafkaRecord<String>> boschInputStream) {
		return boschInputStream.map(new MapFunction<KafkaRecord<String>, Tuple2<Integer, KafkaRecord<String>>>() {

			private static final long serialVersionUID = 1L;
			String rowKey = null;

			@Override
			public Tuple2<Integer, KafkaRecord<String>> map(KafkaRecord<String> value) throws Exception {
				String vin = DAFCT2Constant.UNKNOWN;
				String transId = DAFCT2Constant.UNKNOWN;
				Integer recordType = DAFCT2Constant.MEASUREMENT_DATA;

				try {
					JsonNode jsonNodeRec = JsonMapper.configuring().readTree(value.getValue());
					logger.info("Bosch source rec :: " + jsonNodeRec);

					if (jsonNodeRec != null && jsonNodeRec.get("metaData") != null) {
						// Measurement Record
						if (jsonNodeRec.get("metaData").get("vehicle") != null) {
							if (jsonNodeRec.get("metaData").get("vehicle").get("vin") != null) {
								vin = jsonNodeRec.get("metaData").get("vehicle").get("vin").asText();
							}

							if (jsonNodeRec.get("metaData").get("vehicle").get("TransID") != null) {
								transId = jsonNodeRec.get("metaData").get("vehicle").get("TransID").asText();
							}

							((ObjectNode) jsonNodeRec).put("kafkaProcessingTS",
									TimeFormatter.getInstance().getCurrentUTCTime());
						}
					} else {
						// TCU Record
						recordType = DAFCT2Constant.TCU_DATA;
						if (jsonNodeRec.get("vin") != null) {
							vin = jsonNodeRec.get("vin").asText();
						}

						if (jsonNodeRec.get("deviceIdentifier") != null) {
							transId = jsonNodeRec.get("deviceIdentifier").asText();
						}

						((ObjectNode) jsonNodeRec).put("ReferenceDate",
								TimeFormatter.getInstance().getCurrentUTCTimeInStringFmt());
						System.out.println("In Bosch message processing , received TCU msg :: " + jsonNodeRec);
					}

					rowKey = transId + "_" + vin + "_" + TimeFormatter.getInstance().getCurrentUTCTime();
					value.setValue(JsonMapper.configuring().writeValueAsString(jsonNodeRec));

				} catch (Exception e) {
					rowKey = "UnknownMessage" + "_" + TimeFormatter.getInstance().getCurrentUTCTime();
					recordType = DAFCT2Constant.UNKNOWN_DATA;
				}

				value.setKey(rowKey);
				return new Tuple2<Integer, KafkaRecord<String>>(recordType, value);
			}
		});
	}

	private static Object getValueByAttributeKey(String key, JSONObject jsonObject) {
		if (jsonObject != null && jsonObject.containsKey(key)) {
			return jsonObject.get(key);
		} else {
			return null;
		}
	}

	public static Integer[] getIntResult(Double value) {
		if (value != null) {
			Set<Integer> setOfInteger = new HashSet<>(Arrays.asList((int) Math.round(value)));
			int[] temp = setOfInteger.stream().mapToInt(Integer::intValue).toArray();
			Integer[] intarray = IntStream.of(temp).boxed().toArray(Integer[]::new);
			return intarray;
		} else {
			return null;
		}
	}

	protected Long[] getLongResult(Double value) {
		if (value != null) {
			return null;
		} else {
			return null;
		}
	}

	
	public static Double[] getDoubleResult(Double value) {
		if (value != null) {
			Set<Double> setOfInteger = new HashSet<>(Arrays.asList(value));
			double[] temp = setOfInteger.stream().mapToDouble(Double::doubleValue).toArray();

			Double[] intarray = DoubleStream.of(temp).boxed().toArray(Double[]::new);
			return intarray;
		} else {
			return null;
		}

	}

	public static Integer getValueAsInteger(Double value) {
		if (value != null) {
			return value.intValue();
		} else {
			return null;
		}
	}

	private static JSONObject transformMessages(String message) {
		JSONParser parser = new JSONParser();
		JSONObject result = new JSONObject();

		JSONObject documentelement = new JSONObject();
		try {
			JSONObject jsonObject = (JSONObject) new JSONParser().parse(message);
			JSONObject metaData = (JSONObject) getValueByAttributeKey("metaData", jsonObject);
			JSONObject resultData = (JSONObject) getValueByAttributeKey("resultData", jsonObject);
			JSONArray seriesPartsList = (JSONArray) getValueByAttributeKey("seriesParts", resultData);
			JSONObject correlations = (JSONObject) getValueByAttributeKey("correlations", jsonObject);

			JSONObject vehicle = (JSONObject) getValueByAttributeKey("vehicle", metaData);
			String manufacturerName = (String) getValueByAttributeKey("manufacturerName", vehicle);
			String vehicleName = (String) getValueByAttributeKey("vehicleName", vehicle);
			String vehicleId = (String) getValueByAttributeKey("vehicleId", vehicle);
			String vin = (String) getValueByAttributeKey("vin", vehicle);
			String end = (String) getValueByAttributeKey("end", resultData);
			String start = (String) getValueByAttributeKey("start", resultData);
			String testStepId = (String) getValueByAttributeKey("testStepId", resultData);

			result.put("manufacturerName", manufacturerName);
			result.put("vehicleName", vehicleName);
			result.put("vehicleId", vehicleId);
			result.put("vin", vin);
			result.put("end", end);
			result.put("start", start);
			result.put("testStepId", testStepId);
			result.put("timeStamp", start);
			Iterator<JSONObject> iterator = seriesPartsList.iterator();

			JSONObject document = new JSONObject();

			for (int i = 0; i < seriesPartsList.size(); i++) {
				JSONObject seriesParts1 = (JSONObject) seriesPartsList.get(i);
				JSONArray dataPoints1 = (JSONArray) getValueByAttributeKey("dataPoints", seriesParts1);//// seriesParts1.get("dataPoints");

				if (dataPoints1.size() >= 1) {
					JSONObject dataPoint1 = (JSONObject) dataPoints1.get(dataPoints1.size() - 1);
					JSONObject series = (JSONObject) getValueByAttributeKey("series", seriesParts1);// seriesParts1.get("series");
					String seriesName = (String) getValueByAttributeKey("seriesName", series);
					document.put(seriesName, getValueByAttributeKey("value", dataPoint1));

					if ("TripCounter".equals(seriesName.trim())) {
						document.put("tripId", getValueByAttributeKey("value", dataPoint1) + "-"
								+ getValueByAttributeKey("bootCycleId", correlations));
					}
				}
			}

			result.put("Document", document);

		} catch (Exception e) {
			e.printStackTrace();
		}
		return result;
	}

	private static String getStringAsValue(Double value) {
		if (value != null) {
			return value.toString();
		} else {
			return null;
		}
	}
	
	

	private static Long getValueAsLong(Double value) {
		if(value != null ){
			return value.longValue();
		} else {
			return null;
		}
	}
}
