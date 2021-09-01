package net.atos.ct2.kafka;

import org.junit.Test;

import java.text.SimpleDateFormat;
import java.time.Duration;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.Locale;

public class UtilTest {

    @Test
    public void drivingTimeTest() throws Exception {
        SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'", Locale.getDefault());

        String gpsStartDateTime = "2021-03-28T01:38:00.000Z";
        String gpsEndDateTime   = "2021-03-28T01:38:56.000Z";

        LocalDateTime endTime = LocalDateTime.parse(gpsEndDateTime, formatter);
        LocalDateTime startTime = LocalDateTime.parse(gpsStartDateTime, formatter);
        Duration duration = Duration.between(startTime, endTime);
        System.out.println(duration.getSeconds());
    }
}
