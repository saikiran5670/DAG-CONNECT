package net.atos.daf.ct2.util;

import java.io.Serializable;
import java.util.HashMap;
import java.util.Map;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.exception.FuelDeviationAuditServiceException;

public class FuelDeviationAuditService implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger log = LogManager.getLogger(FuelDeviationAuditService.class);
	  
	
	public void auditTrail(String grpcSever, String grpcPort, String jobName, String message) throws FuelDeviationAuditServiceException{
		AuditETLJobClient auditETLJobClient = null;
	    try {
	    	auditETLJobClient =
	          new AuditETLJobClient(grpcSever,
	              Integer.valueOf(grpcPort));
	      
	      Map<String, String> auditMap = new HashMap<String, String>();

	      auditMap.put(FuelDeviationConstants.JOB_EXEC_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
	      auditMap.put(FuelDeviationConstants.AUDIT_PERFORMED_BY, jobName);
	      auditMap.put(FuelDeviationConstants.AUDIT_COMPONENT_NAME, jobName);
	      auditMap.put(FuelDeviationConstants.AUDIT_SERVICE_NAME, FuelDeviationConstants.AUDIT_SERVICE);
	      auditMap.put(FuelDeviationConstants.AUDIT_EVENT_TYPE, FuelDeviationConstants.AUDIT_CREATE_EVENT_TYPE);
	      auditMap.put(
	          FuelDeviationConstants.AUDIT_EVENT_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTime()));
	      auditMap.put(FuelDeviationConstants.AUDIT_EVENT_STATUS, FuelDeviationConstants.AUDIT_EVENT_STATUS_START);
	      auditMap.put(FuelDeviationConstants.AUDIT_MESSAGE, message);
	      auditMap.put(FuelDeviationConstants.AUDIT_SOURCE_OBJECT_ID, FuelDeviationConstants.DEFAULT_OBJECT_ID);
	      auditMap.put(FuelDeviationConstants.AUDIT_TARGET_OBJECT_ID, FuelDeviationConstants.DEFAULT_OBJECT_ID);

	      auditETLJobClient.auditTrialGrpcCall(auditMap);
	      log.info("Audit Trial Started for job :: "+jobName);
	          
	    } catch (Exception e) {
	    	System.out.println("Issue ::"+e.getMessage());
	    	throw new FuelDeviationAuditServiceException(" Issue while calling audit service ", e);
	    }finally{
	    	if(auditETLJobClient != null)
		      auditETLJobClient.closeChannel();
		}
	  }
	
	
}
