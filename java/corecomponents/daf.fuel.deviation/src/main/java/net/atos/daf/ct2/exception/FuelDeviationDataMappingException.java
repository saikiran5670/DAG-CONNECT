package net.atos.daf.ct2.exception;

import net.atos.daf.common.ct2.exception.TechnicalException;

public class FuelDeviationDataMappingException  extends TechnicalException {

    /**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	public FuelDeviationDataMappingException(String errorMessage, Throwable throwable){
        super(errorMessage, throwable);
    }
}