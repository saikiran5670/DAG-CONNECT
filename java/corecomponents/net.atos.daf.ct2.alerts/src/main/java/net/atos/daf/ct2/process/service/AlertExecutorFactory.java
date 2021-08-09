package net.atos.daf.ct2.process.service;

import java.util.Optional;

public final class AlertExecutorFactory {

    private static Optional<AlertExecutorConfigImpl> alertExecutorConfig = Optional.empty();

    private AlertExecutorFactory(){

    }

    public static AlertExecutorConfig getAlertExecutorConfig(){
        if(! alertExecutorConfig.isPresent()){
            synchronized (AlertExecutorFactory.class) {
                if(! alertExecutorConfig.isPresent()){
                    alertExecutorConfig = Optional.of(new AlertExecutorConfigImpl());
                    return alertExecutorConfig.get();
                }
            }
        }
        return alertExecutorConfig.get();
    }
}
