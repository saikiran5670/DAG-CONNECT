package net.atos.daf.ct2.common.util;

import java.io.Serializable;
import java.util.HashMap;
import java.util.Map;

import org.apache.flink.api.java.utils.ParameterTool;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.common.ct2.utc.TimeFormatter;

public class TripIndexAuditTrail implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static Logger logger = LoggerFactory.getLogger(TripIndexAuditTrail.class);

	public static void auditTrail(ParameterTool envParams, String jobStatus, String jobName, String message,
			String evtType) {
		try {
			Map<String, String> auditMap = createAuditMap(jobStatus, jobName, message, evtType);
			logger.debug("Inside auditTrail meesage = "+ message);

			AuditETLJobClient auditing = new AuditETLJobClient(envParams.get(DafConstants.GRPC_SERVER),
					Integer.valueOf(envParams.get(DafConstants.GRPC_PORT)));
			auditing.auditTrialGrpcCall(auditMap);
			auditing.closeChannel();
		} catch (Exception ex) {
			logger.error("Issue while auditing :: " + ex.getMessage());
		}
	}

	private static Map<String, String> createAuditMap(String jobStatus, String jobName, String message,
			String evtType) {
		Map<String, String> auditMap = new HashMap<>();
		logger.debug("Inside auditMap message :: " + message);
		
		auditMap.put(DafConstants.JOB_EXEC_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
		auditMap.put(DafConstants.AUDIT_PERFORMED_BY, jobName);
		auditMap.put(DafConstants.AUDIT_COMPONENT_NAME, jobName);
		auditMap.put(DafConstants.AUDIT_SERVICE_NAME, DafConstants.AUDIT_SERVICE);
		auditMap.put(DafConstants.AUDIT_EVENT_TYPE, evtType);// check
		auditMap.put(DafConstants.AUDIT_EVENT_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
		auditMap.put(DafConstants.AUDIT_EVENT_STATUS, jobStatus);
		auditMap.put(DafConstants.AUDIT_MESSAGE, message);
		auditMap.put(DafConstants.AUDIT_SOURCE_OBJECT_ID, DafConstants.DEFAULT_OBJECT_ID);
		auditMap.put(DafConstants.AUDIT_TARGET_OBJECT_ID, DafConstants.DEFAULT_OBJECT_ID);
		auditMap.put(DafConstants.AUDIT_UPDATED_DATA, null);

		return auditMap;
	}
}
