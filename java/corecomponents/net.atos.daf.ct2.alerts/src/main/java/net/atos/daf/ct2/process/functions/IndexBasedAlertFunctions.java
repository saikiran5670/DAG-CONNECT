package net.atos.daf.ct2.process.functions;

import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.process.Message;
import net.atos.daf.ct2.models.process.Target;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.process.service.AlertLambdaExecutor;
import net.atos.daf.ct2.util.Utils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.Optional;

import static net.atos.daf.ct2.util.Utils.*;

public class IndexBasedAlertFunctions implements Serializable {
    private static final long serialVersionUID = -2623908626314058510L;
    private static final Logger logger = LoggerFactory.getLogger(IndexBasedAlertFunctions.class);

    public static AlertLambdaExecutor<Message, Target> hoursOfServiceFun = (Message s) -> {
        Index index = (Index) s.getPayload().get();
        Map<String,Object> threshold = (Map<String, Object>) s.getMetaData().getThreshold().get();
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas = (List<AlertUrgencyLevelRefSchema>) threshold.get("hoursOfService");

        List<String> priorityList = Arrays.asList("C", "W", "A");

        for(String priority : priorityList){
            for(AlertUrgencyLevelRefSchema schema : urgencyLevelRefSchemas){
                if(schema.getUrgencyLevelType().equalsIgnoreCase(priority)){
                    String currentDayOfWeek = getCurrentDayOfWeek();
                    String dayOfWeekFromDbArr = getDayOfWeekFromDbArr(schema.getDayTypeArray());
                    if(currentDayOfWeek.equalsIgnoreCase(dayOfWeekFromDbArr)){
                        if(schema.getPeriodType().equalsIgnoreCase("A")){
                            return getTarget(index, schema,index.getEvtDateTime());
                        }
                        if(schema.getPeriodType().equalsIgnoreCase("C")){
                            int currentTimeInSecond = getCurrentTimeInSecond();
                            if(schema.getStartTime() <= currentTimeInSecond && schema.getEndTime() > currentTimeInSecond){
                                return getTarget(index, schema,index.getEvtDateTime());
                            }
                        }
                    }
                }
            }
        }

        return Target.builder().metaData(s.getMetaData()).payload(s.getPayload()).alert(Optional.empty()).build();
    };


    private static Target getTarget(Index index, AlertUrgencyLevelRefSchema urgency, Object valueAtAlertTime) {
        return Target.builder()
                .alert(Optional.of(Alert.builder()
                        .tripid(index.getDocument() !=null ? index.getDocument().getTripID() : "")
                        .vin(index.getVin())
                        .categoryType(urgency.getAlertCategory())
                        .type(urgency.getAlertType())
                        .alertid("" + urgency.getAlertId())
                        .alertGeneratedTime(String.valueOf(System.currentTimeMillis()))
                        .thresholdValue("" + urgency.getThresholdValue())
                        .thresholdValueUnitType(urgency.getUnitType())
                        .valueAtAlertTime(""+valueAtAlertTime)
                        .urgencyLevelType(urgency.getUrgencyLevelType())
                        .build()))
                .build();
    }
}
