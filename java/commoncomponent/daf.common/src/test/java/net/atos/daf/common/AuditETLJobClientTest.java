package net.atos.daf.common;

import static org.mockito.AdditionalAnswers.delegatesTo;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.verify;

import java.util.HashMap;
import java.util.Map;

import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.junit.runners.JUnit4;
import org.mockito.ArgumentCaptor;
import org.mockito.ArgumentMatchers;

import audittrail.AuditServiceGrpc;
import audittrail.Audittrail.AuditResponce;
import io.grpc.ManagedChannel;
import io.grpc.inprocess.InProcessChannelBuilder;
import io.grpc.inprocess.InProcessServerBuilder;
import io.grpc.testing.GrpcCleanupRule;

/**
 * Unit tests for {@link AuditETLJobClient}. For demonstrating how to write gRPC
 * unit test only.
 */
@RunWith(JUnit4.class)
public class AuditETLJobClientTest {
	/**
	 * This rule manages automatic graceful shutdown for the registered servers
	 * and channels at the end of test.
	 */
	@Rule
	public final GrpcCleanupRule grpcCleanup = new GrpcCleanupRule();

	private AuditServiceGrpc.AuditServiceBlockingStub stub;

	private final AuditServiceGrpc.AuditServiceImplBase serviceImpl = mock(AuditServiceGrpc.AuditServiceImplBase.class,
			delegatesTo(new AuditServiceGrpc.AuditServiceImplBase() {

				@Override
				public void addlogs(audittrail.Audittrail.AuditRecord request,
						io.grpc.stub.StreamObserver<audittrail.Audittrail.AuditResponce> responseObserver) {
					responseObserver.onNext(AuditResponce.getDefaultInstance());
					responseObserver.onCompleted();
				}
			}));

	private AuditETLJobClient client;

	@Before
	public void setUp() throws Exception {
		// Generate a unique in-process server name.
		String serverName = InProcessServerBuilder.generateName();

		// Create a server, add service, start, and register for automatic
		// graceful shutdown.
		grpcCleanup.register(
				InProcessServerBuilder.forName(serverName).directExecutor().addService(serviceImpl).build().start());

		// Create a client channel and register for automatic graceful shutdown.
		ManagedChannel channel = grpcCleanup
				.register(InProcessChannelBuilder.forName(serverName).directExecutor().build());

		// Create a AuditETLJobClient using the in-process channel;
		client = new AuditETLJobClient("52.236.153.224", 80);
		stub = AuditServiceGrpc.newBlockingStub(channel);
		client.setStub(stub);
	}

	/**
	 * To test the client, call from the client against the fake server, and
	 * verify behaviors or state changes from the server side.
	 */
	@Test
	public void greet_messageDeliveredToServer() {
		ArgumentCaptor<audittrail.Audittrail.AuditRecord> requestCaptor = ArgumentCaptor
				.forClass(audittrail.Audittrail.AuditRecord.class);

		Map<String, String> jobDetail = new HashMap<String, String>();
		jobDetail.put("Milliseconds", "43578");
		jobDetail.put("performedBy", "1");
		jobDetail.put("componentName", "ETL");
		jobDetail.put("serviceName", "FLINK");
		jobDetail.put("eventType", "1");
		jobDetail.put("eventstatus", "1");
		jobDetail.put("message", "Message");
		jobDetail.put("sourceObjectId", "00");
		jobDetail.put("targetObjectId", "00");
		jobDetail.put("updateddata", "_");

		client.auditTrialGrpcCall(jobDetail);

		verify(serviceImpl).addlogs(requestCaptor.capture(),
				ArgumentMatchers.<io.grpc.stub.StreamObserver<audittrail.Audittrail.AuditResponce>>any());
	}
}