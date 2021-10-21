package net.atos.daf.ct2.models;

import lombok.*;

import java.util.Objects;

@ToString
@NoArgsConstructor
@AllArgsConstructor
@Builder
@Setter
@Getter
public class LandMarkDetails {
    private Integer landMarkId;
    private String landMarkType;
    private Boolean isInside=Boolean.FALSE;

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        LandMarkDetails that = (LandMarkDetails) o;
        return landMarkId.equals(that.landMarkId);
    }

    @Override
    public int hashCode() {
        return Objects.hash(landMarkId);
    }
}
