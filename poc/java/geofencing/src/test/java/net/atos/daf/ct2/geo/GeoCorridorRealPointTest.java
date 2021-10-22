package net.atos.daf.ct2.geo;

import com.fasterxml.jackson.databind.ObjectMapper;
import net.atos.daf.ct2.geo.models.RoutePoint;
import org.junit.BeforeClass;
import org.junit.Test;
import org.slf4j.LoggerFactory;

import java.io.InputStream;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertTrue;

public class GeoCorridorRealPointTest {

    /** Test route, linear. */
    private static double[][] route;

    /** The linear geo corridor to test. */
    private static GeoCorridor instance;

    private static ObjectMapper objectMapper;
    private static RoutePoint routePoint;

    /**
     * Setup corridor definitions for linear and elliptic case.
     * Use this method so that the instances are created once as opposed to for every test method.
     * @throws Exception in case of error
     */
    /** The logger to use. */
    private static final org.slf4j.Logger log = LoggerFactory.getLogger(GeoCorridorRealPointTest.class);


    @BeforeClass
    public static void setUpBeforeClass() throws Exception {
        ClassLoader classLoader = GeoCorridorRealPointTest.class.getClassLoader();
        InputStream inputStream = classLoader.getResourceAsStream("linearPathJson.json");
        objectMapper = new ObjectMapper();
        routePoint = objectMapper.readValue(inputStream, RoutePoint.class);
        List<double[]> points = new ArrayList<>();
        routePoint.getRoute().getRpt()
                .forEach(route -> {
                    points.add(new double[]{
                            Double.valueOf(route.getLat()),
                            Double.valueOf(route.getLon())}
                    );
                });


        route = points.toArray(new double[0][0]);
        instance = new GeoCorridor(route, 1);

    }

    @Test
    public void testLinearCorridorRoutePoints() {
        log.info("Test data");
//		{ lat: 37.772, lng: -122.214 },
        Arrays.stream(route).forEach(point ->
                log.debug("{lat: {}, lng: {} }",point[0],point[1])
        );
        for (double[] point: route) {
            assertTrue(instance.liesWithin(point[0], point[1]));
        }
    }

    @Test
    public void testLinearFarOutside() {

        // outside bounding box
        assertFalse(instance.liesWithin(46.93083333, 1.81222222));

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
//            assertTrue(instance.liesWithin(point[0], point[1] + 0.0144));
//            assertTrue(instance.liesWithin(point[0], point[1] - 0.0144));
        }
    }
}
