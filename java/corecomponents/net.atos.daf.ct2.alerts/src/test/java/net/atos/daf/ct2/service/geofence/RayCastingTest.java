package net.atos.daf.ct2.service.geofence;

import junit.framework.TestCase;

public class RayCastingTest extends TestCase {

    public void testIsInside() {

        Double [][] polygonPoints = {
                {51.4301963,5.5313448},
                {51.43046,5.53892},
                {51.426998,5.5416274},
                {51.4266886,5.532953},
        };

        Double point[] ={51.42855125796569,5.537232922486132};
        Boolean inside = RayCasting.isInside(polygonPoints, point);
        System.out.println(inside);

        Double point2[] ={51.41659457495235,5.510225269449434};
        inside = RayCasting.isInside(polygonPoints, point2);
        System.out.println(inside);
    }
}