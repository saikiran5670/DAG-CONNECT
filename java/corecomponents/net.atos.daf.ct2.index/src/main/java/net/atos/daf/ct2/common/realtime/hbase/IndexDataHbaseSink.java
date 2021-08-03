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
import net.atos.daf.ct2.common.realtime.dataprocess.IndexDataProcess;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.hbase.connection.HbaseAdapter;
import net.atos.daf.hbase.connection.HbaseConnection;
import net.atos.daf.hbase.connection.HbaseConnectionPool;

public class IndexDataHbaseSink extends RichSinkFunction<KafkaRecord<Index>> {
	/*
	 * This class is used to write Index message data in a HBase table.
	 */

	private static final long serialVersionUID = 3842371782145886991L;
	Logger log = LoggerFactory.getLogger(IndexDataProcess.class);

	private Table table = null;
	private HbaseConnection conn = null;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		
		// this function is used to set up a connection to hbase table

		log.info("########## In Index Data HBase ##############");
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
				System.out.println("get connection from pool failed");

			}
			TableName tabName = TableName.valueOf(tableName);
			table = conn.getConnection().getTable(tabName);

			log.info("table_name  -- " + tableName);
			
			System.out.println("table_name  -- " + tableName);
			

		} catch (IOException e) {
			log.error("create connection failed from the configuration" + e.getMessage());
			throw e;
		} catch (Exception e) {

			log.error("there is an exception" + e.getMessage());
			throw e;
		}

		log.info("Index table_name -- " + tableName);
	}

	public void invoke(KafkaRecord<Index> value, Context context) throws Exception {
		//this function is used to write data into Hbase table
		Long currentTimeStamp=TimeFormatter.getInstance().getCurrentUTCTime();
		
		if(value.getValue().getEvtDateTime()!=null) {
		      currentTimeStamp = TimeFormatter.getInstance().convertUTCToEpochMilli(
		                        value.getValue().getEvtDateTime(), DafConstants.DTM_TS_FORMAT);
		}
		
		      Put put = new Put(Bytes.toBytes(value.getValue().getTransID() + "_" +
		    		  value.getValue().getDocument().getTripID() + "_" + value.getValue().getVid()
		    		  + "_" + currentTimeStamp));
		
		log.info("Index Data Row_Key :: "
				+ (value.getValue().getTransID() + "_" + value.getValue().getDocument().getTripID() + "_"
						+ value.getValue().getVid() + "_" + currentTimeStamp));
		System.out.println("Index Data Row_Key :: "
				+ (value.getValue().getTransID() + "_" + value.getValue().getDocument().getTripID() + "_"
						+ value.getValue().getVid() + "_" + currentTimeStamp));
		
		
		
		/*
		 * Put put = new Put(Bytes.toBytes(value.getValue().getTransID() + "_" +
		 * value.getValue().getDocument().getTripID() + "_" + value.getValue().getVid()
		 * + "_" + value.getValue().getReceivedTimestamp()));
		 */

		/*
		 * log.info("Index Data Row_Key :: " + (value.getValue().getTransID() + "_" +
		 * value.getValue().getDocument().getTripID() + "_" + value.getValue().getVid()
		 * + "_" + value.getValue().getReceivedTimestamp()));
		 * System.out.println("Index Data Row_Key :: " + (value.getValue().getTransID()
		 * + "_" + value.getValue().getDocument().getTripID() + "_" +
		 * value.getValue().getVid() + "_" + value.getValue().getReceivedTimestamp()));
		 */
		
		// we have commented some of the below column values as we need to use them in future

		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TransID"),
				Bytes.toBytes(String.valueOf(value.getValue().getTransID())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VID"),
				Bytes.toBytes(String.valueOf(value.getValue().getVid())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VIN"),
				Bytes.toBytes(String.valueOf(value.getValue().getVin())));

		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VEvtID"),
				Bytes.toBytes(String.valueOf(value.getValue().getVEvtID())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VDist"),
				Bytes.toBytes(String.valueOf(value.getValue().getVDist())));

		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSSpeed"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getGpsSpeed())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VFuelLevel"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVFuelLevel1())));

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
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VCumulatedFuel"),
				Bytes.toBytes(String.valueOf(value.getValue().getVCumulatedFuel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSLatitude"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsLatitude())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Jobname"),
				Bytes.toBytes(String.valueOf(value.getValue().getJobName())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("NumSeq"),
				Bytes.toBytes(String.valueOf(value.getValue().getNumSeq())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VUsedFuel"),
				Bytes.toBytes(String.valueOf(value.getValue().getVUsedFuel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("ROmodel"),
				Bytes.toBytes(String.valueOf(value.getValue().getRoModel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSDateTime"),
				Bytes.toBytes(String.valueOf(value.getValue().getGpsDateTime())));
		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TargetM2M"),
		// Bytes.toBytes(String.valueOf(value.getValue().getTargetM2M())));
		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Reserve1"),
		// Bytes.toBytes(String.valueOf(value.getValue().getReserve1())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("DriverID"),
				Bytes.toBytes(String.valueOf(value.getValue().getDriverID())));
		put.addColumn(Bytes.toBytes("f"), Bytes.toBytes("ROName"),
				Bytes.toBytes(String.valueOf(value.getValue().getRoName())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VIdleDuration"),
				Bytes.toBytes(String.valueOf(value.getValue().getVIdleDuration())));

		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("EvtDateTime"),
				Bytes.toBytes(String.valueOf(value.getValue().getEvtDateTime())));

		// put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("messageKey"),
		// Bytes.toBytes(String.valueOf(value.getValue().getMessageKey())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("receivedTimestamp"),
				Bytes.toBytes(String.valueOf(value.getValue().getReceivedTimestamp())));
		// DOC FIELDS

		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSHdop"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getGpsHdop())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("SegmentHaversineDistance"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getSegmentHaversineDistance())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("GPSSegmentDist"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getGpsSegmentDist())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Driver1CardInserted"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriver1CardInserted())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Driver1RemainingRestTime"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriver1RemainingRestTime())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Driver1RemainingDrivingTime"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriver1RemainingDrivingTime())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Driver2CardInserted"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriver2CardInserted())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VAmbientAirTemperature"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVAmbiantAirTemperature())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VFuelCumulatedLR"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVFuelCumulatedIdle())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VFuelCumulated"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVFuelCumulated())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VTachographSpeed"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVTachographSpeed())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VWheelBasedSpeed"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVWheelBasedSpeed())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Driver1WorkingState"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriver1WorkingState())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VDEFTankLevel"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVDEFTankLevel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VGrossWeightCombination"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVGrossWeightCombination())));

		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VAmbiantAirTemperature"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVAmbiantAirTemperature())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VEngineCoolantTemperature"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVEngineCoolantTemperature())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VServiceBrakeAirPressure1"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVServiceBrakeAirPressure1())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VServiceBrakeAirPressure2"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVServiceBrakeAirPressure2())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Driver2WorkingState"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriver2WorkingState())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TT_Norm"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getTt_Norm())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Driver2ID"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getDriver2ID())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VTankDiff"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVTankDiff())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("VSegmentFuelLevel1"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVSegmentFuelLevel1())));

		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("StartEltsTime"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getStartEltsTime())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("Period"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getPeriod())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TripID"),
				Bytes.toBytes(String.valueOf(value.getValue().getDocument().getTripID())));

		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TT_ListValue"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getTt_ListValue())));
	System.out.println("below ttList");
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("EngineCoolantLevel"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getEngineCoolantLevel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("EngineOilLevel"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getEngineOilLevel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("EngineOilTemperature"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getEngineOilTemperature())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("EngineOilPressure"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getEngineOilPressure())));

		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("AdBlueLevel"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getAdBlueLevel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("EngineCoolantTemperature"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getEngineCoolantTemperature())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("AirPressure"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getAirPressure())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("AmbientPressure"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getAmbientPressure())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("EngineLoad"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getEngineLoad())));

		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("EngineSpeed"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getEngineSpeed())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("FuelLevel"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getFuelLevel())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("InletAirPressureInInletManifold"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getInletAirPressureInInletManifold())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("FuelTemperature"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getFuelTemperature())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TachoVehicleSpeed"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getTachoVehicleSpeed())));
		put.addColumn(Bytes.toBytes("t"), Bytes.toBytes("TotalTachoMileage"),
				Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getTotalTachoMileage())));
		System.out.println("hbase techo values-->");

		table.put(put);
		
		System.out.println("table data added-->");
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
			log.error("Error in IndexDataHBase" + e.getMessage());
		}

	}

}
