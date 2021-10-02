package net.atos.daf.ct2.service.geofence;

import org.junit.Assert;
import org.junit.Test;

import static org.junit.Assert.*;

public class CircularGeofenceTest {

    @Test
    public void isInsideByHaversine() {

        Double[] point ={51.423511878846824,5.490974188067415};

        Double centerOfCircle[]={51.42801,5.5333};
        Double radiusOfCircle=750.0;

        Boolean inside = CircularGeofence.isInsideByHaversine(centerOfCircle, point, radiusOfCircle);
        Assert.assertEquals(Boolean.FALSE,inside);

        Double[] insidePoint ={51.4284050844773,5.536781501896497};
        inside = CircularGeofence.isInsideByHaversine(centerOfCircle, insidePoint, radiusOfCircle);
        Assert.assertEquals(Boolean.TRUE,inside);
    }
}