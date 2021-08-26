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
        AlertUrgencyLevelRefSchema that = (AlertUrgencyLevelRefSchema) o;
        return Objects.equals(alertId, that.alertId) && Objects.equals(urgencyLevelType, that.urgencyLevelType) && Objects.equals(thresholdValue, that.thresholdValue) && Objects.equals(unitType, that.unitType) && Objects.equals(alertCategory, that.alertCategory) && Objects.equals(alertType, that.alertType) && Objects.equals(alertState, that.alertState);
    }

    @Override
    public int hashCode() {
        return Objects.hash(alertId, urgencyLevelType, thresholdValue, unitType, alertCategory, alertType, alertState);
    }

    @Override
    public int compareTo(AlertUrgencyLevelRefSchema o) {
        return this.getUrgencyLevelType().compareTo(o.getUrgencyLevelType());
    }
}