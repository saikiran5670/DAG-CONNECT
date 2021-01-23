package net.atos.daf.common;

import java.util.Map;
import java.util.concurrent.TimeUnit;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import audittrail.AuditServiceGrpc;
import audittrail.Audittrail.AuditRecord;
import audittrail.Audittrail.AuditResponce;
import io.grpc.ManagedChannel;
import io.grpc.ManagedChannelBuilder;
import net.atos.daf.common.ct2.utc.TimeFormatter;

public class AuditETLJobClient {
	private static final Logger logger = LoggerFactory.getLogger(AuditETLJobClient.class);

	// ETL will call this service with map, ip & Port
	// once execution done,channel should be closed by ETL Job

	private ManagedChannel channel;
	AuditServiceGrpc.AuditServiceBlockingStub stub;

	// public AuditETLJobClient(String serverDetails, int port) {
	// channel = ManagedChannelBuilder.forAddress("52.236.153.224",
	// 80).usePlaintext().build();
	// stub = AuditServiceGrpc.newBlockingStub(channel);
	// }

	public AuditETLJobClient(String serverDetails, int port) {
		channel = ManagedChannelBuilder.forAddress(serverDetails, port).usePlaintext().build();
		stub = AuditServiceGrpc.newBlockingStub(channel);
	}

	public ManagedChannel getChannel() {
		return channel;
	}

	public void setChannel(ManagedChannel channel) {
		this.channel = channel;
	}

	public AuditServiceGrpc.AuditServiceBlockingStub getStub() {
		return stub;
	}

	public void setStub(AuditServiceGrpc.AuditServiceBlockingStub stub) {
		this.stub = stub;
	}

	// Close Grpc Channel
	public void closeChannel() {
		try {
			channel.shutdownNow().awaitTermination(5, TimeUnit.SECONDS);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	public void auditTrialGrpcCall(Map<String, String> jobDetail) {
		Long jobExecTime;
		if (jobDetail.get("jobExecTime") != null)
			jobExecTime = Long.valueOf(jobDetail.get("jobExecTime"));
		else
			jobExecTime = TimeFormatter.getCurrentUTCTime();

		AuditResponce auditResponce = stub.addlogs(AuditRecord.newBuilder()
				.setPerformedAt(com.google.protobuf.Timestamp.newBuilder().setSeconds(jobExecTime).build())
				.setPerformedBy(conversionInt(jobDetail.get("0"))).setComponentName(jobDetail.get("componentName"))
				.setServiceName(jobDetail.get("serviceName"))
				.setType(audittrail.Audittrail.AuditRecord.Event_type
						.forNumber(conversionInt(jobDetail.get("eventType"))))
				.setStatus(audittrail.Audittrail.AuditRecord.Event_status
						.forNumber(conversionInt(jobDetail.get("eventstatus"))))
				.setMessage(jobDetail.get("message")).setSourceobjectId(conversionInt(jobDetail.get("sourceObjectId")))
				.setTargetobjectId(conversionInt(jobDetail.get("targetObjectId"))).setUpdatedData("").build());
		System.out.println(auditResponce);
	}

	public static Integer conversionInt(String value) {
		if (null != value)
			return Integer.valueOf(value);
		else {
			return 0;
		}
	}
}
