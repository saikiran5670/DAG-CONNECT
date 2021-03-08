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
 * Used to raise Technical Exception for any technical issue.
 *
 */
public class TechnicalException extends Exception {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	public TechnicalException(String message) {
		super(message);
	}

	public TechnicalException(Throwable t) {
		super(t);
	}

	public TechnicalException(String message, Throwable t) {
		super(message, t);
	}

}
