package net.atos.daf.ct2.util;

import net.atos.daf.ct2.app.TripBasedTest;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.IndexDocument;
import org.apache.flink.streaming.api.functions.source.SourceFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.text.SimpleDateFormat;
import java.util.Date;

import static net.atos.daf.ct2.util.Utils.convertMillisecondToDateTime;

public class IndexGenerator implements SourceFunction<Index> {

    volatile boolean isRunning=true;
    private static final Logger logger = LoggerFactory.getLogger(IndexGenerator.class);
    @Override
    public void run(SourceContext sourceContext) throws Exception {
        SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
        long counter = 154L;
        while (isRunning){
            long currentTimeMillis = System.currentTimeMillis();
            Date date = new Date();
            String format1 = format.format(date);
            Index idx = new Index();
           /* if(counter%2==0){
                idx.setVid("XLR0998HGFFT74611"+((int)(Math.random()*2)));
                idx.setVin("XLR0998HGFFT74611"+((int)(Math.random()*2)));
            }else{
                idx.setVid("XLRAE75PC0E348696");
                idx.setVin("XLRAE75PC0E348696");
            }*/
            idx.setVid("XLRAE75PC0E348696");
            idx.setVin("XLRAE75PC0E348696");

            idx.setVDist(1000L);
            idx.setGpsLatitude(124.3433);
            idx.setGpsLongitude(124.3433);
            idx.setVEvtID(4);
            idx.setReceivedTimestamp(currentTimeMillis);
            IndexDocument indexDocument = new IndexDocument();
            indexDocument.setVEngineSpeed(10L);
            indexDocument.setVWheelBasedSpeed(20L);
            indexDocument.setVFuelLevel1(counter+0.0);
            indexDocument.setTripID("XLR0998HGFFT70000-XLR0998HGFFT7"/*+(int)(Math.random()*3)*/);
            idx.setDocument(indexDocument);
            // 4.3 hours 15480000  -> advisory
            // 5.3 hours 19080000  -> warning
            // 6.3 hours 22680000  -> critical
            idx.setEvtDateTime(convertMillisecondToDateTime(currentTimeMillis));
            sourceContext.collect(idx);
            logger.info("DATA SEND :: {}" , Utils.writeValueAsString(idx));
            counter+=1L;
            // Sleep for 1 second -> 1000 , 1 minute -> 60000
            Thread.sleep(60000);

        }

    }

    @Override
    public void cancel() {
        isRunning = false;
    }
}
