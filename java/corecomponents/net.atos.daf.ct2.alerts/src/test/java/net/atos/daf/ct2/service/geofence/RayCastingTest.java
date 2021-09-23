package net.atos.daf.ct2.service.geofence;

import junit.framework.TestCase;

public class RayCastingTest extends TestCase {

    public void testIsInside() {

        Double [][] polygonPoints = {
                {46.3703,3.33054},
                {45.6978,7.1094},
                {44.9596353,2.5354615}
        };

        Double point[] ={45.6978,7.1094};
        Boolean inside = RayCasting.isInside(polygonPoints, point);
        System.out.println(inside);

        Double point2[] ={47.44836,23.98412};
        inside = RayCasting.isInside(polygonPoints, point2);
        System.out.println(inside);
    }
}