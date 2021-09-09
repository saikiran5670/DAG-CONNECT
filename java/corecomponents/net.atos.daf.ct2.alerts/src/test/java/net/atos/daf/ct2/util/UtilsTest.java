package net.atos.daf.ct2.util;

import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.kafka.AlertCdc;
import net.atos.daf.ct2.models.kafka.CdcPayloadWrapper;
import net.atos.daf.ct2.models.process.Target;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.table.planner.expressions.In;
import org.junit.Assert;
import org.junit.Test;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.time.*;
import java.time.format.DateTimeFormatter;
import java.util.*;
import java.util.stream.Collectors;

import static java.time.DayOfWeek.TUESDAY;
import static net.atos.daf.ct2.props.AlertConfigProp.*;
import static net.atos.daf.ct2.util.Utils.*;
import static org.junit.Assert.*;

public class UtilsTest {

    @Test
    public void round() {
        Utils.round(2.345654, 2);
        Assert.assertEquals(2.34,Utils.round(2.345654, 2),0.1);
    }

    @Test
    public void writeValueAsString() {
        Alert alert = Alert
                .builder()
                .tripid("tripId")
                .urgencyLevelType("C")
                .type("L")
                .alertid("id")
                .build();

        Assert.assertNotNull(Utils.writeValueAsString(alert));
    }

    @Test
    public void readValueAsObject() throws Exception {
        Alert alert = Alert
                .builder()
                .tripid("tripId")
                .urgencyLevelType("C")
                .type("L")
                .alertid("id")
                .build();
        String string = Utils.writeValueAsString(alert);
        Alert asObject =(Alert)Utils.readValueAsObject(string, Alert.class);
        Assert.assertEquals(alert.getAlertid(),asObject.getAlertid());
        Assert.assertEquals(alert.getTripid(),asObject.getTripid());
        Assert.assertEquals(alert.getType(),asObject.getType());
    }

    @Test
    public void testReadValueAsObject() throws Exception {
        String cdcPayload = "{\n" +
                "  \"schema\": \"master.vehiclealertref\",\n" +
                "  \"payload\": \"{\\\"alertId\\\":506,\\\"vinOps\\\":[{\\\"vin\\\":\\\"XLR0998HGFFT80000\\\",\\\"op\\\":\\\"I\\\"}]}\",\n" +
                "  \"operation\": \"A\",\n" +
                "  \"namespace\": \"alerts\",\n" +
                "  \"timeStamp\": 1629365824563\n" +
                "}";
        CdcPayloadWrapper cdc=(CdcPayloadWrapper) Utils.readValueAsObject(cdcPayload, CdcPayloadWrapper.class);
        System.out.println(cdc);

        if (cdc.getNamespace().equalsIgnoreCase("alerts")){
            AlertCdc alertCdc= (AlertCdc) Utils.readValueAsObject(cdc.getPayload(), AlertCdc.class);
            alertCdc.setOperation(cdc.getOperation());
            System.out.println(alertCdc);
        }
    }

    @Test
    public void getKafkaConnectProperties() throws IOException {
        File resourcesFile = new File("src/test/resources/application-local-test.properties");
        ParameterTool parameterTool = ParameterTool.fromPropertiesFile(resourcesFile);
        Properties properties = Utils.getKafkaConnectProperties(parameterTool);
        Assert.assertNotNull(properties.getProperty(KAFKA_GRP_ID));
        Assert.assertNotNull(properties.getProperty(KAFKA_BOOTSTRAP_SERVER));
        Assert.assertNotNull(properties.getProperty("request.timeout.ms"));

    }


    @Test
    public void drivingTimeTest() throws Exception {
        SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'", Locale.getDefault());

        String gpsStartDateTime = "2021-03-28T12:00:00.000Z";
        String gpsEndDateTime   = "2021-03-28T12:08:30.000Z";

        LocalDateTime endTime = LocalDateTime.parse(gpsEndDateTime, formatter);
        LocalDateTime startTime = LocalDateTime.parse(gpsStartDateTime, formatter);
        Duration duration = Duration.between(startTime, endTime);

        long diffBetweenDatesInSeconds = timeDiffBetweenDates(gpsStartDateTime, gpsEndDateTime);
        long diffSeconds = diffBetweenDatesInSeconds - 0;
        System.out.println(diffSeconds);
        if (diffSeconds > 28800) {
            System.out.println("Alert found");
        }

       /* String currentTime = Utils.convertMillisecondToDateTime(System.currentTimeMillis());
        System.out.println("current time : "+currentTime);
        String gpsStartDateTime = "2021-09-07T08:40:39.555Z";
        long eventTimeInMillis = Utils.convertDateToMillis(gpsStartDateTime);
        long eventTimeInSeconds = Utils.millisecondsToSeconds(eventTimeInMillis);
        long fromTimeInSeconds = Utils.millisecondsToSeconds(System.currentTimeMillis()) - 28800L;
        long endTimeInSeconds = Utils.millisecondsToSeconds(System.currentTimeMillis());
        if(eventTimeInSeconds > fromTimeInSeconds && eventTimeInSeconds <= endTimeInSeconds) {
            System.out.println("Critical");
        }*/
    }

    @Test
    public void setTimeFormat(){
        SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
        String pattern = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'";
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern(pattern, Locale.getDefault());
        Date date = new Date();
        String format1 = format.format(date);
        System.out.println(format1);

        //Returns: The day-of-week, from 1 (Monday) to 7 (Sunday)
        String value = LocalDate.now().getDayOfWeek().name();
        System.out.println("current day of week :: "+value);
        String weekArr = "0001000";
        String dbWeekArr[] = new String[]{"SUNDAY","MONDAY","TUESDAY","WEDNESDAY","THURSDAY","FRIDAY","SATURDAY"};
        String dayWeek="";
        for(int i=0;i < weekArr.length();i++){
            if(weekArr.charAt(i)=='1'){
                dayWeek = dbWeekArr[i];
                break;
            }
        }
        System.out.println(" "+dayWeek);
        long second = System.currentTimeMillis()/1000;
        System.out.println(" current time in seconds "+second);

       // DateTimeFormatter dtf = DateTimeFormatter.ofPattern("HH:mm:ss");
        LocalTime localTime = LocalTime.now();
      //  System.out.println(dtf.format(localTime));
        System.out.println(localTime.toSecondOfDay());

    }

    @Test
    public void duplicateCheck(){
        List<Integer> lst = new ArrayList<>();
        lst.add(1);

        System.out.println("SIZE:: "+lst.size());
        System.out.println("mod op:: "+21%2);

        for(int i=1; i < lst.size(); i++){
            System.out.println(lst.get(i-1)+" :: "+lst.get(i));
        }
    }

    @Test
    public void flatMapTest(){
        Target target = Target.builder()
                .payload(
                        Optional.of(Arrays.asList(Arrays.asList(1, 2, 3, 4, 5, 6, 7, 8, 9),
                        Arrays.asList(1, 2, 3, 4, 5, 6, 7, 8, 9))))
        .build();
        List<List<Integer>> lst = (List<List<Integer>>) target.getPayload()
                .get();

        List<Integer> collect = lst.stream().flatMap(integers -> integers.stream().map(i -> i))
                .collect(Collectors.toList());

        System.out.println(lst);
        System.out.println(collect);

    }
}