package net.atos.daf.ct2.processing;

import java.io.Serializable;
import java.util.HashMap;
import java.util.Map;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.constant.KafkaCT2Constant;

public class KafkaAuditService implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger log = LogManager.getLogger(KafkaAuditService.class);
	  
	
	public void auditTrail(String grpcSever, String grpcPort, String jobName, String message) {
		AuditETLJobClient auditETLJobClient = null;
	    try {
	    	auditETLJobClient =
	          new AuditETLJobClient(grpcSever,
	              Integer.valueOf(grpcPort));
	      
	      Map<String, String> auditMap = new HashMap<String, String>();

	      auditMap.put(KafkaCT2Constant.JOB_EXEC_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
	      auditMap.put(KafkaCT2Constant.AUDIT_PERFORMED_BY, jobName);
	      auditMap.put(KafkaCT2Constant.AUDIT_COMPONENT_NAME, jobName);
	      auditMap.put(KafkaCT2Constant.AUDIT_SERVICE_NAME, KafkaCT2Constant.AUDIT_SERVICE);
	      auditMap.put(KafkaCT2Constant.AUDIT_EVENT_TYPE, KafkaCT2Constant.AUDIT_CREATE_EVENT_TYPE);
	      auditMap.put(
	          KafkaCT2Constant.AUDIT_EVENT_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTime()));
	      auditMap.put(KafkaCT2Constant.AUDIT_EVENT_STATUS, KafkaCT2Constant.AUDIT_EVENT_STATUS_START);
	      auditMap.put(KafkaCT2Constant.AUDIT_MESSAGE, message);
	      auditMap.put(KafkaCT2Constant.AUDIT_SOURCE_OBJECT_ID, KafkaCT2Constant.DEFAULT_OBJECT_ID);
	      auditMap.put(KafkaCT2Constant.AUDIT_TARGET_OBJECT_ID, KafkaCT2Constant.DEFAULT_OBJECT_ID);

	      auditETLJobClient.auditTrialGrpcCall(auditMap);

	      log.info("Audit Trial Started for job :: "+jobName);
	            
	    } catch (Exception e) {
	      log.error("Unable to initialize Audit Trials", e);
	    }finally{
		      auditETLJobClient.closeChannel();
		}
	  }
	
	
}
