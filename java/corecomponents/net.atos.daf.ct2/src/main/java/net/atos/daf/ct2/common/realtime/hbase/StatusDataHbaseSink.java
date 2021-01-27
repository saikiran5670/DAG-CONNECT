package net.atos.daf.ct2.common.realtime.hbase;

import java.io.IOException;
import java.util.Arrays;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.hbase.TableName;
import org.apache.hadoop.hbase.client.Put;
import org.apache.hadoop.hbase.client.Table;
import org.apache.hadoop.hbase.util.Bytes;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.common.realtime.dataprocess.IndexDataProcess;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.hbase.connection.HbaseAdapter;
import net.atos.daf.hbase.connection.HbaseConnection;
import net.atos.daf.hbase.connection.HbaseConnectionPool;

public class StatusDataHbaseSink extends RichSinkFunction<KafkaRecord<Status>> {

	/**
	 * 
	 */
	private static final long serialVersionUID = 3842371782145886991L;
	Logger log = LoggerFactory.getLogger(IndexDataProcess.class); 
	
	private Configuration conf = null;
	private String tableName = null;
	private Table table = null;
	private HbaseConnection conn = null;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		super.open(parameters);
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
		
HbaseAdapter hbaseAdapter = HbaseAdapter.getInstance();
		
		String tableName = envParams.get(DafConstants.HBASE_TABLE_NAME);
		
		HbaseConnectionPool connectionPool = hbaseAdapter.getConnection(
		  envParams.get(DafConstants.HBASE_ZOOKEEPER_QUORUM),
		  envParams.get(DafConstants.HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT),
		  envParams.get(DafConstants.ZOOKEEPER_ZNODE_PARENT),
		  envParams.get(DafConstants.HBASE_REGIONSERVER),
		  envParams.get(DafConstants.HBASE_MASTER),
		  envParams.get(DafConstants.HBASE_REGIONSERVER_PORT), tableName);
		
		
		//HbaseConnection conn = null;
		try{
			conn = connectionPool.getHbaseConnection();
			if (null == conn) {
				log.warn("get connection from pool failed");  
				
			}
			TableName tabName = TableName.valueOf(tableName);
			table = conn.getConnection().getTable(tabName);

			System.out.println("table_name anshu2 -- " + tableName );
			
		}catch(IOException e){
	            log.error("create connection failed from the configuration" + e.getMessage());
	            throw e;
		}catch (Exception e) {
			// TODO: handle exception
            log.error("there is an exception" + e.getMessage());
            throw e;
		}
//		finally {
//            if (conn != null) {
//                connectionPool.releaseConnection(conn);
//            }
//        } 
//		table = HBaseConfigUtil.getTable(
//				HBaseConfigUtil.getHbaseClientConnection(HBaseConfigUtil.createConf(envParams)),
//				envParams.get(DafConstants.HBASE_TABLE_NAME));

		System.out.println("Status table name - " + tableName);
	}

	@SuppressWarnings("rawtypes")
	public void invoke(KafkaRecord<Status> value, Context context) throws Exception {

		//Put put = new Put(Bytes.toBytes(value.getValue().getVid()+"_"+value.getValue().getTransID()+"_"+value.getValue().getDocument().getTripID()+"_"+ value.getValue().getReceivedTimestamp()));
		Put put = new Put(Bytes.toBytes(value.getValue().getTransID()+"_"+value.getValue().getDocument().getTripID()+"_"+value.getValue().getVid()+"_"+ value.getValue().getReceivedTimestamp()));
        
        //COLUMN FAMILY DATA
          //Common Fields
          System.out.println("Common Data Fields in Status Data");
          put.addColumn(Bytes.toBytes("f"), Bytes.toBytes (" DocFormat ") , Bytes.toBytes(String.valueOf(value.getValue(). getDocFormat ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" DocVersion ") , Bytes.toBytes(String.valueOf(value.getValue(). getDocVersion ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" DriverID ") , Bytes.toBytes(String.valueOf(value.getValue(). getDriverID ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" EventDateTimeFirstIndex ") , Bytes.toBytes(String.valueOf(value.getValue(). getEventDateTimeFirstIndex ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" EvtDateTime ") , Bytes.toBytes(String.valueOf(value.getValue(). getEvtDateTime ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" GPSEndDateTime ") , Bytes.toBytes(String.valueOf(value.getValue(). getGpsEndDateTime ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" GPSEndLatitude ") , Bytes.toBytes(String.valueOf(value.getValue(). getGpsEndLatitude ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" GPSEndLongitude ") , Bytes.toBytes(String.valueOf(value.getValue(). getGpsEndLongitude ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" GPSStartDateTime ") , Bytes.toBytes(String.valueOf(value.getValue(). getGpsStartDateTime ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" GPSStartLatitude ") , Bytes.toBytes(String.valueOf(value.getValue(). getGpsStartLatitude ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" GPSStartLongitude ") , Bytes.toBytes(String.valueOf(value.getValue(). getGpsStartLongitude ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" GPSStartVehDist ") , Bytes.toBytes(String.valueOf(value.getValue(). getGpsStartVehDist ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" GPSStopVehDist ") , Bytes.toBytes(String.valueOf(value.getValue(). getGpsStopVehDist ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Increment ") , Bytes.toBytes(String.valueOf(value.getValue(). getIncrement ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Jobname ") , Bytes.toBytes(String.valueOf(value.getValue(). getJobName ())));
          //put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" messageKey ") , Bytes.toBytes(String.valueOf(value.getValue(). getMessageKey ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" NumberOfIndexMessage ") , Bytes.toBytes(String.valueOf(value.getValue(). getNumberOfIndexMessage ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" NumSeq ") , Bytes.toBytes(String.valueOf(value.getValue(). getNumSeq ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" receivedTimestamp ") , Bytes.toBytes(String.valueOf(value.getValue(). getReceivedTimestamp ())));
          //put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Reserve0 ") , Bytes.toBytes(String.valueOf(value.getValue(). getReserve0 ())));
         // put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Reserve1 ") , Bytes.toBytes(String.valueOf(value.getValue(). getReserve1 ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" ROmodel ") , Bytes.toBytes(String.valueOf(value.getValue(). getRoModel ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" ROName ") , Bytes.toBytes(String.valueOf(value.getValue(). getRoName ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" ROProfil ") , Bytes.toBytes(String.valueOf(value.getValue(). getRoProfil ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" ROrelease ") , Bytes.toBytes(String.valueOf(value.getValue(). getRoRelease ())));
      //    put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" TargetM2M ") , Bytes.toBytes(String.valueOf(value.getValue(). getTargetM2M ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" TenantID ") , Bytes.toBytes(String.valueOf(value.getValue(). getTenantID ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" TransID ") , Bytes.toBytes(String.valueOf(value.getValue(). getTransID ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VBrakeDuration ") , Bytes.toBytes(String.valueOf(value.getValue(). getVBrakeDuration ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VCruiseControlDist ") , Bytes.toBytes(String.valueOf(value.getValue(). getVCruiseControlDist ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VEvtID ") , Bytes.toBytes(String.valueOf(value.getValue(). getVEvtID ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VHarshBrakeDuration ") , Bytes.toBytes(String.valueOf(value.getValue(). getVHarshBrakeDuration ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VID ") , Bytes.toBytes(String.valueOf(value.getValue(). getVid ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VIdleDuration ") , Bytes.toBytes(String.valueOf(value.getValue(). getVIdleDuration ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VNegAltitudeVariation ") , Bytes.toBytes(String.valueOf(value.getValue(). getVNegAltitudeVariation ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VPosAltitudeVariation ") , Bytes.toBytes(String.valueOf(value.getValue(). getVPosAltitudeVariation ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VPTOCnt ") , Bytes.toBytes(String.valueOf(value.getValue(). getVptoCnt ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VPTODist ") , Bytes.toBytes(String.valueOf(value.getValue(). getVptoDist ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VPTODuration ") , Bytes.toBytes(String.valueOf(value.getValue(). getVptoDuration ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VStartFuel ") , Bytes.toBytes(String.valueOf(value.getValue(). getVStartFuel ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VStartTankLevel ") , Bytes.toBytes(String.valueOf(value.getValue(). getVStartTankLevel ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VStopFuel ") , Bytes.toBytes(String.valueOf(value.getValue(). getVStopFuel ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VStopTankLevel ") , Bytes.toBytes(String.valueOf(value.getValue(). getVStopTankLevel ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VUsedFuel ") , Bytes.toBytes(String.valueOf(value.getValue(). getVUsedFuel ())));
         
         // doc 
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VIdleFuelConsumed ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVIdleFuelConsumed())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" TripHaversineDistance ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument().getTripHaversineDistance())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VTripAccelerationTime ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument(). getVTripAccelerationTime ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VMaxThrottlePaddleDuration ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument(). getVMaxThrottlePaddleDuration ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VSumTripDPABrakingScore ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument(). getVSumTripDPABrakingScore ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VSumTripDPAAnticipationScore ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument(). getVSumTripDPAAnticipationScore ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VTripDPABrakingCount ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument(). getVTripDPABrakingCount ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VTripDPAAnticipationCount ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument(). getVTripDPAAnticipationCount ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VCruiseControlFuelConsumed ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument(). getVCruiseControlFuelConsumed ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" AxeVariantId ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument(). getAxeVariantId ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VTripIdleFuelConsumed ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument(). getVTripIdleFuelConsumed ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" GPSTripDist ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument(). getGpsTripDist ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" TripID ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument(). getTripID ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VEvtCause ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument(). getVEvtCause ())));
          
          //VCruiseControlDistanceDistr
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VCruiseDistrMinRangeInt ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVCruiseControlDistanceDistr(). getDistrMinRangeInt ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VCruiseDistrMaxRangeInt ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVCruiseControlDistanceDistr(). getDistrMaxRangeInt ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VCruiseDistrStep ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVCruiseControlDistanceDistr(). getDistrStep ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VCruiseDistrArrayInt ") , Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVCruiseControlDistanceDistr(). getDistrArrayInt ())));
          
        //getStatusVRpmTorque
          System.out.println("getStatusVRpmTorque Data Fields in Status Data");
          
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Vrpm_abs ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVRpmTorque(). getAbs ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Vrpm_ord ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVRpmTorque(). getOrd ())));
         
          //arrays
           put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Vrpm_A ") , Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVRpmTorque(). getA ())));
           put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Vrpm_IA ") , Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVRpmTorque(). getIa ())));
           put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Vrpm_JA ") , Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVRpmTorque(). getJa ())));
         //arrays
          System.out.println("getStatusVSpeedRpm Data Fields in Status Data");
          
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Vspeed_abs ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVSpeedRpm(). getAbs ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Vspeed_ord ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVSpeedRpm(). getOrd ())));
          
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Vspeed_A ") , Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVSpeedRpm(). getA ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Vspeed_IA ") , Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVSpeedRpm(). getIa ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" Vspeed_JA ") , Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVSpeedRpm(). getJa ())));
          
          
          //VAccelerationSpeed 
          System.out.println("VAccelerationPedalDistr Data Fields in Status Data");
          
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VAcceleration_abs ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVAccelerationSpeed(). getAbs ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VAcceleration_ord ") , Bytes.toBytes(String.valueOf(value.getValue().getDocument().getVAccelerationSpeed(). getOrd ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VAcceleration_A ") , Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVAccelerationSpeed(). getA ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VAcceleration_A_VBrake ") , Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVAccelerationSpeed(). getA_VBrake ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VAcceleration_IA ") , Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVAccelerationSpeed(). getIa ())));
          put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VAcceleration_JA ") , Bytes.toBytes(Arrays.toString(value.getValue().getDocument().getVAccelerationSpeed(). getJa ())));
         
        
         //put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" M2M_in ") , Bytes.toBytes(String.valueOf(value.getValue().getTimestamps().getM2M_in())));
        //  put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" M2M_out ") , Bytes.toBytes(String.valueOf(value.getValue().getTimestamps().getM2M_out())));
          
          
          //VIdleDurationDistr 
          System.out.println("VIdleDurationDistr Data Fields in Status Data");

            put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VIdleDistrStep ") , Bytes.toBytes(String.valueOf(value.getValue().getVIdleDurationDistr(). getDistrStep ())));
            put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VIdleDistrMaxRangeInt ") , Bytes.toBytes(String.valueOf(value.getValue().getVIdleDurationDistr(). getDistrMaxRangeInt ())));
            put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VIdleDistrMinRangeInt ") , Bytes.toBytes(String.valueOf(value.getValue().getVIdleDurationDistr(). getDistrMinRangeInt ())));
            put.addColumn(Bytes.toBytes("t"), Bytes.toBytes (" VIdleDistrArrayInt ") , Bytes.toBytes(Arrays.toString(value.getValue().getVIdleDurationDistr(). getDistrArrayInt ())));
            System.out.println("VIdleDurationDistr Data Fields in Status Data" +String.valueOf(value.getValue().getVIdleDurationDistr(). getDistrArrayInt ()));
            
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
