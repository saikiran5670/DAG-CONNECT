package net.atos.daf.ct2.etl.common.hbase;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import org.apache.flink.api.common.functions.RichFlatMapFunction;
import org.apache.flink.api.java.tuple.Tuple11;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.util.Collector;
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
import net.atos.daf.ct2.etl.common.bo.TripStatusData;
import net.atos.daf.ct2.etl.common.util.ETLConstants;
import net.atos.daf.hbase.connection.HbaseAdapter;
import net.atos.daf.hbase.connection.HbaseConnection;
import net.atos.daf.hbase.connection.HbaseConnectionPool;

public class TripIndexData
		extends RichFlatMapFunction<TripStatusData, Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String>> {
	private static final Logger logger = LoggerFactory.getLogger(TripIndexData.class);

	private static final long serialVersionUID = 1L;

	private Table table = null;
	private Scan scan = null;
	private String tableName = null;
	private FilterList filterLst = null;
	private List<Long> timeRangeLst = null;
	private Map<String, List<String>> colFamMap = null;
	private HbaseConnection conn = null;
	private Integer vGrossWtThreshold = 0;

	public TripIndexData(String tblNm, Map<String, List<String>> colFamMap, FilterList filterList) {
		this.colFamMap = colFamMap;
		tableName = tblNm;
		filterLst = filterList;
	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		super.open(parameters);
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		HbaseAdapter hbaseAdapter = HbaseAdapter.getInstance();
		HbaseConnectionPool connectionPool = hbaseAdapter.getConnection(
				envParams.get(ETLConstants.HBASE_ZOOKEEPER_QUORUM),
				envParams.get(ETLConstants.HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT),
				envParams.get(ETLConstants.ZOOKEEPER_ZNODE_PARENT), envParams.get(ETLConstants.HBASE_REGIONSERVER),
				envParams.get(ETLConstants.HBASE_MASTER), envParams.get(ETLConstants.HBASE_REGIONSERVER_PORT),
				tableName);
		
		if(envParams.get(ETLConstants.VEHICLE_GROSS_WEIGHT_THRESHOLD) != null)
			vGrossWtThreshold = Integer.valueOf(envParams.get(ETLConstants.VEHICLE_GROSS_WEIGHT_THRESHOLD));

		try {
			conn = connectionPool.getHbaseConnection();
			if (null == conn) {
				logger.warn("get connection from pool failed");

			}
			TableName tabName = TableName.valueOf(tableName);
			table = conn.getConnection().getTable(tabName);

			//logger.info("tableName " + tableName );

		} catch (IOException e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue, Failed to get HBase connection in trip streaming job :: " + e);
			throw e;
		} catch (Exception e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing HBase connection in Trip streaming Job :: " + e);
			throw e;
		}

		logger.info("Index tableName :: " + tableName);

		/*scan = new Scan();
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

		if (null != timeRangeLst && 2 == timeRangeLst.size())
			scan.setTimeRange(timeRangeLst.get(0), timeRangeLst.get(1));*/
	}

	@Override
	public void flatMap(TripStatusData stsData,
			Collector<Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String>> out) throws Exception {

		try {
			
			scan = new Scan();
			if (colFamMap != null)
				colFamMap.forEach((cf, colmns) -> {
					if (colmns != null) {
						for (String clmn : colmns) {
							scan.addColumn(Bytes.toBytes(cf), Bytes.toBytes(clmn));
						}
					}
				});
			
			PrefixFilter rowPrefixFilter = new PrefixFilter(
					Bytes.toBytes(ETLConstants.INDEX_MSG_TRANSID + "_" + stsData.getTripId()));
			scan.setFilter(rowPrefixFilter);

			logger.info("Index filter :: " + (ETLConstants.INDEX_MSG_TRANSID + "_" + stsData.getTripId()));
			logger.info("table is " + table);
			
			ResultScanner rs = table.getScanner(scan);
			logger.info("ResultScanner is " + rs +" for trip ::"+stsData.getTripId());
			Iterator<Result> iterator = rs.iterator();
			logger.info("iterator is " + iterator+" for trip ::"+stsData.getTripId());

			List<Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long,Integer, String>> indexDataList = new ArrayList<>();
			while (iterator.hasNext()) {

				Result result = iterator.next();
				
				String driver2Id = null;
				String driverId = null;
				String tripId = null;
				String vid = null;
				Integer vTachographSpeed = ETLConstants.ZERO;
				Integer vGrossWeightCombination = ETLConstants.ZERO;
				String jobNm = "";
				Long increment = ETLConstants.ZERO_VAL;
				Long evtDateTime = ETLConstants.ZERO_VAL;
				Long vDist = ETLConstants.ZERO_VAL;
				Integer grossWtRec = ETLConstants.ONE;
				logger.info("inside while loop key :: "+Bytes.toString(result.getRow()));
				
				for (Cell cell : result.listCells()) {
					try {
						String family = Bytes.toString(CellUtil.cloneFamily(cell));
						String column = Bytes.toString(CellUtil.cloneQualifier(cell));
						byte[] value = CellUtil.cloneValue(cell);
						
						if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_TRIP_ID.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							tripId = Bytes.toString(value);
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_VID.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							vid = Bytes.toString(value);
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_V_TACHOGRAPH_SPEED.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							vTachographSpeed = Integer.valueOf(Bytes.toString(value));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_V_GROSSWEIGHT_COMBINATION.equals(column)
								&& null != Bytes.toString(value) && !"null".equals(Bytes.toString(value)))
							vGrossWeightCombination = Integer.valueOf(Bytes.toString(value));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_DRIVER2_ID.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							driver2Id = Bytes.toString(value);
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_DRIVER_ID.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							driverId = Bytes.toString(value);
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_JOBNAME.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							jobNm = Bytes.toString(value);
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_INCREMENT.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							increment = Long.valueOf(Bytes.toString(value));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_VDIST.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							vDist = Long.valueOf(Bytes.toString(value));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_EVT_DATETIME.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value))){
							evtDateTime = TimeFormatter.getInstance().convertUTCToEpochMilli(Bytes.toString(value), ETLConstants.DATE_FORMAT);
						}
						
					} catch (Exception e) {
						logger.error("Issue while reading Index message data for prefix filter :: " +rowPrefixFilter + " exception is :: "+ e);
					}
				}

				logger.info(" Index tripId  :: " + tripId);
				logger.info("increment: "+increment + " vGrossWeightCombination: "+vGrossWeightCombination + " vGrossWtThreshold : "+vGrossWtThreshold);
				
				/*if(vGrossWeightCombination < vGrossWtThreshold){
					Tuple9<String, String, String, Integer, Integer, String, Long, Long, Long> tuple9 = new Tuple9<>();
					tuple9.setFields(tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination, jobNm, evtDateTime, vDist, increment);

					logger.info("increment: "+increment + " vGrossWeightCombination : "+vGrossWeightCombination);
					indexDataList.add(tuple9);
				}else{
					logger.info("Ignored index record increment: "+increment + " vGrossWeightCombination : "+vGrossWeightCombination);
				}*/
				
				if (vGrossWtThreshold < vGrossWeightCombination) {
					logger.info("Ignored index record increment: "+increment + " vGrossWeightCombination : "+vGrossWeightCombination);
					vGrossWeightCombination = ETLConstants.ZERO;
					grossWtRec = ETLConstants.ZERO;
				}
				
				Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String> tuple11 = new Tuple11<>();
				tuple11.setFields(tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination, jobNm, evtDateTime,
						vDist, increment, grossWtRec, driverId);

				indexDataList.add(tuple11);
			}
			
			Collections.sort(indexDataList,
					new Comparator<Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String>>() {
						@Override
						public int compare(
								Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String> tuple1,
								Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String> tuple2) {
							return Long.compare(tuple1.f6, tuple2.f6);
						}
					});
			
			for(Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String> tuple11 : indexDataList){
				logger.info("lookup data for trip :: "+tuple11);
				out.collect(tuple11);
			}
		} catch (Exception e) {
			logger.error("Issue while fetching hbase lookup data "+stsData  );
			logger.error("Issue while fetching hbase lookup data "+e.getMessage() );
			e.printStackTrace();
		}

	}

	@Override
	public void close() {
		try {
			if (table != null) {
				table.close();
			}
			if (conn != null) {
				conn.releaseConnection();
			}
		} catch (IOException e) {
			logger.error("Issue while Closing HBase table :: ", e);
			// TODO Need to throw an error
		}
	}

}
