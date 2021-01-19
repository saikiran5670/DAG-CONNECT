package net.atos.daf.common;

import java.util.HashMap;
import java.util.Map;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import audittrail.AuditServiceGrpc;
import audittrail.Audittrail.AuditRecord;
import audittrail.Audittrail.AuditResponce;
import io.grpc.ManagedChannel;
import io.grpc.ManagedChannelBuilder;


public class AuditETLJobClient {
	private static final Logger logger = LoggerFactory.getLogger(AuditETLJobClient.class);
	
	/* private final AudittrailGrpc.AudittrailBlockingStub blockingStub;

	public AuditETLJobClient(Channel channel) {
		blockingStub = AudittrailGrpc.newBlockingStub(channel); 
	} */
	
	public void auditTrialGrpcCall(Map<String, String> jobDetail) {
		// public static void main(String args[]) {

		ManagedChannel channel = ManagedChannelBuilder.forAddress("51.105.160.79", 80).usePlaintext().build();

		AuditServiceGrpc.AuditServiceBlockingStub stub = AuditServiceGrpc.newBlockingStub(channel);

		/*
		 * HashMap<String, String> jobDetail = new HashMap<String, String>();
		 * jobDetail.put("Milliseconds", "675432168");
		 * jobDetail.put("performedBy", "121"); jobDetail.put("componentName",
		 * "CompoNameETL2me"); jobDetail.put("serviceName",
		 * "ServiceNameETL2me"); jobDetail.put("eventType", "1");
		 * //jobDetail.put("eventtime", "EventTimememe");
		 * jobDetail.put("eventstatus", "1"); jobDetail.put("message",
		 * "MessageETL2me"); jobDetail.put("sourceObjectId", "1");
		 * jobDetail.put("targetObjectId", "1"); jobDetail.put("updateddata",
		 * "UpdatedData");
		 */
		// time

		// Have to put null check for these values?

		AuditResponce auditResponce = stub.addlogs(AuditRecord.newBuilder()

				// .setAudittrailid()

				.setPerformedAt(com.google.protobuf.Timestamp.newBuilder()
						.setSeconds(conversionInt(jobDetail.get("675432168"))).build())
				.setPerformedBy(conversionInt(jobDetail.get("0")))
				.setComponentName(jobDetail.get("componentName")).setServiceName(jobDetail.get("serviceName"))
				.setType(audittrail.Audittrail.AuditRecord.Event_type
						.forNumber(conversionInt(jobDetail.get("eventType"))))
				.setStatus(audittrail.Audittrail.AuditRecord.Event_status
						.forNumber(conversionInt(jobDetail.get("eventstatus"))))
				// .setType(Event_type.forNumber(1))
				// .setType(Event_type.valueOf(1))
				// .setStatus(Event_status.valueOf(1))
				.setMessage(jobDetail.get("message")).setSourceobjectId(conversionInt(jobDetail.get("sourceObjectId")))
				.setTargetobjectId(conversionInt(jobDetail.get("targetObjectId")))
				.setUpdatedData("").build());
		System.out.println(auditResponce);

		/*
		 * .setPerformedBy(Integer.valueOf(auditMap.get("performedBy")))
		 * .setComponentName(auditMap.get("componentName"))
		 * .setServicename(auditMap.get("serviceName"))
		 * .setEventType(auditMap.get("eventType"))
		 * .setEventtime(auditMap.get("eventstatus"))
		 * .setEventstatus(auditMap.get("eventstatus"))
		 * .setMessage(auditMap.get("message"))
		 * .setSourceObjectId(Integer.valueOf(auditMap.get("sourceObjectId")))
		 * .setTargetObjectId(Integer.valueOf(auditMap.get("targetObjectId")))
		 * .setUpdateddata(auditMap.get("updateddata"))
		 */

	}
	
	public static Integer conversionInt(String value) {

		if (null != value)

			return Integer.valueOf(value);
		else {
			return 0;
		}

	}
}
