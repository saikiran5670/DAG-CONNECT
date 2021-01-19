// package: identity
// file: src/app/protos/Identity/Identity.proto

import * as src_app_protos_Identity_Identity_pb from "../../../../src/app/protos/Identity/Identity_pb";
import {grpc} from "@improbable-eng/grpc-web";

type AuthServiceGenerateToken = {
  readonly methodName: string;
  readonly service: typeof AuthService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Identity_Identity_pb.UserRequest;
  readonly responseType: typeof src_app_protos_Identity_Identity_pb.TokenResponse;
};

export class AuthService {
  static readonly serviceName: string;
  static readonly GenerateToken: AuthServiceGenerateToken;
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

export class AuthServiceClient {
  readonly serviceHost: string;

  constructor(serviceHost: string, options?: grpc.RpcOptions);
  generateToken(
    requestMessage: src_app_protos_Identity_Identity_pb.UserRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Identity_Identity_pb.TokenResponse|null) => void
  ): UnaryResponse;
  generateToken(
    requestMessage: src_app_protos_Identity_Identity_pb.UserRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Identity_Identity_pb.TokenResponse|null) => void
  ): UnaryResponse;
}

