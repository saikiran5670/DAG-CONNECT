package net.atos.daf.ct2.models;

import lombok.*;

import java.io.Serializable;
import java.util.Objects;

@ToString
@NoArgsConstructor
@AllArgsConstructor
@Builder
@Setter
@Getter
public class VehicleGeofenceState implements Serializable {
    private static final long serialVersionUID = -4683496582524846984L;
    private Integer landMarkId;
    private Boolean isInside=Boolean.FALSE;

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        VehicleGeofenceState that = (VehicleGeofenceState) o;
        return landMarkId.equals(that.landMarkId);
    }

    @Override
    public int hashCode() {
        return Objects.hash(landMarkId);
    }
}
