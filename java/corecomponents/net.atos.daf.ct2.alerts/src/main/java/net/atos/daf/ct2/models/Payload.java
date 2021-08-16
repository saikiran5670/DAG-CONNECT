package net.atos.daf.ct2.models;

import lombok.*;

import java.util.HashMap;
import java.util.Map;
import java.util.Optional;

@Builder
@Getter
@Setter
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class Payload<T> {

    private Optional<T> data;
    private Map<String, Object> additionalProperties = new HashMap<String, Object>();
    @Setter(AccessLevel.NONE)
    private final Long createdTimestamp = System.currentTimeMillis();
}
