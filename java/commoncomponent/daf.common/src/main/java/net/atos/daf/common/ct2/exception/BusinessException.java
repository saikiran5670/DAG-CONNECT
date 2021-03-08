/************************************************
*
*  Copyright  2020
*  DAF
*
*  @project: DAF Connect 2.0
*
*  @author: A676107
*
*  Created: 28-Dec-2020
*
*****************************************************/
package net.atos.daf.common.ct2.exception;

/**
 * 
 * @author Atos
 *
 */
public class BusinessException extends Exception {

	
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	public BusinessException(String message) {
		super(message);
	}

	public BusinessException(Throwable t) {
		super(t);
	}

	public BusinessException(String message, Throwable t) {
		super(message, t);
	}

}
