// package: identity
// file: src/app/protos/Identity/Identity.proto

var src_app_protos_Identity_Identity_pb = require("../../../../src/app/protos/Identity/Identity_pb");
var grpc = require("@improbable-eng/grpc-web").grpc;

var AuthService = (function () {
  function AuthService() {}
  AuthService.serviceName = "identity.AuthService";
  return AuthService;
}());

AuthService.GenerateToken = {
  methodName: "GenerateToken",
  service: AuthService,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Identity_Identity_pb.UserRequest,
  responseType: src_app_protos_Identity_Identity_pb.TokenResponse
};

exports.AuthService = AuthService;

function AuthServiceClient(serviceHost, options) {
  this.serviceHost = serviceHost;
  this.options = options || {};
}

AuthServiceClient.prototype.generateToken = function generateToken(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(AuthService.GenerateToken, {
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

exports.AuthServiceClient = AuthServiceClient;

