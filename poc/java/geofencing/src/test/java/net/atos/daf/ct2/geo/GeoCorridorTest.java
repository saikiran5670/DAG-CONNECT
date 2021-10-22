package net.atos.daf.ct2.geo;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.junit.AfterClass;
import org.junit.BeforeClass;
import org.junit.Ignore;
import org.junit.Test;
import org.slf4j.LoggerFactory;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static org.junit.Assert.*;

public class GeoCorridorTest {

    /** Test route, linear. */
    private static double[][] route;

    /** Test route, ellipse. */
    private static double[][] route2;

    /** The linear geo corridor to test. */
    private static GeoCorridor instance;

    /** The ellipse geo corridor to test. */
    private static GeoCorridor instance2;

    /**
     * Setup corridor definitions for linear and elliptic case.
     * Use this method so that the instances are created once as opposed to for every test method.
     * @throws Exception in case of error
     */
    /** The logger to use. */
    private static final org.slf4j.Logger log = LoggerFactory.getLogger(GeoCorridorTest.class);

    @BeforeClass
    public static void setUpBeforeClass() throws Exception {

        // route utrecht (52.092876, 5.104480)
        // to eindhoven (51.441642, 5.4697225)
        // = 91.7km with points a minute distance at 80km/h
        int nrPoints = (int) Math.round(92.0 / (80.0 / 60.0));

        List<double[]> points = new ArrayList<>();

        for (int n = 0; n < nrPoints; n++) {
            double[] point = {
                    52.092876 + n * (51.441642 - 52.092876) / nrPoints,
                    5.104480  + n * (5.4697225 - 5.104480) / nrPoints
            };
            points.add(point);
        }
        route = points.toArray(new double[0][0]);
        instance = new GeoCorridor(route, 1);

        // ellipse around eindhoven, center
        // 51.43823985925925, 5.476861544431422
        // dlat  0.011 =~ 1.2km
        // dlong 0.03  =~ 2.1km

        //System.out.println("dlat " + GPSLocation.gpsDistance(51.43823985925925, 5.476861544431422, 51.43823985925925 + 0.011, 5.476861544431422));
        //System.out.println("dlng " + GPSLocation.gpsDistance(51.43823985925925, 5.476861544431422, 51.43823985925925, 5.476861544431422 + 0.03));

        points.clear();

        for (int n = 11; n < 89; n += 3) {
            double[] point = {
                    51.43823985 + 0.011 * Math.sin(2 * Math.PI * n / 100),
                    5.47686154 + 0.03  * Math.cos(2 * Math.PI * n / 100)
            };
            points.add(point);
        }

        route2 = points.toArray(new double[0][0]);
        instance2 = new GeoCorridor(route2, 1);
    }

    @AfterClass
    public static void tearDownAfterClass() throws Exception {
        route = null;
        instance = null;
        route2 = null;
        instance2 = null;
    }

    @Test
    public void testLinearCorridorRoutePoints() {
        log.info("Test data");
//		{ lat: 37.772, lng: -122.214 },
        Arrays.stream(route).forEach(point ->
                log.info("{lat: {}, lng: {} }",point[0],point[1])
        );
        for (double[] point: route) {
            assertTrue(instance.liesWithin(point[0], point[1]));
        }
    }

    @Test
    public void testLinearFarOutside() {

        // outside bounding box
        assertFalse(instance.liesWithin(30, 30));
        assertFalse(instance.liesWithin(51, 5));
        assertFalse(instance.liesWithin(52, 4));
        assertFalse(instance.liesWithin(51.5, 7));
    }

    @Test
    public void testLinearInside() {

        // 1km =~ 0.009 lat, 0.0146 long
        for (double[] point: route) {
            assertTrue(instance.liesWithin(point[0] + 0.008, point[1]));
            assertTrue(instance.liesWithin(point[0] - 0.008, point[1]));
            assertTrue(instance.liesWithin(point[0], point[1] + 0.0144));
            assertTrue(instance.liesWithin(point[0], point[1] - 0.0144));
        }
    }

    @Test
    public void testLinearOutside() {

        // 1km =~ 0.009 lat, 0.0146 long
        for (double[] point: route) {
            assertFalse(instance.liesWithin(point[0] + 0.028, point[1]));
            assertFalse(instance.liesWithin(point[0] - 0.028, point[1]));
            assertFalse(instance.liesWithin(point[0], point[1] + 0.016));
            assertFalse(instance.liesWithin(point[0], point[1] - 0.016));
        }
    }

    @Test
    public void testEllipticCorridorRoutePoints() {

        for (double[] point: route2) {
            assertTrue(instance2.liesWithin(point[0], point[1]));
        }
    }

    @Test
    public void testEllipticFarOutside() {

        // outside bounding box
        assertFalse(instance2.liesWithin(30, 30));
        assertFalse(instance2.liesWithin(51, 5));
        assertFalse(instance2.liesWithin(52, 4));
        assertFalse(instance2.liesWithin(51.5, 7));
    }

    @Test
    public void testEllipticInside() {

        // small deviation from route points
        // 1km =~ 0.009 lat, 0.0146 long
        for (double[] point: route2) {
            assertTrue(instance2.liesWithin(point[0] + 0.008, point[1]));
            assertTrue(instance2.liesWithin(point[0] - 0.008, point[1]));
            assertTrue(instance2.liesWithin(point[0], point[1] + 0.013));
            assertTrue(instance2.liesWithin(point[0], point[1] - 0.013));
        }
    }

    @Test
    public void testEllipticInsideInnerBoundary() {

        // also check time to run
        long now = System.currentTimeMillis();

        // ellipse just inside inner boundary,
        // 1km =~ 0.009 lat, 0.0146 long
        // dlat  0.011 =~ 1.2km
        // dlong 0.03  =~ 2.1km
        for (int n = 12; n < 88; n++) {
            double[] point = {
                    51.43823985 + 0.00209 * Math.sin(2 * Math.PI * n / 100),
                    5.47686154  + 0.01539 * Math.cos(2 * Math.PI * n / 100)
            };
            assertTrue("n=" + n, instance2.liesWithin(point[0], point[1]));
        }

        long ms = System.currentTimeMillis() - now;
        assertTrue("Elliptic inside inner took too long ms=" + ms, ms < 7);
    }

    @Test
    public void testEllipticInsideOuterBoundary() {

        // also check time to run
        long now = System.currentTimeMillis();

        // ellipse just inside outer boundary,
        // 1km =~ 0.009 lat, 0.0146 long
        // dlat  0.011 =~ 1.2km
        // dlong 0.03  =~ 2.1km
        for (int n = 12; n < 88; n++) {
            double[] point = {
                    51.43823985 + 0.0195 * Math.sin(2 * Math.PI * n / 100),
                    5.47686154  + 0.0441 * Math.cos(2 * Math.PI * n / 100)
            };
            assertTrue("n=" + n, instance2.liesWithin(point[0], point[1]));
        }

        long ms = System.currentTimeMillis() - now;
        assertTrue("Elliptic inside outer took too long ms=" + ms, ms < 7);
    }

    @Test
    public void testEllipticOutside() {

        // center of ellipse
        assertFalse(instance2.liesWithin(51.44, 5.48));

        // also check time to run
        long now = System.currentTimeMillis();

        // ellipse further outside,
        // dlat  0.02 =~ 2.2km
        // dlong 0.05 =~ 3.4km
        for (int n = 1; n < 100; n++) {
            double[] point = {
                    51.43823985 + 0.021 * Math.sin(2 * Math.PI * n / 100),
                    5.47686154  + 0.05  * Math.cos(2 * Math.PI * n / 100)
            };
            assertFalse("n=" + n, instance2.liesWithin(point[0], point[1]));
        }

        long ms = System.currentTimeMillis() - now;
        assertTrue("Elliptic outside took too long ms=" + ms, ms < 30);
    }

    @Test
    @Ignore
    public void testHugeCorridor() {

        // route from berlin (52.52006501550076, 13.40494926538985)
        // to calais (50.951309096725495, 1.8586368165796654)
        // one position every minute
        // adding a 'swagger' around the route
        // corridor width 5 km

        int nrRoutePoints = (int) Math.round(925.0 / (80.0 / 12.0));

        List<double[]> points = new ArrayList<>();

        for (int n = 0; n < nrRoutePoints; n++) {
            double[] point = {
                    52.52006501550076 + n * (50.951309096725495 - 52.52006501550076) / nrRoutePoints + 0.02 * Math.sin(2 * Math.PI * n / 100),
                    13.40494926538985 + n * (1.8586368165796654 - 13.40494926538985) / nrRoutePoints + 0.03 * Math.cos(2 * Math.PI * n / 100)
            };
            points.add(point);
        }

        final double[][] hugeRoute = points.toArray(new double[0][0]);
        final GeoCorridor hugeCorridor = new GeoCorridor(hugeRoute, 5);

        hugeCorridor.setStrategy(GeoCorridor.ORDERING);

        // also check time to run
        long now = System.currentTimeMillis();

        int nrTravelPoints = (int) Math.round(925.0 / (80.0 / 150.0));

        // same route but different swagger
        for (int n = 1; n < nrTravelPoints; n++) {
            double[] point = {
                    52.52006501550076 + n * (50.951309096725495 - 52.52006501550076) / nrTravelPoints + 0.02 * Math.cos(3 * Math.PI * n / 100),
                    13.40494926538985 + n * (1.8586368165796654 - 13.40494926538985) / nrTravelPoints + 0.03 * Math.sin(3 * Math.PI * n / 100)
            };
            assertTrue("n=" + n, hugeCorridor.liesWithin(point[0], point[1]));
        }

        long ms = System.currentTimeMillis() - now;
        assertTrue("Huge corridor took too long ms=" + ms, ms < 50);
    }

}