package net.atos.daf.ct2.util;

import java.io.Serializable;
import java.util.HashMap;
import java.util.Map;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.exception.MileageAuditServiceException;

public class MileageAuditService implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger log = LogManager.getLogger(MileageAuditService.class);
	  
	
	public void auditTrail(String grpcSever, String grpcPort, String jobName, String message) throws MileageAuditServiceException{
		AuditETLJobClient auditETLJobClient = null;
	    try {
	    	auditETLJobClient =
	          new AuditETLJobClient(grpcSever,
	              Integer.valueOf(grpcPort));
	      
	      Map<String, String> auditMap = new HashMap<String, String>();

	      auditMap.put(MileageConstants.JOB_EXEC_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
	      auditMap.put(MileageConstants.AUDIT_PERFORMED_BY, jobName);
	      auditMap.put(MileageConstants.AUDIT_COMPONENT_NAME, jobName);
	      auditMap.put(MileageConstants.AUDIT_SERVICE_NAME, MileageConstants.AUDIT_SERVICE);
	      auditMap.put(MileageConstants.AUDIT_EVENT_TYPE, MileageConstants.AUDIT_CREATE_EVENT_TYPE);
	      auditMap.put(
	          MileageConstants.AUDIT_EVENT_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTime()));
	      auditMap.put(MileageConstants.AUDIT_EVENT_STATUS, MileageConstants.AUDIT_EVENT_STATUS_START);
	      auditMap.put(MileageConstants.AUDIT_MESSAGE, message);
	      auditMap.put(MileageConstants.AUDIT_SOURCE_OBJECT_ID, MileageConstants.DEFAULT_OBJECT_ID);
	      auditMap.put(MileageConstants.AUDIT_TARGET_OBJECT_ID, MileageConstants.DEFAULT_OBJECT_ID);

	      auditETLJobClient.auditTrialGrpcCall(auditMap);
	      log.debug("Audit Trial Started for job ::{} ",jobName);
	          
	    } catch (Exception e) {
	    	log.error("Issue in MileageAuditService ::{}",e.getMessage());
	    	throw new MileageAuditServiceException(" Issue while calling audit service ", e);
	    }finally{
	    	if(auditETLJobClient != null)
		      auditETLJobClient.closeChannel();
		}
	  }
	
	
}
