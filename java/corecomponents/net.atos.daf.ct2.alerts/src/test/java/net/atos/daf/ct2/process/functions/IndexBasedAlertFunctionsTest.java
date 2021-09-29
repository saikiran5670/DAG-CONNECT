package net.atos.daf.ct2.process.functions;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import net.atos.daf.ct2.models.VehicleGeofenceState;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.service.geofence.RayCasting;
import org.apache.flink.api.common.state.MapState;
import org.junit.Assert;
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
    private Map<String, VehicleGeofenceState> vehicleGeofenceSateMap = new HashMap<>();

    @Test
    public void checkGeofenceForEntering() throws IOException {
        Index idx = new Index();
        idx.setJobName(UUID.randomUUID().toString());
        idx.setVin("XLR0998HGFFT80000");

        AlertUrgencyLevelRefSchema schema1 = AlertUrgencyLevelRefSchema.builder().landMarkType("C").landmarkId(1).alertId(123L).build();
        AlertUrgencyLevelRefSchema schema2 = AlertUrgencyLevelRefSchema.builder().landMarkType("C").landmarkId(2).alertId(123L).build();
        AlertUrgencyLevelRefSchema schema3 = AlertUrgencyLevelRefSchema.builder().landMarkType("C").landmarkId(3).alertId(123L).build();

        boolean test1 = testVehicleStateForZone(idx, vehicleGeofenceSateMap, schema1,Boolean.FALSE, "enterZone");
        logger.info("Enter zone Test1 outside circle 1 :: {}",test1);
        Assert.assertEquals(false,test1);
        boolean test2  = testVehicleStateForZone(idx, vehicleGeofenceSateMap, schema1,Boolean.TRUE, "enterZone");
        logger.info("Enter zone Test2 inside circle 1:: {}",test2);
        Assert.assertEquals(true,test2);
        boolean test3 = testVehicleStateForZone(idx, vehicleGeofenceSateMap, schema1,Boolean.FALSE, "enterZone");
        logger.info("Enter zone Exit Test3 outside circle 1 :: {}",test3);
        Assert.assertEquals(false,test3);
        boolean test4 = testVehicleStateForZone(idx, vehicleGeofenceSateMap, schema2,Boolean.TRUE, "enterZone");
        logger.info("Enter zone Test4 inside circle 2:: {}",test4);
        Assert.assertEquals(true,test4);
        boolean test5 = testVehicleStateForZone(idx, vehicleGeofenceSateMap, schema2,Boolean.FALSE, "enterZone");
        logger.info("Enter zone Exit Test3 outside circle 2 :: {}",test5);
        Assert.assertEquals(false,test5);

        // vehicle enter circle 3
        boolean test6  = testVehicleStateForZone(idx, vehicleGeofenceSateMap, schema3,Boolean.TRUE, "enterZone");
        logger.info("Enter zone Test6 inside circle 3:: {}",test6);
        Assert.assertEquals(true,test6);
        // vehicle inside the circle 3
        boolean test7  = testVehicleStateForZone(idx, vehicleGeofenceSateMap, schema3,Boolean.TRUE, "enterZone");
        logger.info("Enter zone Test7 inside circle 3:: {}",test7);
        Assert.assertEquals(false,test7);

    }


    public static boolean testVehicleStateForZone(Index idx, Map<String, VehicleGeofenceState> vehicleGeofenceSateMap,
                                                  AlertUrgencyLevelRefSchema schema,Boolean inside, String alertType){
        VehicleGeofenceState vehicleState=VehicleGeofenceState.builder().isInside(Boolean.FALSE).landMarkId(-1).build();
        if(vehicleGeofenceSateMap.containsKey(idx.getVin())){
            vehicleState= vehicleGeofenceSateMap.get(idx.getVin());
        }else{
            vehicleGeofenceSateMap.put(idx.getVin(),vehicleState);
        }
        return checkVehicleStateForZone(idx,vehicleGeofenceSateMap,vehicleState,schema, inside,alertType);
    }

    public static boolean checkVehicleStateForZone(Index index, Map<String, VehicleGeofenceState> vehicleGeofenceSate,
                                                   VehicleGeofenceState vehicleState, AlertUrgencyLevelRefSchema tempSchema,
                                                   Boolean inside, String alertType) {

        String messageUUId = String.format(INCOMING_MESSAGE_UUID, index.getJobName());
        //Enter zone check
        Boolean enterZoneTrue = ! vehicleState.getIsInside() && tempSchema.getLandmarkId() != vehicleState.getLandMarkId()   && inside && alertType.equalsIgnoreCase("enterZone");
        Boolean enterZoneFalse = vehicleState.getIsInside()  && tempSchema.getLandmarkId() == vehicleState.getLandMarkId()  && !inside && alertType.equalsIgnoreCase("enterZone");

        //Exit zone check
        Boolean exitZoneTrue = vehicleState.getIsInside() && tempSchema.getLandmarkId() == vehicleState.getLandMarkId() && !inside && alertType.equalsIgnoreCase("exitZone");
        Boolean exitZoneFalse = !vehicleState.getIsInside() && tempSchema.getLandmarkId() != vehicleState.getLandMarkId() && inside && alertType.equalsIgnoreCase("exitZone");

        if (enterZoneTrue || exitZoneTrue) {
            if(enterZoneTrue)
                logger.info("Vehicle enter into zone vin {} landmark type {} landmarkId {} {} ",index.getVin(),tempSchema.getLandMarkType(), tempSchema.getLandmarkId(), messageUUId);
            if(exitZoneTrue)
                logger.info("Vehicle exit from zone vin {} landmark type {} landmarkId {} {} ",index.getVin(),tempSchema.getLandMarkType(), tempSchema.getLandmarkId(), messageUUId);
            logger.info(alertType + " alert generated for vin {} for alertId {} {}", index.getVin(), tempSchema.getAlertId(), messageUUId );
            try {
                vehicleGeofenceSate.put(index.getVin(), VehicleGeofenceState.builder().isInside(inside).landMarkId(tempSchema.getLandmarkId()).build());
            } catch (Exception e) {
                logger.error("Error while retrieve previous state for vin {} " + alertType + " {}", index.getVin(), messageUUId);
            }
            return true;
        }
        // If the state change raise an alert for exiting zone
        if (enterZoneFalse || exitZoneFalse) {
            try {
                if(enterZoneFalse)
                    logger.info("Vehicle exit from zone vin {} landmark type {} landmarkId {} {} ",index.getVin(),tempSchema.getLandMarkType(), tempSchema.getLandmarkId(), messageUUId);
                if(exitZoneFalse)
                    logger.info("Vehicle enter into zone vin {} landmark type {} landmarkId {} {} ",index.getVin(),tempSchema.getLandMarkType(), tempSchema.getLandmarkId(), messageUUId);
                vehicleGeofenceSate.put(index.getVin(), VehicleGeofenceState.builder().isInside(inside).landMarkId(tempSchema.getLandmarkId()).build());
            } catch (Exception e) {
                logger.error("Error while retrieve previous state for vin {} " + alertType + " {} error {}", index.getVin(), messageUUId, e);
            }
        }
        return false;
    }

}