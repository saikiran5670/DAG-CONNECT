// package: greet
// file: src/app/protos/Greet/greet.proto

var src_app_protos_Greet_greet_pb = require("../../../../src/app/protos/Greet/greet_pb");
var grpc = require("@improbable-eng/grpc-web").grpc;

var Greeter = (function () {
  function Greeter() {}
  Greeter.serviceName = "greet.Greeter";
  return Greeter;
}());

Greeter.SayHello = {
  methodName: "SayHello",
  service: Greeter,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Greet_greet_pb.HelloRequest,
  responseType: src_app_protos_Greet_greet_pb.HelloReply
};

Greeter.ConnectionStringKey = {
  methodName: "ConnectionStringKey",
  service: Greeter,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_Greet_greet_pb.ConnectionStringRequest,
  responseType: src_app_protos_Greet_greet_pb.ConnectionStringResponse
};

exports.Greeter = Greeter;

function GreeterClient(serviceHost, options) {
  this.serviceHost = serviceHost;
  this.options = options || {};
}

GreeterClient.prototype.sayHello = function sayHello(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(Greeter.SayHello, {
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

GreeterClient.prototype.connectionStringKey = function connectionStringKey(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(Greeter.ConnectionStringKey, {
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

exports.GreeterClient = GreeterClient;

