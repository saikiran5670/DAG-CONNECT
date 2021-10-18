package net.atos.daf.ct2.etl.common.hbase;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import org.apache.flink.api.common.functions.RichFlatMapFunction;
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
import net.atos.daf.ct2.etl.common.bo.TripAggregatedData;
import net.atos.daf.ct2.etl.common.util.ETLConstants;
import net.atos.daf.hbase.connection.HbaseAdapter;
import net.atos.daf.hbase.connection.HbaseConnection;
import net.atos.daf.hbase.connection.HbaseConnectionPool;
import net.atos.daf.postgre.bo.IndexTripData;

public class HbaseLookupDataSource extends
		RichFlatMapFunction<TripAggregatedData, TripAggregatedData> {
	private static final Logger logger = LoggerFactory.getLogger(TripIndexData.class);

	private static final long serialVersionUID = 1L;

	private Table table = null;
	private Scan scan = null;
	private String tableName = null;
	private FilterList filterLst = null;
	private List<Long> timeRangeLst = null;
	private Map<String, List<String>> colFamMap = null;
	private HbaseConnection conn = null;
	private Long vGrossWtThreshold = 0L;

	public HbaseLookupDataSource(String tblNm, Map<String, List<String>> colFamMap, FilterList filterList) {
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

		if (envParams.get(ETLConstants.VEHICLE_GROSS_WEIGHT_THRESHOLD) != null)
			vGrossWtThreshold = Long.valueOf(envParams.get(ETLConstants.VEHICLE_GROSS_WEIGHT_THRESHOLD));

		try {
			conn = connectionPool.getHbaseConnection();
			if (null == conn) {
				logger.warn("get connection from pool failed");

			}
			TableName tabName = TableName.valueOf(tableName);
			table = conn.getConnection().getTable(tabName);

			// logger.info("tableName " + tableName );

		} catch (IOException e) {
			logger.error("Issue, Failed to get HBase connection in trip streaming job :: " + e);
			throw e;
		} catch (Exception e) {
			logger.error("Issue while establishing HBase connection in Trip streaming Job :: " + e);
			throw e;
		}

		logger.info("Index tableName :: " + tableName);

		/*
		 * scan = new Scan(); if (colFamMap != null) colFamMap.forEach((cf,
		 * colmns) -> { if (colmns != null) { for (String clmn : colmns) {
		 * scan.addColumn(Bytes.toBytes(cf), Bytes.toBytes(clmn)); } } });
		 * 
		 * if (null != filterLst) scan.setFilter(filterLst);
		 * 
		 * if (null != timeRangeLst && 2 == timeRangeLst.size())
		 * scan.setTimeRange(timeRangeLst.get(0), timeRangeLst.get(1));
		 */
	}

	@Override
	public void flatMap(TripAggregatedData stsData,
			Collector<TripAggregatedData> out)
			throws Exception {

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
			
			ResultScanner rs = table.getScanner(scan);
			Iterator<Result> iterator = rs.iterator();
			
			List<IndexTripData> tripIdxLst = new ArrayList<>();
			while (iterator.hasNext()) {

				Result result = iterator.next();
				IndexTripData indxData = new IndexTripData();

				for (Cell cell : result.listCells()) {
					try {
						String family = Bytes.toString(CellUtil.cloneFamily(cell));
						String column = Bytes.toString(CellUtil.cloneQualifier(cell));
						byte[] value = CellUtil.cloneValue(cell);

						if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_TRIP_ID.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							indxData.setTripId(Bytes.toString(value));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_VID.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							indxData.setVid(Bytes.toString(value));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_VEVT_ID.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							indxData.setVEvtId(Integer.valueOf(Bytes.toString(value)));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_V_TACHOGRAPH_SPEED.equals(column)
								&& null != Bytes.toString(value) && !"null".equals(Bytes.toString(value)))
							indxData.setVTachographSpeed(Integer.valueOf(Bytes.toString(value)));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_V_GROSSWEIGHT_COMBINATION.equals(column)
								&& null != Bytes.toString(value) && !"null".equals(Bytes.toString(value)))
							indxData.setVGrossWeightCombination(Long.valueOf(Bytes.toString(value)));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_DRIVER2_ID.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							indxData.setDriver2Id(Bytes.toString(value));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_DRIVER_ID.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							indxData.setDriverId(Bytes.toString(value));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_JOBNAME.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							indxData.setJobName(Bytes.toString(value));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_INCREMENT.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							indxData.setIncrement(Long.valueOf(Bytes.toString(value)));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_VDIST.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value)))
							indxData.setVDist(Long.valueOf(Bytes.toString(value)));
						else if (ETLConstants.INDEX_MSG_COLUMNFAMILY_T.equals(family)
								&& ETLConstants.INDEX_MSG_EVT_DATETIME.equals(column) && null != Bytes.toString(value)
								&& !"null".equals(Bytes.toString(value))) {
							indxData.setEvtDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(Bytes.toString(value),
									ETLConstants.DATE_FORMAT));
						}

					} catch (Exception e) {
						logger.error("Issue while reading Index message data for prefix filter :: " + rowPrefixFilter
								+ " exception is :: " + e);
					}
				}

				tripIdxLst.add(indxData);
			}

			Stream<IndexTripData>  streamData = tripIdxLst.stream().distinct().sorted(new Comparator<IndexTripData>() {
				@Override
				public int compare(
						IndexTripData indxData1,
						IndexTripData indxData2) {
					return Long.compare(indxData1.getEvtDateTime(), indxData2.getEvtDateTime());
				}
			});
			
			List<IndexTripData> indxLst = streamData.collect(Collectors.toList());

			long vDistDiff = 0;
			long prevVDist = 0;
			
			String driver2Id = null;
			String driverId = null;
			Long grossWtRec = ETLConstants.ZERO_VAL;
			Integer maxSpeed =0;
			Double vGrossWtSum = 0.0;
			Double calAvgGrossWtSum = 0.0;
			Long calVDistDiff =0L;
			
			for (IndexTripData indxData : indxLst) {
				
				if(4 != indxData.getVEvtId()){
					vDistDiff =indxData.getVDist() - prevVDist ;
					
					/*if(vDistDiff == 0)
						vDistDiff = 1;*/
				}/*else{
					prevVDist = indxData.getVDist();
				}*/
			
				prevVDist = indxData.getVDist();
				
				if (vGrossWtThreshold.compareTo(indxData.getVGrossWeightCombination()) < 0) {
					indxData.setVGrossWeightCombination(0L);
				}else
					grossWtRec = grossWtRec +1;
				
				if(Objects.nonNull(driver2Id) && Objects.nonNull(indxData.getDriver2Id()))
					driver2Id= indxData.getDriver2Id();
			
				if(Objects.nonNull(driverId) && Objects.nonNull(indxData.getDriverId()))
					driverId= indxData.getDriverId();
				
				if(indxData.getVTachographSpeed() > maxSpeed)
					maxSpeed = indxData.getVTachographSpeed();
				vGrossWtSum = vGrossWtSum + indxData.getVGrossWeightCombination();
				calAvgGrossWtSum  = calAvgGrossWtSum + Double.valueOf(indxData.getVGrossWeightCombination()) * vDistDiff;
				
				calVDistDiff = calVDistDiff + vDistDiff;
				logger.info("New approach Final grossObj indxData:{} vGrossWtSum:{} vDistDiff:{} calAvgGrossWtSum :{}",indxData , vGrossWtSum, vDistDiff, calAvgGrossWtSum);
			}
			
			stsData.setDriver2Id(driver2Id);
			if(!Objects.nonNull(stsData.getDriverId()))
					stsData.setDriverId(driverId);
			if(0 != grossWtRec)
				stsData.setVGrossWeightCombination(vGrossWtSum/grossWtRec);
			else
				stsData.setVGrossWeightCombination(0.0);
			
			stsData.setVTachographSpeed(Double.valueOf(maxSpeed));
			
			if(0 != stsData.getTripCalDist())
				stsData.setTripCalAvgGrossWtComb(calAvgGrossWtSum/stsData.getTripCalDist());
			else
				stsData.setTripCalAvgGrossWtComb(0.0);
			
			
			stsData.setVGrossWtSum(vGrossWtSum);
			stsData.setNumberOfIndexMessage(grossWtRec);
			stsData.setVGrossWtCmbCount(calVDistDiff);
			stsData.setTripProcessingTS(TimeFormatter.getInstance().getCurrentUTCTime());
			
			logger.info("Final trip statistics {} ",stsData);
			out.collect(stsData);
			
		} catch (Exception e) {
			logger.error("Issue while fetching hbase lookup data " + stsData);
			logger.error("Issue while fetching hbase lookup data " + e.getMessage());
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
