package net.atos.daf.ct2.etl.common.processing;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.etl.common.bo.TripAggregatedData;
import net.atos.daf.ct2.etl.common.hbase.HbaseLookupDataSource;
import net.atos.daf.ct2.etl.common.postgre.IndexDataSourceLookup;
import net.atos.daf.ct2.etl.common.util.ETLConstants;

public class TripAggregationProcessor implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(TripAggregationProcessor.class);

		
	private Map<String, List<String>> getTripIndexColumns() {
		Map<String, List<String>> tripIndxClmns = new HashMap<>();
		List<String> indxClmns = new ArrayList<>();

		indxClmns.add(ETLConstants.INDEX_MSG_TRIP_ID);
		indxClmns.add(ETLConstants.INDEX_MSG_VID);
		indxClmns.add(ETLConstants.INDEX_MSG_V_TACHOGRAPH_SPEED);
		indxClmns.add(ETLConstants.INDEX_MSG_V_GROSSWEIGHT_COMBINATION);
		indxClmns.add(ETLConstants.INDEX_MSG_DRIVER2_ID);
		indxClmns.add(ETLConstants.INDEX_MSG_DRIVER_ID);
		indxClmns.add(ETLConstants.INDEX_MSG_JOBNAME);
		indxClmns.add(ETLConstants.INDEX_MSG_INCREMENT);
		indxClmns.add(ETLConstants.INDEX_MSG_VDIST);
		indxClmns.add(ETLConstants.INDEX_MSG_EVT_DATETIME);
		indxClmns.add(ETLConstants.INDEX_MSG_VEVT_ID);
		tripIndxClmns.put(ETLConstants.INDEX_MSG_COLUMNFAMILY_T, indxClmns);

		return tripIndxClmns;
	}
		
	/*
	Logic to address current issues
	*/
	
	public SingleOutputStreamOperator<TripAggregatedData> getHbaseLookUpData(SingleOutputStreamOperator<TripAggregatedData>  hbaseStsData, ParameterTool envParams)
	{
		Map<String, List<String>> tripIndxClmns = getTripIndexColumns();
		SingleOutputStreamOperator<TripAggregatedData> indxData = hbaseStsData
				//.keyBy(value -> value.getTripId())
				.flatMap(new HbaseLookupDataSource(envParams.get(ETLConstants.INDEX_TABLE_NM), tripIndxClmns, null));

		return indxData;
	}
	
	public SingleOutputStreamOperator<TripAggregatedData> getTripGranularData(SingleOutputStreamOperator<TripAggregatedData> tripStatusData)throws Exception
	{
		return tripStatusData
				//.keyBy(value -> value.getTripId())
				.flatMap(new IndexDataSourceLookup());
	}
	
	/*public DataStream<Trip> getTripStatisticData(SingleOutputStreamOperator<TripAggregatedData> tripAggrData, StreamTableEnvironment tableEnv)
	{
		tableEnv.createTemporaryView("tripAggrData", tripAggrData);
		Table tripStatisticData =tableEnv.sqlQuery(ETLQueries.TRIP_QRY);
		
		return tableEnv.toRetractStream(tripStatisticData, Trip.class).map(rec -> rec.f1);
	}
	
	public DataStream<EcoScore> getEcoScoreData(SingleOutputStreamOperator<TripAggregatedData> tripAggrData, StreamTableEnvironment tableEnv)
	{
		tableEnv.createTemporaryView("tripAggrDataForEcoScore", tripAggrData);
		Table tripStatisticData =tableEnv.sqlQuery(ETLQueries.ECOSCORE_QRY);
		
		return tableEnv.toRetractStream(tripStatisticData, EcoScore.class).map(rec -> rec.f1);
	}*/
	
}
