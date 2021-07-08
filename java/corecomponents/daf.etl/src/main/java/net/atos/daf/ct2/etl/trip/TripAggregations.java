package net.atos.daf.ct2.etl.trip;


import java.io.Serializable;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.functions.RichMapFunction;
import org.apache.flink.api.common.state.ValueState;
import org.apache.flink.api.common.state.ValueStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.functions.KeySelector;
import org.apache.flink.api.java.tuple.Tuple10;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.tuple.Tuple8;
import org.apache.flink.api.java.tuple.Tuple9;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.table.api.Table;
import org.apache.flink.table.api.bridge.java.StreamTableEnvironment;
import org.apache.flink.types.Row;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.etl.common.bo.TripAggregatedData;
import net.atos.daf.ct2.etl.common.bo.TripStatusAggregation;
import net.atos.daf.ct2.etl.common.bo.TripStatusData;
import net.atos.daf.ct2.etl.common.hbase.TripIndexData;
import net.atos.daf.ct2.etl.common.postgre.TripCo2Emission;
import net.atos.daf.ct2.etl.common.util.ETLConstants;
import net.atos.daf.ct2.etl.common.util.ETLQueries;
import net.atos.daf.postgre.bo.EcoScore;
import net.atos.daf.postgre.bo.Trip;
import net.atos.daf.common.ct2.utc.TimeFormatter;

public class TripAggregations implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static Logger logger = LoggerFactory.getLogger(TripAggregations.class);

	
	public SingleOutputStreamOperator<Tuple10<String, String, String, Integer, Integer, String, Long, Long, Long, Integer>> getTripIndexData(SingleOutputStreamOperator<TripStatusData> hbaseStsData, StreamTableEnvironment tableEnv , ParameterTool envParams)
	{
		Map<String, List<String>> tripIndxClmns = getTripIndexColumns();
		SingleOutputStreamOperator<Tuple10<String, String, String, Integer, Integer, String, Long, Long, Long, Integer>> indxData = hbaseStsData
				.keyBy(value -> value.getTripId())
				.flatMap(new TripIndexData(envParams.get(ETLConstants.INDEX_TABLE_NM), tripIndxClmns, null));

		/*if ("true".equals(envParams.get(ETLConstants.WRITE_OUTPUT)))
			indxData.writeAsText(envParams.get(ETLConstants.WRITE_OUTPUT)+"indexData.txt", FileSystem.WriteMode.OVERWRITE)
			.name("writeIndexDataToFile");*/

		return indxData;
	}
	
	private Map<String, List<String>> getTripIndexColumns() {
		Map<String, List<String>> tripIndxClmns = new HashMap<>();
		List<String> indxClmns = new ArrayList<>();

		indxClmns.add(ETLConstants.INDEX_MSG_TRIP_ID);
		indxClmns.add(ETLConstants.INDEX_MSG_VID);
		indxClmns.add(ETLConstants.INDEX_MSG_V_TACHOGRAPH_SPEED);
		indxClmns.add(ETLConstants.INDEX_MSG_V_GROSSWEIGHT_COMBINATION);
		indxClmns.add(ETLConstants.INDEX_MSG_DRIVER2_ID);
		indxClmns.add(ETLConstants.INDEX_MSG_JOBNAME);
		indxClmns.add(ETLConstants.INDEX_MSG_INCREMENT);
		indxClmns.add(ETLConstants.INDEX_MSG_VDIST);
		indxClmns.add(ETLConstants.INDEX_MSG_EVT_DATETIME);
		tripIndxClmns.put(ETLConstants.INDEX_MSG_COLUMNFAMILY_T, indxClmns);

		return tripIndxClmns;
	}
	
	private DataStream<Tuple8<String, String, String, Integer, Double, Double, Double, Integer>> getTripIndexAggregatedData(SingleOutputStreamOperator<TripStatusData> hbaseStsData, StreamTableEnvironment tableEnv, SingleOutputStreamOperator<Tuple10<String, String, String, Integer, Integer, String, Long, Long, Long, Integer>> indxData, Long timeInMilli)
	{

		tableEnv.createTemporaryView("indexData", indxData);
		// tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination, jobNm, evtDateTime, vDist, increment, grossWtRecCnt
		// tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination, evtDateTime, vDist, increment, grossWtRecCnt
		Table indxTblResult = tableEnv.sqlQuery(ETLQueries.TRIP_INDEX_DUPLICATE_QRY);

		//tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination,evtDateTime, vDist, increment
		DataStream<Tuple9<String, String, String, Integer, Double, Long, Long, Long, Integer>> firstLevelAggrData = tableEnv
				.toRetractStream(indxTblResult, Row.class).map(new MapRowToTuple9());

		//tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination,vDist, previousVdist, increment, formula for avgWt
		DataStream<Tuple10<String, String, String, Integer, Double, Long, Long, Long, Double, Integer>> grossWtCombData = getGrossWtCombData(firstLevelAggrData, timeInMilli);
		
		tableEnv.createTemporaryView("grossWtCombData", grossWtCombData);
		
		Table indxTblAggrResult = tableEnv.sqlQuery(ETLQueries.TRIP_INDEX_AGGREGATION_QRY);
		
		return tableEnv.toRetractStream(indxTblAggrResult, Row.class).map(new MapRowToTuple());
	}
		
	public DataStream<TripAggregatedData> getConsolidatedTripData(SingleOutputStreamOperator<TripStatusData> stsData, SingleOutputStreamOperator<Tuple10<String, String, String, Integer, Integer, String, Long, Long, Long, Integer>> indxData, Long timeInMilli, StreamTableEnvironment tableEnv)
	{
		DataStream<Tuple8<String, String, String, Integer, Double, Double, Double, Integer>> secondLevelAggrData = getTripIndexAggregatedData(stsData, tableEnv, indxData, timeInMilli);
		
		tableEnv.createTemporaryView("tripStsData", stsData);
		
		Table stsTblData = tableEnv.sqlQuery(ETLQueries.TRIP_STATUS_AGGREGATION_QRY);
		DataStream<TripStatusAggregation> stsAggregatedData = tableEnv.toRetractStream(stsTblData, TripStatusAggregation.class).map(rec -> {return rec.f1;});
		
		tableEnv.createTemporaryView("secondLevelAggrData", secondLevelAggrData);
		tableEnv.createTemporaryView("stsAggregatedData", stsAggregatedData);
		Table tripTblData = tableEnv.sqlQuery(ETLQueries.CONSOLIDATED_TRIP_QRY);
		
		return tableEnv.toRetractStream(tripTblData, TripAggregatedData.class).map(rec -> { rec.f1.setTripProcessingTS(TimeFormatter.getInstance().getCurrentUTCTime()); return rec.f1;});
	}
	
	public DataStream<Trip> getTripStatisticData(DataStream<TripAggregatedData> tripAggrData, StreamTableEnvironment tableEnv)
	{
		tableEnv.createTemporaryView("tripAggrData", tripAggrData);
		Table tripStatisticData =tableEnv.sqlQuery(ETLQueries.TRIP_QRY);
		
		return tableEnv.toRetractStream(tripStatisticData, Trip.class).map(rec -> rec.f1);
	}
	
	public DataStream<EcoScore> getEcoScoreData(DataStream<TripAggregatedData> tripAggrData, StreamTableEnvironment tableEnv)
	{
		tableEnv.createTemporaryView("tripAggrDataForEcoScore", tripAggrData);
		Table tripStatisticData =tableEnv.sqlQuery(ETLQueries.ECOSCORE_QRY);
		
		return tableEnv.toRetractStream(tripStatisticData, EcoScore.class).map(rec -> rec.f1);
	}
	
	public SingleOutputStreamOperator<TripStatusData> getTripStsWithCo2Emission(
			SingleOutputStreamOperator<TripStatusData> tripStsData) throws Exception {
		return tripStsData
				.keyBy(value -> value.getTripId())
				.flatMap(new TripCo2Emission());
	}

	private DataStream<Tuple10<String, String, String, Integer, Double, Long, Long, Long, Double, Integer>> getGrossWtCombData(
			DataStream<Tuple9<String, String, String, Integer, Double, Long, Long, Long, Integer>> firstLevelAggrData,
			Long timeInMilli) {

		return firstLevelAggrData
					//keyBy(value -> value.f0)
					.keyBy(new KeySelector<Tuple9<String, String, String, Integer, Double, Long, Long, Long, Integer>, String>() {
						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;

						@Override
						public String getKey(Tuple9<String, String, String, Integer, Double, Long, Long, Long, Integer> value)
							throws Exception {
						return value.f0;
						}
					})
					.map(new RichMapFunction<Tuple9<String, String, String, Integer, Double, Long, Long, Long, Integer>,Tuple10<String, String, String, Integer, Double, Long, Long, Long, Double, Integer>> (){

						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;
						private transient ValueState<Tuple2<String, Long>> prevVDistState ;

						
						@Override
						public Tuple10<String, String, String, Integer, Double, Long, Long, Long, Double, Integer> map(
								Tuple9<String, String, String, Integer, Double, Long, Long, Long, Integer> row)
								throws Exception {
							Tuple2<String, Long> currentTripVDist = prevVDistState.value();
							long vDistDiff = 0;
							long prevVDist = 0;
							if (currentTripVDist != null){
								vDistDiff = (Long) (row.getField(6)) - currentTripVDist.f1 ;
								prevVDist = currentTripVDist.f1;
								
							}else{
								currentTripVDist = new Tuple2<String, Long>();
							}
							

							if(vDistDiff == 0)
								vDistDiff = 1;
							
							currentTripVDist.f0 = (String) (row.getField(0));
							currentTripVDist.f1 = (Long) (row.getField(6));
							prevVDistState.update(currentTripVDist);
							
							logger.info(" GrossWt Info :: "+new Tuple10<String, String, String, Integer, Double, Long, Long, Long, Double, Integer>(
									(String) (row.getField(0)), (String) (row.getField(1)),
									(String) (row.getField(2)), (Integer) (row.getField(3)),
									(Double) (row.getField(4)), (Long) (row.getField(6)), prevVDist,
									(Long) (row.getField(7)), ((Double) (row.getField(4)) * vDistDiff),
									(Integer) (row.getField(8))));
							
							logger.info(" vDistDiff :"+vDistDiff +" row.getField(4): "+((Double) (row.getField(4))) );
							
							//tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination,vDist, previousVdist, increment, formula for avgWt, grossWtRecCnt
							return new Tuple10<String, String, String, Integer, Double, Long, Long, Long, Double, Integer>(
									(String) (row.getField(0)), (String) (row.getField(1)),
									(String) (row.getField(2)), (Integer) (row.getField(3)),
									(Double) (row.getField(4)), (Long) (row.getField(6)), prevVDist,
									(Long) (row.getField(7)), ((Double) (row.getField(4)) * vDistDiff),
									(Integer) (row.getField(8)));
						}
						
						@Override
					public void open(org.apache.flink.configuration.Configuration config) {
						ValueStateDescriptor<Tuple2<String, Long>> descriptor = new ValueStateDescriptor<Tuple2<String, Long>>(// the state name
								"tripPrevVDist", TypeInformation.of(new TypeHint<Tuple2<String, Long>>() {
								}));
						prevVDistState = getRuntimeContext().getState(descriptor);
					}
					});
	}
		
	private class MapRowToTuple
			implements MapFunction<Tuple2<Boolean, Row>, Tuple8<String, String, String, Integer, Double, Double, Double, Integer>> {

		private static final long serialVersionUID = 1L;

		@Override
		public Tuple8<String, String, String, Integer, Double, Double, Double, Integer> map(Tuple2<Boolean, Row> tuple2) throws Exception {

			Row row = tuple2.f1;
			return new Tuple8<String, String, String, Integer, Double, Double, Double, Integer>((String) (row.getField(0)),
					(String) (row.getField(1)), (String) (row.getField(2)), (Integer) (row.getField(3)),
					(Double) (row.getField(4)), (Double) (row.getField(5)), (Double) (row.getField(6)), (Integer) (row.getField(7)));
		}
	}

	private class MapRowToTuple9 implements
			MapFunction<Tuple2<Boolean, Row>, Tuple9<String, String, String, Integer, Double, Long, Long, Long, Integer>> {

		private static final long serialVersionUID = 1L;

		@Override
		public Tuple9<String, String, String, Integer, Double, Long, Long, Long, Integer> map(Tuple2<Boolean, Row> tuple2)
				throws Exception {

			Row row = tuple2.f1;
			return new Tuple9<String, String, String, Integer, Double, Long, Long, Long, Integer>((String) (row.getField(0)),
					(String) (row.getField(1)), (String) (row.getField(2)), (Integer) (row.getField(3)),
					(Double) (row.getField(4)), (Long) (row.getField(5)), (Long) (row.getField(6)),
					(Long) (row.getField(7)), (Integer) (row.getField(8)));
		}
	 }
	
}
