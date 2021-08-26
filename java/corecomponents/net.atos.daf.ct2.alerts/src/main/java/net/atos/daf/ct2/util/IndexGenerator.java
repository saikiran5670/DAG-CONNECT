package net.atos.daf.ct2.util;

import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.IndexDocument;
import org.apache.flink.streaming.api.functions.source.SourceFunction;

import java.text.SimpleDateFormat;
import java.util.Date;

public class IndexGenerator implements SourceFunction<Index> {

    volatile boolean isRunning=true;
    @Override
    public void run(SourceContext sourceContext) throws Exception {
        SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
        while (isRunning){
            Date date = new Date();
            String format1 = format.format(date);
            Index idx = new Index();
            idx.setVid("XLR0998HGFFT70000");
            idx.setVin("XLR0998HGFFT70000");
            idx.setReceivedTimestamp(System.currentTimeMillis());
            IndexDocument indexDocument = new IndexDocument();
            indexDocument.setVEngineSpeed(10L);
            indexDocument.setVWheelBasedSpeed(20L);
            indexDocument.setTripID("XLR0998HGFFT70000-XLR0998HGFFT7"+(int)(Math.random()*3));
            idx.setDocument(indexDocument);
            idx.setEvtDateTime(format1);
            sourceContext.collect(idx);
            System.out.println("DATA SEND :: " + idx);
            // Sleep for 1 second
            Thread.sleep(1000);
        }

    }

    @Override
    public void cancel() {
        isRunning = false;

    }
}
