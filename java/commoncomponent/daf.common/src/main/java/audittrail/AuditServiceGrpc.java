package audittrail;

import static io.grpc.MethodDescriptor.generateFullMethodName;
import static io.grpc.stub.ClientCalls.asyncBidiStreamingCall;
import static io.grpc.stub.ClientCalls.asyncClientStreamingCall;
import static io.grpc.stub.ClientCalls.asyncServerStreamingCall;
import static io.grpc.stub.ClientCalls.asyncUnaryCall;
import static io.grpc.stub.ClientCalls.blockingServerStreamingCall;
import static io.grpc.stub.ClientCalls.blockingUnaryCall;
import static io.grpc.stub.ClientCalls.futureUnaryCall;
import static io.grpc.stub.ServerCalls.asyncBidiStreamingCall;
import static io.grpc.stub.ServerCalls.asyncClientStreamingCall;
import static io.grpc.stub.ServerCalls.asyncServerStreamingCall;
import static io.grpc.stub.ServerCalls.asyncUnaryCall;
import static io.grpc.stub.ServerCalls.asyncUnimplementedStreamingCall;
import static io.grpc.stub.ServerCalls.asyncUnimplementedUnaryCall;

/**
 */
@javax.annotation.Generated(
    value = "by gRPC proto compiler (version 1.15.0)",
    comments = "Source: audittrail.proto")
public final class AuditServiceGrpc {

  private AuditServiceGrpc() {}

  public static final String SERVICE_NAME = "audittrail.AuditService";

  // Static method descriptors that strictly reflect the proto.
  private static volatile io.grpc.MethodDescriptor<audittrail.Audittrail.AuditRecord,
      audittrail.Audittrail.AuditResponce> getAddlogsMethod;

  @io.grpc.stub.annotations.RpcMethod(
      fullMethodName = SERVICE_NAME + '/' + "Addlogs",
      requestType = audittrail.Audittrail.AuditRecord.class,
      responseType = audittrail.Audittrail.AuditResponce.class,
      methodType = io.grpc.MethodDescriptor.MethodType.UNARY)
  public static io.grpc.MethodDescriptor<audittrail.Audittrail.AuditRecord,
      audittrail.Audittrail.AuditResponce> getAddlogsMethod() {
    io.grpc.MethodDescriptor<audittrail.Audittrail.AuditRecord, audittrail.Audittrail.AuditResponce> getAddlogsMethod;
    if ((getAddlogsMethod = AuditServiceGrpc.getAddlogsMethod) == null) {
      synchronized (AuditServiceGrpc.class) {
        if ((getAddlogsMethod = AuditServiceGrpc.getAddlogsMethod) == null) {
          AuditServiceGrpc.getAddlogsMethod = getAddlogsMethod = 
              io.grpc.MethodDescriptor.<audittrail.Audittrail.AuditRecord, audittrail.Audittrail.AuditResponce>newBuilder()
              .setType(io.grpc.MethodDescriptor.MethodType.UNARY)
              .setFullMethodName(generateFullMethodName(
                  "audittrail.AuditService", "Addlogs"))
              .setSampledToLocalTracing(true)
              .setRequestMarshaller(io.grpc.protobuf.ProtoUtils.marshaller(
                  audittrail.Audittrail.AuditRecord.getDefaultInstance()))
              .setResponseMarshaller(io.grpc.protobuf.ProtoUtils.marshaller(
                  audittrail.Audittrail.AuditResponce.getDefaultInstance()))
                  .setSchemaDescriptor(new AuditServiceMethodDescriptorSupplier("Addlogs"))
                  .build();
          }
        }
     }
     return getAddlogsMethod;
  }

  private static volatile io.grpc.MethodDescriptor<audittrail.Audittrail.AuditLogRequest,
      audittrail.Audittrail.AuditLogResponse> getGetAuditLogsMethod;

  @io.grpc.stub.annotations.RpcMethod(
      fullMethodName = SERVICE_NAME + '/' + "GetAuditLogs",
      requestType = audittrail.Audittrail.AuditLogRequest.class,
      responseType = audittrail.Audittrail.AuditLogResponse.class,
      methodType = io.grpc.MethodDescriptor.MethodType.UNARY)
  public static io.grpc.MethodDescriptor<audittrail.Audittrail.AuditLogRequest,
      audittrail.Audittrail.AuditLogResponse> getGetAuditLogsMethod() {
    io.grpc.MethodDescriptor<audittrail.Audittrail.AuditLogRequest, audittrail.Audittrail.AuditLogResponse> getGetAuditLogsMethod;
    if ((getGetAuditLogsMethod = AuditServiceGrpc.getGetAuditLogsMethod) == null) {
      synchronized (AuditServiceGrpc.class) {
        if ((getGetAuditLogsMethod = AuditServiceGrpc.getGetAuditLogsMethod) == null) {
          AuditServiceGrpc.getGetAuditLogsMethod = getGetAuditLogsMethod = 
              io.grpc.MethodDescriptor.<audittrail.Audittrail.AuditLogRequest, audittrail.Audittrail.AuditLogResponse>newBuilder()
              .setType(io.grpc.MethodDescriptor.MethodType.UNARY)
              .setFullMethodName(generateFullMethodName(
                  "audittrail.AuditService", "GetAuditLogs"))
              .setSampledToLocalTracing(true)
              .setRequestMarshaller(io.grpc.protobuf.ProtoUtils.marshaller(
                  audittrail.Audittrail.AuditLogRequest.getDefaultInstance()))
              .setResponseMarshaller(io.grpc.protobuf.ProtoUtils.marshaller(
                  audittrail.Audittrail.AuditLogResponse.getDefaultInstance()))
                  .setSchemaDescriptor(new AuditServiceMethodDescriptorSupplier("GetAuditLogs"))
                  .build();
          }
        }
     }
     return getGetAuditLogsMethod;
  }

  /**
   * Creates a new async stub that supports all call types for the service
   */
  public static AuditServiceStub newStub(io.grpc.Channel channel) {
    return new AuditServiceStub(channel);
  }

  /**
   * Creates a new blocking-style stub that supports unary and streaming output calls on the service
   */
  public static AuditServiceBlockingStub newBlockingStub(
      io.grpc.Channel channel) {
    return new AuditServiceBlockingStub(channel);
  }

  /**
   * Creates a new ListenableFuture-style stub that supports unary calls on the service
   */
  public static AuditServiceFutureStub newFutureStub(
      io.grpc.Channel channel) {
    return new AuditServiceFutureStub(channel);
  }

  /**
   */
  public static abstract class AuditServiceImplBase implements io.grpc.BindableService {

    /**
     */
    public void addlogs(audittrail.Audittrail.AuditRecord request,
        io.grpc.stub.StreamObserver<audittrail.Audittrail.AuditResponce> responseObserver) {
      asyncUnimplementedUnaryCall(getAddlogsMethod(), responseObserver);
    }

    /**
     */
    public void getAuditLogs(audittrail.Audittrail.AuditLogRequest request,
        io.grpc.stub.StreamObserver<audittrail.Audittrail.AuditLogResponse> responseObserver) {
      asyncUnimplementedUnaryCall(getGetAuditLogsMethod(), responseObserver);
    }

    @java.lang.Override public final io.grpc.ServerServiceDefinition bindService() {
      return io.grpc.ServerServiceDefinition.builder(getServiceDescriptor())
          .addMethod(
            getAddlogsMethod(),
            asyncUnaryCall(
              new MethodHandlers<
                audittrail.Audittrail.AuditRecord,
                audittrail.Audittrail.AuditResponce>(
                  this, METHODID_ADDLOGS)))
          .addMethod(
            getGetAuditLogsMethod(),
            asyncUnaryCall(
              new MethodHandlers<
                audittrail.Audittrail.AuditLogRequest,
                audittrail.Audittrail.AuditLogResponse>(
                  this, METHODID_GET_AUDIT_LOGS)))
          .build();
    }
  }

  /**
   */
  public static final class AuditServiceStub extends io.grpc.stub.AbstractStub<AuditServiceStub> {
    private AuditServiceStub(io.grpc.Channel channel) {
      super(channel);
    }

    private AuditServiceStub(io.grpc.Channel channel,
        io.grpc.CallOptions callOptions) {
      super(channel, callOptions);
    }

    @java.lang.Override
    protected AuditServiceStub build(io.grpc.Channel channel,
        io.grpc.CallOptions callOptions) {
      return new AuditServiceStub(channel, callOptions);
    }

    /**
     */
    public void addlogs(audittrail.Audittrail.AuditRecord request,
        io.grpc.stub.StreamObserver<audittrail.Audittrail.AuditResponce> responseObserver) {
      asyncUnaryCall(
          getChannel().newCall(getAddlogsMethod(), getCallOptions()), request, responseObserver);
    }

    /**
     */
    public void getAuditLogs(audittrail.Audittrail.AuditLogRequest request,
        io.grpc.stub.StreamObserver<audittrail.Audittrail.AuditLogResponse> responseObserver) {
      asyncUnaryCall(
          getChannel().newCall(getGetAuditLogsMethod(), getCallOptions()), request, responseObserver);
    }
  }

  /**
   */
  public static final class AuditServiceBlockingStub extends io.grpc.stub.AbstractStub<AuditServiceBlockingStub> {
    private AuditServiceBlockingStub(io.grpc.Channel channel) {
      super(channel);
    }

    private AuditServiceBlockingStub(io.grpc.Channel channel,
        io.grpc.CallOptions callOptions) {
      super(channel, callOptions);
    }

    @java.lang.Override
    protected AuditServiceBlockingStub build(io.grpc.Channel channel,
        io.grpc.CallOptions callOptions) {
      return new AuditServiceBlockingStub(channel, callOptions);
    }

    /**
     */
    public audittrail.Audittrail.AuditResponce addlogs(audittrail.Audittrail.AuditRecord request) {
      return blockingUnaryCall(
          getChannel(), getAddlogsMethod(), getCallOptions(), request);
    }

    /**
     */
    public audittrail.Audittrail.AuditLogResponse getAuditLogs(audittrail.Audittrail.AuditLogRequest request) {
      return blockingUnaryCall(
          getChannel(), getGetAuditLogsMethod(), getCallOptions(), request);
    }
  }

  /**
   */
  public static final class AuditServiceFutureStub extends io.grpc.stub.AbstractStub<AuditServiceFutureStub> {
    private AuditServiceFutureStub(io.grpc.Channel channel) {
      super(channel);
    }

    private AuditServiceFutureStub(io.grpc.Channel channel,
        io.grpc.CallOptions callOptions) {
      super(channel, callOptions);
    }

    @java.lang.Override
    protected AuditServiceFutureStub build(io.grpc.Channel channel,
        io.grpc.CallOptions callOptions) {
      return new AuditServiceFutureStub(channel, callOptions);
    }

    /**
     */
    public com.google.common.util.concurrent.ListenableFuture<audittrail.Audittrail.AuditResponce> addlogs(
        audittrail.Audittrail.AuditRecord request) {
      return futureUnaryCall(
          getChannel().newCall(getAddlogsMethod(), getCallOptions()), request);
    }

    /**
     */
    public com.google.common.util.concurrent.ListenableFuture<audittrail.Audittrail.AuditLogResponse> getAuditLogs(
        audittrail.Audittrail.AuditLogRequest request) {
      return futureUnaryCall(
          getChannel().newCall(getGetAuditLogsMethod(), getCallOptions()), request);
    }
  }

  private static final int METHODID_ADDLOGS = 0;
  private static final int METHODID_GET_AUDIT_LOGS = 1;

  private static final class MethodHandlers<Req, Resp> implements
      io.grpc.stub.ServerCalls.UnaryMethod<Req, Resp>,
      io.grpc.stub.ServerCalls.ServerStreamingMethod<Req, Resp>,
      io.grpc.stub.ServerCalls.ClientStreamingMethod<Req, Resp>,
      io.grpc.stub.ServerCalls.BidiStreamingMethod<Req, Resp> {
    private final AuditServiceImplBase serviceImpl;
    private final int methodId;

    MethodHandlers(AuditServiceImplBase serviceImpl, int methodId) {
      this.serviceImpl = serviceImpl;
      this.methodId = methodId;
    }

    @java.lang.Override
    @java.lang.SuppressWarnings("unchecked")
    public void invoke(Req request, io.grpc.stub.StreamObserver<Resp> responseObserver) {
      switch (methodId) {
        case METHODID_ADDLOGS:
          serviceImpl.addlogs((audittrail.Audittrail.AuditRecord) request,
              (io.grpc.stub.StreamObserver<audittrail.Audittrail.AuditResponce>) responseObserver);
          break;
        case METHODID_GET_AUDIT_LOGS:
          serviceImpl.getAuditLogs((audittrail.Audittrail.AuditLogRequest) request,
              (io.grpc.stub.StreamObserver<audittrail.Audittrail.AuditLogResponse>) responseObserver);
          break;
        default:
          throw new AssertionError();
      }
    }

    @java.lang.Override
    @java.lang.SuppressWarnings("unchecked")
    public io.grpc.stub.StreamObserver<Req> invoke(
        io.grpc.stub.StreamObserver<Resp> responseObserver) {
      switch (methodId) {
        default:
          throw new AssertionError();
      }
    }
  }

  private static abstract class AuditServiceBaseDescriptorSupplier
      implements io.grpc.protobuf.ProtoFileDescriptorSupplier, io.grpc.protobuf.ProtoServiceDescriptorSupplier {
    AuditServiceBaseDescriptorSupplier() {}

    @java.lang.Override
    public com.google.protobuf.Descriptors.FileDescriptor getFileDescriptor() {
      return audittrail.Audittrail.getDescriptor();
    }

    @java.lang.Override
    public com.google.protobuf.Descriptors.ServiceDescriptor getServiceDescriptor() {
      return getFileDescriptor().findServiceByName("AuditService");
    }
  }

  private static final class AuditServiceFileDescriptorSupplier
      extends AuditServiceBaseDescriptorSupplier {
    AuditServiceFileDescriptorSupplier() {}
  }

  private static final class AuditServiceMethodDescriptorSupplier
      extends AuditServiceBaseDescriptorSupplier
      implements io.grpc.protobuf.ProtoMethodDescriptorSupplier {
    private final String methodName;

    AuditServiceMethodDescriptorSupplier(String methodName) {
      this.methodName = methodName;
    }

    @java.lang.Override
    public com.google.protobuf.Descriptors.MethodDescriptor getMethodDescriptor() {
      return getServiceDescriptor().findMethodByName(methodName);
    }
  }

  private static volatile io.grpc.ServiceDescriptor serviceDescriptor;

  public static io.grpc.ServiceDescriptor getServiceDescriptor() {
    io.grpc.ServiceDescriptor result = serviceDescriptor;
    if (result == null) {
      synchronized (AuditServiceGrpc.class) {
        result = serviceDescriptor;
        if (result == null) {
          serviceDescriptor = result = io.grpc.ServiceDescriptor.newBuilder(SERVICE_NAME)
              .setSchemaDescriptor(new AuditServiceFileDescriptorSupplier())
              .addMethod(getAddlogsMethod())
              .addMethod(getGetAuditLogsMethod())
              .build();
        }
      }
    }
    return result;
  }
}
