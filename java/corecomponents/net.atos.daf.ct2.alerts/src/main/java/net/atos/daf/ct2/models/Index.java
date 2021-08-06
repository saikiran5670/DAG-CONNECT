package net.atos.daf.ct2.models;

import lombok.*;

import java.io.Serializable;

@Builder
@Getter
@Setter
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class Index implements Serializable {

    private String vin;
    private String gpsLat;
    private String gpslong;
}
