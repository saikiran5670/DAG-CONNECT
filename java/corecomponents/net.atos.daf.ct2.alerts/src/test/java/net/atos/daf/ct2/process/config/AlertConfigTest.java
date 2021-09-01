package net.atos.daf.ct2.process.config;

import lombok.NonNull;
import net.atos.daf.ct2.models.MetaData;
import net.atos.daf.ct2.models.process.Message;
import net.atos.daf.ct2.models.process.Target;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.process.service.AlertLambdaExecutor;
import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;

import java.util.*;

import static net.atos.daf.ct2.process.functions.LogisticAlertFunction.excessiveGlobalMileage;
import static org.junit.Assert.*;

public class AlertConfigTest {

    private Status status;
    private Map<Object, Object> configMap;
    private Map<String, Object> functionThresh = new HashMap<>();

    @Before
    public void init() throws Exception{
        configMap = new HashMap() {{
            put("functions", Arrays.asList(
                    excessiveGlobalMileage
            ));
        }};
        Set<AlertUrgencyLevelRefSchema> refSchemas = new HashSet<>();

        AlertUrgencyLevelRefSchema urgencyLevelRefSchema = AlertUrgencyLevelRefSchema.builder().build();
        urgencyLevelRefSchema.setAlertId(123L);
        urgencyLevelRefSchema.setUrgencyLevelType("A");
        urgencyLevelRefSchema.setAlertCategory("L");
        urgencyLevelRefSchema.setUnitType("G");
        urgencyLevelRefSchema.setThresholdValue(1500.0);

        refSchemas.add(urgencyLevelRefSchema);
        functionThresh.put("excessiveGlobalMileage", refSchemas);

        status = new Status();
        status.setVin("abc");
        status.setVid("vid");
        status.setGpsStopVehDist(2000L);
        status.setGpsStartVehDist(1000L);
    }

    @Test
    public void buildMessage() {

        Message message=AlertConfig
                .buildMessage(status, configMap, functionThresh);

        Assert.assertNotNull(message);

        MetaData metaData = message.getMetaData();
        Map<Object, Object> config = metaData.getConfig().get();
        List functions = (List) config.get("functions");
        AlertLambdaExecutor<Message, Target> excessiveDistanceDone = (AlertLambdaExecutor<Message, Target>) functions.get(0);
        Assert.assertNotNull(excessiveDistanceDone);

        Status status1 = (Status)message.getPayload().get();
        Assert.assertEquals("abc",status1.getVin());
        Assert.assertEquals("vid",status1.getVid());
        Assert.assertEquals(Optional.of(2000L),Optional.of(status1.getGpsStopVehDist()));
    }

    @Test
    public void testBuildMessage() {
        Message message=AlertConfig
                .buildMessage(status, configMap);
        Assert.assertNotNull(message);

        MetaData metaData = message.getMetaData();
        Map<Object, Object> config = metaData.getConfig().get();
        List functions = (List) config.get("functions");
        AlertLambdaExecutor<Message, Target> excessiveDistanceDone = (AlertLambdaExecutor<Message, Target>) functions.get(0);
        Assert.assertNotNull(excessiveDistanceDone);

        Status status1 = (Status)message.getPayload().get();
        Assert.assertEquals("abc",status1.getVin());
        Assert.assertEquals("vid",status1.getVid());
        Assert.assertEquals(Optional.of(2000L),Optional.of(status1.getGpsStopVehDist()));

        Assert.assertEquals(Optional.empty(),message.getMetaData().getThreshold());
    }
}