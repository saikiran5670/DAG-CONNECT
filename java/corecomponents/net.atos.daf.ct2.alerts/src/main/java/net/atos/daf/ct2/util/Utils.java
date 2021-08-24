package net.atos.daf.ct2.util;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.apache.flink.api.java.utils.ParameterTool;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.math.BigDecimal;
import java.math.RoundingMode;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.time.Duration;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.Date;
import java.util.Locale;
import java.util.Objects;
import java.util.Properties;

public class Utils implements Serializable {
    private static final long serialVersionUID = -2623908626314058510L;
    private static final Logger logger = LoggerFactory.getLogger(Utils.class);
    private static ObjectMapper mapper;
    private static final SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
    private static final DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'", Locale.getDefault());
    static {
        mapper = new ObjectMapper();
    }
    public static double round(double value, int places) {
        if (places < 0) throw new IllegalArgumentException();
        BigDecimal bd = BigDecimal.valueOf(value);
        bd = bd.setScale(places, RoundingMode.HALF_UP);
        return bd.doubleValue();
    }

    public static String writeValueAsString(Object obj){
        try {
            return mapper.writeValueAsString(obj);
        } catch (JsonProcessingException e) {
            return "";
        }
    }

    public static Object readValueAsObject(String json, Class clazz) throws Exception {
        try {
            return mapper.readValue(json,clazz);
        } catch (JsonProcessingException e) {
            return clazz.getDeclaredConstructor().newInstance();
        }
    }

    public static Object readValueAsObject(String json, TypeReference clazz) throws Exception {
        try {
            return mapper.readValue(json,clazz);
        } catch (JsonProcessingException e) {
            return null;
        }
    }

    public static Properties getKafkaConnectProperties(ParameterTool parameterTool){
        Properties kafkaTopicProp = new Properties();
        kafkaTopicProp.put("request.timeout.ms", parameterTool.get("request.timeout.ms"));
        kafkaTopicProp.put("client.id", parameterTool.get("client.id"));
        kafkaTopicProp.put("auto.offset.reset", parameterTool.get("auto.offset.reset"));
        kafkaTopicProp.put("group.id", parameterTool.get("group.id"));
        kafkaTopicProp.put("bootstrap.servers", parameterTool.get("bootstrap.servers"));
        if(Objects.nonNull(parameterTool.get("security.protocol")))
          kafkaTopicProp.put("security.protocol", parameterTool.get("security.protocol"));
        if(Objects.nonNull(parameterTool.get("sasl.jaas.config")))
          kafkaTopicProp.put("sasl.jaas.config", parameterTool.get("sasl.jaas.config"));
        if(Objects.nonNull(parameterTool.get("sasl.mechanism")))
           kafkaTopicProp.put("sasl.mechanism", parameterTool.get("sasl.mechanism"));
        return kafkaTopicProp;
    }

    public static long timeDiffBetweenDates(String startDateTime, String endDateTime,DateTimeFormatter formatter) {
        LocalDateTime endTime = LocalDateTime.parse(endDateTime, formatter);
        LocalDateTime startTime = LocalDateTime.parse(startDateTime, formatter);
        Duration duration = Duration.between(startTime, endTime);
        return duration.getSeconds();
    }
    public static long timeDiffBetweenDates(String startDateTime,String endDateTime) {
        return timeDiffBetweenDates(startDateTime,endDateTime,formatter);
    }

    public static long millisecondsToSeconds(long milliseconds){
        // Convert milliseconds to seconds
        long seconds_difference = (milliseconds / 1000)% 60;
        return seconds_difference;
    }



}
