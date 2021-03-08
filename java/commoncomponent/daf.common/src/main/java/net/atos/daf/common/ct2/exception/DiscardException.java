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
 * Used to Discard message due to a any Business Exception.
 *
 * @author Atos
 *
 */
public class DiscardException extends BusinessException {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	public DiscardException(String message) {
		super(message);
	}

	public DiscardException(Throwable t) {
		super(t);
	}

	public DiscardException(String message, Throwable t) {
		super(message, t);
	}

}

