package net.atos.daf.ct2.models;

import lombok.*;

import java.io.Serializable;
import java.util.Map;
import java.util.Optional;

@Builder
@Getter
@Setter
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class MetaData implements Serializable {
    private static final long serialVersionUID = -4683496582524846984L;

    private Optional<Map<Object,Object>> config;
    private Optional<Object> threshold;

}
