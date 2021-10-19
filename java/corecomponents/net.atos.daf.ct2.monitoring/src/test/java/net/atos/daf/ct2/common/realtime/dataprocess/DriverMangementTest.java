package net.atos.daf.ct2.common.realtime.dataprocess;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import net.atos.daf.ct2.common.processing.DriverCalculation;
import net.atos.daf.ct2.common.processing.DriverProcessing;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.MonitorDocument;
import org.apache.flink.streaming.api.datastream.DataStreamSource;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.junit.After;
import org.junit.Before;
import org.junit.Ignore;
import org.junit.Test;

import java.text.SimpleDateFormat;
import java.util.*;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.stream.Collectors;

import static net.atos.daf.ct2.common.util.Utils.convertMillisecondToDateTime;

public class DriverMangementTest {

    private StreamExecutionEnvironment env;
    private static SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
    private DriverProcessing driverProcessing;
    private static final Logger logger = LogManager.getLogger(DriverMangementTest.class);
    private static ObjectMapper mapper;
    private DriverCalculation driverCalculation;



    @Before
    public void init(){
        env = StreamExecutionEnvironment.getExecutionEnvironment();
        driverProcessing = new DriverProcessing();
        driverCalculation = new DriverCalculation();
        mapper = new ObjectMapper();
        mapper.setSerializationInclusion(JsonInclude.Include.NON_NULL);
    }

    @Test
    @Ignore
    public void testDriveTime() throws Exception {

        logger.info("Testing started.......");
        DataStreamSource<Monitor> monitorDataStreamSource = env.fromCollection(getMonitorTestData());
        driverProcessing.driverManagementProcessing(monitorDataStreamSource, 3000)
                        .print();
        env.execute();

    }


    @Test
    @Ignore
    public void testDriveTimeMap() throws Exception {
        logger.info("Testing started.......");
        final List<Monitor> monitorTestData = getMonitorTestData();
        final int chunkSize = 3;
        final AtomicInteger counter = new AtomicInteger();
        /*final Collection<List<Monitor>> chucks = monitorTestData.stream()
                .collect(Collectors.groupingBy(it -> counter.getAndIncrement() / chunkSize))
                .values();
        logger.info("chunk of list size:: {}",chucks.size());*/
        /*chucks.stream()
                .forEach(lst -> {
            try {
                driverCalculation.getStartEndTime(lst);
            } catch (Exception e) {
                logger.error("Error in test :: {}",e);
            }
        });*/
        List<Monitor> predefineData = getPredefineData(3,2,2,3,3,2,2,2,7,3);

        driverCalculation.getStartEndTime(predefineData)
                .forEach(System.out::println);

    }

    @After
    public void execute() throws Exception {

    }


    public static Monitor getDriverData(long timeMillis){
        return getDriverData(timeMillis,3);
    }

    public static Monitor getDriverData(long timeMillis,Integer driverWorkingState){
        Monitor mdx = new Monitor();
        mdx.setReceivedTimestamp(timeMillis);
        MonitorDocument monitorDoc = new MonitorDocument();
        monitorDoc.setTripID("tripTest1");
        monitorDoc.setDriverID("Raju");
        monitorDoc.setDriver1WorkingState(driverWorkingState);
        monitorDoc.setDriver2ID("Bholu");
        monitorDoc.setDriver2WorkingState(2);
        mdx.setDocument(monitorDoc);
        mdx.setEvtDateTime(convertMillisecondToDateTime(timeMillis));
        return mdx;
    }

    public static List<Monitor> getPredefineData(Integer... states){
        List<Monitor> monitorList = Arrays.stream(states).map(state -> {
            Monitor driverData = new Monitor();
            try {
                if (state == 7)
                    Thread.sleep(3000);
                else Thread.sleep(1000);
                long currentTimeMillis = System.currentTimeMillis();
                driverData = getDriverData(currentTimeMillis, state);
            } catch (InterruptedException e) {
                logger.error("Error while creating data {}", e);
            }
            return driverData;
        }).collect(Collectors.toList());
        monitorList.forEach(d-> {
            try {
                logger.info(mapper.writeValueAsString(d));
            } catch (JsonProcessingException e) {
                logger.error("error while logging data {}",e);
            }
        });
        return monitorList;
    }

    public static List<Monitor> getMonitorTestData() throws InterruptedException {
        List<Monitor> data = new ArrayList<>();
        for(int i=0; i < 15 ;i++){
            Thread.sleep(1000);
            long currentTimeMillis = System.currentTimeMillis();
            Monitor driverData = getDriverData(currentTimeMillis);
            if(i >= 5 && i < 7  ){
                driverData.getDocument().setDriver1WorkingState(2);
                data.add(driverData);
            }
            if(i == 7){
                driverData.getDocument().setDriver1WorkingState(7);
                data.add(driverData);
            }
            if(i == 9){
                driverData.getDocument().setDriver1WorkingState(7);
                data.add(driverData);
            }
            if( i == 8){
                data.add(driverData);
            }
            if( i <= 4){
                data.add(driverData);
            }
            if( i == 10){
                driverData.getDocument().setDriver1WorkingState(3);
                data.add(driverData);
            }
            if( i >= 11 && i <= 13){
                driverData.getDocument().setDriver1WorkingState(3);
                data.add(driverData);
            }
            if( i > 13){
                driverData.getDocument().setDriver1WorkingState(2);
                data.add(driverData);
            }
        }
        data.sort(Comparator.comparing(Monitor::getReceivedTimestamp));
        data.forEach(d-> {
            try {
                logger.info(mapper.writeValueAsString(d));
            } catch (JsonProcessingException e) {
                logger.error("error while logging data {}",e);
            }
        });
        return data;
    }


    @Test
    public void groupingTest(){
//        List<Integer> lst = Arrays.asList( 3, 3, 3, 2, 2, 7, 3, 3, 7, 2, 2, 2, 7);
//        List<Integer> lst = Arrays.asList(3,3,3);
        List<Integer> lst = Arrays.asList(3);
        List<List<Integer>> chuck = new ArrayList<>();
        for(int i=0;i<lst.size();) {
            List<Integer> tmp = new ArrayList<>();
            Integer first = lst.get(i);
            while (i < lst.size()) {
                Integer second = lst.get(i % lst.size());
                if (first == second) {
                    tmp.add(lst.get(i % lst.size()));
                    i += 1;
                } else {
                    break;
                }
            }
            chuck.add(tmp);
        }

        System.out.println(chuck);
    }
}
