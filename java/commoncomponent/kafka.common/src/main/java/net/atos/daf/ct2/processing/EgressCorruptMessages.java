package net.atos.daf.ct2.processing;

import java.io.Serializable;
import java.util.Properties;

import org.apache.flink.api.common.functions.FilterFunction;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;

public class EgressCorruptMessages implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	 private static final Logger logger = LoggerFactory.getLogger(EgressCorruptMessages.class);
	
	public void egressCorruptMessages(DataStream<Tuple2<Integer, KafkaRecord<String>>> srcStream, Properties properties,
			String sinkTopicName) {

		srcStream.filter(new FilterFunction<Tuple2<Integer, KafkaRecord<String>>>() {
			/**
			 * 
			 */
			private static final long serialVersionUID = 1L;

			@Override
			public boolean filter(Tuple2<Integer, KafkaRecord<String>> rec) throws Exception {
				return rec.f0 == 0;
			}
		}).map(new MapFunction<Tuple2<Integer, KafkaRecord<String>>, KafkaRecord<String>>() {

			/**
			 * 
			 */
			private static final long serialVersionUID = 1L;

			@Override
			public KafkaRecord<String> map(Tuple2<Integer, KafkaRecord<String>> arg0) throws Exception {
				//System.out.println("Received corrupt message from source system :: " + arg0.f1.getValue());
				return arg0.f1;
			}
		}).addSink(new FlinkKafkaProducer<KafkaRecord<String>>(sinkTopicName,
				new KafkaMessageSerializeSchema<String>(sinkTopicName), properties,
				FlinkKafkaProducer.Semantic.AT_LEAST_ONCE));
	}


	public void egressCorruptMessages(SingleOutputStreamOperator<KafkaRecord<String>> srcStream, Properties properties,
			String sinkTopicName) {

		srcStream.map(rec -> {
			logger.info(" Egress corrupt record :: {}", rec);
			return rec;
		}).returns(new TypeHint<KafkaRecord<String>>() {
		}.getTypeInfo())
				.addSink(new FlinkKafkaProducer<KafkaRecord<String>>(sinkTopicName,
						new KafkaMessageSerializeSchema<String>(sinkTopicName), properties,
						FlinkKafkaProducer.Semantic.AT_LEAST_ONCE));
	}
}
