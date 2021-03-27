package net.atos.daf.ct2.processing;

import java.io.Serializable;
import java.util.UUID;

import org.apache.flink.api.common.functions.FilterFunction;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.datastream.DataStream;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.constant.KafkaCT2Constant;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.utils.JsonMapper;

public class ValidateSourceStream implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	public DataStream<Tuple2<Integer, KafkaRecord<String>>> isValidJSON(DataStream<KafkaRecord<String>> srcDataStream) {

		return srcDataStream.map(new MapFunction<KafkaRecord<String>, Tuple2<Integer, KafkaRecord<String>>>() {

			/**
			* 
			*/
			private static final long serialVersionUID = 1L;
			ObjectMapper objectMapper = new ObjectMapper();

			@Override
			public Tuple2<Integer, KafkaRecord<String>> map(KafkaRecord<String> rec) throws Exception {

				try {
					JsonNode inputMsg = objectMapper.readTree(rec.getValue());
					long kafkaProcessingTS = TimeFormatter.getInstance().getCurrentUTCTime();
					((ObjectNode) inputMsg).put("kafkaProcessingTS", kafkaProcessingTS);
					
					KafkaRecord<String> kafkaRecord = new KafkaRecord<String>();
					kafkaRecord.setKey(UUID.randomUUID().toString());
                    kafkaRecord.setValue( JsonMapper.configuring().writeValueAsString(inputMsg));
                    return new Tuple2<Integer, KafkaRecord<String>>(KafkaCT2Constant.VALID_DATA, kafkaRecord);
				} catch (Exception e) {
					return new Tuple2<Integer, KafkaRecord<String>>(KafkaCT2Constant.UNKNOWN_DATA, rec);
				}
			}
		}).returns(new TypeHint<Tuple2<Integer, KafkaRecord<String>>>() {
		}.getTypeInfo());
	}

	public DataStream<KafkaRecord<String>> getValidSourceMessages(
			DataStream<Tuple2<Integer, KafkaRecord<String>>> streamWithValidSts, int filterSts) {

		return streamWithValidSts.filter(new FilterFunction<Tuple2<Integer, KafkaRecord<String>>>() {
			/**
			 * 
			 */
			private static final long serialVersionUID = 1L;

			public boolean filter(Tuple2<Integer, KafkaRecord<String>> rec) throws Exception {
				return rec.f0 == filterSts;
			}

		}).map(new MapFunction<Tuple2<Integer, KafkaRecord<String>>, KafkaRecord<String>>() {

			/**
			 * 
			 */
			private static final long serialVersionUID = 1L;

			public KafkaRecord<String> map(Tuple2<Integer, KafkaRecord<String>> rec) throws Exception {
				return rec.f1;
			}

		});
	}
	
	public DataStream<String> getValidSourceMessageAsString(
			DataStream<Tuple2<Integer, KafkaRecord<String>>> streamWithValidSts, int filterSts) {

		return streamWithValidSts.filter(new FilterFunction<Tuple2<Integer, KafkaRecord<String>>>() {
			/**
			 * 
			 */
			private static final long serialVersionUID = 1L;

			public boolean filter(Tuple2<Integer, KafkaRecord<String>> rec) throws Exception {
				return rec.f0 == filterSts;
			}

		}).map(new MapFunction<Tuple2<Integer, KafkaRecord<String>>, String>() {

			/**
			 * 
			 */
			private static final long serialVersionUID = 1L;

			public String map(Tuple2<Integer, KafkaRecord<String>> rec) throws Exception {
				return rec.f1.getValue();
			}

		});
	}


}
