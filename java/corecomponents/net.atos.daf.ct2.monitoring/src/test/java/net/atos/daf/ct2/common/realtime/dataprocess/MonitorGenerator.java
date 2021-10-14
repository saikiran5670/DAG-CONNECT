package net.atos.daf.ct2.common.realtime.dataprocess;

import java.text.SimpleDateFormat;
import java.util.Date;

import net.atos.daf.ct2.common.util.Utils;
import org.apache.flink.streaming.api.functions.source.SourceFunction;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;


import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.MonitorDocument;
import static net.atos.daf.ct2.common.util.Utils.convertMillisecondToDateTime;


public class MonitorGenerator implements SourceFunction<Monitor> {

    volatile boolean isRunning = true;

    @Override
    public void run(SourceContext sourceContext) throws Exception {
        SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
        long counter = 154L;
        while (isRunning) {
            long currentTimeMillis = System.currentTimeMillis();
            Date date = new Date();
            String format1 = format.format(date);
            //Index idx = new Index();
            Monitor mdx = new Monitor();

            mdx.setVid("XLRAE75PC0E348696");
            mdx.setVin("XLRAE75PC0E348696");

            mdx.setReceivedTimestamp(currentTimeMillis);
            // IndexDocument indexDocument = new IndexDocument();
            MonitorDocument monitorDoc = new MonitorDocument();
            monitorDoc.setTripID("XLR0998HGFFT70000-XLR0998HGFFT7"/*+(int)(Math.random()*3)*/);
            monitorDoc.setDriverID("Raju");
            monitorDoc.setDriver1WorkingState(3);
            monitorDoc.setDriver2ID("Bholu");
            monitorDoc.setDriver2WorkingState(2);


            mdx.setDocument(monitorDoc);
            // 4.3 hours 15480000  -> advisory
            // 5.3 hours 19080000  -> warning
            // 6.3 hours 22680000  -> critical
            mdx.setEvtDateTime(convertMillisecondToDateTime(currentTimeMillis));
            sourceContext.collect(mdx);
            System.out.println("DATA SEND :: "+Utils.writeValueAsString(mdx));
            counter += 1L;
            // Sleep for 1 second -> 1000 , 1 minute -> 60000
            Thread.sleep(1000);

        }

    }

    @Override
    public void cancel() {
        isRunning = false;
    }

}
