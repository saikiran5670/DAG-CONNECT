// package: 
// file: src/app/protos/customer.proto

import * as jspb from "google-protobuf";

export class CustomerLookupModel extends jspb.Message {
  getUserid(): number;
  setUserid(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): CustomerLookupModel.AsObject;
  static toObject(includeInstance: boolean, msg: CustomerLookupModel): CustomerLookupModel.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: CustomerLookupModel, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): CustomerLookupModel;
  static deserializeBinaryFromReader(message: CustomerLookupModel, reader: jspb.BinaryReader): CustomerLookupModel;
}

export namespace CustomerLookupModel {
  export type AsObject = {
    userid: number,
  }
}

export class CustomerModel extends jspb.Message {
  getFirstname(): string;
  setFirstname(value: string): void;

  getLastname(): string;
  setLastname(value: string): void;

  getEmailaddress(): string;
  setEmailaddress(value: string): void;

  getIsalive(): boolean;
  setIsalive(value: boolean): void;

  getAge(): number;
  setAge(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): CustomerModel.AsObject;
  static toObject(includeInstance: boolean, msg: CustomerModel): CustomerModel.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: CustomerModel, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): CustomerModel;
  static deserializeBinaryFromReader(message: CustomerModel, reader: jspb.BinaryReader): CustomerModel;
}

export namespace CustomerModel {
  export type AsObject = {
    firstname: string,
    lastname: string,
    emailaddress: string,
    isalive: boolean,
    age: number,
  }
}

