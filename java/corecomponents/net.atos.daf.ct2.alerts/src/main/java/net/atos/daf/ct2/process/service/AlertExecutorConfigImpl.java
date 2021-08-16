package net.atos.daf.ct2.process.service;




import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.process.Message;
import net.atos.daf.ct2.models.process.Target;

import java.io.Serializable;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

public final class AlertExecutorConfigImpl implements AlertExecutorConfig<Message>, Serializable {
    private static final long serialVersionUID = -7036632852140554711L;

    protected AlertExecutorConfigImpl(){

    }

    @Override
    public Message apply(Message s) {
        List<AlertLambdaExecutor<Message, Target>> functions = (List<AlertLambdaExecutor<Message,Target>>) s.getMetaData()
        .getConfig()
        .get()
        .get("functions");

        List<Target> targetList = functions
                .stream()
                .map(fun -> fun.apply(s))
                .collect(Collectors.toList());

        List<Alert> alerts = targetList.stream()
                .filter(t -> t.getAlert().isPresent())
                .map(t -> t.getAlert().get())
                .collect(Collectors.toList());

        if(! alerts.isEmpty()){
            return getMessage(s,alerts);
        }else {
           return getMessage(s,alerts);
        }
    }

    private Message getMessage(Message s,List<Alert> alerts) {
        Optional<List<Alert>> alert = alerts.isEmpty() ? Optional.empty() : Optional.of(alerts);
        return Message.builder()
                .payload(s.getPayload())
                .metaData(s.getMetaData())
                .alertExecutorConfig(s.getAlertExecutorConfig())
                .originalPayload(s.getOriginalPayload())
                .alert(alert)
                .build();
    }
}
