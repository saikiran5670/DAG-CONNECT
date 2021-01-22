// package: greet
// file: src/app/protos/Greet/greet.proto

import * as jspb from "google-protobuf";

export class HelloRequest extends jspb.Message {
  getName(): string;
  setName(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): HelloRequest.AsObject;
  static toObject(includeInstance: boolean, msg: HelloRequest): HelloRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: HelloRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): HelloRequest;
  static deserializeBinaryFromReader(message: HelloRequest, reader: jspb.BinaryReader): HelloRequest;
}

export namespace HelloRequest {
  export type AsObject = {
    name: string,
  }
}

export class HelloReply extends jspb.Message {
  getMessage(): string;
  setMessage(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): HelloReply.AsObject;
  static toObject(includeInstance: boolean, msg: HelloReply): HelloReply.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: HelloReply, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): HelloReply;
  static deserializeBinaryFromReader(message: HelloReply, reader: jspb.BinaryReader): HelloReply;
}

export namespace HelloReply {
  export type AsObject = {
    message: string,
  }
}

export class ConnectionStringRequest extends jspb.Message {
  getMessage(): string;
  setMessage(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ConnectionStringRequest.AsObject;
  static toObject(includeInstance: boolean, msg: ConnectionStringRequest): ConnectionStringRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: ConnectionStringRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ConnectionStringRequest;
  static deserializeBinaryFromReader(message: ConnectionStringRequest, reader: jspb.BinaryReader): ConnectionStringRequest;
}

export namespace ConnectionStringRequest {
  export type AsObject = {
    message: string,
  }
}

export class ConnectionStringResponse extends jspb.Message {
  getMessage(): string;
  setMessage(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ConnectionStringResponse.AsObject;
  static toObject(includeInstance: boolean, msg: ConnectionStringResponse): ConnectionStringResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: ConnectionStringResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ConnectionStringResponse;
  static deserializeBinaryFromReader(message: ConnectionStringResponse, reader: jspb.BinaryReader): ConnectionStringResponse;
}

export namespace ConnectionStringResponse {
  export type AsObject = {
    message: string,
  }
}

