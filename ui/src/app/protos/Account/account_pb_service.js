// package: net.atos.daf.ct2.accountservice
// file: src/app/protos/Account/account.proto

var src_app_protos_Account_account_pb = require("../../../../src/app/protos/Account/account_pb");
var grpc = require("@improbable-eng/grpc-web").grpc;

var AccountService = (function () {
  function AccountService() {}
  AccountService.serviceName = "net.atos.daf.ct2.accountservice.AccountService";
  return AccountService;
}());

AccountService.Create = {
  methodName: "Create",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountRequest,
  responseType: src_app_protos_Account_account_pb.AccountData
};

AccountService.Update = {
  methodName: "Update",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountRequest,
  responseType: src_app_protos_Account_account_pb.AccountData
};

AccountService.Delete = {
  methodName: "Delete",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountRequest,
  responseType: src_app_protos_Account_account_pb.AccountResponse
};

AccountService.ChangePassword = {
  methodName: "ChangePassword",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountRequest,
  responseType: src_app_protos_Account_account_pb.AccountResponse
};

AccountService.Get = {
  methodName: "Get",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountFilter,
  responseType: src_app_protos_Account_account_pb.AccountDataList
};

AccountService.GetAccountDetail = {
  methodName: "GetAccountDetail",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountGroupDetailsRequest,
  responseType: src_app_protos_Account_account_pb.AccountDetailsResponse
};

AccountService.CreatePreference = {
  methodName: "CreatePreference",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountPreference,
  responseType: src_app_protos_Account_account_pb.AccountPreferenceResponse
};

AccountService.UpdatePreference = {
  methodName: "UpdatePreference",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountPreference,
  responseType: src_app_protos_Account_account_pb.AccountPreferenceResponse
};

AccountService.DeletePreference = {
  methodName: "DeletePreference",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountPreferenceFilter,
  responseType: src_app_protos_Account_account_pb.AccountPreferenceResponse
};

AccountService.GetPreference = {
  methodName: "GetPreference",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountPreferenceFilter,
  responseType: src_app_protos_Account_account_pb.AccountPreferenceDataList
};

AccountService.CreateAccessRelationship = {
  methodName: "CreateAccessRelationship",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccessRelationship,
  responseType: src_app_protos_Account_account_pb.AccessRelationshipResponse
};

AccountService.UpdateAccessRelationship = {
  methodName: "UpdateAccessRelationship",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccessRelationship,
  responseType: src_app_protos_Account_account_pb.AccessRelationshipResponse
};

AccountService.DeleteAccessRelationship = {
  methodName: "DeleteAccessRelationship",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccessRelationship,
  responseType: src_app_protos_Account_account_pb.AccessRelationshipResponse
};

AccountService.GetAccessRelationship = {
  methodName: "GetAccessRelationship",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccessRelationshipFilter,
  responseType: src_app_protos_Account_account_pb.AccessRelationshipDataList
};

AccountService.CreateGroup = {
  methodName: "CreateGroup",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountGroupRequest,
  responseType: src_app_protos_Account_account_pb.AccountGroupResponce
};

AccountService.UpdateGroup = {
  methodName: "UpdateGroup",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountGroupRequest,
  responseType: src_app_protos_Account_account_pb.AccountGroupResponce
};

AccountService.DeleteGroup = {
  methodName: "DeleteGroup",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.IdRequest,
  responseType: src_app_protos_Account_account_pb.AccountGroupResponce
};

AccountService.GetAccountGroup = {
  methodName: "GetAccountGroup",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountGroupFilterRequest,
  responseType: src_app_protos_Account_account_pb.AccountGroupDataList
};

AccountService.GetAccountGroupDetail = {
  methodName: "GetAccountGroupDetail",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountGroupDetailsRequest,
  responseType: src_app_protos_Account_account_pb.AccountGroupDetailsDataList
};

AccountService.AddRoles = {
  methodName: "AddRoles",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountRoleRequest,
  responseType: src_app_protos_Account_account_pb.AccountRoleResponse
};

AccountService.RemoveRoles = {
  methodName: "RemoveRoles",
  service: AccountService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Account_account_pb.AccountRole,
  responseType: src_app_protos_Account_account_pb.AccountRoleResponse
};

exports.AccountService = AccountService;

function AccountServiceClient(serviceHost, options) {
  this.serviceHost = serviceHost;
  this.options = options || {};
}

AccountServiceClient.prototype.create = function create(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.Create, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.update = function update(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.Update, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.delete = function pb_delete(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.Delete, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.changePassword = function changePassword(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.ChangePassword, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.get = function get(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.Get, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.getAccountDetail = function getAccountDetail(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.GetAccountDetail, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.createPreference = function createPreference(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.CreatePreference, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.updatePreference = function updatePreference(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.UpdatePreference, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.deletePreference = function deletePreference(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.DeletePreference, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.getPreference = function getPreference(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.GetPreference, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.createAccessRelationship = function createAccessRelationship(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.CreateAccessRelationship, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.updateAccessRelationship = function updateAccessRelationship(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.UpdateAccessRelationship, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.deleteAccessRelationship = function deleteAccessRelationship(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.DeleteAccessRelationship, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.getAccessRelationship = function getAccessRelationship(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.GetAccessRelationship, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.createGroup = function createGroup(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.CreateGroup, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.updateGroup = function updateGroup(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.UpdateGroup, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.deleteGroup = function deleteGroup(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.DeleteGroup, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.getAccountGroup = function getAccountGroup(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.GetAccountGroup, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.getAccountGroupDetail = function getAccountGroupDetail(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.GetAccountGroupDetail, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.addRoles = function addRoles(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.AddRoles, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

AccountServiceClient.prototype.removeRoles = function removeRoles(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AccountService.RemoveRoles, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

exports.AccountServiceClient = AccountServiceClient;

