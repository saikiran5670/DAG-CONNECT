package net.atos.daf.ct2.processing;

import java.io.Serializable;
import java.util.Iterator;
import java.util.Properties;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.JsonMappingException;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.node.ObjectNode;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.MonitorDocument;
import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;
import net.atos.daf.ct2.utils.JsonMapper;

public class MessageProcessing<U, T> implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(MessageProcessing.class);

	public void consumeBoschMessage(DataStream<KafkaRecord<U>> messageDataStream, String messageType, String key,
			String sinkTopicName, Properties properties, Class<T> tClass) {

		messageDataStream.map(new MapFunction<KafkaRecord<U>, KafkaRecord<T>>() {

			/**
			* 
			*/
			private static final long serialVersionUID = 1L;

			public KafkaRecord<T> map(KafkaRecord<U> value) throws Exception {

				//Temporary code as we are receiving only monitoring data
				Monitor monitoringObj = new Monitor();
				MonitorDocument monitorDocument = new MonitorDocument();

				try {
					System.out.println("received Bosch message for mapping ::"+value.getValue().toString());
					JsonNode jsonNodeRec = JsonMapper.configuring().readTree(value.getValue().toString());
					if (jsonNodeRec != null) {

						JsonNode metaData = (JsonNode) jsonNodeRec.get("metaData");
						if (metaData != null) {
							JsonNode vehicle = (JsonNode) metaData.get("vehicle");
							if(vehicle != null){
								if (vehicle.get("vin") != null)
									monitoringObj.setVin(vehicle.get("vin").asText());
								else
									monitoringObj.setVin(DAFCT2Constant.UNKNOWN);
							}
							
							// TODO Dummy Variable
							monitorDocument.setTripID("BOSCH_" + TimeFormatter.getInstance().getCurrentUTCTime());
						}

						if (jsonNodeRec.get("kafkaProcessingTS") != null)
							monitoringObj.setKafkaProcessingTS(jsonNodeRec.get("kafkaProcessingTS").asText());

						JsonNode resultData = (JsonNode) jsonNodeRec.get("resultData");
						if (resultData != null) {
							if (resultData.get("start") != null)
								monitoringObj.setEvtDateTime(resultData.get("start").asText());
							if (resultData.get("start") != null)
								monitoringObj.setGpsDateTime(resultData.get("start").asText());
						

						JsonNode seriesPartsList = (JsonNode) resultData.get("seriesParts");
						if (seriesPartsList != null) {
							Iterator<JsonNode> seriesPartelements = seriesPartsList.elements();

							if (seriesPartelements != null)
								while (seriesPartelements.hasNext()) {
									JsonNode serieElement = seriesPartelements.next();
									if (serieElement != null) {
										JsonNode series = serieElement.get("series");
										if (series != null) {
											if (series.get("uid") != null && "GPS".equals(series.get("uid").asText())) {
												if (serieElement.get("dataPoints") != null
														&& serieElement.get("dataPoints").size() > 0) {
													JsonNode gpsCoordinates = serieElement.get("dataPoints").get(0)
															.get("value");
													if (gpsCoordinates != null) {
														if (gpsCoordinates.get("longitude") != null)
															monitoringObj.setGpsLongitude(Double
																	.valueOf(gpsCoordinates.get("longitude").asText()));
														if (gpsCoordinates.get("latitude") != null)
															monitoringObj.setGpsLatitude(Double
																	.valueOf(gpsCoordinates.get("latitude").asText()));
														if (gpsCoordinates.get("altitude") != null)
															monitoringObj.setGpsAltitude(Integer
																	.valueOf(gpsCoordinates.get("altitude").asText()));
														if (gpsCoordinates.get("gpsSpeed") != null)
															monitorDocument.setGpsSpeed((int) Double
																	.valueOf(gpsCoordinates.get("gpsSpeed").asText())
																	.doubleValue());
														if (gpsCoordinates.get("direction") != null)
															monitoringObj.setGpsHeading(Double
																	.valueOf(gpsCoordinates.get("direction").asText()));
													}
												}
											}
										}
									}
								}
						}
					}
					}
					monitoringObj.setDocument(monitorDocument);

				} catch (JsonMappingException e) {
					logger.error("Issue while reading Bosch message :: " + e.getMessage());
					logger.error("Issue while mapping streaming record to Daf Standard format :: "
							+ value.getValue().toString());
				} catch (JsonProcessingException e) {
					logger.error("Issue while processing Bosch message :: " + e.getMessage());
					logger.error("Issue while mapping streaming record to Daf Standard format :: "
							+ value.getValue().toString());
				}

				T record = JsonMapper.configuring().readValue(monitoringObj.toString(), tClass);
				// (T) JsonMapper.configuring().writeValueAsString(monitoringObj.toString())
				KafkaRecord<T> kafkaRecord = new KafkaRecord<T>();
				kafkaRecord.setKey(key);
				kafkaRecord.setValue(record);
				
				System.out.println("before publishing monitoringObj.toString() record :: "+monitoringObj.toString());
				System.out.println("before publishing kafka record :: "+record);
				return kafkaRecord;
			}
		}).addSink(
				new FlinkKafkaProducer<KafkaRecord<T>>(sinkTopicName, new KafkaMessageSerializeSchema<T>(sinkTopicName),
						properties, FlinkKafkaProducer.Semantic.EXACTLY_ONCE));
	}
	
	public DataStream<Tuple2<Integer, KafkaRecord<String>>> boschSourceStreamStatus(
			DataStream<KafkaRecord<String>> boschInputStream) {
		return boschInputStream.map(new MapFunction<KafkaRecord<String>, Tuple2<Integer, KafkaRecord<String>>>() {

			/**
			 * 
			 */
			private static final long serialVersionUID = 1L;
			String rowKey = null;

			@Override
			public Tuple2<Integer, KafkaRecord<String>> map(KafkaRecord<String> value) throws Exception {
				String vin = DAFCT2Constant.UNKNOWN;
				String transId = DAFCT2Constant.UNKNOWN;
				Integer recordType = DAFCT2Constant.MEASUREMENT_DATA;

				try {
					JsonNode jsonNodeRec = JsonMapper.configuring().readTree(value.getValue());
					logger.info("Bosch source rec :: " + jsonNodeRec);

					if (jsonNodeRec != null && jsonNodeRec.get("metaData") != null) {
						// Measurement Record
						if (jsonNodeRec.get("metaData").get("vehicle") != null) {
							if (jsonNodeRec.get("metaData").get("vehicle").get("vin") != null) {
								vin = jsonNodeRec.get("metaData").get("vehicle").get("vin").asText();
							}

							if (jsonNodeRec.get("metaData").get("vehicle").get("TransID") != null) {
								transId = jsonNodeRec.get("metaData").get("vehicle").get("TransID").asText();
							}
							
							((ObjectNode) jsonNodeRec).put("kafkaProcessingTS", TimeFormatter.getInstance().getCurrentUTCTime());
						}
					} else {
						// TCU Record
						recordType = DAFCT2Constant.TCU_DATA;
						if (jsonNodeRec.get("vin") != null) {
							vin = jsonNodeRec.get("vin").asText();
						}

						if (jsonNodeRec.get("deviceIdentifier") != null) {
							transId = jsonNodeRec.get("deviceIdentifier").asText();
						}
						
						((ObjectNode) jsonNodeRec).put("ReferenceDate", TimeFormatter.getInstance().getCurrentUTCTimeInStringFmt());
						System.out.println("In Bosch message processing , received TCU msg :: "+jsonNodeRec);
					}

					rowKey = transId + "_" + vin + "_" + TimeFormatter.getInstance().getCurrentUTCTime();
					value.setValue(JsonMapper.configuring().writeValueAsString(jsonNodeRec));

				} catch (Exception e) {
					rowKey = "UnknownMessage" + "_" + TimeFormatter.getInstance().getCurrentUTCTime();
					recordType = DAFCT2Constant.UNKNOWN_DATA;
				}

				value.setKey(rowKey);
				return new Tuple2<Integer, KafkaRecord<String>>(recordType, value);
			}
		});
	}

}
