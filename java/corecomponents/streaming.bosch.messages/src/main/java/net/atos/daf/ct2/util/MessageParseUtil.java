package net.atos.daf.ct2.util;

import java.time.Instant;
import java.util.Arrays;
import java.util.HashSet;
import java.util.Iterator;
import java.util.Objects;
import java.util.Properties;
import java.util.Set;
import java.util.stream.DoubleStream;
import java.util.stream.IntStream;
import java.util.stream.LongStream;

import org.apache.commons.lang.exception.ExceptionUtils;
import org.apache.flink.api.common.functions.FilterFunction;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.common.ct2.util.DAFConstants;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Distribution;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.IndexDocument;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.MonitorDocument;
import net.atos.daf.ct2.pojo.standard.SpareMatrixAcceleration;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.pojo.standard.StatusDocument;
import net.atos.daf.ct2.processing.StoreHistoricalData;
import net.atos.daf.ct2.utils.JsonMapper;

public class MessageParseUtil {
	private static final Logger logger = LogManager.getLogger(MessageParseUtil.class);

	public static DataStream<KafkaRecord<String>> filterDataInputStream(
			DataStream<KafkaRecord<String>> boschInputStream, String messageType) {
		return boschInputStream.filter(new FilterFunction<KafkaRecord<String>>() {
			@Override
			public boolean filter(KafkaRecord<String> message) {
				try {
					System.out.println("filter Bosch message for mapping ::" + message.getValue());
					logger.info("INFO : filter Bosch message for mapping ::" + message.getValue());
					JsonNode jsonNodeRec = JsonMapper.configuring().readTree(message.getValue());
					if (jsonNodeRec != null) {
						// TODO
						String transId = getTransIdValue(message.getValue());
						System.out.println("filter Trans ID    " + transId + " message type ::" + messageType);
						logger.info("filter Trans ID " + transId + " message type ::" + messageType);
						if (transId != null && messageType != null && transId.trim().contains(messageType.trim())) {
							return true;
						}

						/*
						 * JsonNode corrleation = (JsonNode)
						 * jsonNodeRec.get("correlations"); if (corrleation !=
						 * null) {
						 * 
						 * String measurmentConfigName =
						 * corrleation.get("measurementConfigurationName").
						 * asText(); System.out.println( "filter Trans ID    " +
						 * measurmentConfigName + " message type ::" +
						 * messageType); logger.info("filter Trans ID " +
						 * measurmentConfigName + " message type ::" +
						 * messageType); if (measurmentConfigName != null &&
						 * messageType != null &&
						 * measurmentConfigName.contains(messageType)) { return
						 * true; }
						 * 
						 * }
						 */

					}
				} catch (Exception ex) {
					ex.printStackTrace();
					logger.error(" transid filter process failed " + ex.getMessage());
					logger.error("ERROR : filter Bosch message for mapping Raw messag is ::" + message.getValue());
				}

				return false;
				/*
				 * try { JSONObject jsonObject =
				 * transformMessages(value.toString());
				 * 
				 * Object documentobj = getValueByAttributeKey("Document",
				 * jsonObject); JSONObject document = (JSONObject) documentobj;
				 * if (document != null) { String transId = ""; Integer
				 * intTransId = getValueAsInteger((Double)
				 * getValueByAttributeKey("TransID", document)); if (intTransId
				 * != null) { transId = intTransId.toString(); }
				 * System.out.println("filter Trans ID    " + transId);
				 * logger.info("filter Trans ID " + transId); return
				 * messageType.equalsIgnoreCase(transId); }
				 * 
				 * } catch (Exception ex) {
				 * logger.error(" transid filter process failed " +
				 * ex.getMessage());
				 * 
				 * } return false;
				 */

			}
		});
	}

	public static Index processIndexBoschMessage(String value, Properties properties, String kafkaProcessingTimeStamp) {
		logger.info("Index Raw Message processing start ::====>" + value.toString());
		System.out.println("Index Raw Message processing start ::====>" + value.toString());

		Index indexobj = new Index();
		IndexDocument indexDocument = new IndexDocument();
		try {
			/* json message parsing and store in jsonObject */
			JsonNode jsonNode = transformMessages(value);

			String vin = (String) getValueByAttributeKey("vin", jsonNode);

			String start = (String) getValueByAttributeKey("start", jsonNode);
			JsonNode document = (JsonNode) jsonNode.get("Document");
			if (document != null) {

				JsonNode gps = (JsonNode) document.get("GPS");
				JsonNode reasonData = (JsonNode) jsonNode.get("reasonData");

				Double latitude = getValueAsDouble((String) getValueByAttributeKey("latitude", gps));
				Double longitude = getValueAsDouble((String) getValueByAttributeKey("longitude", gps));
				Double direction = getValueAsDouble((String) getValueByAttributeKey("direction", gps));
				Long altitude = getValueAsLong((String) getValueByAttributeKey("elevation", gps));
				String gpsSpeed = (String) getValueByAttributeKey("gpsSpeed", gps);
				String tripid = (String) getValueByAttributeKey("tripId", document);
				Long gpsTime = getValueAsLong((String) getValueByAttributeKey("lastReadingTime", gps));
				String vehicleId = (String) getValueByAttributeKey("vehicleId", jsonNode);
				String transId = (String) getValueByAttributeKey("TransID", document);

				Long receiveTimeStamp = TimeFormatter.getInstance().getCurrentUTCTime();
				indexobj.setReceivedTimestamp(receiveTimeStamp);
				indexobj.setEvtDateTime(start);

				// validation check for each mesage

				if (transId != null && transId.contains(".")) {
					transId = transId.substring(0, transId.indexOf("."));// 1000.0
				}

				if (transId != null && !transId.isEmpty()) {

					indexobj.setTransID(getContiTransId(transId, properties));
				}

				System.out.println(" Trans id :" + transId + " , trip id ==>" + tripid);
				logger.info(" Trans id :" + tripid + " , trip id ==>" + tripid);

				indexDocument.setTripID(tripid);

				indexobj.setVid(vehicleId);

				if (vin != null && !vin.isEmpty()) {
					indexobj.setVin(vin);
				} else {
					indexobj.setVin(DAFCT2Constant.UNKNOWN);
				}

				indexobj.setKafkaProcessingTS(kafkaProcessingTimeStamp);

				String jobName = (String) getValueByAttributeKey("jobName", jsonNode);
				indexobj.setJobName(jobName);

				Long numSeq = getValueAsLong((String) getValueByAttributeKey("packageIndex", jsonNode));
				indexobj.setNumSeq(numSeq);
				Integer vEvtID = null;

				if (reasonData == null) {
					vEvtID = 1;
				} else if (reasonData != null && vEvtID == null) {
					vEvtID = getVEvtIdByIndexMessageByTripStart(reasonData);
					if (vEvtID == null) {
						vEvtID = getVEvtIdByIndexMessageByTripEnd(reasonData);
					}

				}

				if (vEvtID != null) {
					indexobj.setVEvtID(vEvtID);
				} else {
					indexobj.setVEvtID(1);
				}

				if (gpsTime != null) {
					indexobj.setGpsDateTime(gpsTime.toString());
				}

				indexobj.setGpsLatitude(latitude);
				indexobj.setGpsAltitude(altitude);
				indexobj.setGpsLongitude(longitude);
				indexobj.setGpsHeading(direction);

				// TODO take value form kafkaRecord object

				indexobj.setKafkaProcessingTS(kafkaProcessingTimeStamp);
				Long vDist = getValueAsLong(
						(String) getValueByAttributeKey("TotalVehicleDistanceHighResolution", document));
				indexobj.setVDist(vDist);

				String driver1Identification = (String) getValueByAttributeKey("Driver1Identification", document);
				if (driver1Identification != null && !driver1Identification.isEmpty()) {
					indexobj.setDriverID(driver1Identification);
				}

				String vUsedFuel = (String) getValueByAttributeKey("VUsedFuel", document);
				indexobj.setVUsedFuel(getValueAsLong(vUsedFuel));

				Double gspHdop = 0d;
				indexDocument.setGpsHdop(gspHdop);

				String strvIdleDuration = (String) getValueByAttributeKey("VIdleDuration", document);

				Long vIdleDuration1 = getValueAsLong(strvIdleDuration);

				indexobj.setVIdleDuration(vIdleDuration1);

				if (gpsSpeed != null) {
					indexDocument.setGpsSpeed(getValueAsLong(gpsSpeed));
				}

				Double driver1CardInserted = getValueAsDouble(
						(String) getValueByAttributeKey("DriverCardDriver1", document));

				if (driver1CardInserted != null) {
					indexDocument.setDriver1CardInserted(true);
				} else {
					indexDocument.setDriver1CardInserted(false);
				}

				Long driver1RemainingDrivingTime = getValueAsLong(
						(String) getValueByAttributeKey("RemainingCurrentDrivingTime", document));

				indexDocument.setDriver1RemainingDrivingTime(driver1RemainingDrivingTime);

				Long driver1RemainingRestTime = getValueAsLong(
						(String) getValueByAttributeKey("RemDurationOfCurrentBreakRest", document));

				indexDocument.setDriver1RemainingRestTime(driver1RemainingRestTime);

				Double driver1WorkingState = getValueAsDouble(
						(String) getValueByAttributeKey("Driver1WorkingState", document));
				if (driver1WorkingState != null) {
					indexDocument.setDriver1WorkingState((int) Math.round(driver1WorkingState));
				}

				String driver2Identification = (String) getValueByAttributeKey("Driver2Identification", document);
				indexDocument.setDriver2ID(driver2Identification);

				Double driverCardDriver2insert = getValueAsDouble(
						(String) getValueByAttributeKey("DriverCardDriver2", document));

				if (driverCardDriver2insert != null) {
					indexDocument.setDriver2CardInserted(true);
				} else {
					indexDocument.setDriver2CardInserted(false);
				}

				Integer driver2WorkingState = getValueAsInteger(
						(String) getValueByAttributeKey("Driver2WorkingState", document));

				if (driver2WorkingState != null) {
					indexDocument.setDriver2WorkingState((int) Math.round(driver2WorkingState));
				}

				Long ambientAirTemperature = getValueAsLong(
						(String) getValueByAttributeKey("AmbientAirTemperature", document));
				indexDocument.setVAmbiantAirTemperature(ambientAirTemperature);

				Double vAcceleration = getValueAsDouble((String) getValueByAttributeKey("VAcceleration", document));
				indexDocument.setVAcceleration(vAcceleration);

				Integer vDEFTankLevel = getValueAsInteger(
						(String) getValueByAttributeKey("Aftertreatment1DieselExhaustFluidTankVolume", document));
				indexDocument.setVDEFTankLevel(vDEFTankLevel);
				Integer[] engineCoolantTemperature = getIntResult(
						(String) getValueByAttributeKey("EngineCoolantTemperature", document));
				indexDocument.setEngineCoolantTemperature(engineCoolantTemperature);

				Integer[] engineLoad = getIntResult(
						(String) getValueByAttributeKey("ActualEnginePercentTorque", document));
				Integer vEngineLoad = getValueAsInteger(
						(String) getValueByAttributeKey("ActualEnginePercentTorque", document));
				// indexDocument.setEngineLoad(engineLoad);
				indexDocument.setVEngineLoad(vEngineLoad);
				Long vEngineSpeed = getValueAsLong((String) getValueByAttributeKey("EngineSpeed", document));
				indexDocument.setVEngineSpeed(vEngineSpeed);

				Long vEngineTotalHours = getValueAsLong(
						(String) getValueByAttributeKey("EngineTotalHoursOfOperation", document));
				indexDocument.setVEngineTotalHours(vEngineTotalHours);

				Long vEngineTotalHoursIdle = getValueAsLong(
						(String) getValueByAttributeKey("VEngineTotalIdleHours", document));
				indexDocument.setVEngineTotalHoursIdle(vEngineTotalHoursIdle);

				Long vFuelCumulated = getValueAsLong(
						(String) getValueByAttributeKey("EngineTotalFuelUsedHighResolution", document));
				indexDocument.setVFuelCumulated(vFuelCumulated);

				Long vFuelCumulatedIdle = getValueAsLong(
						(String) getValueByAttributeKey("VEngineTotalIdleFuelUsed", document));
				indexDocument.setVFuelCumulatedIdle(vFuelCumulatedIdle);
				Long vFuelCumulatedLR = getValueAsLong(
						(String) getValueByAttributeKey("EngineTotalFuelUsed", document));

				indexDocument.setVFuelCumulatedLR(vFuelCumulatedLR);
				Double vFuelLevel1 = getValueAsDouble((String) getValueByAttributeKey("FuelLevel1", document));
				indexDocument.setVFuelLevel1(vFuelLevel1);
				Integer vGearCurrent = getValueAsInteger(
						(String) getValueByAttributeKey("TransmissionCurrentGear", document));
				indexDocument.setVGearCurrent(vGearCurrent);

				Long vGrossWeightCombination = getValueAsLong(
						(String) getValueByAttributeKey("GrossCombinationVehicleWeight", document));
				indexDocument.setVGrossWeightCombination(vGrossWeightCombination);
				Double vPedalAcceleratorPosition1 = getValueAsDouble(
						(String) getValueByAttributeKey("AcceleratorPedalPosition1", document));
				indexDocument.setVPedalAcceleratorPosition1(vPedalAcceleratorPosition1);

				Double slIBatteryPackStateOfCharge = getValueAsDouble(
						(String) getValueByAttributeKey("SLIBatteryPackStateofCharge", document));
				indexDocument.setVPowerBatteryChargeLevel(slIBatteryPackStateOfCharge);

				Double batteryPotentialPowerInput = getValueAsDouble(
						(String) getValueByAttributeKey("BatteryPotentialPowerInput1", document));
				indexDocument.setVPowerBatteryVoltage(batteryPotentialPowerInput);
				Double vRetarderTorqueActual = getValueAsDouble(
						(String) getValueByAttributeKey("ActualRetarderPercentTorque", document));
				indexDocument.setVRetarderTorqueActual(vRetarderTorqueActual);
				Integer vRetarderTorqueMode = getValueAsInteger(
						(String) getValueByAttributeKey("RetarderTorqueMode", document));

				indexDocument.setVRetarderTorqueMode(vRetarderTorqueMode);

				Long vServiceBrakeAirPressure1 = getValueAsLong(
						(String) getValueByAttributeKey("ServiceBrakeCircuit1AirPressure", document));
				indexDocument.setVServiceBrakeAirPressure1(vServiceBrakeAirPressure1);
				Long vserviceBrakeAirPressure2 = getValueAsLong(
						(String) getValueByAttributeKey("ServiceBrakeCircuit2AirPressure", document));
				indexDocument.setVServiceBrakeAirPressure2(vserviceBrakeAirPressure2);

				Integer vTachographSpeed = getValueAsInteger(
						(String) getValueByAttributeKey("TachographVehicleSpeed", document));
				indexDocument.setVTachographSpeed(vTachographSpeed);

				Long vWheelBasedSpeed = getValueAsLong(
						(String) getValueByAttributeKey("WheelBasedVehicleSpeed", document));
				indexDocument.setVWheelBasedSpeed(vWheelBasedSpeed);
				// TODO delta value calculate from all list of datapoints
				Long gpsSegmentDist = getValueAsLong((String) getValueByAttributeKey("GPSTripDistance", document));
				indexDocument.setGpsSegmentDist(gpsSegmentDist);

				Long period = getValueAsLong((String) getValueByAttributeKey("period", document));
				indexDocument.setPeriod(period);
				String strStartEltsTime = (String) getValueByAttributeKey("start", jsonNode);
				// getValueAsLong((String) getValueByAttributeKey("start",
				// jsonNode));
				if (strStartEltsTime != null) {// "2021-09-15T20:50:08.695Z"
					// SimpleDateFormat startEltsFmt = new
					// SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSSZ");
					// Date startEltsDte=startEltsFmt.parse(strStartEltsTime);
					try {
						// Instant startEltsDte =
						// Instant.parse(strStartEltsTime);
						indexDocument.setStartEltsTime(convertDateStringToTS(strStartEltsTime));

					} catch (Exception ex) {
						System.err.println("startEtltTIme parsing date failed ==>" + ex.getMessage());
					}
				}
				Double[] adBlueLevel = getDoubleResult(
						(String) getValueByAttributeKey("Aftertreatment1DieselExhaustFluidTankVolume", document));

				indexDocument.setAdBlueLevel(adBlueLevel);
				Long[] airPressure = getLongResult((String) getValueByAttributeKey("AirPressure", document));
				indexDocument.setAirPressure(airPressure);

				Double[] ambientPressure = getDoubleResult(
						(String) getValueByAttributeKey("BarometricPressure", document));
				indexDocument.setAmbientPressure(ambientPressure);
				Double[] engineCoolantLevel = getDoubleResult(
						(String) getValueByAttributeKey("EngineCoolantLevel1", document));
				indexDocument.setEngineCoolantLevel(engineCoolantLevel);
				Integer vengineCoolantTemperature = getValueAsInteger(
						(String) getValueByAttributeKey("EngineCoolantTemperature", document));

				indexDocument.setVEngineCoolantTemperature(vengineCoolantTemperature);

				Double[] engineOilLevel = getDoubleResult((String) getValueByAttributeKey("EngineOilLevel", document));
				indexDocument.setEngineOilLevel(engineOilLevel);

				Integer[] engineOilTemperature = getIntResult(
						(String) getValueByAttributeKey("EngineOilTemperature1", document));

				indexDocument.setEngineOilTemperature(engineOilTemperature);
				Integer[] engineOilPressure = getIntResult(
						(String) getValueByAttributeKey("EngineOilPressure", document));

				indexDocument.setEngineOilPressure(engineOilPressure);

				Long[] engineSpeed = getLongResult((String) getValueByAttributeKey("EngineSpeed", document));
				indexDocument.setEngineSpeed(engineSpeed);

				Double[] fuelTemperature = getDoubleResult(
						(String) getValueByAttributeKey("EngineFuelTemperature1", document));
				indexDocument.setFuelTemperature(fuelTemperature);

				Long[] inletAirPressureInInletManifold = getLongResult(
						(String) getValueByAttributeKey("EngineIntakeAirPressure", document));
				indexDocument.setInletAirPressureInInletManifold(inletAirPressureInInletManifold);

				Long[] totalTachoMileage = getLongResult(
						(String) getValueByAttributeKey("TotalVehicleDistanceHighResolution", document));
				indexDocument.setTotalTachoMileage(totalTachoMileage);

			}

		} catch (Exception ex) {
			logger.error("Index type message - Data preparing in indexobject is failed : Raw message is ::" + value);
			ex.printStackTrace();
			logger.error(
					"Index type message - error while parsing the message for prepare index obj for publish on kafka . Error is :"
							+ ex.getMessage());
			System.out.println(
					"Index type messaeg -error while parsing the message for prepare index obj for publish on kafka. Error is:"
							+ ex.getMessage());

		}
		indexobj.setDocument(indexDocument);
		return indexobj;

	}

	public static Monitor processMonitorBoschMessage(String value, Properties properties,
			String kafkaProcessingTimeStamp) {
		logger.info("Monitor Raw Message processing start ::====>" + value.toString());
		System.out.println("Monitor Raw Message processing start ::====>" + value.toString());

		Monitor monitorObj = new Monitor();
		MonitorDocument monitorDocument = new MonitorDocument();
		try {
			/* json message parsing and store in jsonObject */

			JsonNode jsonNode = transformMessages(value);
			System.out.println("jsonNode" + jsonNode);

			String vin = (String) getValueByAttributeKey("vin", jsonNode);

			String start = (String) getValueByAttributeKey("start", jsonNode);
			JsonNode document = (JsonNode) jsonNode.get("Document");

			if (document != null) {
				JsonNode gps = (JsonNode) document.get("GPS");
				System.out.println(" GPS is ==>" + gps);
				JsonNode reasonData = (JsonNode) jsonNode.get("reasonData");

				Double latitude = getValueAsDouble((String) getValueByAttributeKey("latitude", gps));
				Double longitude = getValueAsDouble((String) getValueByAttributeKey("longitude", gps));
				Double direction = getValueAsDouble((String) getValueByAttributeKey("direction", gps));
				Long altitude = getValueAsLong((String) getValueByAttributeKey("altitude", gps));
				Long gpsSpeed = getValueAsLong((String) getValueByAttributeKey("gpsSpeed", gps));
				String tripid = (String) getValueByAttributeKey("tripId", document);
				Long gpsTime = getValueAsLong((String) getValueByAttributeKey("lastReadingTime", gps));
				String vid = (String) getValueByAttributeKey("vehicleId", jsonNode);
				String transId = (String) getValueByAttributeKey("TransID", document);

				if (transId != null && transId.contains(".")) {
					transId = transId.substring(0, transId.indexOf("."));
				}

				monitorDocument.setTripID(tripid);
				System.out.println(" Trans id :" + transId + " , trip id ==>" + tripid);
				logger.info(" Trans id :" + tripid + " , trip id ==>" + tripid);

				if (vin != null && !vin.isEmpty()) {
					monitorObj.setVin(vin);
				} else {
					monitorObj.setVin(DAFCT2Constant.UNKNOWN);
				}

				if (vid != null && !vid.isEmpty()) {
					monitorObj.setVid(vid);
				} else {
					monitorObj.setVid(DAFCT2Constant.UNKNOWN);
				}

				if (transId != null && !transId.isEmpty()) {

					monitorObj.setTransID(getContiTransId(transId, properties));
				}

				Integer vEvtID = getVEvtIdByMonitorMessage(reasonData);
				monitorObj.setVEvtID(vEvtID);

				System.out.println("After changed Trans id :" + monitorObj.getTransID() + " , trip id ==>" + tripid);

				monitorObj.setEvtDateTime(start);

				Long receivedTimestamp = System.currentTimeMillis();
				Long storedTimestamp = null;
				String evtDateTime = start;
				Long increment = null;
				String roProfil = null;
				String tenantID = null;

				// String kafkaProcessingTS = kafkaProcessingTimeStamp
				String jobName = (String) getValueByAttributeKey("jobName", jsonNode);

				Integer messagetype = getValueAsInteger((String) getValueByAttributeKey("MessageType", document));
				// monitorObj.setMessageType(messagetype);
				Double gpsHdop = 0d;// getValueAsDouble((String)
									// getValueByAttributeKey(" GPSHdop",
									// document));
				monitorDocument.setGpsHdop(gpsHdop);
				Long packageIndex = getValueAsLong((String) getValueByAttributeKey("packageIndex", jsonNode));
				String lastReadingTime = (String) getValueByAttributeKey("lastReadingTime", gps);
				String driverID = (String) getValueByAttributeKey("Driver1identification", document);
				Boolean driver1CardInserted = getValueAsBoolean(
						(String) getValueByAttributeKey("Drivercarddriver1", document));
				Integer driver1WorkingState = getValueAsInteger(
						(String) getValueByAttributeKey("Driver1WorkingState", document));
				String driver2ID = (String) getValueByAttributeKey("Driver2identification", document);

				Boolean driver2CardInserted = getValueAsBoolean(
						(String) getValueByAttributeKey("Drivercarddriver2", document));

				Integer driver2WorkingState = getValueAsInteger(
						(String) getValueByAttributeKey("Driver2WorkingState", document));

				Long vAmbiantAirTemperature = getValueAsLong(
						(String) getValueByAttributeKey("AmbientAirTemperature", document));
				monitorDocument.setVAmbiantAirTemperature(vAmbiantAirTemperature);
				
				Integer vCruiseControl = getValueAsInteger(
						(String) getValueByAttributeKey("CruiseControlActive", document));

				Integer vDEFTankLevel = getValueAsInteger(
						(String) getValueByAttributeKey("Aftertreatment1DieselExhaustFluidTankVolume", document));

				Long vDist = getValueAsLong(
						(String) getValueByAttributeKey("TotalVehicleDistanceHighResolution", document));
				Long vDistanceUntilService = getValueAsLong(
						(String) getValueByAttributeKey("ServiceDistance", document));
				Integer vEngineCoolantTemperature = getValueAsInteger(
						(String) getValueByAttributeKey("EngineCoolantTemperature", document));
				Integer vEngineLoad = getValueAsInteger(
						(String) getValueByAttributeKey("ActualEnginePercentTorque", document));
				Long vEngineSpeed = getValueAsLong((String) getValueByAttributeKey("EngineSpeed", document));
				Long vEngineTotalHours = getValueAsLong(
						(String) getValueByAttributeKey("EngineTotalHoursofOperation", document));
				
				Long vEngineTotalHoursIdle = getValueAsLong(
						(String) getValueByAttributeKey("EngineTotalIdleHours", document));
				Long vFuelCumulated = getValueAsLong(
						(String) getValueByAttributeKey("EngineTotalFuelUsedHighResolution", document));
				Integer vFuelCumulatedIdle = getValueAsInteger(
						(String) getValueByAttributeKey("EngineTotalIdleHours", document));
				Long vFuelCumulatedLR = getValueAsLong(
						(String) getValueByAttributeKey("EngineTotalFuelUsed", document));
				Double vFuelLevel1 = getValueAsDouble((String) getValueByAttributeKey("FuelLevel1", document));
				Integer vGearCurrent = getValueAsInteger(
						(String) getValueByAttributeKey("TransmissionCurrentGear", document));
				Integer vGearSelected = getValueAsInteger(
						(String) getValueByAttributeKey("TransmissionSelectedGear", document));
				Long vGrossWeightCombination = getValueAsLong(
						(String) getValueByAttributeKey("GrossCombinationVehicleWeight", document));
				Integer vIgnitionState = getValueAsInteger(
						(String) getValueByAttributeKey("OperatorKeyIgnitionSwitch", document));
				Double vPedalBreakPosition1 = getValueAsDouble(
						(String) getValueByAttributeKey("BrakePedalPosition", document));
				Double vPowerBatteryChargeLevel = getValueAsDouble(
						(String) getValueByAttributeKey("SLIBatteryPackStateofCharge", document));
				Double vPowerBatteryVoltage = getValueAsDouble(
						(String) getValueByAttributeKey("BatteryPotentialPowerInput1", document));
				Integer vPTOEngaged = getValueAsInteger(
						(String) getValueByAttributeKey("AtLeastOnePtoEngaged", document));
				Double vRetarderTorqueActual = getValueAsDouble(
						(String) getValueByAttributeKey("ActualRetarderPercentTorque", document));
				Integer vRetarderTorqueMode = getValueAsInteger(
						(String) getValueByAttributeKey("RetarderTorqueMode", document));
				Long vServiceBrakeAirPressure1 = getValueAsLong(
						(String) getValueByAttributeKey("ServiceBrakeCircuit1AirPressure", document));
				Long vserviceBrakeAirPressure2 = getValueAsLong(
						(String) getValueByAttributeKey("ServiceBrakeCircuit2AirPressure", document));
				Integer vTachographSpeed = getValueAsInteger(
						(String) getValueByAttributeKey("Tachographvehiclespeed", document));
				Long vWheelBasedSpeed = getValueAsLong(
						(String) getValueByAttributeKey("WheelBasedVehicleSpeed", document));

				monitorObj.setReceivedTimestamp(receivedTimestamp);
				monitorObj.setStoredTimestamp(storedTimestamp);
				monitorObj.setEvtDateTime(evtDateTime);
				monitorObj.setIncrement(increment);
				monitorObj.setRoProfil(roProfil);
				monitorObj.setTenantID(tenantID);

				monitorObj.setKafkaProcessingTS(kafkaProcessingTimeStamp);
				monitorObj.setJobName(jobName);
				monitorObj.setMessageType(messagetype);
				monitorObj.setNumSeq(packageIndex);
				monitorObj.setGpsDateTime(lastReadingTime);
				monitorObj.setGpsAltitude(altitude);
				monitorObj.setGpsLatitude(latitude);
				monitorObj.setGpsLongitude(longitude);
				monitorObj.setGpsHeading(direction);
				monitorDocument.setGpsSpeed(gpsSpeed);

				if (driverID != null && !driverID.isEmpty()) {
					monitorDocument.setDriverID(driverID);
				}

				monitorDocument.setDriver1CardInserted(driver1CardInserted);
				if (driver1WorkingState != null) {
					monitorDocument.setDriver1WorkingState(driver1WorkingState);
				}

				// else {
				// // TODO - 3.0 hardcode for testing
				// driver1WorkingState = 3;
				// monitorDocument.setDriver1WorkingState(driver1WorkingState);
				// }

				if (driver2ID != null) {
					monitorDocument.setDriver2ID(driver2ID);
				}

				// else {
				// driver2ID = "NL-MONITER-TEST-DRIVER";
				// monitorDocument.setDriver2ID(driver2ID);
				// }

				monitorDocument.setDriver2CardInserted(driver2CardInserted);
				if (driver2WorkingState != null) {
					monitorDocument.setDriver2WorkingState(driver2WorkingState);
				}

				// else {
				// // TODO for tetsing
				// driver2WorkingState = 0;
				// monitorDocument.setDriver2WorkingState(driver2WorkingState);
				// }

				monitorDocument.setVCruiseControl(vCruiseControl);
				monitorDocument.setVDEFTankLevel(vDEFTankLevel);
				monitorDocument.setVDist(vDist);
				monitorDocument.setVDistanceUntilService(vDistanceUntilService);
				monitorDocument.setVEngineCoolantTemperature(vEngineCoolantTemperature);
				monitorDocument.setVEngineLoad(vEngineLoad);
				monitorDocument.setVEngineSpeed(vEngineSpeed);
				monitorDocument.setVEngineTotalHours(vEngineTotalHours);
				monitorDocument.setVEngineTotalHoursIdle(vEngineTotalHoursIdle);
				monitorDocument.setVFuelCumulated(vFuelCumulated);
				monitorDocument.setVFuelCumulatedIdle(vFuelCumulatedIdle);
				monitorDocument.setVFuelCumulatedLR(vFuelCumulatedLR);
				monitorDocument.setVFuelLevel1(vFuelLevel1);
				monitorDocument.setVGearCurrent(vGearCurrent);
				monitorDocument.setVGearSelected(vGearSelected);
				monitorDocument.setVGrossWeightCombination(vGrossWeightCombination);
				monitorDocument.setVIgnitionState(vIgnitionState);
				monitorDocument.setVPedalBreakPosition1(vPedalBreakPosition1);
				monitorDocument.setVPowerBatteryChargeLevel(vPowerBatteryChargeLevel);
				monitorDocument.setVPowerBatteryVoltage(vPowerBatteryVoltage);
				monitorDocument.setVPTOEngaged(vPTOEngaged);
				monitorDocument.setVRetarderTorqueActual(vRetarderTorqueActual);
				monitorDocument.setVRetarderTorqueMode(vRetarderTorqueMode);
				monitorDocument.setVServiceBrakeAirPressure1(vServiceBrakeAirPressure1);
//				Long vServiceBrakeAirPressure2 = getValueAsLong(
//						(String) getValueByAttributeKey("ServiceBrakeCircuit2AirPressure", jsonNode));
				monitorDocument.setVServiceBrakeAirPressure2(vserviceBrakeAirPressure2);
				monitorDocument.setVTachographSpeed(vTachographSpeed);
				monitorDocument.setVWheelBasedSpeed(vWheelBasedSpeed);

				monitorObj.setDocument(monitorDocument);

				if (driver2WorkingState != null) {
					monitorDocument.setDriver2WorkingState((int) Math.round(driver2WorkingState));
				}

				if (driver1WorkingState != null) {
					monitorDocument.setDriver1WorkingState((int) Math.round(driver1WorkingState));
				}

				monitorDocument.setVEngineSpeed(vEngineSpeed);

				if (driver2CardInserted != null) {
					monitorDocument.setDriver2CardInserted(driver2CardInserted);
				}

				monitorDocument.setDriver1CardInserted(driver1CardInserted);

			}
		} catch (Exception ex) {
			logger.error("Monitor type message - Data preparing in indexobject is failed : Raw message is ::" + value);
			ex.printStackTrace();
			logger.error(
					"Monitor type message - error while parsing the message for prepare index obj for publish on kafka"
							+ ex.getMessage());
			System.out.println(
					"Monitor type messaeg -error while parsing the message for prepare index obj for publish on kafka"
							+ ex.getMessage());

		}
		monitorObj.setDocument(monitorDocument);
		// logger.info("Monitor - Before publishing
		// monitoringObj.toString()
		// record :: " + monitorObj.toString());
		System.out.println("Monitor -  before publishing kafka record :: " + monitorObj.toString());
		return monitorObj;

	}

	public static Status processStatusBoschMessage(String value, Properties properties, String kafkaProcessingTS) {
		logger.info("Status Raw Message processing start ::====>" + value.toString());
		System.out.println("Status Raw Message processing start ::====>" + value.toString());
		Status statusObj = new Status();
		StatusDocument statusDocument = new StatusDocument();

		try {
			/* json message parsing and store in jsonObject */
			/* json message parsing and store in jsonObject */
			JsonNode jsonNode = transformMessages(value);

			Long receivedTimestamp = System.currentTimeMillis();
			String vid = (String) getValueByAttributeKey("vehicleId", jsonNode);
			String vin = (String) getValueByAttributeKey("vin", jsonNode);
			String start = (String) getValueByAttributeKey("start", jsonNode);
			JsonNode reasonData = (JsonNode) jsonNode.get("reasonData");
			JsonNode document = (JsonNode) jsonNode.get("Document");
			if (document != null) {

				JsonNode gps = (JsonNode) document.get("GPS");

				Double latitude = getValueAsDouble((String) getValueByAttributeKey("latitude", gps));
				Double longitude = getValueAsDouble((String) getValueByAttributeKey("longitude", gps));
				Double direction = getValueAsDouble((String) getValueByAttributeKey("direction", gps));
				Long altitude = (Long) getValueByAttributeKey("altitude", gps);
				Long gpsSpeed = getValueAsLong((String) getValueByAttributeKey("gpsSpeed", gps));
				String tripid = (String) getValueByAttributeKey("tripId", document);
				String gpsTime = (String) getValueByAttributeKey("gpsTime", gps);

				String vehicleId = (String) getValueByAttributeKey("vehicleId", jsonNode);
				String transId = (String) getValueByAttributeKey("TransID", document);
				System.out.println("transId ==>" + transId);

				if (transId != null && transId.contains(".")) {
					transId = transId.substring(0, transId.indexOf("."));
				}

				statusDocument.setTripID(tripid);
				System.out.println(" Trans id :" + transId + " , trip id ==>" + tripid);
				logger.info(" Trans id :" + tripid + " , trip id ==>" + tripid);
				if (vid != null && !vid.isEmpty()) {
					statusObj.setVid(vid);
				} else {
					statusObj.setVid(DAFCT2Constant.UNKNOWN);
				}
				if (vin != null && !vin.isEmpty()) {
					statusObj.setVin(vin);
				} else {
					statusObj.setVin(DAFCT2Constant.UNKNOWN);
				}

				if (transId != null && !transId.isEmpty()) {

					statusObj.setTransID(getContiTransId(transId, properties));
				}

				Double packageIndex = getValueAsDouble((String) getValueByAttributeKey("packageIndex", jsonNode));
				if (packageIndex != null) {
					statusObj.setNumSeq(packageIndex.longValue());
				}

				String driver1Identification = (String) getValueByAttributeKey("Driver1Identification", document);

				Long vBreakDuration = getValueAsLong((String) getValueByAttributeKey("VBrakeDuration", document));
				Long vHarshBrakeDuration = getValueAsLong(
						(String) getValueByAttributeKey("VHarshBrakeDuration", document));
				Long vIdleDuration = getValueAsLong((String) getValueByAttributeKey("VIdleDuration", document));
				Long vPosAltitudeVariation = getValueAsLong(
						(String) getValueByAttributeKey("VPosAltitudeVariation", document));

				Long vNegAltitudeVariation = getValueAsLong(
						(String) getValueByAttributeKey("VNegAltitudeVariation", document));
				Long vpTOCnt = getValueAsLong((String) getValueByAttributeKey("VPTOCnt", document));
				Long vptoDuration = getValueAsLong((String) getValueByAttributeKey("VPTODuration", document));
				Long vpTODist = getValueAsLong((String) getValueByAttributeKey("VPTODist", document));
				Long vStopFuel = getValueAsLong(
						(String) getValueByAttributeKey("VEngineTotalFuelUsedHighResolution", document));

				statusObj.setDriverID(driver1Identification);

				String jobName = (String) getValueByAttributeKey("jobName", jsonNode);

				Long vStartFuel = getValueAsLong(
						(String) getValueByAttributeKey("VStartEngineTotalFuelUsedHighResolution", document));
				Integer vStopTankLevel = getValueAsInteger((String) getValueByAttributeKey("VFuelLevel1", document));
				Integer vStartTankLevel = getValueAsInteger(
						(String) getValueByAttributeKey("VStartFuelLevel1", document));
				Long vUsedFuel = getValueAsLong((String) getValueByAttributeKey("VUsedFuel", document));
				Long gpsTripDist = getValueAsLong((String) getValueByAttributeKey("GPSTripDist", document));
				Long VCruiseControlFuelConsumed = getValueAsLong(
						(String) getValueByAttributeKey("VCruiseControlFuelConsumed", document));
				Long vIdleFuelConsumed = getValueAsLong((String) getValueByAttributeKey("VIdleFuelConsumed", document));
				Long vMaxThrottlePaddleDuration = getValueAsLong(
						(String) getValueByAttributeKey("VMaxThrottlePaddleDuration", document));
				Long vSumTripDPABrakingScore = getValueAsLong(
						(String) getValueByAttributeKey("VSumTripDPABrakingScore", document));
				Long vSumTripDPAAnticipationScore = getValueAsLong(
						(String) getValueByAttributeKey("VSumTripDPAAnticipationScore", document));
				Long vTripAccelerationTime = getValueAsLong(
						(String) getValueByAttributeKey("VTripAccelerationTime", document));
				Long vTripCoastDist = getValueAsLong((String) getValueByAttributeKey("VTripCoastDist", document));
				Long vTripCoastDuration = getValueAsLong(
						(String) getValueByAttributeKey("VTripCoastDuration", document));
				Long vTripCoastFuelConsumed = getValueAsLong(
						(String) getValueByAttributeKey("VTripCoastFuelConsumed", document));
				Long vTripCruiseControlDuration = getValueAsLong(
						(String) getValueByAttributeKey("VTripCruiseControlDuration", document));
				Long vTripDPABrakingCount = getValueAsLong(
						(String) getValueByAttributeKey("VTripDPABrakingCount", document));
				Long vTripDPAAnticipationCount = getValueAsLong(
						(String) getValueByAttributeKey("VTripDPAAnticipationCount", document));
				Long vTripIdleFuelConsumed = getValueAsLong(
						(String) getValueByAttributeKey("VTripIdleFuelConsumed", document));
				Long vTripIdlePTODuration = getValueAsLong(
						(String) getValueByAttributeKey("VTripIdlePTODuration", document));
				Long vTripIdlePTOFuelConsumed = getValueAsLong(
						(String) getValueByAttributeKey("VTripIdlePTOFuelConsumed", document));
				Long vTripIdleWithoutPTODuration = getValueAsLong(
						(String) getValueByAttributeKey("VTripIdleWithoutPTODuration", document));

				Long vTripIdleWithoutPTOFuelConsumed = getValueAsLong(
						(String) getValueByAttributeKey("VTripIdleWithoutPTOFuelConsumed", document));
				statusDocument.setVTripIdleWithoutPTOFuelConsumed(vTripIdleWithoutPTOFuelConsumed);
				Long vTripMotionDuration = getValueAsLong(
						(String) getValueByAttributeKey("VTripMotionDuration", document));
				Long vTripMotionFuelConsumed = getValueAsLong(
						(String) getValueByAttributeKey("VTripMotionFuelConsumed", document));
				Long vTripMotionBrakeCount = getValueAsLong(
						(String) getValueByAttributeKey("VTripMotionBrakeCount", document));
				Long vTripMotionBrakeDist = getValueAsLong(
						(String) getValueByAttributeKey("VTripMotionBrakeDist", document));
				Long vTripMotionBrakeDuration = getValueAsLong(
						(String) getValueByAttributeKey("VTripMotionBrakeDuration", document));
				Long vTripMotionPTODuration = getValueAsLong(
						(String) getValueByAttributeKey("VTripMotionPTODuration", document));
				Long vTripMotionPTOFuelConsumed = getValueAsLong(
						(String) getValueByAttributeKey("VTripMotionPTOFuelConsumed", document));
				Long vTripPTOFuelConsumed = getValueAsLong(
						(String) getValueByAttributeKey("VTripPTOFuelConsumed", document));

				statusObj.setReceivedTimestamp(receivedTimestamp);
				statusObj.setEvtDateTime(start);
				statusObj.setVid(vehicleId);
				statusObj.setKafkaProcessingTS(kafkaProcessingTS);
				statusObj.setJobName(jobName);
				statusObj.setVBrakeDuration(vBreakDuration);
				statusObj.setVIdleDuration(vIdleDuration);
				statusObj.setVHarshBrakeDuration(vHarshBrakeDuration);
				statusObj.setVPosAltitudeVariation(vPosAltitudeVariation);
				statusObj.setVNegAltitudeVariation(vNegAltitudeVariation);
				statusObj.setVptoCnt(vpTOCnt);
				statusObj.setVptoDuration(vptoDuration);
				statusObj.setVptoDist(vpTODist);
				statusObj.setVStopFuel(vStopFuel);
				statusObj.setVStartFuel(vStartFuel);
				statusObj.setVStopTankLevel(vStopTankLevel);
				statusObj.setVStartTankLevel(vStartTankLevel);
				statusObj.setVUsedFuel(vUsedFuel);
				statusDocument.setGpsTripDist(gpsTripDist);
				statusDocument.setVCruiseControlFuelConsumed(VCruiseControlFuelConsumed);
				statusDocument.setVIdleFuelConsumed(vIdleFuelConsumed);
				statusDocument.setVMaxThrottlePaddleDuration(vMaxThrottlePaddleDuration);
				statusDocument.setVSumTripDPABrakingScore(vSumTripDPABrakingScore);
				statusDocument.setVSumTripDPAAnticipationScore(vSumTripDPAAnticipationScore);
				statusDocument.setVTripAccelerationTime(vTripAccelerationTime);
				statusDocument.setVTripCoastDist(vTripCoastDist);
				statusDocument.setVTripCoastDuration(vTripCoastDuration);
				statusDocument.setVTripCoastFuelConsumed(vTripCoastFuelConsumed);
				statusDocument.setVTripCruiseControlDuration(vTripCruiseControlDuration);
				statusDocument.setVTripDPABrakingCount(vTripDPABrakingCount);
				statusDocument.setVTripDPAAnticipationCount(vTripDPAAnticipationCount);
				statusDocument.setVTripIdleFuelConsumed(vTripIdleFuelConsumed);
				statusDocument.setVTripIdlePTODuration(vTripIdlePTODuration);
				statusDocument.setVTripIdlePTOFuelConsumed(vTripIdlePTOFuelConsumed);
				statusDocument.setVTripIdleWithoutPTODuration(vTripIdleWithoutPTODuration);
				statusDocument.setVTripIdlePTOFuelConsumed(vTripIdlePTOFuelConsumed);
				statusDocument.setVTripMotionDuration(vTripMotionDuration);
				statusDocument.setVTripMotionFuelConsumed(vTripMotionFuelConsumed);
				statusDocument.setVTripMotionBrakeCount(vTripMotionBrakeCount);
				statusDocument.setVTripMotionBrakeDist(vTripMotionBrakeDist);
				statusDocument.setVTripMotionBrakeDuration(vTripMotionBrakeDuration);
				statusDocument.setVTripMotionPTODuration(vTripMotionPTODuration);
				statusDocument.setVTripMotionPTOFuelConsumed(vTripMotionPTOFuelConsumed);
				statusDocument.setVTripPTOFuelConsumed(vTripPTOFuelConsumed);

				Distribution VIdleDurationDistrDistrBiute = getDistributedValuteByKey("VIdleDurationDistr", document);

				statusObj.setVIdleDurationDistr(VIdleDurationDistrDistrBiute);

				if (gpsSpeed != null) {
					statusDocument.setGpsSpeed(gpsSpeed);
				}

				Long vCruiseControlDist = getValueAsLong(
						(String) getValueByAttributeKey("VCruiseControlDist", document));
				statusObj.setVCruiseControlDist(vCruiseControlDist);
				Distribution vCruiseControlDistanceDistr = getDistributedValuteByKey("VCruiseControlDistanceDistr",
						document);

				statusDocument.setVCruiseControlDistanceDistr(vCruiseControlDistanceDistr);

				// TODO Distribution type
				Distribution vAccelerationPedalDistr = getDistributedValuteByKey("VAccelerationPedalDistr", document);

				statusDocument.setVAccelerationPedalDistr(vAccelerationPedalDistr);

				Distribution vEngineLoadAtEngineSpeedDistr = getDistributedValuteByKey("VEngineLoadAtEngineSpeedDistr",
						document);

				statusDocument.setVEngineLoadAtEngineSpeedDistr(vEngineLoadAtEngineSpeedDistr);

				Distribution vRetarderTorqueActualDistr = getDistributedValuteByKey("VRetarderTorqueActualDistr",
						document);

				statusDocument.setVRetarderTorqueActualDistr(vRetarderTorqueActualDistr);

				SpareMatrixAcceleration vAccelerationSpeed = getSparseMatrixValuesByKey("VAccelerationSpeed", document);

				statusDocument.setVAccelerationSpeed(vAccelerationSpeed);

				SpareMatrixAcceleration vRpmTorque = getSparseMatrixValuesByKey("VRpmTorque", document);

				statusDocument.setVRpmTorque(vRpmTorque);

				SpareMatrixAcceleration vSpeedRpm = getSparseMatrixValuesByKey("VSpeedRpm", document);

				statusDocument.setVSpeedRpm(vSpeedRpm);
				Integer vEvtID = getVEvtIdByIndexMessageByTripEnd(reasonData);
				statusObj.setVEvtID(vEvtID);
				String gpsStartDateTime = (String) getValueByAttributeKey("VTripDuration", document);

				statusObj.setGpsStartDateTime(gpsStartDateTime);
				statusObj.setGpsEndDateTime(gpsTime);

				Double gpsStartLatitude = getValueAsDouble(
						(String) getValueByAttributeKey("GPSStartLatitude", document));

				statusObj.setGpsStartLatitude(gpsStartLatitude);
				Double gpsEndLatitude = getValueAsDouble((String) getValueByAttributeKey("latitude", gps));

				statusObj.setGpsEndLatitude(gpsEndLatitude);

				Double gpsStartLongitude = getValueAsDouble(
						(String) getValueByAttributeKey("GPSStartLongitude", document));

				statusObj.setGpsStartLongitude(gpsStartLongitude);

				Double gpsEndLongitude = getValueAsDouble((String) getValueByAttributeKey("longitude", gps));

				statusObj.setGpsEndLongitude(gpsEndLongitude);

				Long gpsStartVehDist = getValueAsLong(
						(String) getValueByAttributeKey("StartTotalVehicleDistanceHighResolution", document));

				statusObj.setGpsStartVehDist(gpsStartVehDist);

				Long gpsStopVehDist = getValueAsLong(
						(String) getValueByAttributeKey("TotalVehicleDistanceHighResolution", document));

				statusObj.setGpsStopVehDist(gpsStopVehDist);
				statusObj.setVStartTankLevel(vStartTankLevel);

			}

		} catch (Exception ex) {
			logger.error("Status type message - Data preparing in indexobject is failed : Raw message is ::" + value);
			ex.printStackTrace();
			logger.error(
					"Status type message - error while parsing the message for prepare index obj for publish on kafka"
							+ ex.getMessage());
			System.out.println(
					"Status type messaeg -error while parsing the message for prepare index obj for publish on kafka"
							+ ex.getMessage());
		}

		statusObj.setDocument(statusDocument);

		logger.info(
				" Status type message - before publishing monitoringObj.toString() record :: " + statusObj.toString());
		System.out.println(" Status type message - before publishing kafka record :: " + statusObj.toString());
		return statusObj;
	}

	public static ObjectNode transformMessages(String message) throws DAFCT2Exception {

		ObjectMapper objectMapper = new ObjectMapper();
		ObjectNode result = objectMapper.createObjectNode();
		try {
			JsonNode jsonNodeRec = objectMapper.readTree(message);

			JsonNode metaData = (JsonNode) jsonNodeRec.get("metaData");
			JsonNode correlations = (JsonNode) jsonNodeRec.get("correlations");
			JsonNode resultData = (JsonNode) jsonNodeRec.get("resultData");

			JsonNode seriesPartsList = (JsonNode) resultData.get("seriesParts");
			JsonNode vehicle = (JsonNode) metaData.get("vehicle");
			JsonNode reasonData = (JsonNode) jsonNodeRec.get("reasonData");

			(result).put("manufacturerName", vehicle.get("manufacturerName").asText());
			result.put("vehicleName", vehicle.get("vehicleName").asText());
			result.put("vehicleId", vehicle.get("vehicleId").asText());
			result.put("vin", vehicle.get("vin").asText());
			result.put("end", resultData.get("end").asText());
			result.put("start", resultData.get("start").asText());
			result.put("testStepId", resultData.get("testStepId").asText());
			result.put("timeStamp", resultData.get("start").asText());
			result.put("jobName", correlations.get("measurementConfigurationName").asText());
			result.put("packageIndex", resultData.get("packageIndex").asText());
			result.put("reasonData", reasonData);

			String period = null;
			ObjectNode document = objectMapper.createObjectNode();
			if (seriesPartsList != null) {
				Iterator<JsonNode> seriesPartelements = seriesPartsList.elements();
				if (seriesPartelements != null) {
					while (seriesPartelements.hasNext()) {
						JsonNode serieElement = seriesPartelements.next();
						JsonNode dataPoints = serieElement.get("dataPoints");
						JsonNode series = serieElement.get("series");
						if (series != null && period == null) {
							document.put("period", series.get("samplingRate").asText());
						}
						if (dataPoints != null && dataPoints.size() > 0) {
							int lenght = dataPoints.size();
							JsonNode dataPoint = dataPoints.get(lenght - 1);
							document.put(series.get("seriesName").asText(), dataPoint.get("value"));

							if ("VTripDuration".equals(series.get("seriesName").asText().trim())) {
								if (dataPoint.get("value") != null) {
									JsonNode vtripDurationJsonNode = dataPoint.get("value");
									if (vtripDurationJsonNode != null) {
										String vtripDuration = vtripDurationJsonNode.get("startTime").asText();
										document.put("VTripDuration", vtripDuration);
									}
								}
							}
							if ("TripCounter".equals(series.get("seriesName").asText().trim())) {
								if (dataPoint.get("value").asText() != null
										&& correlations.get("bootCycleId").asText() != null) {
									document.put("tripId", dataPoint.get("value").asText() + "-"
											+ correlations.get("bootCycleId").asText());
								}
							}
						}

					}
					result.put("Document", document);

				}

			}

		} catch (Exception e) {
			logger.error("issue while data parsing in tranformation " + ExceptionUtils.getFullStackTrace(e));
			logger.error("data parsing failed " + e.getMessage());
			System.out.println("Error while parsing the message failed " + e.getMessage() + "\n" + ", Raw  Message is :"
					+ message);
			e.printStackTrace();
			throw new DAFCT2Exception("issue while data parsing in tranformation. Error is: " + e.getMessage(), e);

		}

		return result;
	}

	public static ObjectNode transformMonitorMessages(String message) throws DAFCT2Exception {

		ObjectMapper objectMapper = new ObjectMapper();
		ObjectNode result = objectMapper.createObjectNode();
		try {
			JsonNode jsonNodeRec = objectMapper.readTree(message);

			JsonNode metaData = (JsonNode) jsonNodeRec.get("metaData");
			JsonNode correlations = (JsonNode) jsonNodeRec.get("correlations");
			JsonNode resultData = (JsonNode) jsonNodeRec.get("resultData");

			JsonNode seriesPartsList = (JsonNode) resultData.get("seriesParts");
			JsonNode vehicle = (JsonNode) metaData.get("vehicle");
			JsonNode reasonData = (JsonNode) jsonNodeRec.get("reasonData");

			(result).put("manufacturerName", vehicle.get("manufacturerName").asText());
			result.put("vehicleName", vehicle.get("vehicleName").asText());
			result.put("vehicleId", vehicle.get("vehicleId").asText());
			result.put("vin", vehicle.get("vin").asText());
			result.put("end", resultData.get("end").asText());
			result.put("start", resultData.get("start").asText());
			result.put("testStepId", resultData.get("testStepId").asText());
			result.put("timeStamp", resultData.get("start").asText());
			result.put("jobName", correlations.get("measurementConfigurationName").asText());
			result.put("packageIndex", resultData.get("packageIndex").asText());
			result.put("reasonData", reasonData);

			String period = null;
			ObjectNode document = objectMapper.createObjectNode();
			if (seriesPartsList != null) {
				Iterator<JsonNode> seriesPartelements = seriesPartsList.elements();
				if (seriesPartelements != null) {
					while (seriesPartelements.hasNext()) {
						JsonNode serieElement = seriesPartelements.next();
						JsonNode dataPoints = serieElement.get("dataPoints");
						JsonNode series = serieElement.get("series");
						if (series != null && period == null) {
							document.put("period", series.get("samplingRate").asText());
						}
						if (dataPoints != null && dataPoints.size() > 0) {
							int lenght = dataPoints.size();
							JsonNode dataPoint = dataPoints.get(lenght - 1);
							document.put(series.get("seriesName").asText(), dataPoint.get("value"));

							if ("VTripDuration".equals(series.get("seriesName").asText().trim())) {
								if (dataPoint.get("value") != null) {
									JsonNode vtripDurationJsonNode = dataPoint.get("value");
									if (vtripDurationJsonNode != null) {
										String vtripDuration = vtripDurationJsonNode.get("startTime").asText();
										document.put("VTripDuration", vtripDuration);
									}
								}
							}
							if ("TripCounter".equals(series.get("seriesName").asText().trim())) {
								if (dataPoint.get("value").asText() != null
										&& correlations.get("bootCycleId").asText() != null) {
									document.put("tripId", dataPoint.get("value").asText() + "-"
											+ correlations.get("bootCycleId").asText());
								}
							}
						}

					}
					result.put("Document", document);

				}

			}

		} catch (Exception e) {
			logger.error("issue while data parsing in tranformation " + ExceptionUtils.getFullStackTrace(e));
			logger.error("data parsing failed " + e.getMessage());
			System.out.println("Error while parsing the message failed " + e.getMessage() + "\n" + ", Raw  Message is :"
					+ message);
			e.printStackTrace();
			throw new DAFCT2Exception("issue while data parsing in tranformation. Error is: " + e.getMessage(), e);

		}

		return result;
	}

	private static Double getValueAsDouble(String value) {
		if (value != null && !value.trim().isEmpty()) {
			return Double.valueOf(value);
		} else {
			return null;
		}
	}

	public static Object getValueByAttributeKey(String key, JsonNode jsonNode) throws DAFCT2Exception {
		try {

			if (jsonNode != null && key != null && null != jsonNode.get(key)) {

				return jsonNode.get(key).asText();
			}
		} catch (Exception ex) {
			ex.printStackTrace();
			logger.error("Error while getting the value from key = " + key + "from Json." + ex.getMessage());
			System.err.println("Error while getting the value from key =" + key + "from Json." + ex.getMessage());
			throw new DAFCT2Exception("Error while getting the value from key =" + key + "from Json." + ex.getMessage(),
					ex);
		}

		return null;
	}

	private static Object getDoubleByAttributeKey(String key, JsonNode jsonNode) {
		if (jsonNode != null && null != jsonNode.get(key)) {
			return jsonNode.get(key).doubleValue();
		} else {
			return null;
		}
	}

	public static String getTransIdValue(String message) {

		String transId = "";
		ObjectMapper objectMapper = new ObjectMapper();
		ObjectNode result = objectMapper.createObjectNode();
		try {
			JsonNode jsonNodeRec = objectMapper.readTree(message);

			JsonNode resultData = (JsonNode) jsonNodeRec.get("resultData");
			JsonNode seriesPartsList = (JsonNode) resultData.get("seriesParts");

			if (seriesPartsList != null) {
				Iterator<JsonNode> seriesPartelements = seriesPartsList.elements();
				if (seriesPartelements != null) {
					while (seriesPartelements.hasNext()) {
						JsonNode serieElement = seriesPartelements.next();
						JsonNode dataPoints = serieElement.get("dataPoints");
						JsonNode series = serieElement.get("series");

						if (dataPoints != null) {
							int lenght = dataPoints.size();
							JsonNode dataPoint = dataPoints.get(lenght - 1);

							if ("TransID".equalsIgnoreCase(series.get("seriesName").asText().trim())) {

								transId = dataPoint.get("value").asText();
								if (transId != null && transId.contains(".")) {
									transId = transId.substring(0, transId.indexOf("."));
								}
								break;
							}
						}

					}

				}

			}

		} catch (Exception e) {
			/// logger.error("issue while transId value parsing " +
			/// ExceptionUtils.getFullStackTrace(e));
			// logger.error("issue while transId value parsing " +
			/// e.getMessage());
			e.printStackTrace();

		}

		System.out.println(" tranid value extracing" + transId);
		// logger.info(" tranid value extracing" + transId);)
		return transId;

	}

	public static Integer[] getIntResult(String value) {
		if (value != null && !value.trim().isEmpty()) {
			Double doubleValue = Double.parseDouble(value);
			Set<Integer> setOfInteger = new HashSet<>(Arrays.asList((int) Math.round(doubleValue)));
			int[] temp = setOfInteger.stream().mapToInt(Integer::intValue).toArray();
			Integer[] intarray = IntStream.of(temp).boxed().toArray(Integer[]::new);
			return intarray;
		} else {
			return null;
		}
	}

	private static Long getValueAsLong(String value) {
		try {
			if (value != null && !value.trim().isEmpty()) {
				Double doubleValue = Double.parseDouble(value);
				return doubleValue.longValue();
			}
		} catch (Exception ex) {
			ex.printStackTrace();
			logger.error("castring the value from string type to long type. value is:" + value + ex.getMessage());

		}
		return null;
	}

	public static Integer getValueAsInteger(String value) {
		if (value != null && !value.trim().isEmpty()) {
			Double doubleValue = Double.parseDouble(value);
			return doubleValue.intValue();
		} else {
			return null;
		}
	}

	private static String getStringAsValue(Double value) {
		if (value != null) {
			return value.toString();
		} else {
			return null;
		}
	}

	public static Double[] getDoubleResult(String value) {
		if (value != null && !value.trim().isEmpty()) {
			Double parseDoubleValue = Double.parseDouble(value);
			Set<Double> setOfInteger = new HashSet<>(Arrays.asList(parseDoubleValue));
			double[] temp = setOfInteger.stream().mapToDouble(Double::doubleValue).toArray();

			Double[] intarray = DoubleStream.of(temp).boxed().toArray(Double[]::new);
			return intarray;
		} else {
			return null;
		}

	}

	public static Long[] getLongResult(String strValue) {

		if (strValue != null) {
			Double value = Double.parseDouble(strValue);
			Set<Long> setOfInteger = new HashSet<>(Arrays.asList(value.longValue()));
			long[] temp = setOfInteger.stream().mapToLong(Long::longValue).toArray();

			Long[] intarray = LongStream.of(temp).boxed().toArray(Long[]::new);
			return intarray;
		} else {
			return null;
		}
	}

	/*
	 * private static String getVEvtIdByIndexMessageByTripStart(JsonNode
	 * reasonData) { String vEvtid = null; // Timer Event// Timer Event if
	 * (reasonData != null) { JsonNode startNode = (JsonNode)
	 * reasonData.get("start"); if (startNode != null && startNode.get("name")
	 * != null) { String vEvtidTempValue = startNode.get("name").asText(); if
	 * (vEvtidTempValue != null) { String[] arrVEvTId =
	 * vEvtidTempValue.split("_"); vEvtid = arrVEvTId[0]; }
	 * 
	 * } }
	 * 
	 * return vEvtid; }
	 */

	private static Integer getVEvtIdByIndexMessageByTripStart(JsonNode reasonData) {
		Integer vEvtid = null;
		try {
			if (reasonData != null) {
				JsonNode startNode = (JsonNode) reasonData.get("start");
				if (startNode != null && startNode.get("triggerElements") != null) {
					JsonNode triggerElementNode = startNode.get("triggerElements");
					System.out.println(triggerElementNode);
					Iterator<JsonNode> triggerElementNodes = triggerElementNode.elements();

					if (triggerElementNodes != null) {
						while (triggerElementNodes.hasNext()) {
							JsonNode triggerInnerElementNode = triggerElementNodes.next();
							String vEvtidTempValue = triggerInnerElementNode.get("name").asText();
							String[] arrVEvTId = vEvtidTempValue.split("_");
							if (arrVEvTId != null && arrVEvTId.length > 0) {
								vEvtid = Integer.parseInt(arrVEvTId[0]);
								break;

							}
						}

					}

				}
			}
		} catch (Exception ex) {
			logger.error("Error while extracting VEvtid field value for start trip: VEvtid :" + vEvtid + " Error is "
					+ ex.getMessage());
			System.out.println("Error while extracting VEvtid field value for start trip: VEvtid :" + vEvtid
					+ " Error is " + ex.getMessage());
		}

		return vEvtid;
	}

	private static Integer getVEvtIdByIndexMessageByTripEnd(JsonNode reasonData) {
		Integer vEvtid = 1; // Timer Event
		try {
			if (reasonData != null) {
				JsonNode startNode = (JsonNode) reasonData.get("start");
				if (startNode != null && startNode.get("name") != null) {
					String vEvtidTempValue = startNode.get("name").asText();
					if (vEvtidTempValue != null) {
						String[] arrVEvTId = vEvtidTempValue.split("_");
						vEvtid = Integer.parseInt(arrVEvTId[0]);
					}

				}
			}
		} catch (Exception ex) {
			logger.error("Error while extracting VEvtid field value for End trip: VEvtid :" + vEvtid + " Error is "
					+ ex.getMessage());
			System.out.println("Error while extracting VEvtid field value for End trip: VEvtid :" + vEvtid
					+ " Error is " + ex.getMessage());
		}

		return vEvtid;
	}

	public static String getContiTransId(String transId, Properties properties) {
		if (transId != null) {
			if (properties.getProperty(DAFCT2Constant.INDEX_TRANSID).trim().equalsIgnoreCase(transId)) {
				return properties.getProperty(DAFCT2Constant.CONTI_INDEX_TRANSID);
			} else if (properties.getProperty(DAFCT2Constant.MONITOR_TRANSID).trim().equalsIgnoreCase(transId)) {
				return properties.getProperty(DAFCT2Constant.CONTI_MONITOR_TRANSID);
			} else if (properties.getProperty(DAFCT2Constant.STATUS_TRANSID).trim().equalsIgnoreCase(transId)) {
				return properties.getProperty(DAFCT2Constant.CONTI_STATUS_TRANSID);
			}
		}
		return null;
	}

	/*
	 * public static Long[] getSparkMatrixAttributeValue(String string, JsonNode
	 * innerSparseMatrixJsonNode) { JsonNode jsonAObj =
	 * innerSparseMatrixJsonNode.get("A");
	 * System.out.println(" Key ==> Avalue inner ==>" +
	 * jsonAObj.toPrettyString()); if (jsonAObj != null &&
	 * jsonAObj.toPrettyString() != null) { String aTextValue =
	 * jsonAObj.toPrettyString(); aTextValue = aTextValue.substring(1,
	 * aTextValue.length() - 1); Long[] valueArrLong =
	 * getLongArrByStringValue(aTextValue);
	 * System.out.println(" A value is after replacing :" + aTextValue); return
	 * valueArrLong; } return null;
	 * 
	 * }
	 */

	/*
	 * public static Long[] getLongArrByStringValue(String value) {
	 * 
	 * if (value != null) {
	 * 
	 * String[] arr = value.split(","); Long[] longArr = new Long[arr.length];
	 * for (int i = 0; i < arr.length; i++) { longArr[i] =
	 * getValueAsLong(arr[i]); } return longArr; }
	 * 
	 * return null; }
	 */

	/*
	 * public static SpareMatrixAcceleration getSparseMatrixValuesByKey(String
	 * key, JsonNode document) { SpareMatrixAcceleration matrix = new
	 * SpareMatrixAcceleration(); if (key != null && document != null) { if
	 * (document.get(key) != null) { JsonNode innerJsonNode = document.get(key);
	 * System.out.println(" Key ==> A ==>" + innerJsonNode.toPrettyString()); if
	 * (innerJsonNode.get("sparseMatrix") != null) { JsonNode
	 * innerSparseMatrixJsonNode = innerJsonNode.get("sparseMatrix"); Long[]
	 * aArr = getSparkMatrixAttributeValue("A", innerSparseMatrixJsonNode);
	 * 
	 * matrix.setA(aArr); Long[] iaArr = getSparkMatrixAttributeValue("IA",
	 * innerSparseMatrixJsonNode);
	 * 
	 * matrix.setIa(iaArr); Long[] jaArr = getSparkMatrixAttributeValue("JA",
	 * innerSparseMatrixJsonNode);
	 * 
	 * matrix.setJa(jaArr); }
	 * 
	 * } } return matrix; }
	 */

	/*
	 * public static Distribution getDistributedValuteByKey(String key, JsonNode
	 * document) { Distribution distribute = new Distribution();
	 * System.out.println("NdArray jons obj ==>" + document); if (key != null &&
	 * document != null) { if (document.get(key) != null) { JsonNode
	 * innerJsonNode = document.get(key);
	 * System.out.println("NdArray jons obj ==>" + innerJsonNode); if
	 * (innerJsonNode.get("ndArray") != null) { // JsonNode
	 * innerSparseMatrixJsonNode = // innerJsonNode.get("ndArray"); Integer[]
	 * distrArrayInt = getSparkDistributedAttributeValue("ndArray",
	 * innerJsonNode); System.out.println("ndArray for Distribued ==>" +
	 * distrArrayInt); distribute.setDistrArrayInt(distrArrayInt); }
	 * 
	 * } } return distribute; }
	 */

	/*
	 * public static Integer[] getSparkDistributedAttributeValue(String key,
	 * JsonNode innerJsonNode) { JsonNode jsonAObj = innerJsonNode.get(key);
	 * 
	 * if (jsonAObj != null && jsonAObj.toPrettyString() != null) { String
	 * aTextValue = jsonAObj.toPrettyString(); aTextValue =
	 * aTextValue.substring(1, aTextValue.length() - 1); Integer[] valueArrLong
	 * = getIntegerArrByStringValue(aTextValue);
	 * 
	 * return valueArrLong; } return null; }
	 */

	/*
	 * private static Integer[] getIntegerArrByStringValue(String value) {
	 * 
	 * if (value != null) {
	 * 
	 * String[] arr = value.split(","); Integer[] longArr = new
	 * Integer[arr.length]; for (int i = 0; i < arr.length; i++) { longArr[i] =
	 * getValueAsInteger(arr[i]); } return longArr; }
	 * 
	 * return null; }
	 */

	public static Boolean getValueAsBoolean(String value) {
		if (value != null && !value.trim().isEmpty()) {
			return Boolean.parseBoolean(value);
		}

		return null;
	}

	// updted fix issue
	private static SpareMatrixAcceleration getSparseMatrixValuesByKey(String key, JsonNode document) {
		SpareMatrixAcceleration matrix = new SpareMatrixAcceleration();
		try {
			if (key != null && document != null) {
				if (document.get(key) != null) {
					JsonNode innerJsonNode = document.get(key);

					if (innerJsonNode.get("sparseMatrix") != null) {
						JsonNode innerSparseMatrixJsonNode = innerJsonNode.get("sparseMatrix");
						Long[] aArr = getSparkMatrixAttributeValue("A", innerSparseMatrixJsonNode);

						matrix.setA(aArr);
						Long[] iaArr = getSparkMatrixAttributeValue("IA", innerSparseMatrixJsonNode);

						matrix.setIa(iaArr);
						Long[] jaArr = getSparkMatrixAttributeValue("JA", innerSparseMatrixJsonNode);

						matrix.setJa(jaArr);
					}

				}
			}
		} catch (Exception ex) {

			ex.printStackTrace();
			System.out
					.println("Error while getting sparkmatrix value for key =>" + key + " ,ERROR =>" + ex.getMessage());
			logger.error("Error while getting sparkmatrix value for key =>" + key + ", ERROR =>" + ex.getMessage());
		}
		return matrix;
	}

	private static Distribution getDistributedValuteByKey(String key, JsonNode document) {
		Distribution distribute = new Distribution();
		try {
			System.out.println("NdArray jons obj ==>" + document);
			if (key != null && document != null) {
				if (document.get(key) != null) {
					JsonNode innerJsonNode = document.get(key);

					if (innerJsonNode.get("ndArray") != null) {

						Long[] distrArrayInt = getSparkDistributedAttributeValue("ndArray", innerJsonNode);
						System.out.println("ndArray for Distribued ==>" + distrArrayInt);
						distribute.setDistrArrayInt(distrArrayInt);
					}

				}
			}
		} catch (Exception ex) {
			logger.error("Error while parsing distributed vlaue for key =>" + key + " , Error is " + ex.getMessage());
			System.err.println(
					"Error while parsing distributed vlaue for key =>" + key + " , Error is " + ex.getMessage());
		}
		return distribute;
	}

	private static Long[] getSparkDistributedAttributeValue(String key, JsonNode innerJsonNode) {
		JsonNode jsonAObj = innerJsonNode.get(key);

		if (jsonAObj != null && jsonAObj.toPrettyString() != null) {
			String aTextValue = jsonAObj.toPrettyString();
			if (aTextValue != null && !aTextValue.trim().isEmpty()) {
				aTextValue = aTextValue.substring(1, aTextValue.length() - 1);
				Long[] valueArrLong = getLongArrByStringValue(aTextValue);

				return valueArrLong;
			}
		}
		return null;
	}

	private static Long[] getSparkMatrixAttributeValue(String key, JsonNode innerSparseMatrixJsonNode) {
		JsonNode jsonAObj = innerSparseMatrixJsonNode.get(key);

		if (jsonAObj != null && jsonAObj.toPrettyString() != null) {
			String aTextValue = jsonAObj.toPrettyString();
			aTextValue = aTextValue.substring(1, aTextValue.length() - 1);
			if (aTextValue != null && aTextValue.length() > 0) {
				Long[] valueArrLong = getLongArrByStringValue(aTextValue.trim());
				return valueArrLong;
			}

		}
		return null;

	}

	private static Long[] getLongArrByStringValue(String value) {

		if (value != null && !value.trim().isEmpty()) {

			String[] arr = value.split(",");
			Long[] longArr = new Long[arr.length];
			for (int i = 0; i < arr.length; i++) {

				longArr[i] = getValueAsLong(arr[i]);
			}
			return longArr;
		}

		return null;
	}

	private static Integer[] getIntegerArrByStringValue(String value) {

		if (value != null && !value.trim().isEmpty()) {

			String[] arr = value.split(",");
			Integer[] longArr = new Integer[arr.length];
			for (int i = 0; i < arr.length; i++) {
				longArr[i] = getValueAsInteger(arr[i]);
			}
			return longArr;
		}

		return null;
	}

	public static void storeDataInHbase(DataStream<KafkaRecord<String>> boschInputStream, Properties properties) {
		boschInputStream.map(new MapFunction<KafkaRecord<String>, KafkaRecord<String>>() {

			@Override
			public KafkaRecord<String> map(KafkaRecord<String> message) throws Exception {
				/* json message parsing and store in jsonObject */
				StringBuilder rowKey = new StringBuilder();
				String rowkeyAppender = "/";
				String vid = DAFCT2Constant.UNKNOWN;
				String transId = DAFCT2Constant.UNKNOWN;
				String source = DAFCT2Constant.UNKNOWN;
				String tripid = DAFCT2Constant.UNKNOWN;
				try {
					JsonNode jsonNode = MessageParseUtil.transformMessages(message.getValue());

					vid = (String) MessageParseUtil.getValueByAttributeKey("vehicleId", jsonNode);
					// vin = (String)
					// MessageParseUtil.getValueByAttributeKey("vin", jsonNode);
					JsonNode document = (JsonNode) jsonNode.get("Document");

					source = (String) MessageParseUtil.getValueByAttributeKey("jobName", jsonNode);

					if (source != null && source.toLowerCase()
							.contains(properties.getProperty(DAFCT2Constant.INDEXKEY).toLowerCase())) {
						source = properties.getProperty(DAFCT2Constant.INDEXKEY);
					} else if (source != null && source.toLowerCase()
							.contains(properties.getProperty(DAFCT2Constant.MONITORKEY).toLowerCase())) {
						source = properties.getProperty(DAFCT2Constant.MONITORKEY);
					} else if (source != null && source.toLowerCase()
							.contains(properties.getProperty(DAFCT2Constant.STATUSKEY).toLowerCase())) {
						source = properties.getProperty(DAFCT2Constant.STATUSKEY);
					}

					if (document != null) {

						tripid = (String) MessageParseUtil.getValueByAttributeKey("tripId", document);
						transId = (String) MessageParseUtil.getValueByAttributeKey("TransID", document);

						if (transId != null && transId.contains(".")) {
							transId = transId.substring(0, transId.indexOf("."));// 1000.0
						}
						System.out.println(transId);
						if (transId != null && !transId.isEmpty()) {

							transId = MessageParseUtil.getContiTransId(transId, properties);
						}

					}

				} catch (Exception ex) {
					logger.error("Error while preparing rowkey for Hbase row key  Error :" + ex.getMessage());
					System.err.println("Error while preparing rowkey for Hbase row key Error :" + ex.getMessage());
				}

				if (vid == null) {
					vid = DAFCT2Constant.UNKNOWN;
				}
				if (transId == null) {
					transId = DAFCT2Constant.UNKNOWN;
				}
				if (source == null) {
					source = DAFCT2Constant.UNKNOWN;
				}
				if (tripid == null) {
					tripid = DAFCT2Constant.UNKNOWN;
				}

				rowKey.append(source).append(rowkeyAppender).append(vid).append(rowkeyAppender).append(transId)
						.append(rowkeyAppender).append(tripid).append(rowkeyAppender)
						.append(TimeFormatter.getInstance().getCurrentUTCTime());
				message.setKey(rowKey.toString());
				System.out.println(rowKey.toString());

				return message;
			}
		}).addSink(new StoreHistoricalData(properties.getProperty(DAFCT2Constant.HBASE_ZOOKEEPER_QUORUM),
				properties.getProperty(DAFCT2Constant.HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT),
				properties.getProperty(DAFCT2Constant.ZOOKEEPER_ZNODE_PARENT),
				properties.getProperty(DAFCT2Constant.HBASE_REGIONSERVER),
				properties.getProperty(DAFCT2Constant.HBASE_MASTER),
				properties.getProperty(DAFCT2Constant.HBASE_REGIONSERVER_PORT),
				properties.getProperty(DAFCT2Constant.HBASE_BOSCH_HISTORICAL_TABLE_NAME),
				properties.getProperty(DAFCT2Constant.HBASE_BOSCH_HISTORICAL_TABLE_CF)));
	}

	/*
	 * private static Long[] getLongArrByStringValue(String value) {
	 * 
	 * if (value != null && !value.trim().isEmpty()) {
	 * 
	 * String[] arr = value.split(","); if (arr != null && arr.length > 0) {
	 * Long[] longArr = new Long[arr.length]; for (int i = 0; i < arr.length;
	 * i++) { longArr[i] = getValueAsLong(arr[i]); } return longArr; } }
	 * 
	 * return null; }
	 */

	private static long convertDateStringToTS(String dateStr) {

		try {
			if (Objects.nonNull(dateStr)) {
				return TimeFormatter.getInstance().convertUTCToEpochMilli(dateStr, DAFCT2Constant.DATE_FORMAT);
			} else {
				return 0;
			}
		} catch (Exception e) {
			// logger.error("Issue while converting Date String to epoch milli :
			// "+dateStr + " message :"+ stsMsg +" Exception: "+ e.getMessage()
			// );
			return 0;
		}

	}

	private static Integer getVEvtIdByMonitorMessage(JsonNode reasonData) {
		Integer vEvtid = null;
		try {
			if (reasonData != null) {
				JsonNode startNode = (JsonNode) reasonData.get("start");
				if (startNode != null && startNode.get("triggerElements") != null) {
					JsonNode triggerElementNode = startNode.get("triggerElements");

					Iterator<JsonNode> triggerElementsNode = triggerElementNode.elements();

					// if (triggerElementsNode != null) {
					while (triggerElementsNode != null && triggerElementsNode.hasNext()) {
						JsonNode triggerInnerElementNode = triggerElementsNode.next();
						// System.out.println(" testing outer name " +
						// triggerInnerElementNode.get("name").asText());
						if (triggerInnerElementNode != null && triggerInnerElementNode.get("name").asText() != null) {
							String vEvtidTempValue = triggerInnerElementNode.get("name").asText();
							String[] arrVEvTId = vEvtidTempValue.split("_");
							if (arrVEvTId != null && arrVEvTId.length > 1) {
								System.out.println("parsing VEVtid is :" + arrVEvTId[0]);
								vEvtid = Integer.parseInt(arrVEvTId[0]);
								break;
							}
						}
					}

				}
			}
		} catch (Exception ex) {
			logger.error("Error while parsing VEVtid of monitoring " + ex.getMessage());
			System.out.println("Error while parsing VEVtid of monitoring " + ex.getMessage());
		}
		return vEvtid;
	}

}
