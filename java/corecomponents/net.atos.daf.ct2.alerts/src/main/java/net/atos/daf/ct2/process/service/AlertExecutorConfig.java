package net.atos.daf.ct2.process.service;

import java.io.Serializable;

@FunctionalInterface
public interface AlertExecutorConfig<T> extends Serializable {

    T apply(T s);

}
