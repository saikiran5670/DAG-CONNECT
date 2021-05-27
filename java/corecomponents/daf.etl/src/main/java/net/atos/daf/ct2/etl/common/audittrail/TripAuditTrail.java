package net.atos.daf.ct2.etl.common.audittrail;

import java.io.Serializable;
import java.util.HashMap;
import java.util.Map;

import org.apache.flink.api.java.utils.ParameterTool;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.etl.common.util.ETLConstants;

public class TripAuditTrail implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static Logger logger = LoggerFactory.getLogger(TripAuditTrail.class);

	public static void auditTrail(ParameterTool envParams, String jobStatus, String jobName, String message,
			String evtType) {
		try {
			Map<String, String> auditMap = createAuditMap(jobStatus, jobName, message, evtType);
			logger.info("Inside auditTrail meesage = "+ message);

			AuditETLJobClient auditing = new AuditETLJobClient(envParams.get(ETLConstants.GRPC_SERVER),
					Integer.valueOf(envParams.get(ETLConstants.GRPC_PORT)));
			auditing.auditTrialGrpcCall(auditMap);
			auditing.closeChannel();
		} catch (Exception ex) {
			logger.error("Issue while auditing :: " + ex.getMessage());
		}
	}

	private static Map<String, String> createAuditMap(String jobStatus, String jobName, String message,
			String evtType) {
		Map<String, String> auditMap = new HashMap<>();
		logger.info("Inside auditMap message :: " + message);
		
		auditMap.put(ETLConstants.JOB_EXEC_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
		auditMap.put(ETLConstants.AUDIT_PERFORMED_BY, jobName);
		auditMap.put(ETLConstants.AUDIT_COMPONENT_NAME, jobName);
		auditMap.put(ETLConstants.AUDIT_SERVICE_NAME, ETLConstants.AUDIT_SERVICE);
		auditMap.put(ETLConstants.AUDIT_EVENT_TYPE, evtType);// check
		auditMap.put(ETLConstants.AUDIT_EVENT_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
		auditMap.put(ETLConstants.AUDIT_EVENT_STATUS, jobStatus);
		auditMap.put(ETLConstants.AUDIT_MESSAGE, message);
		auditMap.put(ETLConstants.AUDIT_SOURCE_OBJECT_ID, ETLConstants.DEFAULT_OBJECT_ID);
		auditMap.put(ETLConstants.AUDIT_TARGET_OBJECT_ID, ETLConstants.DEFAULT_OBJECT_ID);
		auditMap.put(ETLConstants.AUDIT_UPDATED_DATA, null);

		return auditMap;
	}
}
