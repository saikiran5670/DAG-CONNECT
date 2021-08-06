package net.atos.daf.ct2.models.process;


import lombok.AccessLevel;
import lombok.Builder;
import lombok.Getter;
import lombok.NonNull;
import lombok.Setter;
import lombok.ToString;
import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.MetaData;
import net.atos.daf.ct2.process.service.AlertExecutorConfig;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.List;
import java.util.Optional;
import java.util.function.Function;
import java.util.function.Predicate;
import java.util.stream.Collectors;

@Builder
@Getter
@Setter
@ToString
public final class Message implements Serializable {
    private static final long serialVersionUID = 6779905776897492797L;

    private static final Logger logger = LoggerFactory.getLogger(Message.class);

    private MetaData metaData;
    private Optional<Object> payload;
    private Optional<List<Alert>> alert;
    private AlertExecutorConfig alertExecutorConfig;

    @Setter(AccessLevel.NONE)
    private final Object originalPayload;


    public Message msgMap(@NonNull Function<Message,Message> mapFun){
        return mapFun.apply(this);
    }

    public Object map(@NonNull Function<Message,Object> mapFun){
        return mapFun.apply(this);
    }


    public Message msgAlertFilter(@NonNull Predicate<Alert> filterFun){
        if(this.getAlert().isPresent()){
            alert = Optional.ofNullable(
                    this.getAlert().get()
                    .stream()
                    .filter(filterFun)
                    .collect(Collectors.toList())
            );
        }else alert = Optional.empty();
        return this;

    }


    public  Message process(){
        return (Message) this.getAlertExecutorConfig().apply(this);
    }


    public Message trace(){
        if(logger.isTraceEnabled())
            logger.trace(String.valueOf(this));
        return this;
    }

    public Message info(){
        if(logger.isInfoEnabled()) {
            logger.info(String.valueOf(Message.builder().payload(this.getPayload()).originalPayload(this.getOriginalPayload()).alert(this.getAlert()).build()));
        }
        return this;
    }

    public Message debug(){
        if(logger.isDebugEnabled())
            logger.debug(String.valueOf(this));
        return this;
    }


}
