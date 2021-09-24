package net.atos.daf.ct2.service.geofence;

import junit.framework.TestCase;

public class RayCastingTest extends TestCase {

    public void testIsInside() {

        Double [][] polygonPoints = {
                {49.6594544,3.0665322},
                {53.1121239,16.4257716},
                {49.31089,21.35733},
                {48.5647,9.65262},
        };

        Double point[] ={49.31089,21.35733};
        Boolean inside = RayCasting.isInside(polygonPoints, point);
        System.out.println(inside);

        Double point2[] ={47.44836,23.98412};
        inside = RayCasting.isInside(polygonPoints, point2);
        System.out.println(inside);
    }
}