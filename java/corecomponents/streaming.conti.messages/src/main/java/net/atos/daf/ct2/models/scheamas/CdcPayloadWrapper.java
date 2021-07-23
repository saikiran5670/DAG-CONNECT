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

import java.io.Serializable;
import java.util.List;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor
@ToString
@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonIgnoreProperties(ignoreUnknown = true)
public class CdcPayloadWrapper implements Serializable {

    private static final long serialVersionUID = 1L;

    private List<Schema> schema;
    private Object payload;
    private String operation;
    private String namespace;
    @JsonIgnore
    private Long timeStamp = System.currentTimeMillis();
}
