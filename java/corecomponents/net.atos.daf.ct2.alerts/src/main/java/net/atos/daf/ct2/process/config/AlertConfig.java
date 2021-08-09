package net.atos.daf.ct2.process.config;


import lombok.NonNull;
import net.atos.daf.ct2.models.MetaData;
import net.atos.daf.ct2.models.process.Message;
import net.atos.daf.ct2.process.service.AlertExecutorFactory;

import java.util.Map;
import java.util.Objects;
import java.util.Optional;

public final class AlertConfig {

    private static final AlertConfig alertConfig = new AlertConfig();

    public static Message buildMessage(@NonNull Object payload, @NonNull Map<Object, Object> configMap){
        return buildMessage(payload,configMap,null);
    }

    public static Message buildMessage(@NonNull Object payload, @NonNull Map<Object, Object> configMap, Map<String, Object> thresholdMap){
        return Message.builder()
                .payload(Optional.of(payload))
                .metaData(
                        MetaData.builder()
                                .config(Optional.of(configMap))
                                .threshold(Objects.isNull(thresholdMap) ? Optional.empty() : Optional.of(thresholdMap))
                                .build())
                .alertExecutorConfig(AlertExecutorFactory.getAlertExecutorConfig())
                .originalPayload(payload)
                .build();
    }
}
