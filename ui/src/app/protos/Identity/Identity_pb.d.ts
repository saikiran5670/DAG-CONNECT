// package: identity
// file: src/app/protos/Identity/Identity.proto

import * as jspb from "google-protobuf";

export class UserRequest extends jspb.Message {
  getUsername(): string;
  setUsername(value: string): void;

  getPassword(): string;
  setPassword(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): UserRequest.AsObject;
  static toObject(includeInstance: boolean, msg: UserRequest): UserRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: UserRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): UserRequest;
  static deserializeBinaryFromReader(message: UserRequest, reader: jspb.BinaryReader): UserRequest;
}

export namespace UserRequest {
  export type AsObject = {
    username: string,
    password: string,
  }
}

export class TokenResponse extends jspb.Message {
  getAccesstoken(): string;
  setAccesstoken(value: string): void;

  getExpiresin(): number;
  setExpiresin(value: number): void;

  getRefreshexpiresin(): number;
  setRefreshexpiresin(value: number): void;

  getRefreshtoken(): string;
  setRefreshtoken(value: string): void;

  getTokentype(): string;
  setTokentype(value: string): void;

  getNotbeforepolicy(): number;
  setNotbeforepolicy(value: number): void;

  getSessionstate(): string;
  setSessionstate(value: string): void;

  getScope(): string;
  setScope(value: string): void;

  getError(): string;
  setError(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): TokenResponse.AsObject;
  static toObject(includeInstance: boolean, msg: TokenResponse): TokenResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: TokenResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): TokenResponse;
  static deserializeBinaryFromReader(message: TokenResponse, reader: jspb.BinaryReader): TokenResponse;
}

export namespace TokenResponse {
  export type AsObject = {
    accesstoken: string,
    expiresin: number,
    refreshexpiresin: number,
    refreshtoken: string,
    tokentype: string,
    notbeforepolicy: number,
    sessionstate: string,
    scope: string,
    error: string,
  }
}

