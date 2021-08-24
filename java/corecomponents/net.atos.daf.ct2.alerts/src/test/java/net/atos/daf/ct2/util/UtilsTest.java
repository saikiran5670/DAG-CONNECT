package net.atos.daf.ct2.util;

import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.kafka.AlertCdc;
import net.atos.daf.ct2.models.kafka.CdcPayloadWrapper;
import org.apache.flink.api.java.utils.ParameterTool;
import org.junit.Assert;
import org.junit.Test;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.time.Duration;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.Date;
import java.util.Locale;
import java.util.Properties;

import static net.atos.daf.ct2.props.AlertConfigProp.*;
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
        String gpsEndDateTime = "2021-04-28T02:49:47.000Z";
        String gpsStartDateTime = "2021-03-28T02:38:15.000Z";

        LocalDateTime endTime = LocalDateTime.parse(gpsEndDateTime, formatter);
        LocalDateTime startTime = LocalDateTime.parse(gpsStartDateTime, formatter);
        Duration duration = Duration.between(startTime, endTime);
        System.out.println(duration.getSeconds());
    }
}