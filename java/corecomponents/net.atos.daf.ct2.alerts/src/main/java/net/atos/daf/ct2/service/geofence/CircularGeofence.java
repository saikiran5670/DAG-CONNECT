package net.atos.daf.ct2.service.geofence;

import static java.lang.Math.cos;
import static java.lang.Math.pow;

public class CircularGeofence {

    /** Steps
     * 1. φ0 - Retrieve latitude of centre of circular geofence from geofence definition – we will store this with definition later.
     * 2. r - Radius of earth sphere - 6,378 km as per https://en.wikipedia.org/wiki/Earth_radius
     * 3. λ – longitude of point under consideration
     * 4. φ – latitude of point under consideration
     * 5. calculate - x = r λ cos(φ0)
     * 6. calculate y = r φ
     * 7. calculate x0 = r * longitude of centre *cos( latitude of centre )
     * 8. calculate y0 = r *φ0
     * 9. r^2 < (x0 - x)^2 + (y0 - y)^2
     */

    public static final  double RADIUS_OF_EARTH = 6378 * 1000; //KM  -> meter
    private static final int EARTH_RADIUS = 6378;

    public static Boolean isInside(double[] centerOfCircle, double[] point, double radiusOfCircle){

        double φ0=centerOfCircle[0];
        double centerLongitude=centerOfCircle[1];

        double λ = point[1];
        double φ = point[0];

        double x = RADIUS_OF_EARTH * λ * cos(φ0);
        double y = RADIUS_OF_EARTH * φ;

        double x0 = RADIUS_OF_EARTH * centerLongitude * cos(φ0);
        double y0 = RADIUS_OF_EARTH * φ0;

        double radiusPower = pow(radiusOfCircle, 2);   // radius of circle
        double distanceInMeter = (pow((x0-x),2) + pow((y0-y),2));

        return radiusPower > distanceInMeter;
    }

    /**
     * <p>
     * Method using the Haversine formula to calculate the great-circle distance
     * between tow points by the latitude and longitude coordinates.</p>
     *
     * @param startLati Initial latitude
     * @param startLong Initial longitude
     * @param endLati Final latitude
     * @param endLong Final longitude
     * @return The distance in Kilometers (Km)
     */
    public static double distanceInKm(Double startLati, Double startLong, Double endLati, Double endLong) {

        double diffLati = Math.toRadians(endLati - startLati);
        double diffLong = Math.toRadians(endLong - startLong);
        /**
         * At this point are possible to improve the resources' utilization by
         * assign the new results inside the existing variables, like startLati
         * and endLati.
         */
        double radiusStartLati = Math.toRadians(startLati);
        double radiusEndLati = Math.toRadians(endLati);

        // A and C are the 'sides' from the spherical triangle.
        double a = Math.pow(Math.sin(diffLati / 2), 2)
                + Math.pow(Math.sin(diffLong / 2), 2) * Math.cos(radiusStartLati)
                * Math.cos(radiusEndLati);
        double c = 2 * Math.asin(Math.sqrt(a));

        return EARTH_RADIUS * c;
    }

    public static Boolean isInsideByHaversine(Double centerOfCircle[],Double[] point,Double radiusOfCircle){
        Double distanceInMeter = distanceInKm(centerOfCircle[0], centerOfCircle[1], point[0], point[1]) * 1000;
        return radiusOfCircle > distanceInMeter;
    }
}
