package net.atos.daf.ct2.models.process;

import lombok.*;
import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.MetaData;

import java.io.Serializable;
import java.util.Optional;

@Builder
@Getter
@Setter
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class Target implements Serializable {
    private static final long serialVersionUID = -8957861647045201073L;

    private MetaData metaData;
    private Optional<Object> payload;
    private Optional<Alert> alert;
}
