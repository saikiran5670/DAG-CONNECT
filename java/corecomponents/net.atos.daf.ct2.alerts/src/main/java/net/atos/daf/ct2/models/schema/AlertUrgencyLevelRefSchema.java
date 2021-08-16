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
public class AlertUrgencyLevelRefSchema implements Serializable{

    private Long alertId;
    private String urgencyLevelType;
    private Long thresholdValue;
    private String unitType;
    private String alertCategory;
    private String alertType;
    private String alertState;

    private Long timestamp = System.currentTimeMillis();

    private final static long serialVersionUID = 3317314917107066075L;

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        AlertUrgencyLevelRefSchema that = (AlertUrgencyLevelRefSchema) o;
        return alertId.equals(that.alertId);
    }

    @Override
    public int hashCode() {
        return Objects.hash(alertId);
    }
}