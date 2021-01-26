package net.atos.daf.etl.ct2.common.hbase;

import java.io.IOException;
import java.text.ParseException;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.source.RichParallelSourceFunction;
import org.apache.hadoop.hbase.Cell;
import org.apache.hadoop.hbase.CellUtil;
import org.apache.hadoop.hbase.TableName;
import org.apache.hadoop.hbase.client.Result;
import org.apache.hadoop.hbase.client.ResultScanner;
import org.apache.hadoop.hbase.client.Scan;
import org.apache.hadoop.hbase.client.Table;
import org.apache.hadoop.hbase.filter.FilterList;
import org.apache.hadoop.hbase.filter.PrefixFilter;
import org.apache.hadoop.hbase.util.Bytes;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.etl.ct2.common.bo.TripStatusData;
import net.atos.daf.etl.ct2.common.util.ETLConstants;
import net.atos.daf.hbase.connection.HbaseAdapter;
import net.atos.daf.hbase.connection.HbaseConnection;
import net.atos.daf.hbase.connection.HbaseConnectionPool;

public class TripStatusCompletion extends RichParallelSourceFunction<TripStatusData> {
	private static final Logger logger = LoggerFactory.getLogger(TripStatusCompletion.class);
	
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private volatile boolean isRunning = true;

	private Table table = null;
	private Scan scan = null;
	private String tableName = null;
	private Map<String, List<String>> colFamMap = null;
	private FilterList filterLst = null;
	private List<Long> timeRangeLst = null;
	private boolean isStreaming =false;
	private long lastTripUtcTime = 0;
	private long etlMaxDuration = 0;
	private long etlMinDuration = 0;
	//private HbaseConnection conn = null;
	
	public TripStatusCompletion(String tblNm, Map<String, List<String>> colFamMap, FilterList filterList,
			List<Long> timeRangeList) {
		this.colFamMap = colFamMap;
		tableName = tblNm;
		filterLst = filterList;
		timeRangeLst = timeRangeList;
	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		super.open(parameters);
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
		isStreaming = Boolean.parseBoolean(envParams.get(ETLConstants.IS_TRIP_MINI_ETL_STREAMING));
		etlMaxDuration = Integer.parseInt(envParams.get(ETLConstants.TRIP_ETL_MAX_TIME));
		etlMinDuration = Integer.parseInt(envParams.get(ETLConstants.TRIP_ETL_MIN_TIME));
		
		HbaseAdapter hbaseAdapter=HbaseAdapter.getInstance();
		HbaseConnectionPool connectionPool = hbaseAdapter.getConnection(
				envParams.get(ETLConstants.HBASE_ZOOKEEPER_QUORUM),
				envParams.get(ETLConstants.HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT),
				envParams.get(ETLConstants.ZOOKEEPER_ZNODE_PARENT),
				envParams.get(ETLConstants.HBASE_REGIONSERVER),
				envParams.get(ETLConstants.HBASE_MASTER),
				envParams.get(ETLConstants.HBASE_REGIONSERVER_PORT), tableName);

		HbaseConnection conn = null;
		try{
			conn = connectionPool.getHbaseConnection();
			if (null == conn) {
				logger.warn("get connection from pool failed");  
				
			}
			TableName tabName = TableName.valueOf(tableName);
			table = conn.getConnection().getTable(tabName);

			System.out.println("table_name anshu2 -- " + tableName );
			
		}catch(IOException e){
	            logger.error("create connection failed from the configuration" + e.toString());
		}catch (Exception e) {
			// TODO: handle exception
            logger.error("there is an exception" + e.toString());
		}
		finally {
            if (conn != null) {
                connectionPool.releaseConnection(conn);
            }
        } 

		//TODO need to integrate with common module and close connection
//		table = HbaseUtility.getTable(HbaseUtility.getHbaseClientConnection(HbaseUtility.createConf(envParams)),
//				tableName);

		scan = new Scan();
		
		//TODO Need to cross check
		//scan.setMaxVersions(1);
		
		if (colFamMap != null)
			colFamMap.forEach((cf, colmns) -> {
				if (colmns != null) {
					for (String clmn : colmns) {
						scan.addColumn(Bytes.toBytes(cf), Bytes.toBytes(clmn));
					}
				}
			});

		if (null != filterLst)
			scan.setFilter(filterLst);
		
		//TODO Included prefix filter as all Conti data is loaded to one table
		PrefixFilter rowPrefixFilter = new PrefixFilter(
				Bytes.toBytes(ETLConstants.STATUS_MSG_TRANSID));
		scan.setFilter(rowPrefixFilter);

		if (null != timeRangeLst && 2 == timeRangeLst.size()){
			scan.setTimeRange(timeRangeLst.get(0), timeRangeLst.get(1));
			lastTripUtcTime = timeRangeLst.get(1);
			}
	}

	@Override
	public void run(SourceContext<TripStatusData> ctx) throws Exception {
		logger.info("isStreaming :: " + isStreaming);

		if(isStreaming)
		{
			logger.info("isRunning :: " + isRunning);
			long lastTripUtcTm = lastTripUtcTime;
			while(isRunning)
			{
				//Mini Streaming Batch Processing
				processTripData(ctx);
				long currentUtcTm = TimeFormatter.getCurrentUTCTime();
				//long timeDiff = TimeFormatter.subMilliSecFromUTCTime(currentUtcTm, lastTripUtcTm);
				long timeDiff = TimeFormatter.subPastUtcTmFrmCurrentUtcTm(lastTripUtcTm, currentUtcTm);
				
				//check this out here are we getting diff values*************************************************
								
				logger.info("currentUtcTm :: " + currentUtcTm);
				logger.info("timeDiff :: " + timeDiff);
				
				// test if this condition is required timeDiff < etlDuration
				if( timeDiff < etlMaxDuration){
					//Case when there is no data and mini ETL Job needs to re-executed
					
					//TODO remove ---> case when source system is down and no data available - rare case enabled for testing 
					if(timeDiff < etlMinDuration){
						logger.info(" time bfr sleep : "+TimeFormatter.getUTCStringFromEpochMilli(TimeFormatter.getCurrentUTCTime()));
						Thread.sleep(etlMinDuration);
						logger.info(" time aftr sleep : "+TimeFormatter.getUTCStringFromEpochMilli(TimeFormatter.getCurrentUTCTime()));
						currentUtcTm  = TimeFormatter.getCurrentUTCTime();
						logger.info("sleep :: currentUtcTm : "+TimeFormatter.getUTCStringFromEpochMilli(currentUtcTm) +" lastTripUtcTm :"+ TimeFormatter.getUTCStringFromEpochMilli(lastTripUtcTm) + " timeDiff : "+timeDiff + "  etlMaxDuration : "+ etlMaxDuration +" etlMinDuration :"+etlMinDuration);
						scan.setTimeRange(lastTripUtcTm, currentUtcTm);
					}else{
						logger.info("currentUtcTm : "+TimeFormatter.getUTCStringFromEpochMilli(currentUtcTm) +" lastTripUtcTm :"+ TimeFormatter.getUTCStringFromEpochMilli(lastTripUtcTm) + " timeDiff : "+timeDiff + "  etlMaxDuration : "+ etlMaxDuration +" etlMinDuration :"+etlMinDuration);
						scan.setTimeRange(lastTripUtcTm, currentUtcTm);
					}
				}else {
					currentUtcTm  = TimeFormatter.addMilliSecToUTCTime(lastTripUtcTm, etlMaxDuration);
					logger.info("else currentUtcTm : "+TimeFormatter.getUTCStringFromEpochMilli(currentUtcTm) +" lastTripUtcTm :"+ TimeFormatter.getUTCStringFromEpochMilli(lastTripUtcTm) + " timeDiff : "+timeDiff + "  etlMaxDuration : "+ etlMaxDuration);
					scan.setTimeRange(lastTripUtcTm, currentUtcTm);
				}
				lastTripUtcTm = currentUtcTm;
			}
		}else
		{
			//Normal Batch Processing
			processTripData(ctx);
		}
		
	}

	@Override
	public void cancel() {
		try {
			if (table != null) {
				table.close();
			}
			// if (conn != null) {
			// conn.close();
			// }
		} catch (IOException e) {
			logger.error("Issue while Closing HBase table :: ", e.getMessage());
			//TODO Need to throw an error
		}
	}
	
	private void processTripData(SourceContext<TripStatusData> ctx) throws IOException, ParseException
	{
		ResultScanner rs = table.getScanner(scan);
		Iterator<Result> iterator = rs.iterator();

		while (iterator.hasNext()) {
			Result result = iterator.next();
			//Result.getColumnLatest(family, qualifier)
			String rowkey = Bytes.toString(result.getRow());
			
			TripStatusData tripStsData = populateTripStsData(result);
			
			logger.info("tripStsData rowkey :: "+ rowkey );
			logger.info("tripStsData :: "+ tripStsData );
			
			if(null != tripStsData && tripStsData.getTripId() != null)
				ctx.collect(tripStsData);
		}
		
	}

	private TripStatusData populateTripStsData(Result result) throws ParseException {
		TripStatusData tripStsData = new TripStatusData();
		logger.info(" Calling populateTripStsData :: ");

		long hbaseInsertionTS = 0;
		for (Cell cell : result.listCells()) {
			try {
				String family = Bytes.toString(CellUtil.cloneFamily(cell));
				String column = Bytes.toString(CellUtil.cloneQualifier(cell));
				byte[] value = CellUtil.cloneValue(cell);

				hbaseInsertionTS = cell.getTimestamp();
				// Cell cellObj =
				// result.getColumnLatestCell(CellUtil.cloneFamily(cell),
				// CellUtil.cloneQualifier(cell));
				// byte[] value = CellUtil.cloneValue(cellObj);
				// Result.getColumnLatest(family, qualifier)

				logger.info(" family  : " + family);
				logger.info(" column  : " + column);

				if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family) && ETLConstants.TRIP_ID.equals(column)
						&& null != Bytes.toString(value) && !"null".equals(Bytes.toString(value)))
					tripStsData.setTripId(Bytes.toString(value));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family) && ETLConstants.INCREMENT.equals(column)
						&& null != Bytes.toString(value) && !"null".equals(Bytes.toString(value)))
					tripStsData.setIncrement(Bytes.toString(value));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family) && ETLConstants.VID.equals(column)
						&& null != Bytes.toString(value) && !"null".equals(Bytes.toString(value)))
					tripStsData.setVid(Bytes.toString(value));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family) && ETLConstants.GPS_TRIP_DIST.equals(column)
						&& null != Bytes.toString(value) && !"null".equals(Bytes.toString(value)))
					tripStsData.setGpsTripDist(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.GPS_STOP_VEH_DIST.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setGpsStopVehDist(Long.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.GPS_START_VEH_DIST.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setGpsStartVehDist(Long.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.VIDLE_DURATION.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setVIdleDuration(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.GPS_START_LATITUDE.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setGpsStartLatitude(Double.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.GPS_START_LONGITUDE.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setGpsStartLongitude(Double.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.GPS_END_LATITUDE.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setGpsEndLatitude(Double.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.GPS_END_LONGITUDE.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setGpsEndLongitude(Double.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family) && ETLConstants.VUSED_FUEL.equals(column)
						&& null != Bytes.toString(value) && !"null".equals(Bytes.toString(value)))
					tripStsData.setVUsedFuel(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family) && ETLConstants.VSTOP_FUEL.equals(column)
						&& null != Bytes.toString(value) && !"null".equals(Bytes.toString(value)))
					tripStsData.setVStopFuel(Long.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family) && ETLConstants.VSTART_FUEL.equals(column)
						&& null != Bytes.toString(value) && !"null".equals(Bytes.toString(value)))
					tripStsData.setVStartFuel(Long.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.VTRIP_MOTION_DURATION.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setVTripMotionDuration(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.RECEIVED_TIMESTAMP.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setReceivedTimestamp(Long.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family) && ETLConstants.VPTO_DURATION.equals(column)
						&& null != Bytes.toString(value) && !"null".equals(Bytes.toString(value)))
					tripStsData.setVPTODuration(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.VHARSH_BRAKE_DURATION.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setVHarshBrakeDuration(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.VBRAKE_DURATION.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setVBrakeDuration(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.VMAX_THROTTLE_PADDLE_DURATION.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setVMaxThrottlePaddleDuration(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.VTRIP_ACCELERATION_TIME.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setVTripAccelerationTime(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.VCRUISE_CONTROL_DIST.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setVCruiseControlDist(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.VTRIP_DPA_BRAKINGCOUNT.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setVTripDPABrakingCount(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.VTRIP_DPA_ANTICIPATION_COUNT.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setVTripDPAAnticipationCount(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.VCRUISE_CONTROL_FUEL_CONSUMED.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setVCruiseControlFuelConsumed(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.VIDLE_FUEL_CONSUMED.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setVIdleFuelConsumed(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.VSUM_TRIP_DPA_BRAKING_SCORE.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setVSumTripDPABrakingScore(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.VSUM_TRIP_DPA_ANTICIPATION_SCORE.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setVSumTripDPAAnticipationScore(Integer.valueOf(Bytes.toString(value)));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family) && ETLConstants.DRIVER_ID.equals(column)
						&& null != Bytes.toString(value) && !"null".equals(Bytes.toString(value)))
					tripStsData.setDriverId(Bytes.toString(value));

			/*	else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.EVENT_DATETIME_FIRST_INDEX.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setEventDateTimeFirstIndex(Bytes.toString(value));
				else if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& DafEtlConstants.EVT_DATETIME.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value)))
					tripStsData.setEvtDateTime(Bytes.toString(value));*/

				if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
						&& ETLConstants.EVENT_DATETIME_FIRST_INDEX.equals(column) && null != Bytes.toString(value)
						&& !"null".equals(Bytes.toString(value))) {
					//logger.info("gps date str :: "+Bytes.toString(value) + "   DafEtlConstants.DATE_FORMAT :: "+ETLConstants.DATE_FORMAT +"  triId:: "+ tripStsData.getTripId() +"  inc:: "+ tripStsData.getIncrement());
					tripStsData.setStartDateTime(
							TimeFormatter.convertUTCToEpochMilli(Bytes.toString(value), ETLConstants.DATE_FORMAT));

				} else {
					if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
							&& ETLConstants.GPS_START_DATETIME.equals(column) && null != Bytes.toString(value)
							&& !"null".equals(Bytes.toString(value))) {
						//logger.info(" evt date str :: "+Bytes.toString(value) + "   DafEtlConstants.DATE_FORMAT :: "+ETLConstants.DATE_FORMAT +"  triId:: "+ tripStsData.getTripId()+"  inc:: "+ tripStsData.getIncrement());
						tripStsData.setStartDateTime(
								TimeFormatter.convertUTCToEpochMilli(Bytes.toString(value), ETLConstants.DATE_FORMAT));
					}
				}

				if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family) && ETLConstants.EVT_DATETIME.equals(column)
						&& null != Bytes.toString(value) && !"null".equals(Bytes.toString(value))) {
					//logger.info("end date str :: "+Bytes.toString(value) + "   DafEtlConstants.DATE_FORMAT :: "+ETLConstants.DATE_FORMAT +"  triId:: "+ tripStsData.getTripId()+"  inc:: "+ tripStsData.getIncrement());
					tripStsData.setEndDateTime(
							TimeFormatter.convertUTCToEpochMilli(Bytes.toString(value), ETLConstants.DATE_FORMAT));
				} else {
					if (ETLConstants.STS_MSG_COLUMNFAMILY_T.equals(family)
							&& ETLConstants.GPS_END_DATETIME.equals(column) && null != Bytes.toString(value)) {
						//logger.info("evt end date str :: "+Bytes.toString(value) + "   DafEtlConstants.DATE_FORMAT :: "+ETLConstants.DATE_FORMAT +"  triId:: "+ tripStsData.getTripId()+"  inc:: "+ tripStsData.getIncrement());
						tripStsData.setEndDateTime(
								TimeFormatter.convertUTCToEpochMilli(Bytes.toString(value), ETLConstants.DATE_FORMAT));
					}
				}

			} catch (ParseException e) {
				logger.error("Issue while populating trip data :: " + e.getMessage());
			} catch (Exception e) {
				logger.error("Issue while populating trip data :: " + e.getMessage());
			}

		}

		try {
			if (tripStsData.getStartDateTime() != null && tripStsData.getEndDateTime() != null)
				tripStsData.setTripCalGpsVehTimeDiff(TimeFormatter
						.subPastUtcTmFrmCurrentUtcTm(tripStsData.getStartDateTime(), tripStsData.getEndDateTime()));
			
			if(tripStsData.getTripCalGpsVehTimeDiff() != null){
				double timeDiff = (tripStsData.getTripCalGpsVehTimeDiff()).doubleValue() /3600000;
				tripStsData.setTripCalVehTimeDiffInHr(timeDiff);
			}

			if (tripStsData.getGpsStopVehDist() != null && tripStsData.getGpsStartVehDist() != null)
				tripStsData
						.setTripCalGpsVehDistDiff(tripStsData.getGpsStopVehDist() - tripStsData.getGpsStartVehDist());

			tripStsData.setHbaseInsertionTS(hbaseInsertionTS);
			tripStsData.setEtlProcessingTS(TimeFormatter.getCurrentUTCTime());
		} catch (Exception e) {
			logger.error("Issue while populating trip data :: " + e.getMessage());
			// TODO need not throw an error to abort the process
		}

		logger.info("Final getGpsTripDist ::  " + tripStsData.getGpsTripDist());

		return tripStsData;

	}
}
