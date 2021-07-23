package net.atos.daf.ct2.models.scheamas;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.ToString;
import org.codehaus.jackson.annotate.JsonIgnore;
import org.codehaus.jackson.annotate.JsonProperty;

import java.io.Serializable;
import java.util.Objects;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor
@ToString
@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonIgnoreProperties(ignoreUnknown = true)
public class VehicleStatusSchema implements Serializable {

    private static final long serialVersionUID = 1L;

    @JsonProperty("vin")
    private String vin;
    @JsonProperty("vid")
    private String vid;
    @JsonProperty("status")
    private String status;
    @JsonProperty("fuelType")
    private String fuelType;
    @JsonProperty("fuelTypeCoefficient")
    private Double fuelTypeCoefficient;
    @JsonProperty("operationType")
    private String operationType;
    @JsonProperty("schema")
    private String schema;

    @JsonIgnore
    private Long timeStamp = System.currentTimeMillis();


    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        VehicleStatusSchema that = (VehicleStatusSchema) o;
        return Objects.equals(vin, that.vin) && Objects.equals(vid, that.vid) && Objects.equals(status, that.status) && Objects.equals(fuelType, that.fuelType) && Objects.equals(fuelTypeCoefficient, that.fuelTypeCoefficient);
    }

    @Override
    public int hashCode() {
        return Objects.hash(vin, vid, status, fuelType, fuelTypeCoefficient);
    }
}
