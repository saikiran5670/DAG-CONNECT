package net.atos.daf.ct2.process.functions;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.service.geofence.RayCasting;
import org.junit.Test;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.File;
import java.io.IOException;
import java.util.*;
import java.util.stream.Collectors;

import static net.atos.daf.ct2.props.AlertConfigProp.INCOMING_MESSAGE_UUID;
import static org.junit.Assert.*;

public class IndexBasedAlertFunctionsTest {

    private static final Logger logger = LoggerFactory.getLogger(IndexBasedAlertFunctionsTest.class);

    @Test
    public void checkGeofenceForEntering() throws IOException {

        Index idx = new Index();
        idx.setJobName(UUID.randomUUID().toString());
        idx.setGpsLatitude(123.43);
        idx.setGpsLongitude(123.43);
        idx.setVin("1234");
        idx.setVid("123");

        ObjectMapper mapper = new ObjectMapper();

        ClassLoader classLoader = getClass().getClassLoader();
        File file = new File(classLoader.getResource("entering-zone-test-data.txt").getFile());
        List<AlertUrgencyLevelRefSchema> urgencyLevelRefSchemas  = mapper.readValue(file, new TypeReference<List<AlertUrgencyLevelRefSchema>>(){});
        System.out.println(urgencyLevelRefSchemas);
        urgencyLevelRefSchemas= urgencyLevelRefSchemas.stream().sorted(Comparator.comparing(AlertUrgencyLevelRefSchema::getLandmarkId).thenComparing(AlertUrgencyLevelRefSchema::getNodeSeq)).collect(Collectors.toList());

        System.out.println("urgencyLevelRefSchemas:: "+urgencyLevelRefSchemas.size());
        Map<Integer, List<AlertUrgencyLevelRefSchema>> groupSchema = urgencyLevelRefSchemas.stream()
                .collect(Collectors.groupingBy(AlertUrgencyLevelRefSchema::getLandmarkId));
        System.out.println("groupSchema:: "+groupSchema.size());

        Set<Map.Entry<Integer, List<AlertUrgencyLevelRefSchema>>> entries = groupSchema.entrySet();

        entries.stream()
                .map(entry -> entry.getValue())
                .map(l -> {
                    List<Double> polygonPointList = new ArrayList<>();
                    l.forEach(schema -> {
                        if (schema.getUrgencyLevelType().equalsIgnoreCase("W") && schema.getLatitude() !=0.0 && schema.getLongitude() !=0.0) {
                            polygonPointList.add(schema.getLatitude());
                            polygonPointList.add(schema.getLongitude());
                        }
                    });
                    return polygonPointList;
                })
                .map(polygonPointList -> {
                    Double[] point ={51.50551, 31.28487};
                    logger.info("Polygon boundary nodes {} for {}",polygonPointList,String.format(INCOMING_MESSAGE_UUID,"test"));
                    logger.info("Polygon boundary test point {} for {}",Arrays.asList(point),String.format(INCOMING_MESSAGE_UUID,"test"));
                    Double[][] polygonPoints = new Double[polygonPointList.size() / 2][polygonPointList.size() / 2];
                    int indexCounter=0;
                    for (int i = 0; i < polygonPointList.size(); i=i+2) {
                        polygonPoints[indexCounter] = new Double[]{polygonPointList.get(i), polygonPointList.get((i + 1) % polygonPointList.size())};
                        indexCounter++;
                    }
                    // Check weather point inside or outside of polygon
                    Boolean inside = RayCasting.isInside(polygonPoints, point);
                    logger.info("Ray casting result  {} for {}",inside,String.format(INCOMING_MESSAGE_UUID,"test"));
                    // If the state change raise an alert for entering zone
                   return inside;
                } )
                .forEach(System.out:: println);
//        System.out.println(urgencyLevelRefSchemas);


    }
}