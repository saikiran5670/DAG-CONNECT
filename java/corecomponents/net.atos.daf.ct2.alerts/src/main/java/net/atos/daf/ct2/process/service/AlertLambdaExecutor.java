package net.atos.daf.ct2.process.service;

import java.io.Serializable;

@FunctionalInterface
public interface AlertLambdaExecutor<T,R> extends Serializable {

    R apply(T source);
}
