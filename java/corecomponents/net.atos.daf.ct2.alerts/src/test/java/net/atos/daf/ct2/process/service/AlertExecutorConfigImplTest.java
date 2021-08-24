package net.atos.daf.ct2.process.service;

import lombok.NonNull;
import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.process.Message;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.process.config.AlertConfig;
import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;
import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

import static net.atos.daf.ct2.process.functions.LogisticAlertFunction.excessiveDistanceDone;
import static net.atos.daf.ct2.process.functions.LogisticAlertFunction.excessiveGlobalMileage;

public class AlertExecutorConfigImplTest {

    private AlertExecutorConfig<Message> alertConfig;

    @Before
    public void setUp() throws Exception {
        alertConfig = new AlertExecutorConfigImpl();
    }

    @Test
    public void excessiveGlobalMileageTest() {
        Map<Object, Object> configMap = new HashMap() {{
            put("functions", Arrays.asList(
                    excessiveGlobalMileage
            ));
        }};

        Map<String, Object> functionThresh = new HashMap<>();

        Set<AlertUrgencyLevelRefSchema> refSchemas = new HashSet<>();

        AlertUrgencyLevelRefSchema urgencyLevelRefSchema = AlertUrgencyLevelRefSchema.builder().build();
        urgencyLevelRefSchema.setAlertId(123L);
        urgencyLevelRefSchema.setUrgencyLevelType("A");
        urgencyLevelRefSchema.setAlertCategory("L");
        urgencyLevelRefSchema.setUnitType("G");
        urgencyLevelRefSchema.setThresholdValue(1500L);

        refSchemas.add(urgencyLevelRefSchema);
        functionThresh.put("excessiveGlobalMileage", refSchemas);

        @NonNull Status status = new Status();
        status.setVin("abc");
        status.setVid("vid");
        status.setGpsStopVehDist(2000L);
        status.setGpsStartVehDist(1000L);

        Message message=AlertConfig
                .buildMessage(status, configMap, functionThresh);

        Message outputMessage = alertConfig.apply(message);
        Assert.assertNotNull(outputMessage);

        Alert alert = outputMessage.getAlert().get().get(0);
        Assert.assertEquals("abc",alert.getVin());
        Assert.assertEquals("123",alert.getAlertid());
        Assert.assertEquals("2000",alert.getValueAtAlertTime());
        Assert.assertEquals("1500",alert.getThresholdValue());
        Assert.assertEquals("A",alert.getUrgencyLevelType());
        Assert.assertEquals("L",alert.getCategoryType());

    }


    @Test
    public void excessiveDistanceDoneTest(){

        Map<Object, Object> configMap = new HashMap() {{
            put("functions", Arrays.asList(
                    excessiveDistanceDone
            ));
        }};

        Map<String, Object> functionThresh = new HashMap<>();

        Set<AlertUrgencyLevelRefSchema> refSchemas = new HashSet<>();

        AlertUrgencyLevelRefSchema urgencyLevelRefSchema = AlertUrgencyLevelRefSchema.builder().build();
        urgencyLevelRefSchema.setAlertId(123L);
        urgencyLevelRefSchema.setUrgencyLevelType("C");
        urgencyLevelRefSchema.setAlertCategory("L");
        urgencyLevelRefSchema.setAlertType("D");
        urgencyLevelRefSchema.setUnitType("M");
        urgencyLevelRefSchema.setThresholdValue(900L);

        refSchemas.add(urgencyLevelRefSchema);
        functionThresh.put("excessiveDistanceDone", refSchemas);

        @NonNull Status status = new Status();
        status.setVin("abc");
        status.setVid("vid");
        status.setGpsStopVehDist(2000L);
        status.setGpsStartVehDist(1000L);

        Message message=AlertConfig
                .buildMessage(status, configMap, functionThresh);

        Message outputMessage = alertConfig.apply(message);
        Assert.assertNotNull(outputMessage);

        Alert alert = outputMessage.getAlert().get().get(0);
        Assert.assertEquals("abc",alert.getVin());
        Assert.assertEquals("123",alert.getAlertid());
        Assert.assertEquals("2000",alert.getValueAtAlertTime());
        Assert.assertEquals("900",alert.getThresholdValue());
        Assert.assertEquals("C",alert.getUrgencyLevelType());
        Assert.assertEquals("L",alert.getCategoryType());

    }
}