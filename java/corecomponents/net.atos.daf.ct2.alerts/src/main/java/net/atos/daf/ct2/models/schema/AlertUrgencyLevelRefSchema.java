package net.atos.daf.ct2.models.schema;

import com.fasterxml.jackson.annotation.*;
import lombok.*;

import java.io.Serializable;
import java.util.HashMap;
import java.util.Map;
import java.util.Objects;


@ToString
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class AlertUrgencyLevelRefSchema implements Comparable<AlertUrgencyLevelRefSchema>, Serializable{

    private Long alertId;
    private String urgencyLevelType;
    private Long thresholdValue;
    private String unitType;
    private String alertCategory;
    private String alertType;
    private String alertState;
    private String periodType;
    private String dayTypeArray;
    private Long   startTime;
    private Long   endTime;

    private Long timestamp = System.currentTimeMillis();

    private final static long serialVersionUID = 3317314917107066075L;

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        AlertUrgencyLevelRefSchema schema = (AlertUrgencyLevelRefSchema) o;
        return alertId.equals(schema.alertId) && urgencyLevelType.equals(schema.urgencyLevelType) && thresholdValue.equals(schema.thresholdValue) && unitType.equals(schema.unitType) && alertCategory.equals(schema.alertCategory) && alertType.equals(schema.alertType) && alertState.equals(schema.alertState) && periodType.equals(schema.periodType) && dayTypeArray.equals(schema.dayTypeArray) && startTime.equals(schema.startTime) && endTime.equals(schema.endTime);
    }

    @Override
    public int hashCode() {
        return Objects.hash(alertId, urgencyLevelType, thresholdValue, unitType, alertCategory, alertType, alertState, periodType, dayTypeArray, startTime, endTime);
    }

    @Override
    public int compareTo(AlertUrgencyLevelRefSchema o) {
        return this.getUrgencyLevelType().compareTo(o.getUrgencyLevelType());
    }
}