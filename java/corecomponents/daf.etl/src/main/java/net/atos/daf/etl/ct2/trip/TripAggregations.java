package net.atos.daf.etl.ct2.trip;


import java.io.Serializable;
import java.sql.Timestamp;
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
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.tuple.Tuple3;
import org.apache.flink.api.java.tuple.Tuple6;
import org.apache.flink.api.java.tuple.Tuple7;
import org.apache.flink.api.java.tuple.Tuple8;
import org.apache.flink.api.java.tuple.Tuple9;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.core.fs.FileSystem;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.table.api.Table;
import org.apache.flink.table.api.bridge.java.StreamTableEnvironment;
import org.apache.flink.types.Row;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

//import net.atos.daf.etl.ct2.common.bo.Trip;
import net.atos.daf.etl.ct2.common.bo.TripStatusAggregation;
import net.atos.daf.etl.ct2.common.bo.TripStatusData;
import net.atos.daf.etl.ct2.common.hbase.TripIndexData;
import net.atos.daf.etl.ct2.common.util.ETLConstants;
import net.atos.daf.etl.ct2.common.util.ETLQueries;
import net.atos.daf.postgre.bo.Trip;

public class TripAggregations implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static Logger logger = LoggerFactory.getLogger(TripAggregations.class);

	
	public SingleOutputStreamOperator<Tuple9<String, String, String, Integer, Integer, String, Long, Long, Long>> getTripIndexData(SingleOutputStreamOperator<TripStatusData> hbaseStsData, StreamTableEnvironment tableEnv , ParameterTool envParams)
	{
		Map<String, List<String>> tripIndxClmns = getTripIndexColumns();
		SingleOutputStreamOperator<Tuple9<String, String, String, Integer, Integer, String, Long, Long, Long>> indxData = hbaseStsData
				.keyBy(value -> value.getTripId())
				.flatMap(new TripIndexData(envParams.get(ETLConstants.INDEX_TABLE_NM), tripIndxClmns, null));

		if ("true".equals(envParams.get(ETLConstants.WRITE_OUTPUT)))
			indxData.writeAsText(envParams.get(ETLConstants.WRITE_OUTPUT)+"indexData.txt", FileSystem.WriteMode.OVERWRITE)
			.name("writeIndexDataToFile");

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
	
	private DataStream<Tuple6<String, String, String, Integer, Double, Double>> getTripIndexAggregatedData(SingleOutputStreamOperator<TripStatusData> hbaseStsData, StreamTableEnvironment tableEnv, SingleOutputStreamOperator<Tuple9<String, String, String, Integer, Integer, String, Long, Long, Long>> indxData, Long timeInMilli)
	{

		tableEnv.createTemporaryView("indexData", indxData);
		// tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination, jobNm, evtDateTime, vDist, increment
		// tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination, evtDateTime, vDist, increment
		Table indxTblResult = tableEnv.sqlQuery(ETLQueries.TRIP_INDEX_DUPLICATE_QRY);

		//tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination,evtDateTime, vDist, increment
		DataStream<Tuple8<String, String, String, Integer, Double, Long, Long, Long>> firstLevelAggrData = tableEnv
				.toRetractStream(indxTblResult, Row.class).map(new MapRowToTuple8());

		//tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination,vDist, previousVdist, increment, formula for avgWt
		DataStream<Tuple9<String, String, String, Integer, Double, Long, Long, Long, Double>> grossWtCombData = getGrossWtCombData(firstLevelAggrData, timeInMilli);
		tableEnv.createTemporaryView("grossWtCombData", grossWtCombData);
		
		Table indxTblAggrResult = tableEnv.sqlQuery(ETLQueries.TRIP_INDEX_AGGREGATION_QRY);
		return tableEnv.toRetractStream(indxTblAggrResult, Row.class).map(new MapRowToTuple());
	}
	
	public DataStream<Trip> getConsolidatedTripData(SingleOutputStreamOperator<TripStatusData> hbaseStsData, SingleOutputStreamOperator<Tuple9<String, String, String, Integer, Integer, String, Long, Long, Long>> indxData, DataStream<Tuple3<Double, Double, Double>> vehDieselEmissionFactors, Long timeInMilli, StreamTableEnvironment tableEnv)
	{
		DataStream<Tuple6<String, String, String, Integer, Double, Double>> secondLevelAggrData = getTripIndexAggregatedData(hbaseStsData, tableEnv, indxData, timeInMilli);
		
		tableEnv.createTemporaryView("hbaseStsData", hbaseStsData);
		tableEnv.createTemporaryView("vehDieselEmissionFactors", vehDieselEmissionFactors);
		
		Table stsTblData = tableEnv.sqlQuery(ETLQueries.TRIP_STATUS_AGGREGATION_QRY);
		DataStream<TripStatusAggregation> stsAggregatedData = tableEnv.toRetractStream(stsTblData, TripStatusAggregation.class).map(rec -> rec.f1);
		
		tableEnv.createTemporaryView("secondLevelAggrData", secondLevelAggrData);
		tableEnv.createTemporaryView("stsAggregatedData", stsAggregatedData);
		Table tripTblData = tableEnv.sqlQuery(ETLQueries.CONSOLIDATED_TRIP_QRY);
		return tableEnv.toRetractStream(tripTblData, Trip.class).map(rec -> rec.f1);
	}
	
	public DataStream<Tuple3<Double, Double, Double>> getVehDieselEmissionFactors(ParameterTool envParams,
			StreamExecutionEnvironment env) {

		return env.fromElements(Tuple3.of(Double.valueOf(envParams.get(ETLConstants.DIESEL_WEIGHT_KG)),
				Double.valueOf(envParams.get(ETLConstants.DIESEL_HEATING_VALUE)),
				Double.valueOf(envParams.get(ETLConstants.DIESEL_CO2_EMISSION_FACTOR))));

	}
	
	private DataStream<Tuple9<String, String, String, Integer, Double, Long, Long, Long, Double>> getGrossWtCombData(
			DataStream<Tuple8<String, String, String, Integer, Double, Long, Long, Long>> firstLevelAggrData,
			Long timeInMilli) {

		return firstLevelAggrData
					//keyBy(value -> value.f0)
					.keyBy(new KeySelector<Tuple8<String, String, String, Integer, Double, Long, Long, Long>, String>() {
						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;

						@Override
						public String getKey(Tuple8<String, String, String, Integer, Double, Long, Long, Long> value)
							throws Exception {
						return value.f0;
						}
					})
					.map(new RichMapFunction<Tuple8<String, String, String, Integer, Double, Long, Long, Long>,Tuple9<String, String, String, Integer, Double, Long, Long, Long, Double>> (){

						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;
						private transient ValueState<Tuple2<String, Long>> prevVDistState ;

						
						@Override
						public Tuple9<String, String, String, Integer, Double, Long, Long, Long, Double> map(
								Tuple8<String, String, String, Integer, Double, Long, Long, Long> row)
								throws Exception {
							Tuple2<String, Long> currentTripVDist = prevVDistState.value();
							long vDistDiff = 0;
							long prevVDist = 0;
							if (currentTripVDist != null){
								vDistDiff = (Long) (row.getField(6)) - currentTripVDist.f1 ;
								prevVDist = currentTripVDist.f1;
								
								if(vDistDiff == 0)
									vDistDiff = 1;
							}else{
								currentTripVDist = new Tuple2<String, Long>();
							}
							
							currentTripVDist.f0 = (String) (row.getField(0));
							currentTripVDist.f1 = (Long) (row.getField(6));
							prevVDistState.update(currentTripVDist);
							
							//tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination,vDist, previousVdist, increment, formula for avgWt
							return new Tuple9<String, String, String, Integer, Double, Long, Long, Long, Double>(
									(String) (row.getField(0)), (String) (row.getField(1)),
									(String) (row.getField(2)), (Integer) (row.getField(3)),
									(Double) (row.getField(4)), (Long) (row.getField(6)), prevVDist,
									(Long) (row.getField(7)), ((Double) (row.getField(4)) * vDistDiff));
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
			implements MapFunction<Tuple2<Boolean, Row>, Tuple6<String, String, String, Integer, Double, Double>> {

		private static final long serialVersionUID = 1L;

		@Override
		public Tuple6<String, String, String, Integer, Double, Double> map(Tuple2<Boolean, Row> tuple2) throws Exception {

			Row row = tuple2.f1;
			return new Tuple6<String, String, String, Integer, Double, Double>((String) (row.getField(0)),
					(String) (row.getField(1)), (String) (row.getField(2)), (Integer) (row.getField(3)),
					(Double) (row.getField(4)), (Double) (row.getField(5)));
		}
	}

	private class MapRowToTuple7 implements
			MapFunction<Tuple2<Boolean, Row>, Tuple7<String, String, String, Integer, Double, Timestamp, Long>> {

		private static final long serialVersionUID = 1L;

		@Override
		public Tuple7<String, String, String, Integer, Double, Timestamp, Long> map(Tuple2<Boolean, Row> tuple2)
				throws Exception {

			Row row = tuple2.f1;
			return new Tuple7<String, String, String, Integer, Double, Timestamp, Long>((String) (row.getField(0)),
					(String) (row.getField(1)), (String) (row.getField(2)), (Integer) (row.getField(3)),
					(Double) (row.getField(4)), new Timestamp((Long) row.getField(5)), (Long) (row.getField(6)));
		}
	}

	private class MapRowToTuple8 implements
			MapFunction<Tuple2<Boolean, Row>, Tuple8<String, String, String, Integer, Double, Long, Long, Long>> {

		private static final long serialVersionUID = 1L;

		@Override
		public Tuple8<String, String, String, Integer, Double, Long, Long, Long> map(Tuple2<Boolean, Row> tuple2)
				throws Exception {

			Row row = tuple2.f1;
			return new Tuple8<String, String, String, Integer, Double, Long, Long, Long>((String) (row.getField(0)),
					(String) (row.getField(1)), (String) (row.getField(2)), (Integer) (row.getField(3)),
					(Double) (row.getField(4)), (Long) (row.getField(5)), (Long) (row.getField(6)),
					(Long) (row.getField(7)));
		}
	 }
	
	//tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination,vDist, previousVdist, increment, avgWt
	private class MapRowToTuple9 implements
			MapFunction<Tuple2<Boolean, Row>, Tuple9<String, String, String, Integer, Double, Long, Long, Long, Double>> {

		private static final long serialVersionUID = 1L;

		@Override
		public Tuple9<String, String, String, Integer, Double, Long, Long, Long, Double> map(Tuple2<Boolean, Row> tuple2)
				throws Exception {

			Row row = tuple2.f1;
			return new Tuple9<String, String, String, Integer, Double, Long, Long, Long, Double>((String) (row.getField(0)),
					(String) (row.getField(1)), (String) (row.getField(2)), (Integer) (row.getField(3)),
					(Double) (row.getField(4)), (Long) (row.getField(5)), (Long) (row.getField(6)),
					(Long) (row.getField(7)), (Double) (row.getField(8)));
			}
	}

}
