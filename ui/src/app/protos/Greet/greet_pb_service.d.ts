// package: greet
// file: src/app/protos/Greet/greet.proto

import * as src_app_protos_Greet_greet_pb from "../../../../src/app/protos/Greet/greet_pb";
import {grpc} from "@improbable-eng/grpc-web";

type GreeterSayHello = {
  readonly methodName: string;
  readonly service: typeof Greeter;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Greet_greet_pb.HelloRequest;
  readonly responseType: typeof src_app_protos_Greet_greet_pb.HelloReply;
};

type GreeterConnectionStringKey = {
  readonly methodName: string;
  readonly service: typeof Greeter;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Greet_greet_pb.ConnectionStringRequest;
  readonly responseType: typeof src_app_protos_Greet_greet_pb.ConnectionStringResponse;
};

export class Greeter {
  static readonly serviceName: string;
  static readonly SayHello: GreeterSayHello;
  static readonly ConnectionStringKey: GreeterConnectionStringKey;
}

export type ServiceError = { message: string, code: number; metadata: grpc.Metadata }
export type Status = { details: string, code: number; metadata: grpc.Metadata }

interface UnaryResponse {
  cancel(): void;
}
interface ResponseStream<T> {
  cancel(): void;
  on(type: 'data', handler: (message: T) => void): ResponseStream<T>;
  on(type: 'end', handler: (status?: Status) => void): ResponseStream<T>;
  on(type: 'status', handler: (status: Status) => void): ResponseStream<T>;
}
interface RequestStream<T> {
  write(message: T): RequestStream<T>;
  end(): void;
  cancel(): void;
  on(type: 'end', handler: (status?: Status) => void): RequestStream<T>;
  on(type: 'status', handler: (status: Status) => void): RequestStream<T>;
}
interface BidirectionalStream<ReqT, ResT> {
  write(message: ReqT): BidirectionalStream<ReqT, ResT>;
  end(): void;
  cancel(): void;
  on(type: 'data', handler: (message: ResT) => void): BidirectionalStream<ReqT, ResT>;
  on(type: 'end', handler: (status?: Status) => void): BidirectionalStream<ReqT, ResT>;
  on(type: 'status', handler: (status: Status) => void): BidirectionalStream<ReqT, ResT>;
}

export class GreeterClient {
  readonly serviceHost: string;

  constructor(serviceHost: string, options?: grpc.RpcOptions);
  sayHello(
    requestMessage: src_app_protos_Greet_greet_pb.HelloRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Greet_greet_pb.HelloReply|null) => void
  ): UnaryResponse;
  sayHello(
    requestMessage: src_app_protos_Greet_greet_pb.HelloRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Greet_greet_pb.HelloReply|null) => void
  ): UnaryResponse;
  connectionStringKey(
    requestMessage: src_app_protos_Greet_greet_pb.ConnectionStringRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Greet_greet_pb.ConnectionStringResponse|null) => void
  ): UnaryResponse;
  connectionStringKey(
    requestMessage: src_app_protos_Greet_greet_pb.ConnectionStringRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Greet_greet_pb.ConnectionStringResponse|null) => void
  ): UnaryResponse;
}

