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
    private Double corriLatitude;
    private Double corriLongitude;
    private Integer corriSeq;

    private Long timestamp = System.currentTimeMillis();

    private final static long serialVersionUID = 3317314917107066075L;

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        AlertUrgencyLevelRefSchema that = (AlertUrgencyLevelRefSchema) o;
        return Objects.equals(alertId, that.alertId) && Objects.equals(urgencyLevelType, that.urgencyLevelType) && Objects.equals(thresholdValue, that.thresholdValue) && Objects.equals(unitType, that.unitType) && Objects.equals(alertCategory, that.alertCategory) && Objects.equals(alertType, that.alertType) && Objects.equals(alertState, that.alertState) && Objects.equals(periodType, that.periodType) && Objects.equals(dayTypeArray, that.dayTypeArray) && Objects.equals(startTime, that.startTime) && Objects.equals(endTime, that.endTime) && Objects.equals(nodeSeq, that.nodeSeq) && Objects.equals(latitude, that.latitude) && Objects.equals(longitude, that.longitude) && Objects.equals(landmarkId, that.landmarkId) && Objects.equals(landMarkType, that.landMarkType) && Objects.equals(circleLatitude, that.circleLatitude) && Objects.equals(circleLongitude, that.circleLongitude) && Objects.equals(circleRadius, that.circleRadius) && Objects.equals(width, that.width) && Objects.equals(corriLatitude, that.corriLatitude) && Objects.equals(corriLongitude, that.corriLongitude) && Objects.equals(corriSeq, that.corriSeq);
    }

    @Override
    public int hashCode() {
        return Objects.hash(alertId, urgencyLevelType, thresholdValue, unitType, alertCategory, alertType, alertState, periodType, dayTypeArray, startTime, endTime, nodeSeq, latitude, longitude, landmarkId, landMarkType, circleLatitude, circleLongitude, circleRadius, width, corriLatitude, corriLongitude, corriSeq);
    }

    @Override
    public int compareTo(AlertUrgencyLevelRefSchema o) {
        return this.getUrgencyLevelType().compareTo(o.getUrgencyLevelType());
    }
}