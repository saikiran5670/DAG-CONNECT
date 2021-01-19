// package: 
// file: src/app/protos/customer.proto

var src_app_protos_customer_pb = require("../../../src/app/protos/customer_pb");
var grpc = require("@improbable-eng/grpc-web").grpc;

var Customer = (function () {
  function Customer() {}
  Customer.serviceName = "Customer";
  return Customer;
}());

Customer.GetCustomerInfo = {
  methodName: "GetCustomerInfo",
  service: Customer,
  requestStream: false,
  responseStream: false,
  requestType: src_app_protos_customer_pb.CustomerLookupModel,
  responseType: src_app_protos_customer_pb.CustomerModel
};

exports.Customer = Customer;

function CustomerClient(serviceHost, options) {
  this.serviceHost = serviceHost;
  this.options = options || {};
}

CustomerClient.prototype.getCustomerInfo = function getCustomerInfo(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(Customer.GetCustomerInfo, {
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

exports.CustomerClient = CustomerClient;

