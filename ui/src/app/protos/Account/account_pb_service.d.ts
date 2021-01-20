// package: net.atos.daf.ct2.accountservice
// file: src/app/protos/Account/account.proto

import * as src_app_protos_Account_account_pb from "../../../../src/app/protos/Account/account_pb";
import {grpc} from "@improbable-eng/grpc-web";

type AccountServiceCreate = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountRequest;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountData;
};

type AccountServiceUpdate = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountRequest;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountData;
};

type AccountServiceDelete = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountRequest;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountResponse;
};

type AccountServiceChangePassword = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountRequest;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountResponse;
};

type AccountServiceGet = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountFilter;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountDataList;
};

type AccountServiceGetAccountDetail = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountGroupDetailsRequest;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountDetailsResponse;
};

type AccountServiceCreatePreference = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountPreference;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountPreferenceResponse;
};

type AccountServiceUpdatePreference = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountPreference;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountPreferenceResponse;
};

type AccountServiceDeletePreference = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountPreferenceFilter;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountPreferenceResponse;
};

type AccountServiceGetPreference = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountPreferenceFilter;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountPreferenceDataList;
};

type AccountServiceCreateAccessRelationship = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccessRelationship;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccessRelationshipResponse;
};

type AccountServiceUpdateAccessRelationship = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccessRelationship;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccessRelationshipResponse;
};

type AccountServiceDeleteAccessRelationship = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccessRelationship;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccessRelationshipResponse;
};

type AccountServiceGetAccessRelationship = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccessRelationshipFilter;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccessRelationshipDataList;
};

type AccountServiceCreateGroup = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountGroupRequest;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountGroupResponce;
};

type AccountServiceUpdateGroup = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountGroupRequest;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountGroupResponce;
};

type AccountServiceDeleteGroup = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.IdRequest;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountGroupResponce;
};

type AccountServiceGetAccountGroup = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountGroupFilterRequest;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountGroupDataList;
};

type AccountServiceGetAccountGroupDetail = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountGroupDetailsRequest;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountGroupDetailsDataList;
};

type AccountServiceAddAccountToGroups = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountGroupRefRequest;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountGroupRefResponce;
};

type AccountServiceAddRoles = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountRoleRequest;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountRoleResponse;
};

type AccountServiceRemoveRoles = {
  readonly methodName: string;
  readonly service: typeof AccountService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof src_app_protos_Account_account_pb.AccountRole;
  readonly responseType: typeof src_app_protos_Account_account_pb.AccountRoleResponse;
};

export class AccountService {
  static readonly serviceName: string;
  static readonly Create: AccountServiceCreate;
  static readonly Update: AccountServiceUpdate;
  static readonly Delete: AccountServiceDelete;
  static readonly ChangePassword: AccountServiceChangePassword;
  static readonly Get: AccountServiceGet;
  static readonly GetAccountDetail: AccountServiceGetAccountDetail;
  static readonly CreatePreference: AccountServiceCreatePreference;
  static readonly UpdatePreference: AccountServiceUpdatePreference;
  static readonly DeletePreference: AccountServiceDeletePreference;
  static readonly GetPreference: AccountServiceGetPreference;
  static readonly CreateAccessRelationship: AccountServiceCreateAccessRelationship;
  static readonly UpdateAccessRelationship: AccountServiceUpdateAccessRelationship;
  static readonly DeleteAccessRelationship: AccountServiceDeleteAccessRelationship;
  static readonly GetAccessRelationship: AccountServiceGetAccessRelationship;
  static readonly CreateGroup: AccountServiceCreateGroup;
  static readonly UpdateGroup: AccountServiceUpdateGroup;
  static readonly DeleteGroup: AccountServiceDeleteGroup;
  static readonly GetAccountGroup: AccountServiceGetAccountGroup;
  static readonly GetAccountGroupDetail: AccountServiceGetAccountGroupDetail;
  static readonly AddAccountToGroups: AccountServiceAddAccountToGroups;
  static readonly AddRoles: AccountServiceAddRoles;
  static readonly RemoveRoles: AccountServiceRemoveRoles;
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

export class AccountServiceClient {
  readonly serviceHost: string;

  constructor(serviceHost: string, options?: grpc.RpcOptions);
  create(
    requestMessage: src_app_protos_Account_account_pb.AccountRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountData|null) => void
  ): UnaryResponse;
  create(
    requestMessage: src_app_protos_Account_account_pb.AccountRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountData|null) => void
  ): UnaryResponse;
  update(
    requestMessage: src_app_protos_Account_account_pb.AccountRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountData|null) => void
  ): UnaryResponse;
  update(
    requestMessage: src_app_protos_Account_account_pb.AccountRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountData|null) => void
  ): UnaryResponse;
  delete(
    requestMessage: src_app_protos_Account_account_pb.AccountRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountResponse|null) => void
  ): UnaryResponse;
  delete(
    requestMessage: src_app_protos_Account_account_pb.AccountRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountResponse|null) => void
  ): UnaryResponse;
  changePassword(
    requestMessage: src_app_protos_Account_account_pb.AccountRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountResponse|null) => void
  ): UnaryResponse;
  changePassword(
    requestMessage: src_app_protos_Account_account_pb.AccountRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountResponse|null) => void
  ): UnaryResponse;
  get(
    requestMessage: src_app_protos_Account_account_pb.AccountFilter,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountDataList|null) => void
  ): UnaryResponse;
  get(
    requestMessage: src_app_protos_Account_account_pb.AccountFilter,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountDataList|null) => void
  ): UnaryResponse;
  getAccountDetail(
    requestMessage: src_app_protos_Account_account_pb.AccountGroupDetailsRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountDetailsResponse|null) => void
  ): UnaryResponse;
  getAccountDetail(
    requestMessage: src_app_protos_Account_account_pb.AccountGroupDetailsRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountDetailsResponse|null) => void
  ): UnaryResponse;
  createPreference(
    requestMessage: src_app_protos_Account_account_pb.AccountPreference,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountPreferenceResponse|null) => void
  ): UnaryResponse;
  createPreference(
    requestMessage: src_app_protos_Account_account_pb.AccountPreference,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountPreferenceResponse|null) => void
  ): UnaryResponse;
  updatePreference(
    requestMessage: src_app_protos_Account_account_pb.AccountPreference,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountPreferenceResponse|null) => void
  ): UnaryResponse;
  updatePreference(
    requestMessage: src_app_protos_Account_account_pb.AccountPreference,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountPreferenceResponse|null) => void
  ): UnaryResponse;
  deletePreference(
    requestMessage: src_app_protos_Account_account_pb.AccountPreferenceFilter,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountPreferenceResponse|null) => void
  ): UnaryResponse;
  deletePreference(
    requestMessage: src_app_protos_Account_account_pb.AccountPreferenceFilter,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountPreferenceResponse|null) => void
  ): UnaryResponse;
  getPreference(
    requestMessage: src_app_protos_Account_account_pb.AccountPreferenceFilter,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountPreferenceDataList|null) => void
  ): UnaryResponse;
  getPreference(
    requestMessage: src_app_protos_Account_account_pb.AccountPreferenceFilter,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountPreferenceDataList|null) => void
  ): UnaryResponse;
  createAccessRelationship(
    requestMessage: src_app_protos_Account_account_pb.AccessRelationship,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccessRelationshipResponse|null) => void
  ): UnaryResponse;
  createAccessRelationship(
    requestMessage: src_app_protos_Account_account_pb.AccessRelationship,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccessRelationshipResponse|null) => void
  ): UnaryResponse;
  updateAccessRelationship(
    requestMessage: src_app_protos_Account_account_pb.AccessRelationship,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccessRelationshipResponse|null) => void
  ): UnaryResponse;
  updateAccessRelationship(
    requestMessage: src_app_protos_Account_account_pb.AccessRelationship,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccessRelationshipResponse|null) => void
  ): UnaryResponse;
  deleteAccessRelationship(
    requestMessage: src_app_protos_Account_account_pb.AccessRelationship,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccessRelationshipResponse|null) => void
  ): UnaryResponse;
  deleteAccessRelationship(
    requestMessage: src_app_protos_Account_account_pb.AccessRelationship,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccessRelationshipResponse|null) => void
  ): UnaryResponse;
  getAccessRelationship(
    requestMessage: src_app_protos_Account_account_pb.AccessRelationshipFilter,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccessRelationshipDataList|null) => void
  ): UnaryResponse;
  getAccessRelationship(
    requestMessage: src_app_protos_Account_account_pb.AccessRelationshipFilter,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccessRelationshipDataList|null) => void
  ): UnaryResponse;
  createGroup(
    requestMessage: src_app_protos_Account_account_pb.AccountGroupRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountGroupResponce|null) => void
  ): UnaryResponse;
  createGroup(
    requestMessage: src_app_protos_Account_account_pb.AccountGroupRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountGroupResponce|null) => void
  ): UnaryResponse;
  updateGroup(
    requestMessage: src_app_protos_Account_account_pb.AccountGroupRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountGroupResponce|null) => void
  ): UnaryResponse;
  updateGroup(
    requestMessage: src_app_protos_Account_account_pb.AccountGroupRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountGroupResponce|null) => void
  ): UnaryResponse;
  deleteGroup(
    requestMessage: src_app_protos_Account_account_pb.IdRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountGroupResponce|null) => void
  ): UnaryResponse;
  deleteGroup(
    requestMessage: src_app_protos_Account_account_pb.IdRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountGroupResponce|null) => void
  ): UnaryResponse;
  getAccountGroup(
    requestMessage: src_app_protos_Account_account_pb.AccountGroupFilterRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountGroupDataList|null) => void
  ): UnaryResponse;
  getAccountGroup(
    requestMessage: src_app_protos_Account_account_pb.AccountGroupFilterRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountGroupDataList|null) => void
  ): UnaryResponse;
  getAccountGroupDetail(
    requestMessage: src_app_protos_Account_account_pb.AccountGroupDetailsRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountGroupDetailsDataList|null) => void
  ): UnaryResponse;
  getAccountGroupDetail(
    requestMessage: src_app_protos_Account_account_pb.AccountGroupDetailsRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountGroupDetailsDataList|null) => void
  ): UnaryResponse;
  addAccountToGroups(
    requestMessage: src_app_protos_Account_account_pb.AccountGroupRefRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountGroupRefResponce|null) => void
  ): UnaryResponse;
  addAccountToGroups(
    requestMessage: src_app_protos_Account_account_pb.AccountGroupRefRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountGroupRefResponce|null) => void
  ): UnaryResponse;
  addRoles(
    requestMessage: src_app_protos_Account_account_pb.AccountRoleRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountRoleResponse|null) => void
  ): UnaryResponse;
  addRoles(
    requestMessage: src_app_protos_Account_account_pb.AccountRoleRequest,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountRoleResponse|null) => void
  ): UnaryResponse;
  removeRoles(
    requestMessage: src_app_protos_Account_account_pb.AccountRole,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountRoleResponse|null) => void
  ): UnaryResponse;
  removeRoles(
    requestMessage: src_app_protos_Account_account_pb.AccountRole,
    callback: (error: ServiceError|null, responseMessage: src_app_protos_Account_account_pb.AccountRoleResponse|null) => void
  ): UnaryResponse;
}

