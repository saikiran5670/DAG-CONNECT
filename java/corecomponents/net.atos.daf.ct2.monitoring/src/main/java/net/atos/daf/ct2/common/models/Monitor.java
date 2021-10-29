package net.atos.daf.ct2.common.models;

import lombok.Getter;
import lombok.Setter;
import net.atos.daf.ct2.common.util.Utils;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.io.Serializable;

import static net.atos.daf.ct2.common.util.DafConstants.INCOMING_MESSAGE_UUID;

@Setter
@Getter
public class Monitor extends net.atos.daf.ct2.pojo.standard.Monitor  implements Serializable {
    private static final long serialVersionUID = 1L;

    private long startTime;
    private long endTime;
    private long duration;
    private String driverState;
    private static final Logger logger = LogManager.getLogger(Monitor.class);



    /**
     * Create object from parent
     * @param monitor
     */
    public Monitor constructFromParent(net.atos.daf.ct2.pojo.standard.Monitor monitor){
        Monitor child = new Monitor();
        try {
             child= (Monitor)Utils.readValueAsObject(Utils.writeValueAsString(monitor), Monitor.class);
        } catch (Exception e) {
            logger.error("Error while constructing child object from parent monitor {} {}",e,String.format(INCOMING_MESSAGE_UUID, monitor.getJobName()));
        }
        return child;
    }

    public static Monitor copyOf(Monitor monitor){
        Monitor copy = new Monitor();
        try {
            copy= (Monitor)Utils.readValueAsObject(Utils.writeValueAsString(monitor), Monitor.class);
        } catch (Exception e) {
            logger.error("Error while constructing copy object from  monitor {} {}",e,String.format(INCOMING_MESSAGE_UUID, monitor.getJobName()));
        }
        return copy;
    }

    @Override
    public String toString() {
        return Utils.writeValueAsString(this);
    }
}
