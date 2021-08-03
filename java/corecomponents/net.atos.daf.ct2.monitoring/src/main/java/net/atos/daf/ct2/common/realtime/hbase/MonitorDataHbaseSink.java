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
import net.atos.daf.ct2.common.realtime.dataprocess.MonitorDataProcess;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.hbase.connection.HbaseAdapter;
import net.atos.daf.hbase.connection.HbaseConnection;
import net.atos.daf.hbase.connection.HbaseConnectionPool;

public class MonitorDataHbaseSink extends RichSinkFunction<KafkaRecord<Monitor>> {

	private static final long serialVersionUID = 3842371782145886991L;
	Logger log = LoggerFactory.getLogger(MonitorDataProcess.class);

	private Table table = null;
	private HbaseConnection conn = null;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		log.info("########## In Monitoring Data HBase ##############");
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

		log.info("Monitoring table name - " + tableName);
	}

	public void invoke(KafkaRecord<Monitor> value, Context context) throws Exception {
		
		int messageType = 0;
		Long currentTimeStamp=TimeFormatter.getInstance().getCurrentUTCTime();
		
		if (value.getValue().getMessageType() != null) {
		messageType = value.getValue().getMessageType() ;
		}

		if(value.getValue().getEvtDateTime()!=null) {
		      currentTimeStamp = TimeFormatter.getInstance().convertUTCToEpochMilli(
		                        value.getValue().getEvtDateTime(), DafConstants.DTM_TS_FORMAT);
		}
		
		      Put put = new Put(Bytes.toBytes(value.getValue().getTransID() + "_" +
		    		  value.getValue().getDocument().getTripID() + "_" + value.getValue().getVid()
		    		  + "_" + currentTimeStamp));
		
		log.info("Monitoring Data Row_Key :: "
				+ (value.getValue().getTransID() + "_" + value.getValue().getDocument().getTripID() + "_"
						+ value.getValue().getVid() + "_" + currentTimeStamp));
		System.out.println("Monitoring Data Row_Key :: "
				+ (value.getValue().getTransID() + "_" + value.getValue().getDocument().getTripID() + "_"
						+ value.getValue().getVid() + "_" + currentTimeStamp));
		
		 
		// we have commented some of the below column values as we need to use them in
		// future
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TransID"),
				Bytes.toBytes(String.valueOf(value.getValue().getTransID())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("MessageType"),
				Bytes.toBytes(String.valueOf(messageType)));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VEvtID"),
				Bytes.toBytes(String.valueOf(value.getValue().getVEvtID())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSSpeed"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getGpsSpeed())));
		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VFuelLevel"),
		// Bytes.toBytes(String.valueOf(value.getValue().getDocument().getvFuelLevel1())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VDist"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVDist())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VID"),
				Bytes.toBytes(String.valueOf(value.getValue().getVid())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VIN"),
				Bytes.toBytes(String.valueOf(value.getValue().getVin())));

		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Increment"),
				Bytes.toBytes(String.valueOf(value.getValue().getIncrement())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DocFormat"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocFormat())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TenantID"),
				Bytes.toBytes(String.valueOf(value.getValue().getTenantID())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSHeading"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsHeading())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSLongitude"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsLongitude())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DocVersion"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocVersion())));
		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Reserve0"),
		// Bytes.toBytes(String.valueOf(value.getValue().getReserve0())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("ROProfil"),
				Bytes.toBytes(String.valueOf(value.getValue().getRoProfil())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("ROrelease"),
				Bytes.toBytes(String.valueOf(value.getValue().getRoRelease())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSAltitude"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsAltitude())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSLatitude"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsLatitude())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Jobname"),
				Bytes.toBytes(String.valueOf(value.getValue().getJobName())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("NumSeq"),
				Bytes.toBytes(String.valueOf(value.getValue().getNumSeq())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("ROmodel"),
				Bytes.toBytes(String.valueOf(value.getValue().getRoModel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSDateTime"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsDateTime())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("EvtDateTime"),
				Bytes.toBytes(String.valueOf(value.getValue().getEvtDateTime())));
		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TargetM2M"),
		// Bytes.toBytes(String.valueOf(value.getValue().getTargetM2M())));
		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Reserve1"),
		// Bytes.toBytes(String.valueOf(value.getValue().getReserve1())));
		put.addColumn(Bytes.toBytes("f"), Bytes.toBytes("ROName"),
				Bytes.toBytes(String.valueOf(value.getValue().getRoName())));
		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("MessageKey"),
		// Bytes.toBytes(String.valueOf(value.getValue().getMessageKey())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("receivedTimestamp"),
				Bytes.toBytes(String.valueOf(value.getValue().getReceivedTimestamp())));

		if (messageType == 3) {
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VEvtCause"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVEvtCause())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VEngineSpeed"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVEngineSpeed())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VIgnitionState"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVIgnitionState())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VPowerBatteryChargeLevel"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVPowerBatteryChargeLevel())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VPowerBatteryVoltage"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVPowerBatteryVoltage())));
		}

		if (messageType == 6) {
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DriverID"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriverID())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Driver1CardInserted"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriver1CardInserted())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_SPN"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1SPN())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_FMI"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1FMI())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_active"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1Active())));

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_SA"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1SA())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_data"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1Data())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_AWL"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1AWL())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_MIL"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1MIL())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_RSL"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1RSL())));

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_PLS"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1PLS())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_FAWL"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1AWL())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_FMIL"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1FMIL())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_FRSL"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1FRSL())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_FPLS"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1FPLS())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DM1_OC"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDm1OC())));
		}

		if (messageType == 7) {
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Driver1WorkingState"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriver1WorkingState())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DriverID"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriverID())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Driver1CardInserted"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriver1CardInserted())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Driver2WorkingState"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriver2WorkingState())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Driver2ID"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriver2ID())));

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Driver2CardInserted"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriver2CardInserted())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VWheelBasedSpeed"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVWheelBasedSpeed())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TripID"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getTripID())));

		}

		if (messageType == 8) {
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TT_Id"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getTtId())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TT_Value"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getTtValue())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TT_Norm"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getTtNorm())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VWheelBasedSpeed"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVWheelBasedSpeed())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VTachographSpeed"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVTachographSpeed())));

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VDEFTankLevel"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVDEFTankLevel())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VAmbiantAirTemperature"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVAmbiantAirTemperature())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VAmbientAirTemperature"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVAmbiantAirTemperature())));

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VEngineCoolantTemperature"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVEngineCoolantTemperature())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VServiceBrakeAirPressure1"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVServiceBrakeAirPressure1())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VServiceBrakeAirPressure2"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVServiceBrakeAirPressure2())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TT_ListValue"),
					Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getTtListValue())));

		}

		if (messageType == 10) {
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VRetarderTorqueActual"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVRetarderTorqueActual())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VRetarderTorqueMode"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVRetarderTorqueMode())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VPedalBreakPosition1"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVPedalBreakPosition1())));
			// put.addColumn(Bytes.toBytes("t"),
			// Bytes.toBytes("VPedalAcceleratorPosition1"),
			// Bytes.toBytes(String.valueOf(value.getValue().getDocument().getvPedalAcceleratorPosition1())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VEngineSpeed"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVEngineSpeed())));

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VEngineLoad"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVEngineLoad())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VGearCurrent"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVGearCurrent())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VGearSelected"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVGearSelected())));

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VPTOEngaged"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVPTOEngaged())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VEngineCoolantTemperature"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVEngineCoolantTemperature())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VTachographSpeed"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVTachographSpeed())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VGrossWeightCombination"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVGrossWeightCombination())));

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VCruiseControl"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVCruiseControl())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VAmbiantAirTemperature"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVAmbiantAirTemperature())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VAmbientAirTemperature"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVAmbiantAirTemperature())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VPowerBatteryVoltage"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVPowerBatteryVoltage())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VWarningClass"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVWarningClass())));

			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VWarningNumber"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVWarningNumber())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VWarningState"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVWarningState())));
			put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VWheelBasedSpeed"),
					Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVWheelBasedSpeed())));

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
			log.error("Error in MonitorDataHBase:", e.getMessage());
		}

	}
}
