package net.atos.daf.ct2.common.processing;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

import org.apache.flink.api.common.state.MapState;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction.Context;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.util.Collector;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.common.util.Utils;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.postgre.bo.DriverActivityPojo;
import net.atos.daf.postgre.bo.TwoMinuteRulePojo;

import static net.atos.daf.ct2.common.util.DafConstants.INCOMING_MESSAGE_UUID;
import static net.atos.daf.ct2.common.util.Utils.*;

public class DriverCalculation extends ProcessWindowFunction<Monitor, Monitor, String, TimeWindow> {
    private static final long serialVersionUID = 1L;
    private static final Logger logger = LogManager.getLogger(DriverCalculation.class);
    ParameterTool envParam = null;
    private MapState<String, TwoMinuteRulePojo> driverPreviousRecord;

    @Override
    public void process(String driverId, Context ctx, Iterable<Monitor> values, Collector<Monitor> out) {
        Monitor monitor = new Monitor();
        try {
            logger.trace("Monitor driver activity Window data:: {}",values);
            getStartEndTime(values)
                    .forEach(lst -> out.collect(lst));
        } catch(Exception e) {
           logger.error("Error while processing monitor data for drive time error {}, {}",e,String.format(INCOMING_MESSAGE_UUID, monitor.getJobName()));
        }
    }

    @Override
    public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

        MapStateDescriptor<String, TwoMinuteRulePojo> descriptor = new MapStateDescriptor<String, TwoMinuteRulePojo>("DriverState",
                TypeInformation.of(String.class), TypeInformation.of(TwoMinuteRulePojo.class));
        driverPreviousRecord = getRuntimeContext().getMapState(descriptor);
    }


    private List<Monitor>  getStartEndTime(Iterable<Monitor> values) throws Exception {
        TwoMinuteRulePojo driverPreviousInfo = new TwoMinuteRulePojo();
        List<Monitor> monitorList = new ArrayList();
        List<Monitor> monitorSaveList = new ArrayList();
        values.forEach(monitorList::add);
        monitorList.sort(Comparator.comparing(Monitor::getReceivedTimestamp));

        List<List<Monitor>> chunkMonitorBasedOnDriverState = chunkMonitorBasedOnDriverState(monitorList);
        for(int i=0; i < chunkMonitorBasedOnDriverState.size();i++){
            List<Monitor> monitors = chunkMonitorBasedOnDriverState.get(i);
            monitors.stream().forEach(m -> logger.info("Process monitor message {} {}",m,String.format(INCOMING_MESSAGE_UUID, m.getJobName())));
            List<Monitor> monitorTmpList = new ArrayList();
            for(Monitor monitor : monitors){
                String uuID = String.format(INCOMING_MESSAGE_UUID, monitor.getJobName());
                TwoMinuteRulePojo twoMinuteRulePojo = driverPreviousRecord.get(monitor.getDocument().getDriverID());

                if(twoMinuteRulePojo==null){
                    net.atos.daf.ct2.common.models.Monitor monitorSave = new net.atos.daf.ct2.common.models.Monitor()
                            .constructFromParent(monitor);

                    monitorSave.setStartTime(convertDateToMillis(monitor.getEvtDateTime()));
                    monitorSave.setEndTime(convertDateToMillis(monitorSave.getEvtDateTime()));
                    monitorSave.setDuration(monitorSave.getEndTime()-monitorSave.getStartTime());
                    monitorSave.setDriverState(String.valueOf(monitorSave.getDocument().getDriver1WorkingState()));
                    // Add into save list
                    monitorSaveList.add(monitorSave);
                    logger.info("Driver 1 not found in map hence updating inbuild cahe {} {}",monitor.getDocument().getDriverID(),uuID);
                    updateDriverState(driverPreviousInfo, monitor);
                }else{
                    logger.info("Driver 1 previous stage :: {} current state {} {}",twoMinuteRulePojo.getCode(),monitor.getDocument().getDriver1WorkingState(),uuID);
                    monitorTmpList.add(monitor);
                }
            }
            if(! monitorTmpList.isEmpty()){
                // fetch previous details
                Monitor monitorStartIndex = monitorTmpList.get(0);
                TwoMinuteRulePojo twoMinuteRulePojo = driverPreviousRecord.get(monitorStartIndex.getDocument().getDriverID());

                net.atos.daf.ct2.common.models.Monitor monitorEnd = new net.atos.daf.ct2.common.models.Monitor()
                        .constructFromParent(monitorTmpList.get(monitorTmpList.size()-1));

                Integer driver1WorkingState = monitorStartIndex.getDocument().getDriver1WorkingState();
                if(driver1WorkingState==3 && twoMinuteRulePojo.getCode().equals("7")){
                    // make an entry for rest
                    long startTime = twoMinuteRulePojo.getEnd_time();
                    populateDriverSaveList(monitorSaveList, monitorEnd, startTime,7);
                    populateDriverSaveList(monitorSaveList, monitorEnd, convertDateToMillis(monitorStartIndex.getEvtDateTime()),3);
                }else{
                    long startTime = monitorTmpList.size() == 1 ?
                            Integer.valueOf(twoMinuteRulePojo.getCode()) == driver1WorkingState
                                    ? twoMinuteRulePojo.getEnd_time() : convertDateToMillis(monitorStartIndex.getEvtDateTime())
                            : convertDateToMillis(monitorStartIndex.getEvtDateTime());
                    //add into save list
                    populateDriverSaveList(monitorSaveList, monitorEnd, startTime,monitorStartIndex.getDocument().getDriver1WorkingState());
                }
                //update drive state
                updateDriverState(twoMinuteRulePojo, monitorTmpList.get(monitorTmpList.size()-1));
            }
        }
        //filter record with duration less than 0
        return  monitorSaveList.stream().filter(monitor -> ((net.atos.daf.ct2.common.models.Monitor)monitor).getDuration() > 0).collect(Collectors.toList());
    }

    private void populateDriverSaveList(List<Monitor> monitorSaveList, net.atos.daf.ct2.common.models.Monitor monitorEnd, long startTime,int driverWorkingState) throws Exception {
        monitorEnd.setStartTime(startTime);
        monitorEnd.setEndTime(convertDateToMillis(monitorEnd.getEvtDateTime()));
        monitorEnd.setDuration(monitorEnd.getEndTime()- monitorEnd.getStartTime());
        monitorEnd.setDriverState(String.valueOf(monitorEnd.getDocument().getDriver1WorkingState()));
        //add into save list
        monitorEnd.getDocument().setDriver1WorkingState(driverWorkingState);

        monitorSaveList.add(net.atos.daf.ct2.common.models.Monitor.copyOf(monitorEnd));

    }

    private void updateDriverState(TwoMinuteRulePojo driverPreviousInfo, Monitor monitor) throws Exception {
        driverPreviousInfo.setCode(String.valueOf(monitor.getDocument().getDriver1WorkingState()));
        long dateTime= convertDateToMillis(monitor.getEvtDateTime());
        driverPreviousInfo.setStart_time(dateTime);
        driverPreviousInfo.setEnd_time(dateTime);
        driverPreviousInfo.setDuration(driverPreviousInfo.getEnd_time() - driverPreviousInfo.getStart_time());
        driverPreviousRecord.put(monitor.getDocument().getDriverID(), driverPreviousInfo);
    }


    private List<List<Monitor>> chunkMonitorBasedOnDriverState(List<Monitor> monitors){
        List<List<Monitor>> chuck = new ArrayList<>();
        for (int i = 0; i < monitors.size(); ) {
            List<Monitor> tmp = new ArrayList<>();
            Monitor first = monitors.get(i);
            while (i < monitors.size()) {
                Monitor second = monitors.get(i % monitors.size());
                if (first.getDocument().getDriver1WorkingState().equals(second.getDocument().getDriver1WorkingState())) {
                    tmp.add(monitors.get(i % monitors.size()));
                    i += 1;
                } else {
                    break;
                }
            }
            chuck.add(tmp);
        }
        return chuck;
    }

}