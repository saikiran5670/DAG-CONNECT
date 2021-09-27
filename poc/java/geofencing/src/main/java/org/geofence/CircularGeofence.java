package org.geofence;

public class CircularGeofence {
    /**
     * Steps
     * 1.	φ0 - Retrieve latitude of centre of circular geofence from geofence definition – we will store this with definition later.
     * 2.	r  - Radius of earth sphere - 6,378 km as per  https://en.wikipedia.org/wiki/Earth_radius
     * 3.	λ – longitude of point under consideration
     * 4.	φ – latitude of point under consideration
     * 5.	calculate   - x = r λ cos(φ0)
     * 6.	calculate y = r φ
     * 7.	calculate x0  = r (longitude of centre of geofence) cos(φ0)
     * 8.	calculate y0 = r (latitude of centre of geofence- φ0 -)
     * 9.	r^2 < (x0 - x)^2 + (y0 - y)^2
     * 10.	above equation result is smaller than radius then it’s inside else outside if equal to then on circumference.
     * @return
     */

    public static final  double RADIUS_OF_EARTH = 6378;

    public static Boolean isInside(double[] centerOfCircle, double[] point){
        double centerLatitude=centerOfCircle[0];
        double x = RADIUS_OF_EARTH * point[1] * Math.cos(centerLatitude);
        double y = RADIUS_OF_EARTH * point[0];

        double x0 = RADIUS_OF_EARTH ;

        return Boolean.FALSE;
    }
}
