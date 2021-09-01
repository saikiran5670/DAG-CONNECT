package net.atos.daf.ct2.util;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.pojo.standard.Index;
import org.apache.flink.api.java.utils.ParameterTool;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.math.BigDecimal;
import java.math.RoundingMode;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.time.*;
import java.time.format.DateTimeFormatter;
import java.util.Date;
import java.util.Locale;
import java.util.Objects;
import java.util.Properties;

public class Utils implements Serializable {
    private static final long serialVersionUID = -2623908626314058510L;
    private static final Logger logger = LoggerFactory.getLogger(Utils.class);
    private static ObjectMapper mapper;
    private static final String MSG_EVT_DATE_FORMAT="yyyy-MM-dd'T'HH:mm:ss.SSS'Z'";
    private static final SimpleDateFormat sdf = new SimpleDateFormat(MSG_EVT_DATE_FORMAT);
    private static final DateTimeFormatter formatter = DateTimeFormatter.ofPattern(MSG_EVT_DATE_FORMAT, Locale.getDefault());
    public static final String DB_WEEK_ARR[] = new String[]{"SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY"};
    public static final DateTimeFormatter TIME_FORMATTER = DateTimeFormatter.ofPattern("HH:mm:ss");

    static {
        mapper = new ObjectMapper();
        mapper.setSerializationInclusion(JsonInclude.Include.NON_NULL);
    }

    public static double round(double value, int places) {
        if (places < 0) throw new IllegalArgumentException();
        BigDecimal bd = BigDecimal.valueOf(value);
        bd = bd.setScale(places, RoundingMode.HALF_UP);
        return bd.doubleValue();
    }

    public static String writeValueAsString(Object obj) {
        try {
            return mapper.writeValueAsString(obj);
        } catch (JsonProcessingException e) {
            return "";
        }
    }

    public static Object readValueAsObject(String json, Class clazz) throws Exception {
        try {
            return mapper.readValue(json, clazz);
        } catch (JsonProcessingException e) {
            return clazz.getDeclaredConstructor().newInstance();
        }
    }

    public static Object readValueAsObject(String json, TypeReference clazz) throws Exception {
        try {
            return mapper.readValue(json, clazz);
        } catch (JsonProcessingException e) {
            return null;
        }
    }

    public static Properties getKafkaConnectProperties(ParameterTool parameterTool) {
        Properties kafkaTopicProp = new Properties();
        kafkaTopicProp.put("request.timeout.ms", parameterTool.get("request.timeout.ms"));
        kafkaTopicProp.put("client.id", parameterTool.get("client.id"));
        kafkaTopicProp.put("auto.offset.reset", parameterTool.get("auto.offset.reset"));
        kafkaTopicProp.put("group.id", parameterTool.get("group.id"));
        kafkaTopicProp.put("bootstrap.servers", parameterTool.get("bootstrap.servers"));
        if (Objects.nonNull(parameterTool.get("security.protocol")))
            kafkaTopicProp.put("security.protocol", parameterTool.get("security.protocol"));
        if (Objects.nonNull(parameterTool.get("sasl.jaas.config")))
            kafkaTopicProp.put("sasl.jaas.config", parameterTool.get("sasl.jaas.config"));
        if (Objects.nonNull(parameterTool.get("sasl.mechanism")))
            kafkaTopicProp.put("sasl.mechanism", parameterTool.get("sasl.mechanism"));
        return kafkaTopicProp;
    }

    public static long timeDiffBetweenDates(String startDateTime, String endDateTime, DateTimeFormatter formatter) {
        LocalDateTime endTime = LocalDateTime.parse(endDateTime, formatter);
        LocalDateTime startTime = LocalDateTime.parse(startDateTime, formatter);
        Duration duration = Duration.between(startTime, endTime);
        return duration.getSeconds();
    }

    public static long timeDiffBetweenDates(String startDateTime, String endDateTime) {
        return timeDiffBetweenDates(startDateTime, endDateTime, formatter);
    }

    public static long convertDateToMillis(String dateTime) {
        LocalDateTime localTime = LocalDateTime.parse(dateTime, formatter);
        return localTime.atZone(ZoneId.systemDefault()).toInstant().toEpochMilli();
    }

    public static String convertMillisecondToDateTime(Long timeInMillis) {
        LocalDateTime ldt = LocalDateTime.ofInstant(Instant.ofEpochMilli(timeInMillis), ZoneId.systemDefault());
        return ldt.format(formatter);
    }

    public static long millisecondsToSeconds(long milliseconds) {
        // Convert milliseconds to seconds
        long seconds_difference = (milliseconds / 1000) % 60;
        return seconds_difference;
    }

    /***
     * Return name of day in week
     * From MONDAY to SUNDAY
     */
    public static String getCurrentDayOfWeek() {
        return LocalDate.now().getDayOfWeek().name();
    }

    /**
     * Convert db week array to day in week
     * return name of day in week
     */
    public static String getDayOfWeekFromDbArr(String dbWeekArray) {
        String dayWeek = "";
        for (int i = 0; i < dbWeekArray.length(); i++) {
            if (dbWeekArray.charAt(i) == '1') {
                dayWeek = DB_WEEK_ARR[i];
                break;
            }
        }
        return dayWeek;
    }

    /**
     * Extracts the time as seconds of day, from 0 to 24 * 60 * 60 - 1.
     * Returns: the second-of-day equivalent to this time
     */
    public static int getCurrentTimeInSecond() {
        LocalTime localTime = LocalTime.now();
        return localTime.toSecondOfDay();
    }

    public static Long calculateAverage(Index index1, Index index2) {
        Long odometerDiff = index2.getVDist() - index1.getVDist();
        Long avgValue = 0L;
        try {
            /**
             * Time diff in seconds
             */
            Long timeDiff = ((TimeFormatter.getInstance().convertUTCToEpochMilli(index2.getEvtDateTime().toString(),
                    MSG_EVT_DATE_FORMAT)) - (TimeFormatter.getInstance().convertUTCToEpochMilli(index1.getEvtDateTime().toString(), MSG_EVT_DATE_FORMAT)))
                    / 1000;  // for converting milliseconds to seconds
            logger.trace("time diff :" + timeDiff);
            logger.trace("odometerDiff :" + odometerDiff);
            if (timeDiff > 0) {
                avgValue = odometerDiff / timeDiff;
                logger.trace("average :" + odometerDiff / timeDiff);
            }

        } catch (Exception e) {
            logger.error("Error while calculation avg calculation error:: {} index1:: {}, index2 ::{}", e, index1,index2);
        }
        return avgValue;

    }
}
