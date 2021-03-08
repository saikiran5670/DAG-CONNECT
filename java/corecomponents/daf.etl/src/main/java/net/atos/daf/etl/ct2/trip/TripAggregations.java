package net.atos.daf.etl.ct2.trip;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.tuple.Tuple5;
import org.apache.flink.api.java.tuple.Tuple7;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.core.fs.FileSystem;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.table.api.Table;
import org.apache.flink.table.api.bridge.java.StreamTableEnvironment;
import org.apache.flink.types.Row;

import net.atos.daf.etl.ct2.common.bo.Trip;
import net.atos.daf.etl.ct2.common.bo.TripStatusData;
import net.atos.daf.etl.ct2.common.hbase.TripIndexData;
import net.atos.daf.etl.ct2.common.util.ETLConstants;
import net.atos.daf.etl.ct2.common.util.ETLQueries;

public class TripAggregations implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	
	public static SingleOutputStreamOperator<Tuple7<String, String, String, Integer, Integer, String, Long>> getTripIndexData(SingleOutputStreamOperator<TripStatusData> hbaseStsData, StreamTableEnvironment tableEnv , ParameterTool envParams)
	{
		Map<String, List<String>> tripIndxClmns = getTripIndexColumns();
		SingleOutputStreamOperator<Tuple7<String, String, String, Integer, Integer, String, Long>> indxData = hbaseStsData
				.rebalance().keyBy(value -> value.getTripId())
				.flatMap(new TripIndexData(envParams.get(ETLConstants.INDEX_TABLE_NM), tripIndxClmns, null));

		if ("true".equals(envParams.get(ETLConstants.WRITE_OUTPUT)))
			indxData.writeAsText(envParams.get(ETLConstants.WRITE_OUTPUT)+"indexData.txt", FileSystem.WriteMode.OVERWRITE)
			.name("writeIndexDataToFile");

		return indxData;
	}
	
	private static Map<String, List<String>> getTripIndexColumns() {
		Map<String, List<String>> tripIndxClmns = new HashMap<>();
		List<String> indxClmns = new ArrayList<>();

		indxClmns.add(ETLConstants.INDEX_MSG_TRIP_ID);
		indxClmns.add(ETLConstants.INDEX_MSG_VID);
		indxClmns.add(ETLConstants.INDEX_MSG_V_TACHOGRAPH_SPEED);
		indxClmns.add(ETLConstants.INDEX_MSG_V_GROSSWEIGHT_COMBINATION);
		indxClmns.add(ETLConstants.INDEX_MSG_DRIVER2_ID);
		indxClmns.add(ETLConstants.INDEX_MSG_JOBNAME);
		indxClmns.add(ETLConstants.INDEX_MSG_INCREMENT);
		tripIndxClmns.put(ETLConstants.INDEX_MSG_COLUMNFAMILY_T, indxClmns);

		return tripIndxClmns;
	}
	
	//TODO make this method private after testing
	public static DataStream<Tuple5<String, String, String, Integer, Double>> getTripIndexAggregatedData(SingleOutputStreamOperator<TripStatusData> hbaseStsData, StreamTableEnvironment tableEnv, SingleOutputStreamOperator<Tuple7<String, String, String, Integer, Integer, String, Long>> indxData)
	{
		tableEnv.createTemporaryView("indexData", indxData);
		// tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination
		Table indxTblResult = tableEnv.sqlQuery(ETLQueries.TRIP_INDEX_DUPLICATE_QRY);

		DataStream<Tuple5<String, String, String, Integer, Double>> firstLevelAggrData = tableEnv
				.toRetractStream(indxTblResult, Row.class).map(new MapRowToTuple());

		tableEnv.createTemporaryView("firstLevelAggrData", firstLevelAggrData);
		Table indxTblAggrResult = tableEnv.sqlQuery(ETLQueries.TRIP_INDEX_AGGREGATION_QRY);
		
		return tableEnv
				.toRetractStream(indxTblAggrResult, Row.class).map(new MapRowToTuple());
	}
	
	public static DataStream<Trip> getConsolidatedTripData(SingleOutputStreamOperator<TripStatusData> hbaseStsData, SingleOutputStreamOperator<Tuple7<String, String, String, Integer, Integer, String, Long>> indxData, StreamTableEnvironment tableEnv)
	{
		DataStream<Tuple5<String, String, String, Integer, Double>> secondLevelAggrData = TripAggregations
				.getTripIndexAggregatedData(hbaseStsData, tableEnv, indxData);
		
		tableEnv.createTemporaryView("aggrIndxData", secondLevelAggrData);
		tableEnv.createTemporaryView("hbaseStsData", hbaseStsData);

		// tripCalC02Emission (VUsedFuel * Master table(CO2 Coefficient Fuel))/1000
		Table tripTblData = tableEnv.sqlQuery(ETLQueries.CONSOLIDATED_TRIP_QRY);

		// TODO yet To include getcurrentUTCTime() , tripCalC02Emission (VUsedFuel * Master table(CO2 Coefficient Fuel))/1000 , tripCalAvgGrossWtComb

		return tableEnv.toRetractStream(tripTblData, Trip.class).map(rec -> rec.f1);
	}
	
	public static DataStream<Trip> getConsolidatedTripData(SingleOutputStreamOperator<TripStatusData> hbaseStsData,DataStream<Tuple5<String, String, String, Integer, Double>> secondLevelAggrData, StreamTableEnvironment tableEnv)
	{
		tableEnv.createTemporaryView("aggrIndxData", secondLevelAggrData);
		tableEnv.createTemporaryView("hbaseStsData", hbaseStsData);

		// tripCalC02Emission (VUsedFuel * Master table(CO2 Coefficient Fuel))/1000
		Table tripTblData = tableEnv.sqlQuery(ETLQueries.CONSOLIDATED_TRIP_QRY);

		// TODO yet To include getcurrentUTCTime() , tripCalC02Emission (VUsedFuel * Master table(CO2 Coefficient Fuel))/1000 , tripCalAvgGrossWtComb

		return tableEnv.toRetractStream(tripTblData, Trip.class).map(rec -> rec.f1);
	}
	
	private static class MapRowToTuple
			implements MapFunction<Tuple2<Boolean, Row>, Tuple5<String, String, String, Integer, Double>> {

		private static final long serialVersionUID = 1L;

		@Override
		public Tuple5<String, String, String, Integer, Double> map(Tuple2<Boolean, Row> tuple2) throws Exception {

			Row row = tuple2.f1;
			return new Tuple5<String, String, String, Integer, Double>((String) (row.getField(0)),
					(String) (row.getField(1)), (String) (row.getField(2)), (Integer) (row.getField(3)),
					(Double) (row.getField(4)));
		}
	}


}
