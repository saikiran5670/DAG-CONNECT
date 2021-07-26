package net.atos.daf.ct2.etl.trip;


import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import org.apache.flink.api.common.functions.FlatMapFunction;
import org.apache.flink.api.java.tuple.Tuple11;
import org.apache.flink.runtime.testutils.MiniClusterResourceConfiguration;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.sink.SinkFunction;
import org.apache.flink.table.api.bridge.java.StreamTableEnvironment;
import org.apache.flink.test.util.MiniClusterWithClientResource;
import org.apache.flink.util.Collector;
import org.junit.ClassRule;
import org.junit.Test;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import junit.framework.Assert;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.etl.common.bo.TripAggregatedData;
import net.atos.daf.ct2.etl.common.bo.TripStatusData;
import net.atos.daf.ct2.etl.common.util.ETLConstants;
import net.atos.daf.ct2.etl.common.util.FlinkUtil;

public class TripStreamingUnitTesting {

	private static Logger logger = LoggerFactory.getLogger(TripStreamingUnitTesting.class);

	private static final int NUM_TMS = 1;
	private static final int NUM_SLOTS = 1;
	private static final int PARALLELISM = NUM_SLOTS * NUM_TMS;

	@ClassRule
	public final static MiniClusterWithClientResource MINI_CLUSTER_WITH_CLIENT_RESOURCE = new MiniClusterWithClientResource(
			new MiniClusterResourceConfiguration.Builder().setNumberSlotsPerTaskManager(NUM_SLOTS)
					.setNumberTaskManagers(NUM_TMS).build());
	
	@Test
	public void testConsolidatedTripJob() throws Exception {

		StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
		env.setParallelism(PARALLELISM);
		final StreamTableEnvironment tableEnv = FlinkUtil.createStreamTableEnvironment(env);
		
		TripStatusData tripData = new TripStatusData();
		tripData.setTripId("fa63bf81-dbfb-4acc-a20a-23e2f7e0cdb0");
		
		tripData.setVid("M4A1114");
		tripData.setVin("M4A1114");
		tripData.setGpsTripDist(299L);
		tripData.setGpsStopVehDist(Long.valueOf(443567905));
		tripData.setGpsStartVehDist(Long.valueOf(443567695));
		tripData.setVIdleDuration(22L);
		tripData.setGpsStartLatitude(Double.valueOf("41.8842"));
		tripData.setGpsStartLongitude(Double.valueOf("-87.6388"));
		tripData.setGpsEndLatitude(Double.valueOf("-87.6388"));
		tripData.setGpsEndLongitude(Double.valueOf("-87.6388"));
		tripData.setVUsedFuel(110L);
		tripData.setVStopFuel(118918536L);
		tripData.setVStartFuel(118918426L);
		tripData.setVTripMotionDuration(73L);
		tripData.setReceivedTimestamp(1596775611957L);
		tripData.setVPTODuration(2L);
		tripData.setVHarshBrakeDuration(1L);
		tripData.setVBrakeDuration(10L);
		tripData.setVMaxThrottlePaddleDuration(3L);
		tripData.setVTripAccelerationTime(51L);
		tripData.setVCruiseControlDist(40L);
		tripData.setVTripDPABrakingCount(5L);
		tripData.setVTripDPAAnticipationCount(6L);
		tripData.setVCruiseControlFuelConsumed(7L);
		tripData.setVIdleFuelConsumed(17L);
		tripData.setVSumTripDPABrakingScore(8L);
		tripData.setVSumTripDPAAnticipationScore(9L);
		tripData.setDriverId("NL B000171984000002");
		tripData.setStartDateTime(1596775285000L);
		tripData.setEndDateTime(1596775380000L);
		tripData.setTripCalGpsVehDistDiff(210L);
		tripData.setTripCalGpsVehTimeDiff(95000L);
		tripData.setNumberOfIndexMessage(4L);
		final SingleOutputStreamOperator<TripStatusData> tripStsData= env.fromElements(
				tripData);

		SingleOutputStreamOperator<Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String>> indxData = tripStsData.flatMap(		
		new FlatMapFunction<TripStatusData, Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String>>() {
			/**
			 * 
			 */
			private static final long serialVersionUID = 1L;

			@Override
			public void flatMap(TripStatusData value, Collector<Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String>> out) throws Exception {
				//2020-11-02T16:55:37.000Z
				out.collect(new Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String>("fa63bf81-dbfb-4acc-a20a-23e2f7e0cdb0", "M4A1114", "*", 0, 655350, "index", TimeFormatter.getInstance().convertUTCToEpochMilli("2020-11-02T16:55:35.000Z", ETLConstants.DATE_FORMAT ), 11111991L, 111111L, 0, "testDriver") );
				long ts =TimeFormatter.getInstance().convertUTCToEpochMilli("2020-11-02T16:55:37.000Z", ETLConstants.DATE_FORMAT );
				out.collect(new Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String>("fa63bf81-dbfb-4acc-a20a-23e2f7e0cdb0", "M4A1114", "*", 10, 100000, "index", ts, 11111996L, 111113L, 1, "testDriver") );
				out.collect(new Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String>("fa63bf81-dbfb-4acc-a20a-23e2f7e0cdb0", "M4A1114", "*", 10, 100000, "index", ts, 11111996L, 111113L, 1, "testDriver") );
				out.collect(new Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String>("fa63bf81-dbfb-4acc-a20a-23e2f7e0cdb0", "M4A1114", "*", 20, 600000, "index", TimeFormatter.getInstance().convertUTCToEpochMilli("2020-11-02T16:55:36.000Z", ETLConstants.DATE_FORMAT ), 11111993L, 111112L, 1, "testDriver") );
				out.collect(new Tuple11<String, String, String, Integer, Integer, String, Long, Long, Long, Integer, String>("fa63bf81-dbfb-4acc-a20a-23e2f7e0cdb0", "M4A1114", "*", 20, 600000, "index", TimeFormatter.getInstance().convertUTCToEpochMilli("2020-11-02T16:55:38.000Z", ETLConstants.DATE_FORMAT) , 11111993L, 111114L, 1, "testDriver") );
								
			}
		});
		
		
		//DataStream<Tuple5<String, String, String, Integer, Double>> secondLevelAggrData = TripAggregations.getTripIndexAggregatedData(tripStsData, tableEnv, indxData);
		TripAggregations tripAggregations = new TripAggregations();
		
		//DataStream<Trip> finalTripData = tripAggregations.getConsolidatedTripData(tripStsData, indxData, env.fromElements(Tuple3.of(0.00082, 42.7, 74.3)), 5000L, tableEnv );
		DataStream<TripAggregatedData> finalTripData = tripAggregations.getConsolidatedTripData(tripStsData, indxData, 5000L, tableEnv );
		
		finalTripData.print();
		
		//finalTripData.map(rec ->{System.out.println("TripCalDist :: "+rec.getTripCalDist()); return rec;});
		finalTripData.addSink(new CollectSink());
		
		
		
		// execute
        env.execute();

        // verify your results
       int sz = CollectSink.values.size();
       Assert.assertEquals(CollectSink.values.get(sz-1).getTripCalUsedFuel(), Long.valueOf(110));
	}

	
	 // create a testing sink
    private static class CollectSink implements SinkFunction<TripAggregatedData> {

        /**
		 * 
		 */
		private static final long serialVersionUID = 1L;
		// must be static
        public static final List<TripAggregatedData> values = Collections.synchronizedList(new ArrayList<>());

        @Override
        public void invoke(TripAggregatedData value) throws Exception {
            values.add(value);
        }
    }

}
