// package: net.atos.daf.ct2.accountservice
// file: src/app/protos/Account/account.proto

import * as jspb from "google-protobuf";
import * as google_protobuf_timestamp_pb from "google-protobuf/google/protobuf/timestamp_pb";

export class AccountResponse extends jspb.Message {
  getCode(): ResponcecodeMap[keyof ResponcecodeMap];
  setCode(value: ResponcecodeMap[keyof ResponcecodeMap]): void;

  getMessage(): string;
  setMessage(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountResponse.AsObject;
  static toObject(includeInstance: boolean, msg: AccountResponse): AccountResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountResponse;
  static deserializeBinaryFromReader(message: AccountResponse, reader: jspb.BinaryReader): AccountResponse;
}

export namespace AccountResponse {
  export type AsObject = {
    code: ResponcecodeMap[keyof ResponcecodeMap],
    message: string,
  }
}

export class AccountData extends jspb.Message {
  getCode(): ResponcecodeMap[keyof ResponcecodeMap];
  setCode(value: ResponcecodeMap[keyof ResponcecodeMap]): void;

  getMessage(): string;
  setMessage(value: string): void;

  hasAccount(): boolean;
  clearAccount(): void;
  getAccount(): AccountRequest | undefined;
  setAccount(value?: AccountRequest): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountData.AsObject;
  static toObject(includeInstance: boolean, msg: AccountData): AccountData.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountData, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountData;
  static deserializeBinaryFromReader(message: AccountData, reader: jspb.BinaryReader): AccountData;
}

export namespace AccountData {
  export type AsObject = {
    code: ResponcecodeMap[keyof ResponcecodeMap],
    message: string,
    account?: AccountRequest.AsObject,
  }
}

export class AccountDetails extends jspb.Message {
  hasAccount(): boolean;
  clearAccount(): void;
  getAccount(): AccountRequest | undefined;
  setAccount(value?: AccountRequest): void;

  getRoles(): string;
  setRoles(value: string): void;

  getAccountgroups(): string;
  setAccountgroups(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountDetails.AsObject;
  static toObject(includeInstance: boolean, msg: AccountDetails): AccountDetails.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountDetails, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountDetails;
  static deserializeBinaryFromReader(message: AccountDetails, reader: jspb.BinaryReader): AccountDetails;
}

export namespace AccountDetails {
  export type AsObject = {
    account?: AccountRequest.AsObject,
    roles: string,
    accountgroups: string,
  }
}

export class AccountDetailsResponse extends jspb.Message {
  clearAccountdetailsList(): void;
  getAccountdetailsList(): Array<AccountDetails>;
  setAccountdetailsList(value: Array<AccountDetails>): void;
  addAccountdetails(value?: AccountDetails, index?: number): AccountDetails;

  getCode(): ResponcecodeMap[keyof ResponcecodeMap];
  setCode(value: ResponcecodeMap[keyof ResponcecodeMap]): void;

  getMessage(): string;
  setMessage(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountDetailsResponse.AsObject;
  static toObject(includeInstance: boolean, msg: AccountDetailsResponse): AccountDetailsResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountDetailsResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountDetailsResponse;
  static deserializeBinaryFromReader(message: AccountDetailsResponse, reader: jspb.BinaryReader): AccountDetailsResponse;
}

export namespace AccountDetailsResponse {
  export type AsObject = {
    accountdetailsList: Array<AccountDetails.AsObject>,
    code: ResponcecodeMap[keyof ResponcecodeMap],
    message: string,
  }
}

export class AccountDataList extends jspb.Message {
  getCode(): ResponcecodeMap[keyof ResponcecodeMap];
  setCode(value: ResponcecodeMap[keyof ResponcecodeMap]): void;

  getMessage(): string;
  setMessage(value: string): void;

  clearAccountsList(): void;
  getAccountsList(): Array<AccountRequest>;
  setAccountsList(value: Array<AccountRequest>): void;
  addAccounts(value?: AccountRequest, index?: number): AccountRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountDataList.AsObject;
  static toObject(includeInstance: boolean, msg: AccountDataList): AccountDataList.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountDataList, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountDataList;
  static deserializeBinaryFromReader(message: AccountDataList, reader: jspb.BinaryReader): AccountDataList;
}

export namespace AccountDataList {
  export type AsObject = {
    code: ResponcecodeMap[keyof ResponcecodeMap],
    message: string,
    accountsList: Array<AccountRequest.AsObject>,
  }
}

export class AccountRequest extends jspb.Message {
  getId(): number;
  setId(value: number): void;

  getEmailid(): string;
  setEmailid(value: string): void;

  getSalutation(): string;
  setSalutation(value: string): void;

  getFirstname(): string;
  setFirstname(value: string): void;

  getLastname(): string;
  setLastname(value: string): void;

  getPassword(): string;
  setPassword(value: string): void;

  getOrganizationid(): number;
  setOrganizationid(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountRequest.AsObject;
  static toObject(includeInstance: boolean, msg: AccountRequest): AccountRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountRequest;
  static deserializeBinaryFromReader(message: AccountRequest, reader: jspb.BinaryReader): AccountRequest;
}

export namespace AccountRequest {
  export type AsObject = {
    id: number,
    emailid: string,
    salutation: string,
    firstname: string,
    lastname: string,
    password: string,
    organizationid: number,
  }
}

export class AccountId extends jspb.Message {
  getId(): number;
  setId(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountId.AsObject;
  static toObject(includeInstance: boolean, msg: AccountId): AccountId.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountId, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountId;
  static deserializeBinaryFromReader(message: AccountId, reader: jspb.BinaryReader): AccountId;
}

export namespace AccountId {
  export type AsObject = {
    id: number,
  }
}

export class AccountFilter extends jspb.Message {
  getId(): number;
  setId(value: number): void;

  getOrganizationid(): number;
  setOrganizationid(value: number): void;

  getName(): string;
  setName(value: string): void;

  getEmail(): string;
  setEmail(value: string): void;

  getAccountids(): string;
  setAccountids(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountFilter.AsObject;
  static toObject(includeInstance: boolean, msg: AccountFilter): AccountFilter.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountFilter, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountFilter;
  static deserializeBinaryFromReader(message: AccountFilter, reader: jspb.BinaryReader): AccountFilter;
}

export namespace AccountFilter {
  export type AsObject = {
    id: number,
    organizationid: number,
    name: string,
    email: string,
    accountids: string,
  }
}

export class AccountPreference extends jspb.Message {
  getId(): number;
  setId(value: number): void;

  getRefId(): number;
  setRefId(value: number): void;

  getLanguageid(): number;
  setLanguageid(value: number): void;

  getTimezoneid(): number;
  setTimezoneid(value: number): void;

  getCurrencyid(): number;
  setCurrencyid(value: number): void;

  getUnitid(): number;
  setUnitid(value: number): void;

  getVehicledisplayid(): number;
  setVehicledisplayid(value: number): void;

  getDateformatid(): number;
  setDateformatid(value: number): void;

  getDriverid(): string;
  setDriverid(value: string): void;

  getTimeformatid(): number;
  setTimeformatid(value: number): void;

  getLandingpagedisplayid(): number;
  setLandingpagedisplayid(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountPreference.AsObject;
  static toObject(includeInstance: boolean, msg: AccountPreference): AccountPreference.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountPreference, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountPreference;
  static deserializeBinaryFromReader(message: AccountPreference, reader: jspb.BinaryReader): AccountPreference;
}

export namespace AccountPreference {
  export type AsObject = {
    id: number,
    refId: number,
    languageid: number,
    timezoneid: number,
    currencyid: number,
    unitid: number,
    vehicledisplayid: number,
    dateformatid: number,
    driverid: string,
    timeformatid: number,
    landingpagedisplayid: number,
  }
}

export class AccountPreferenceResponse extends jspb.Message {
  getCode(): ResponcecodeMap[keyof ResponcecodeMap];
  setCode(value: ResponcecodeMap[keyof ResponcecodeMap]): void;

  getMessage(): string;
  setMessage(value: string): void;

  hasAccountpreference(): boolean;
  clearAccountpreference(): void;
  getAccountpreference(): AccountPreference | undefined;
  setAccountpreference(value?: AccountPreference): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountPreferenceResponse.AsObject;
  static toObject(includeInstance: boolean, msg: AccountPreferenceResponse): AccountPreferenceResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountPreferenceResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountPreferenceResponse;
  static deserializeBinaryFromReader(message: AccountPreferenceResponse, reader: jspb.BinaryReader): AccountPreferenceResponse;
}

export namespace AccountPreferenceResponse {
  export type AsObject = {
    code: ResponcecodeMap[keyof ResponcecodeMap],
    message: string,
    accountpreference?: AccountPreference.AsObject,
  }
}

export class AccountPreferenceDataList extends jspb.Message {
  getCode(): ResponcecodeMap[keyof ResponcecodeMap];
  setCode(value: ResponcecodeMap[keyof ResponcecodeMap]): void;

  getMessage(): string;
  setMessage(value: string): void;

  clearPreferenceList(): void;
  getPreferenceList(): Array<AccountPreference>;
  setPreferenceList(value: Array<AccountPreference>): void;
  addPreference(value?: AccountPreference, index?: number): AccountPreference;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountPreferenceDataList.AsObject;
  static toObject(includeInstance: boolean, msg: AccountPreferenceDataList): AccountPreferenceDataList.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountPreferenceDataList, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountPreferenceDataList;
  static deserializeBinaryFromReader(message: AccountPreferenceDataList, reader: jspb.BinaryReader): AccountPreferenceDataList;
}

export namespace AccountPreferenceDataList {
  export type AsObject = {
    code: ResponcecodeMap[keyof ResponcecodeMap],
    message: string,
    preferenceList: Array<AccountPreference.AsObject>,
  }
}

export class AccountPreferenceFilter extends jspb.Message {
  getId(): number;
  setId(value: number): void;

  getRefId(): number;
  setRefId(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountPreferenceFilter.AsObject;
  static toObject(includeInstance: boolean, msg: AccountPreferenceFilter): AccountPreferenceFilter.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountPreferenceFilter, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountPreferenceFilter;
  static deserializeBinaryFromReader(message: AccountPreferenceFilter, reader: jspb.BinaryReader): AccountPreferenceFilter;
}

export namespace AccountPreferenceFilter {
  export type AsObject = {
    id: number,
    refId: number,
  }
}

export class AccountGroupRef extends jspb.Message {
  getGroupId(): number;
  setGroupId(value: number): void;

  getRefId(): number;
  setRefId(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountGroupRef.AsObject;
  static toObject(includeInstance: boolean, msg: AccountGroupRef): AccountGroupRef.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountGroupRef, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountGroupRef;
  static deserializeBinaryFromReader(message: AccountGroupRef, reader: jspb.BinaryReader): AccountGroupRef;
}

export namespace AccountGroupRef {
  export type AsObject = {
    groupId: number,
    refId: number,
  }
}

export class AccountGroupRequest extends jspb.Message {
  getId(): number;
  setId(value: number): void;

  getName(): string;
  setName(value: string): void;

  getOrganizationid(): number;
  setOrganizationid(value: number): void;

  getRefid(): number;
  setRefid(value: number): void;

  getDescription(): string;
  setDescription(value: string): void;

  getGrouprefcount(): number;
  setGrouprefcount(value: number): void;

  clearGrouprefList(): void;
  getGrouprefList(): Array<AccountGroupRef>;
  setGrouprefList(value: Array<AccountGroupRef>): void;
  addGroupref(value?: AccountGroupRef, index?: number): AccountGroupRef;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountGroupRequest.AsObject;
  static toObject(includeInstance: boolean, msg: AccountGroupRequest): AccountGroupRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountGroupRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountGroupRequest;
  static deserializeBinaryFromReader(message: AccountGroupRequest, reader: jspb.BinaryReader): AccountGroupRequest;
}

export namespace AccountGroupRequest {
  export type AsObject = {
    id: number,
    name: string,
    organizationid: number,
    refid: number,
    description: string,
    grouprefcount: number,
    grouprefList: Array<AccountGroupRef.AsObject>,
  }
}

export class AccountGroupResponce extends jspb.Message {
  getId(): number;
  setId(value: number): void;

  getCode(): ResponcecodeMap[keyof ResponcecodeMap];
  setCode(value: ResponcecodeMap[keyof ResponcecodeMap]): void;

  getMessage(): string;
  setMessage(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountGroupResponce.AsObject;
  static toObject(includeInstance: boolean, msg: AccountGroupResponce): AccountGroupResponce.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountGroupResponce, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountGroupResponce;
  static deserializeBinaryFromReader(message: AccountGroupResponce, reader: jspb.BinaryReader): AccountGroupResponce;
}

export namespace AccountGroupResponce {
  export type AsObject = {
    id: number,
    code: ResponcecodeMap[keyof ResponcecodeMap],
    message: string,
  }
}

export class AccountGroupDataList extends jspb.Message {
  getCode(): ResponcecodeMap[keyof ResponcecodeMap];
  setCode(value: ResponcecodeMap[keyof ResponcecodeMap]): void;

  getMessage(): string;
  setMessage(value: string): void;

  clearAccountgrouprequestList(): void;
  getAccountgrouprequestList(): Array<AccountGroupRequest>;
  setAccountgrouprequestList(value: Array<AccountGroupRequest>): void;
  addAccountgrouprequest(value?: AccountGroupRequest, index?: number): AccountGroupRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountGroupDataList.AsObject;
  static toObject(includeInstance: boolean, msg: AccountGroupDataList): AccountGroupDataList.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountGroupDataList, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountGroupDataList;
  static deserializeBinaryFromReader(message: AccountGroupDataList, reader: jspb.BinaryReader): AccountGroupDataList;
}

export namespace AccountGroupDataList {
  export type AsObject = {
    code: ResponcecodeMap[keyof ResponcecodeMap],
    message: string,
    accountgrouprequestList: Array<AccountGroupRequest.AsObject>,
  }
}

export class AccountGroupFilterRequest extends jspb.Message {
  getId(): number;
  setId(value: number): void;

  getGroupref(): boolean;
  setGroupref(value: boolean): void;

  getGrouprefcount(): boolean;
  setGrouprefcount(value: boolean): void;

  getOrganizationid(): number;
  setOrganizationid(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountGroupFilterRequest.AsObject;
  static toObject(includeInstance: boolean, msg: AccountGroupFilterRequest): AccountGroupFilterRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountGroupFilterRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountGroupFilterRequest;
  static deserializeBinaryFromReader(message: AccountGroupFilterRequest, reader: jspb.BinaryReader): AccountGroupFilterRequest;
}

export namespace AccountGroupFilterRequest {
  export type AsObject = {
    id: number,
    groupref: boolean,
    grouprefcount: boolean,
    organizationid: number,
  }
}

export class AccountGroupDetailsRequest extends jspb.Message {
  getAccountid(): number;
  setAccountid(value: number): void;

  getOrganizationid(): number;
  setOrganizationid(value: number): void;

  getGroupid(): number;
  setGroupid(value: number): void;

  getRoleid(): number;
  setRoleid(value: number): void;

  getName(): string;
  setName(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountGroupDetailsRequest.AsObject;
  static toObject(includeInstance: boolean, msg: AccountGroupDetailsRequest): AccountGroupDetailsRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountGroupDetailsRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountGroupDetailsRequest;
  static deserializeBinaryFromReader(message: AccountGroupDetailsRequest, reader: jspb.BinaryReader): AccountGroupDetailsRequest;
}

export namespace AccountGroupDetailsRequest {
  export type AsObject = {
    accountid: number,
    organizationid: number,
    groupid: number,
    roleid: number,
    name: string,
  }
}

export class AccountGroupDetail extends jspb.Message {
  getGroupid(): number;
  setGroupid(value: number): void;

  getAccountgroupname(): string;
  setAccountgroupname(value: string): void;

  getVehiclecount(): number;
  setVehiclecount(value: number): void;

  getAccountcount(): number;
  setAccountcount(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountGroupDetail.AsObject;
  static toObject(includeInstance: boolean, msg: AccountGroupDetail): AccountGroupDetail.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountGroupDetail, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountGroupDetail;
  static deserializeBinaryFromReader(message: AccountGroupDetail, reader: jspb.BinaryReader): AccountGroupDetail;
}

export namespace AccountGroupDetail {
  export type AsObject = {
    groupid: number,
    accountgroupname: string,
    vehiclecount: number,
    accountcount: number,
  }
}

export class AccountGroupDetailsDataList extends jspb.Message {
  clearAccountgroupdetailList(): void;
  getAccountgroupdetailList(): Array<AccountGroupDetail>;
  setAccountgroupdetailList(value: Array<AccountGroupDetail>): void;
  addAccountgroupdetail(value?: AccountGroupDetail, index?: number): AccountGroupDetail;

  getCode(): ResponcecodeMap[keyof ResponcecodeMap];
  setCode(value: ResponcecodeMap[keyof ResponcecodeMap]): void;

  getMessage(): string;
  setMessage(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountGroupDetailsDataList.AsObject;
  static toObject(includeInstance: boolean, msg: AccountGroupDetailsDataList): AccountGroupDetailsDataList.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountGroupDetailsDataList, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountGroupDetailsDataList;
  static deserializeBinaryFromReader(message: AccountGroupDetailsDataList, reader: jspb.BinaryReader): AccountGroupDetailsDataList;
}

export namespace AccountGroupDetailsDataList {
  export type AsObject = {
    accountgroupdetailList: Array<AccountGroupDetail.AsObject>,
    code: ResponcecodeMap[keyof ResponcecodeMap],
    message: string,
  }
}

export class IdRequest extends jspb.Message {
  getId(): number;
  setId(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): IdRequest.AsObject;
  static toObject(includeInstance: boolean, msg: IdRequest): IdRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: IdRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): IdRequest;
  static deserializeBinaryFromReader(message: IdRequest, reader: jspb.BinaryReader): IdRequest;
}

export namespace IdRequest {
  export type AsObject = {
    id: number,
  }
}

export class AccountRole extends jspb.Message {
  getAccountid(): number;
  setAccountid(value: number): void;

  getOrganizationid(): number;
  setOrganizationid(value: number): void;

  getRoleid(): number;
  setRoleid(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountRole.AsObject;
  static toObject(includeInstance: boolean, msg: AccountRole): AccountRole.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountRole, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountRole;
  static deserializeBinaryFromReader(message: AccountRole, reader: jspb.BinaryReader): AccountRole;
}

export namespace AccountRole {
  export type AsObject = {
    accountid: number,
    organizationid: number,
    roleid: number,
  }
}

export class AccountRoleRequest extends jspb.Message {
  clearAccountrolesList(): void;
  getAccountrolesList(): Array<AccountRole>;
  setAccountrolesList(value: Array<AccountRole>): void;
  addAccountroles(value?: AccountRole, index?: number): AccountRole;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountRoleRequest.AsObject;
  static toObject(includeInstance: boolean, msg: AccountRoleRequest): AccountRoleRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountRoleRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountRoleRequest;
  static deserializeBinaryFromReader(message: AccountRoleRequest, reader: jspb.BinaryReader): AccountRoleRequest;
}

export namespace AccountRoleRequest {
  export type AsObject = {
    accountrolesList: Array<AccountRole.AsObject>,
  }
}

export class AccountRoleResponse extends jspb.Message {
  getCode(): ResponcecodeMap[keyof ResponcecodeMap];
  setCode(value: ResponcecodeMap[keyof ResponcecodeMap]): void;

  getMessage(): string;
  setMessage(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountRoleResponse.AsObject;
  static toObject(includeInstance: boolean, msg: AccountRoleResponse): AccountRoleResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccountRoleResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountRoleResponse;
  static deserializeBinaryFromReader(message: AccountRoleResponse, reader: jspb.BinaryReader): AccountRoleResponse;
}

export namespace AccountRoleResponse {
  export type AsObject = {
    code: ResponcecodeMap[keyof ResponcecodeMap],
    message: string,
  }
}

export class AccessRelationship extends jspb.Message {
  getId(): number;
  setId(value: number): void;

  getAccessrelationtype(): string;
  setAccessrelationtype(value: string): void;

  getAccountgroupid(): number;
  setAccountgroupid(value: number): void;

  getVehiclegroupid(): number;
  setVehiclegroupid(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccessRelationship.AsObject;
  static toObject(includeInstance: boolean, msg: AccessRelationship): AccessRelationship.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccessRelationship, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccessRelationship;
  static deserializeBinaryFromReader(message: AccessRelationship, reader: jspb.BinaryReader): AccessRelationship;
}

export namespace AccessRelationship {
  export type AsObject = {
    id: number,
    accessrelationtype: string,
    accountgroupid: number,
    vehiclegroupid: number,
  }
}

export class AccessRelationshipFilter extends jspb.Message {
  getAccountid(): number;
  setAccountid(value: number): void;

  getAccountgroupid(): number;
  setAccountgroupid(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccessRelationshipFilter.AsObject;
  static toObject(includeInstance: boolean, msg: AccessRelationshipFilter): AccessRelationshipFilter.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccessRelationshipFilter, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccessRelationshipFilter;
  static deserializeBinaryFromReader(message: AccessRelationshipFilter, reader: jspb.BinaryReader): AccessRelationshipFilter;
}

export namespace AccessRelationshipFilter {
  export type AsObject = {
    accountid: number,
    accountgroupid: number,
  }
}

export class AccessRelationshipResponse extends jspb.Message {
  getCode(): ResponcecodeMap[keyof ResponcecodeMap];
  setCode(value: ResponcecodeMap[keyof ResponcecodeMap]): void;

  getMessage(): string;
  setMessage(value: string): void;

  hasAccessrelationship(): boolean;
  clearAccessrelationship(): void;
  getAccessrelationship(): AccessRelationship | undefined;
  setAccessrelationship(value?: AccessRelationship): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccessRelationshipResponse.AsObject;
  static toObject(includeInstance: boolean, msg: AccessRelationshipResponse): AccessRelationshipResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccessRelationshipResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccessRelationshipResponse;
  static deserializeBinaryFromReader(message: AccessRelationshipResponse, reader: jspb.BinaryReader): AccessRelationshipResponse;
}

export namespace AccessRelationshipResponse {
  export type AsObject = {
    code: ResponcecodeMap[keyof ResponcecodeMap],
    message: string,
    accessrelationship?: AccessRelationship.AsObject,
  }
}

export class AccessRelationshipDataList extends jspb.Message {
  getCode(): ResponcecodeMap[keyof ResponcecodeMap];
  setCode(value: ResponcecodeMap[keyof ResponcecodeMap]): void;

  getMessage(): string;
  setMessage(value: string): void;

  clearAccessrelationshipList(): void;
  getAccessrelationshipList(): Array<AccessRelationship>;
  setAccessrelationshipList(value: Array<AccessRelationship>): void;
  addAccessrelationship(value?: AccessRelationship, index?: number): AccessRelationship;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccessRelationshipDataList.AsObject;
  static toObject(includeInstance: boolean, msg: AccessRelationshipDataList): AccessRelationshipDataList.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AccessRelationshipDataList, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccessRelationshipDataList;
  static deserializeBinaryFromReader(message: AccessRelationshipDataList, reader: jspb.BinaryReader): AccessRelationshipDataList;
}

export namespace AccessRelationshipDataList {
  export type AsObject = {
    code: ResponcecodeMap[keyof ResponcecodeMap],
    message: string,
    accessrelationshipList: Array<AccessRelationship.AsObject>,
  }
}

export interface AccountTypeMap {
  NONE: 0;
  SYSTEMACCOUNT: 1;
  PORTALACCOUNT: 2;
}

export const AccountType: AccountTypeMap;

export interface ResponcecodeMap {
  SUCCESS: 0;
  FAILED: 1;
}

export const Responcecode: ResponcecodeMap;

export interface ObjectTypeMap {
  VEHICLEGROUP: 0;
  ACCOUNTGROUP: 1;
}

export const ObjectType: ObjectTypeMap;

export interface GroupTypeMap {
  SINGLE: 0;
  GROUP: 1;
  DYNAMIC: 2;
}

export const GroupType: GroupTypeMap;

export interface FunctionEnumMap {
  ALL: 0;
}

export const FunctionEnum: FunctionEnumMap;

