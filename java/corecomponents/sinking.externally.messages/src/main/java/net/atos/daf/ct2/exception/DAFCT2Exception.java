package net.atos.daf.ct2.exception;

import net.atos.daf.common.ct2.exception.FailureException;

public class DAFCT2Exception extends FailureException {

    public DAFCT2Exception(String errorMessage, Throwable throwable){
        super(errorMessage, throwable);
    }
}
