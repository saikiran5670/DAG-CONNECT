package net.atos.daf.ct2.common.realtime.hbase;

import java.io.IOException;
import java.util.Arrays;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.apache.hadoop.hbase.TableName;
import org.apache.hadoop.hbase.client.Put;
import org.apache.hadoop.hbase.client.Table;
import org.apache.hadoop.hbase.util.Bytes;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.realtime.dataprocess.StatusDataProcess;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.hbase.connection.HbaseAdapter;
import net.atos.daf.hbase.connection.HbaseConnection;
import net.atos.daf.hbase.connection.HbaseConnectionPool;

public class StatusDataHbaseSink extends RichSinkFunction<KafkaRecord<Status>> {

	private static final long serialVersionUID = 3842371782145886991L;
	Logger log = LoggerFactory.getLogger(StatusDataProcess.class);

	private Table table = null;
	private HbaseConnection conn = null;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		log.info("########## In Status Data HBase ##############");
		super.open(parameters);
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		HbaseAdapter hbaseAdapter = HbaseAdapter.getInstance();

		String tableName = envParams.get(DafConstants.HBASE_TABLE_NAME);

		HbaseConnectionPool connectionPool = hbaseAdapter.getConnection(
				envParams.get(DafConstants.HBASE_ZOOKEEPER_QUORUM),
				envParams.get(DafConstants.HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT),
				envParams.get(DafConstants.ZOOKEEPER_ZNODE_PARENT), envParams.get(DafConstants.HBASE_REGIONSERVER),
				envParams.get(DafConstants.HBASE_MASTER), envParams.get(DafConstants.HBASE_REGIONSERVER_PORT),
				tableName);

		try {
			conn = connectionPool.getHbaseConnection();
			if (null == conn) {
				log.warn("get connection from pool failed");

			}
			TableName tabName = TableName.valueOf(tableName);
			table = conn.getConnection().getTable(tabName);

			log.info("table_name -- " + tableName);

		} catch (IOException e) {
			log.error("create connection failed from the configuration" + e.getMessage());
			throw e;
		} catch (Exception e) {

			log.error("there is an exception" + e.getMessage());
			throw e;
		}

		log.info("Status table name - " + tableName);
	}

	public void invoke(KafkaRecord<Status> value, Context context) throws Exception {
		Long currentTimeStamp=TimeFormatter.getInstance().getCurrentUTCTime();
		
		
		
		if(value.getValue().getEvtDateTime()!=null) {
		      currentTimeStamp = TimeFormatter.getInstance().convertUTCToEpochMilli(
		                        value.getValue().getEvtDateTime(), DafConstants.DTM_TS_FORMAT);
		}
		
		Put put = new Put(Bytes.toBytes(value.getValue().getTransID() + "_" + value.getValue().getDocument().getTripID()
				+ "_" + value.getValue().getVid() + "_" + currentTimeStamp));

		log.info("Status Data Row_Key :: "
				+ (value.getValue().getTransID() + "_" + value.getValue().getDocument().getTripID() + "_"
						+ value.getValue().getVid() + "_" + currentTimeStamp));
		// we have commented some of the below column values as we need to use them in
		// future
		put.addColumn(Bytes.toBytes("f"), Bytes.toBytes("DocFormat"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocFormat())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DocVersion"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocVersion())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DriverID"),
				Bytes.toBytes(String.valueOf(value.getValue().getDriverID())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("EventDateTimeFirstIndex"),
				Bytes.toBytes(String.valueOf(value.getValue().getEventDateTimeFirstIndex())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("EvtDateTime"),
				Bytes.toBytes(String.valueOf(value.getValue().getEvtDateTime())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSEndDateTime"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsEndDateTime())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSEndLatitude"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsEndLatitude())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSEndLongitude"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsEndLongitude())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSStartDateTime"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsStartDateTime())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSStartLatitude"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsStartLatitude())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSStartLongitude"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsStartLongitude())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSStartVehDist"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsStartVehDist())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSStopVehDist"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsStopVehDist())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Increment"),
				Bytes.toBytes(String.valueOf(value.getValue().getIncrement())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Jobname"),
				Bytes.toBytes(String.valueOf(value.getValue().getJobName())));
		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes ("messageKey") ,
		// Bytes.toBytes(String.valueOf(value.getValue(). getMessageKey ())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("NumberOfIndexMessage"),
				Bytes.toBytes(String.valueOf(value.getValue().getNumberOfIndexMessage())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("NumSeq"),
				Bytes.toBytes(String.valueOf(value.getValue().getNumSeq())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("receivedTimestamp"),
				Bytes.toBytes(String.valueOf(value.getValue().getReceivedTimestamp())));
		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes ("Reserve0") ,
		// Bytes.toBytes(String.valueOf(value.getValue(). getReserve0 ())));
		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes ("Reserve1") ,
		// Bytes.toBytes(String.valueOf(value.getValue(). getReserve1 ())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("ROmodel"),
				Bytes.toBytes(String.valueOf(value.getValue().getRoModel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("ROName"),
				Bytes.toBytes(String.valueOf(value.getValue().getRoName())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("ROProfil"),
				Bytes.toBytes(String.valueOf(value.getValue().getRoProfil())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("ROrelease"),
				Bytes.toBytes(String.valueOf(value.getValue().getRoRelease())));
		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes ("TargetM2M") ,
		// Bytes.toBytes(String.valueOf(value.getValue(). getTargetM2M ())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TenantID"),
				Bytes.toBytes(String.valueOf(value.getValue().getTenantID())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TransID"),
				Bytes.toBytes(String.valueOf(value.getValue().getTransID())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VBrakeDuration"),
				Bytes.toBytes(String.valueOf(value.getValue().getVBrakeDuration())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VCruiseControlDist"),
				Bytes.toBytes(String.valueOf(value.getValue().getVCruiseControlDist())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VEvtID"),
				Bytes.toBytes(String.valueOf(value.getValue().getVEvtID())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VHarshBrakeDuration"),
				Bytes.toBytes(String.valueOf(value.getValue().getVHarshBrakeDuration())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VID"),
				Bytes.toBytes(String.valueOf(value.getValue().getVid())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VIN"),
				Bytes.toBytes(String.valueOf(value.getValue().getVin())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VIdleDuration"),
				Bytes.toBytes(String.valueOf(value.getValue().getVIdleDuration())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VNegAltitudeVariation"),
				Bytes.toBytes(String.valueOf(value.getValue().getVNegAltitudeVariation())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VPosAltitudeVariation"),
				Bytes.toBytes(String.valueOf(value.getValue().getVPosAltitudeVariation())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VPTOCnt"),
				Bytes.toBytes(String.valueOf(value.getValue().getVptoCnt())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VPTODist"),
				Bytes.toBytes(String.valueOf(value.getValue().getVptoDist())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VPTODuration"),
				Bytes.toBytes(String.valueOf(value.getValue().getVptoDuration())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VStartFuel"),
				Bytes.toBytes(String.valueOf(value.getValue().getVStartFuel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VStartTankLevel"),
				Bytes.toBytes(String.valueOf(value.getValue().getVStartTankLevel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VStopFuel"),
				Bytes.toBytes(String.valueOf(value.getValue().getVStopFuel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VStopTankLevel"),
				Bytes.toBytes(String.valueOf(value.getValue().getVStopTankLevel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VUsedFuel"),
				Bytes.toBytes(String.valueOf(value.getValue().getVUsedFuel())));

		// doc
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VIdleFuelConsumed"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVIdleFuelConsumed())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TripHaversineDistance"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getTripHaversineDistance())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VTripAccelerationTime"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVTripAccelerationTime())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VMaxThrottlePaddleDuration"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVMaxThrottlePaddleDuration())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VSumTripDPABrakingScore"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVSumTripDPABrakingScore())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VSumTripDPAAnticipationScore"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVSumTripDPAAnticipationScore())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VTripDPABrakingCount"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVTripDPABrakingCount())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VTripDPAAnticipationCount"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVTripDPAAnticipationCount())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VCruiseControlFuelConsumed"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVCruiseControlFuelConsumed())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("AxeVariantId"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getAxeVariantId())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VTripIdleFuelConsumed"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVTripIdleFuelConsumed())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSTripDist"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getGpsTripDist())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TripID"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getTripID())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VEvtCause"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVEvtCause())));

		// VCruiseControlDistanceDistr
		System.out.println("VCruiseControlDistanceDistr Data Fields in Status Data");

		if (value.getValue().getDocument().getVCruiseControlDistanceDistr() != null) {

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VCruiseDistrMinRangeInt"), Bytes.toBytes(String
					.valueOf(value.getValue().getDocument().getVCruiseControlDistanceDistr().getDistrMinRangeInt())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VCruiseDistrMaxRangeInt"), Bytes.toBytes(String
					.valueOf(value.getValue().getDocument().getVCruiseControlDistanceDistr().getDistrMaxRangeInt())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VCruiseDistrStep"), Bytes.toBytes(
					String.valueOf(value.getValue().getDocument().getVCruiseControlDistanceDistr().getDistrStep())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VCruiseDistrArrayInt"), Bytes.toBytes(Arrays
					.toString(value.getValue().getDocument().getVCruiseControlDistanceDistr().getDistrArrayInt())));
		}
		// VRpmTorque
		System.out.println("getStatusVRpmTorque Data Fields in Status Data");

		if (value.getValue().getDocument().getVRpmTorque() != null) {

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Vrpm_abs"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVRpmTorque().getAbs())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Vrpm_ord"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVRpmTorque().getOrd())));

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Vrpm_A"),
					Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVRpmTorque().getA())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Vrpm_IA"),
					Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVRpmTorque().getIa())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Vrpm_JA"),
					Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVRpmTorque().getJa())));
		}
		// VSpeedRpm

		System.out.println("StatusVSpeedRpm Data Fields in Status Data");

		if (value.getValue().getDocument().getVSpeedRpm() != null) {
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Vspeed_abs"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVSpeedRpm().getAbs())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Vspeed_ord"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVSpeedRpm().getOrd())));

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Vspeed_A"),
					Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVSpeedRpm().getA())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Vspeed_IA"),
					Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVSpeedRpm().getIa())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Vspeed_JA"),
					Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVSpeedRpm().getJa())));
		}

		// VAccelerationSpeed

		System.out.println("VAccelerationSpeed Data Fields in Status Data");

		if (value.getValue().getDocument().getVAccelerationSpeed() != null) {

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VAcceleration_abs"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVAccelerationSpeed().getAbs())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VAcceleration_ord"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVAccelerationSpeed().getOrd())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VAcceleration_A"),
					Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVAccelerationSpeed().getA())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VAcceleration_A_VBrake"), Bytes
					.toBytes(Arrays.toString(value.getValue().getDocument().getVAccelerationSpeed().getA_VBrake())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VAcceleration_IA"),
					Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVAccelerationSpeed().getIa())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VAcceleration_JA"),
					Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVAccelerationSpeed().getJa())));
		}
		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes ("M2M_in") ,
		// Bytes.toBytes(String.valueOf(value.getValue().getTimestamps().getM2M_in())));
		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes ("M2M_out") ,
		// Bytes.toBytes(String.valueOf(value.getValue().getTimestamps().getM2M_out())));

		// VIdleDurationDistr
		System.out.println("VIdleDurationDistr Data Fields in Status Data");
		if (value.getValue().getVIdleDurationDistr() != null) {

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VIdleDistrStep"),
					Bytes.toBytes(String.valueOf(value.getValue().getVIdleDurationDistr().getDistrStep())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VIdleDistrMaxRangeInt"),
					Bytes.toBytes(String.valueOf(value.getValue().getVIdleDurationDistr().getDistrMaxRangeInt())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VIdleDistrMinRangeInt"),
					Bytes.toBytes(String.valueOf(value.getValue().getVIdleDurationDistr().getDistrMinRangeInt())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VIdleDistrArrayInt"),
					Bytes.toBytes(Arrays.toString(value.getValue().getVIdleDurationDistr().getDistrArrayInt())));
		}
		// VAccelerationPedalDistr
		System.out.println("VAccelerationPedalDistr Data Fields in Status Data");
		if (value.getValue().getDocument().getVAccelerationPedalDistr() != null) {
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DistrMinRangeInt"), Bytes.toBytes(
					String.valueOf(value.getValue().getDocument().getVAccelerationPedalDistr().getDistrMinRangeInt())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DistrMaxRangeInt"), Bytes.toBytes(
					String.valueOf(value.getValue().getDocument().getVAccelerationPedalDistr().getDistrMaxRangeInt())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DistrStep"), Bytes.toBytes(
					String.valueOf(value.getValue().getDocument().getVAccelerationPedalDistr().getDistrStep())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DistrArrayTime"), Bytes.toBytes(
					Arrays.toString(value.getValue().getDocument().getVAccelerationPedalDistr().getDistrArrayInt())));
		}
		// VEngineLoadAtEngineSpeedDistr
		System.out.println("VEngineLoadAtEngineSpeedDistr Data Fields in Status Data");

		if (value.getValue().getDocument().getVEngineLoadAtEngineSpeedDistr() != null) {

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DistrMinRangeInt"), Bytes.toBytes(String
					.valueOf(value.getValue().getDocument().getVEngineLoadAtEngineSpeedDistr().getDistrMinRangeInt())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DistrMaxRangeInt"), Bytes.toBytes(String
					.valueOf(value.getValue().getDocument().getVEngineLoadAtEngineSpeedDistr().getDistrMaxRangeInt())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DistrStep"), Bytes.toBytes(
					String.valueOf(value.getValue().getDocument().getVEngineLoadAtEngineSpeedDistr().getDistrStep())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DistrArrayInt"), Bytes.toBytes(Arrays
					.toString(value.getValue().getDocument().getVEngineLoadAtEngineSpeedDistr().getDistrArrayInt())));
		}
		// VRetarderTorqueActualDistr
		System.out.println("VRetarderTorqueActualDistr Data Fields in Status Data");

		if (value.getValue().getDocument().getVRetarderTorqueActualDistr() != null) {
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DistrMinRangeInt"), Bytes.toBytes(String
					.valueOf(value.getValue().getDocument().getVRetarderTorqueActualDistr().getDistrMinRangeInt())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DistrMaxRangeInt"), Bytes.toBytes(String
					.valueOf(value.getValue().getDocument().getVRetarderTorqueActualDistr().getDistrMaxRangeInt())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DistrStep "), Bytes.toBytes(
					String.valueOf(value.getValue().getDocument().getVRetarderTorqueActualDistr().getDistrStep())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DistrArrayInt"), Bytes.toBytes(Arrays
					.toString(value.getValue().getDocument().getVRetarderTorqueActualDistr().getDistrArrayInt())));
		}
		table.put(put);

	}

	@Override
	public void close() throws Exception {

		try {
			if (table != null) {
				table.close();
			}

			if (conn != null) {
				conn.releaseConnection();
			}

		} catch (IOException e) {
			log.error("Close HBase Exception:", e.toString());
		}

	}
}
