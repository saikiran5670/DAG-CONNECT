package net.atos.daf.ct2.util;

import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.IndexDocument;
import org.apache.flink.streaming.api.functions.source.SourceFunction;

import java.text.SimpleDateFormat;
import java.util.Date;

import static net.atos.daf.ct2.util.Utils.convertMillisecondToDateTime;

public class IndexGenerator implements SourceFunction<Index> {

    volatile boolean isRunning=true;
    @Override
    public void run(SourceContext sourceContext) throws Exception {
        SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
        long counter = 154L;
        while (isRunning){
            long currentTimeMillis = System.currentTimeMillis();
            Date date = new Date();
            String format1 = format.format(date);
            Index idx = new Index();
            idx.setVid("XLR0998HGFFT74611");
            idx.setVin("XLR0998HGFFT74611");
            idx.setVDist(1000L + counter);
            idx.setReceivedTimestamp(currentTimeMillis);
            IndexDocument indexDocument = new IndexDocument();
            indexDocument.setVEngineSpeed(10L);
            indexDocument.setVWheelBasedSpeed(20L);
            indexDocument.setTripID("XLR0998HGFFT70000-XLR0998HGFFT7"/*+(int)(Math.random()*3)*/);
            idx.setDocument(indexDocument);
            idx.setEvtDateTime(convertMillisecondToDateTime(currentTimeMillis));
            idx.setEvtDateTime(format1);
            sourceContext.collect(idx);
            System.out.println("DATA SEND :: " + Utils.writeValueAsString(idx));
            counter+=1000L;
            // Sleep for 1 second -> 1000 , 1 minute -> 60000
            Thread.sleep(1000);

        }

    }

    @Override
    public void cancel() {
        isRunning = false;

    }
}
