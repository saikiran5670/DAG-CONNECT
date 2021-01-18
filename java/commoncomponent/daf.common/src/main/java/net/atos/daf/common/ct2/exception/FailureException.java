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
 * Used to end batch processing due to a technical issue.
 *
 * @author Atos
 *
 */
public class FailureException extends TechnicalException {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	public FailureException(String message) {
		super(message);
	}

	public FailureException(Throwable t) {
		super(t);
	}

	public FailureException(String message, Throwable t) {
		super(message, t);
	}

}
