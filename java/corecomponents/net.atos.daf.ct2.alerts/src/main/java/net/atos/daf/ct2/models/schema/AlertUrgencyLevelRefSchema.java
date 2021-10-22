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
    private Double thresholdValue;
    private String unitType;
    private String alertCategory;
    private String alertType;
    private String alertState;
    private String periodType;
    private String dayTypeArray;
    private Long   startTime;
    private Long   endTime;
    private Integer nodeSeq;
    private Double latitude;
    private Double longitude;
    private Integer landmarkId;
    private String landMarkType;
    private Double circleLatitude;
    private Double circleLongitude;
    private Double circleRadius;
    private Integer width;

    private Long timestamp = System.currentTimeMillis();

    private final static long serialVersionUID = 3317314917107066075L;

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        AlertUrgencyLevelRefSchema schema = (AlertUrgencyLevelRefSchema) o;
        return Objects.equals(alertId, schema.alertId) && Objects.equals(urgencyLevelType, schema.urgencyLevelType) && Objects.equals(thresholdValue, schema.thresholdValue) && Objects.equals(unitType, schema.unitType) && Objects.equals(alertCategory, schema.alertCategory) && Objects.equals(alertType, schema.alertType) && Objects.equals(alertState, schema.alertState) && Objects.equals(periodType, schema.periodType) && Objects.equals(dayTypeArray, schema.dayTypeArray) && Objects.equals(startTime, schema.startTime) && Objects.equals(endTime, schema.endTime) && Objects.equals(nodeSeq, schema.nodeSeq) && Objects.equals(latitude, schema.latitude) && Objects.equals(longitude, schema.longitude) && Objects.equals(landmarkId, schema.landmarkId) && Objects.equals(landMarkType, schema.landMarkType) && Objects.equals(circleLatitude, schema.circleLatitude) && Objects.equals(circleLongitude, schema.circleLongitude) && Objects.equals(circleRadius, schema.circleRadius) && Objects.equals(width, schema.width);
    }

    @Override
    public int hashCode() {
        return Objects.hash(alertId, urgencyLevelType, thresholdValue, unitType, alertCategory, alertType, alertState, periodType, dayTypeArray, startTime, endTime, nodeSeq, latitude, longitude, landmarkId, landMarkType, circleLatitude, circleLongitude, circleRadius, width);
    }

    @Override
    public int compareTo(AlertUrgencyLevelRefSchema o) {
        return this.getUrgencyLevelType().compareTo(o.getUrgencyLevelType());
    }
}